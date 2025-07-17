using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using xicheng.utility;

/* 注意事项：
   无可用设备检查   Microphone.devices.Length > 0，否则避免调用录音相关函数
   录音失败        Unity中Start不能在设备未连接时调用，否则将返回null或报错
   多平台差异      Android需要动态权限申请，iOS需在Info.plist中添加NSMicrophoneUsageDescription
   iOS延迟问题     iOS 开启录音后前几帧数据可能为静音，建议预热录制 0.5s
 */
namespace xicheng.mic
{
    /// <summary>
    /// 麦克风管理器
    /// 功能：多平台麦克风控制、录音、音量检测、频谱获取、文件保存
    /// </summary>
    public class MicManager : MonoSingleton<MicManager>
    {
        #region 录音配置参数

        [Header("录音核心配置")] [Tooltip("采样率（Hz），常见值：16000/22050/44100/48000")] [SerializeField]
        private int sampleRate = 44100;

        [Tooltip("单次录音时长（秒），实际录音长度可能受设备限制")] [SerializeField]
        private int recordLength = 3;

        [Tooltip("是否启用回环模式（播放麦克风实时声音）")] [SerializeField]
        private bool useLoopback = true;

        #endregion

        #region 文件保存配置

        [Header("文件保存配置")] [Tooltip("录音文件保存目录（相对于PersistentDataPath）")] [SerializeField]
        private string saveDirectory = "AudioRecordings";

        [Tooltip("默认文件格式（仅支持WAV）")] [SerializeField]
        private AudioFileFormat defaultFileFormat = AudioFileFormat.WAV;

        [Tooltip("停止录音时是否自动保存")] [SerializeField]
        private bool autoSaveOnStop = false;

        [Tooltip("最大保存文件数量（超过时自动删除旧文件）")] [SerializeField]
        private int maxSavedFiles = 50;

        #endregion

        #region 运行时状态字段

        [Tooltip("是否正在录音")] private bool isRecording;

        [Tooltip("是否有可用麦克风设备")] private bool HasMicrophone => Microphone.devices.Length > 0;

        [Tooltip("当前选中的麦克风设备名称")] private string currentDevice;

        [Tooltip("当前录音的AudioClip对象")] private AudioClip microphoneClip;

        [Tooltip("上次获取音频数据的位置（用于循环缓冲）")] private int lastSamplePos;

        [Tooltip("是否正在缓冲音频数据")] private bool isBuffering;

        [Tooltip("存储录音数据的缓冲区（List<float>）")] private List<float> recordingBuffer = new List<float>();

        [Tooltip("音量检测协程")] private Coroutine volumeUpdateCoroutine;

        [Tooltip("当前保存的文件完整路径")] private string currentSavePath;

        [Tooltip("录音最大样本数（recordLength * sampleRate）")]
        private int maxSampleCount;

        #endregion

        #region 回调事件（状态变更通知）

        /// <summary>音量变化时触发（单位：dB）</summary>
        public event Action<float> OnVolumeChanged;

        /// <summary>麦克风设备变更时触发</summary>
        public event Action<string> OnDeviceChanged;

        /// <summary>录音状态变更时触发（true=开始/false=停止）</summary>
        public event Action<bool> OnRecordingStateChanged;

        /// <summary>发生错误时触发（错误信息）</summary>
        public event Action<string> OnError;

        /// <summary>文件保存成功时触发（文件路径）</summary>
        public event Action<string> OnFileSaved;

        #endregion

        #region 生命周期与初始化

        private void Awake()
        {
            // 初始化最大样本数（用于缓冲区容量控制）
            maxSampleCount = recordLength * sampleRate;
            // 首次检查麦克风权限（移动平台需要）
            CheckMicrophonePermission();
        }

        #endregion

        #region 核心功能方法

        /// <summary>
        /// 开始录音（支持指定设备）
        /// </summary>
        /// <param name="deviceName">目标设备名称（null表示使用默认设备）</param>
        /// <returns>是否成功启动录音</returns>
        public bool StartRecording(string deviceName = null)
        {
            // 防止重复启动
            if (isRecording)
            {
                ULog.Info("[Microphone] 录音已在进行中");
                return false;
            }

            // 检查设备可用性
            if (!HasMicrophone)
            {
                ULog.Error("[Microphone] 未找到可用麦克风设备");
                OnError?.Invoke("未找到可用麦克风设备");
                return false;
            }

            try
            {
                // 设备选择逻辑
                var validDevices = Microphone.devices;
                currentDevice = deviceName ?? (validDevices.Length > 0 ? validDevices[0] : null);

                // 设备有效性校验
                if (string.IsNullOrEmpty(currentDevice))
                {
                    ULog.Error("[Microphone] 无法选择麦克风设备");
                    OnError?.Invoke("无法选择麦克风设备");
                    return false;
                }

                // 获取设备支持的采样率范围
                Microphone.GetDeviceCaps(currentDevice, out int minFreq, out int maxFreq);

                // 调整采样率到设备支持范围
                int targetSampleRate = Mathf.Clamp(sampleRate, minFreq, maxFreq);
                if (targetSampleRate != sampleRate)
                {
                    ULog.Info($"采样率调整：{sampleRate}Hz → {targetSampleRate}Hz");
                    sampleRate = targetSampleRate;
                }

                // 启动录音（Unity API）
                microphoneClip = Microphone.Start(
                    currentDevice, // 设备名称
                    useLoopback, // 回环模式
                    recordLength, // 录音时长（秒）
                    sampleRate // 采样率
                );

                if (microphoneClip == null)
                {
                    ULog.Error("[Microphone] 麦克风启动失败");
                    OnError?.Invoke("麦克风启动失败");
                    return false;
                }

                // 初始化录音状态
                isRecording = true;
                lastSamplePos = 0;
                isBuffering = true;
                recordingBuffer.Clear();
                currentSavePath = null; // 重置保存路径（避免重复使用旧路径）

                // 生成本次录音的保存路径
                GenerateSavePath();

                // 启动音量检测协程
                if (volumeUpdateCoroutine != null)
                {
                    StopCoroutine(volumeUpdateCoroutine);
                }

                volumeUpdateCoroutine = StartCoroutine(UpdateVolumeCoroutine());

                // 通知状态变更
                OnRecordingStateChanged?.Invoke(true);
                OnDeviceChanged?.Invoke(currentDevice);

                ULog.Info($"录音启动成功 | 设备：{currentDevice} | 采样率：{sampleRate}Hz | 时长：{recordLength}s");
                return true;
            }
            catch (Exception e)
            {
                ULog.Error($"[Microphone] 启动录音异常：{e.Message}");
                OnError?.Invoke($"启动录音异常：{e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 停止录音（可选自动保存）
        /// </summary>
        /// <param name="autoSave">是否自动保存录音文件</param>
        /// <returns>是否成功停止录音</returns>
        public bool StopRecording(bool autoSave = false)
        {
            if (!isRecording)
            {
                ULog.Info("[Microphone] 未在进行录音");
                return false;
            }

            try
            {
                // 停止录音（Unity API）
                Microphone.End(currentDevice);
                isRecording = false;
                isBuffering = false;

                // 停止音量检测协程
                if (volumeUpdateCoroutine != null)
                {
                    StopCoroutine(volumeUpdateCoroutine);
                    volumeUpdateCoroutine = null;
                }

                // 状态通知
                OnRecordingStateChanged?.Invoke(false);

                // 自动保存逻辑
                if (autoSave || autoSaveOnStop)
                {
                    return SaveRecordingToFile();
                }

                ULog.Info("[Microphone] 录音已停止（未自动保存）");
                return true;
            }
            catch (Exception e)
            {
                ULog.Error($"[Microphone] 停止录音异常：{e.Message}");
                OnError?.Invoke($"停止录音异常：{e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 保存录音数据到文件（内部调用）
        /// </summary>
        private bool SaveRecordingToFile()
        {
            if (recordingBuffer == null || recordingBuffer.Count == 0)
            {
                OnError?.Invoke("没有录音数据可保存");
                return false;
            }

            try
            {
                // 确定保存路径（自定义路径优先）
                string savePath = currentSavePath ?? GenerateSavePath();
                if (string.IsNullOrEmpty(savePath))
                {
                    ULog.Error("[Microphone] 保存路径生成失败");
                    return false; // 或抛出异常
                }

                // 创建目录（如果不存在）
                string directory = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // 根据扩展名保存文件
                string extension = Path.GetExtension(savePath).ToLower();
                bool success;

                switch (extension)
                {
                    case ".wav":
                        success = SaveAsWAV(savePath);
                        break;
                    default:
                        OnError?.Invoke($"不支持的文件格式：{extension}（仅支持WAV）");
                        return false;
                }

                // 保存成功后清理旧文件
                if (success)
                {
                    OnFileSaved?.Invoke(savePath);
                    ULog.Info($"录音已保存：{savePath}");
                    CleanupOldFiles();
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                OnError?.Invoke($"保存录音文件失败：{e.Message}");
                return false;
            }
        }

        #endregion

        #region 音频数据处理

        /// <summary>
        /// 持续更新音量和缓冲音频数据（协程）
        /// </summary>
        private IEnumerator UpdateVolumeCoroutine()
        {
            // 预分配样本数组（根据录音时长和采样率计算最大可能样本数）
            int maxSampleCount = recordLength * sampleRate;
            float[] samples = new float[maxSampleCount];
            int lastSamplePos = 0;
            int currentPos = 0;

            while (isRecording)
            {
                currentPos = Microphone.GetPosition(currentDevice);
                if (currentPos < 0)
                    continue; // 无效位置跳过

                // 处理循环缓冲区（当位置小于上次位置时说明循环）
                if (currentPos < lastSamplePos)
                {
                    // 计算剩余可读取的样本数（从 lastSamplePos 到数组末尾）
                    int remaining = maxSampleCount - lastSamplePos;
                    if (remaining > 0)
                    {
                        // 创建临时数组存放剩余数据
                        float[] tempSamples = new float[remaining];

                        // 使用正确的两个参数方法读取数据
                        bool success = microphoneClip.GetData(tempSamples, lastSamplePos);

                        if (success)
                        {
                            // 将数据存入缓冲区
                            foreach (var t in tempSamples)
                            {
                                if (recordingBuffer.Count < maxSampleCount)
                                {
                                    recordingBuffer.Add(t);
                                }
                            }
                        }
                    }

                    lastSamplePos = 0; // 重置位置
                }

                // 处理当前位置到数组末尾的数据
                if (currentPos > lastSamplePos)
                {
                    int sampleCount = currentPos - lastSamplePos;
                    int safeCount = Mathf.Min(sampleCount, maxSampleCount - recordingBuffer.Count);

                    if (safeCount > 0)
                    {
                        // 创建临时数组存放当前数据
                        float[] tempSamples = new float[safeCount];

                        // 使用正确的两个参数方法读取数据
                        bool success = microphoneClip.GetData(tempSamples, lastSamplePos);

                        if (success)
                        {
                            // 计算音量（RMS）
                            float sumSquares = 0f;
                            for (int i = 0; i < safeCount; i++)
                            {
                                sumSquares += tempSamples[i] * tempSamples[i];
                                if (recordingBuffer.Count < maxSampleCount)
                                {
                                    recordingBuffer.Add(tempSamples[i]);
                                }
                            }

                            float rms = Mathf.Sqrt(sumSquares / safeCount);
                            float db = 20f * Mathf.Log10(Mathf.Max(rms, 0.00001f));
                            OnVolumeChanged?.Invoke(Mathf.Clamp(db, -80f, 0f));
                        }

                        lastSamplePos = currentPos; // 更新位置
                    }
                }

                yield return null; // 每帧执行一次
            }
        }

        #endregion

        #region 文件操作

        /// <summary>
        /// 生成本次录音的保存路径（带时间戳）
        /// </summary>
        private string GenerateSavePath()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"MIC_{timestamp}.{defaultFileFormat.ToString().ToLower()}";
            // 拼接完整路径（确保目录存在）
            string fullPath = Path.Combine(
                Application.persistentDataPath, // 持久化存储根目录
                saveDirectory, // 自定义子目录（如 "AudioRecordings"）
                fileName // 时间戳文件名（如 "MIC_20240717_120000.wav"）
            );
            currentSavePath = fullPath;
            return fullPath;
        }

        /// <summary>
        /// 保存为WAV格式文件
        /// </summary>
        /// <param name="filePath">目标文件路径</param>
        /// <returns>是否保存成功</returns>
        private bool SaveAsWAV(string filePath)
        {
            try
            {
                // 准备WAV文件头数据
                int sampleCount = recordingBuffer.Count;
                int byteRate = sampleRate * 2; // 16位单声道（2字节/样本）
                int dataSize = sampleCount * 2; // 总数据大小（字节）
                int fileSize = 36 + dataSize; // WAV文件总大小（RIFF头+数据）

                // 创建文件流和写入器
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    // RIFF头（"RIFF"标识）
                    writer.Write(Encoding.ASCII.GetBytes("RIFF"));
                    writer.Write(fileSize); // 文件总大小
                    writer.Write(Encoding.ASCII.GetBytes("WAVE")); // WAVE标识

                    // fmt子块（音频格式信息）
                    writer.Write(Encoding.ASCII.GetBytes("fmt ")); // "fmt "标识
                    writer.Write(16); // fmt子块大小（固定16字节）
                    writer.Write((short)1); // 音频格式（1=PCM）
                    writer.Write((short)1); // 声道数（1=单声道）
                    writer.Write(sampleRate); // 采样率（Hz）
                    writer.Write(byteRate); // 字节率（采样率×每样本字节数）
                    writer.Write((short)2); // 块对齐（声道数×每样本字节数）
                    writer.Write((short)16); // 每样本位数（16位）

                    // data子块（音频数据）
                    writer.Write(Encoding.ASCII.GetBytes("data")); // "data"标识
                    writer.Write(dataSize); // 数据大小（字节）

                    // 写入音频样本数据（16位PCM）
                    foreach (float sample in recordingBuffer)
                    {
                        // 将[-1,1]浮点数转换为16位整数（-32768到32767）
                        short pcmValue = (short)(sample * short.MaxValue);
                        writer.Write(pcmValue);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                ULog.Error($"[Microphone] WAV保存失败：{e.Message}");
                OnError?.Invoke($"WAV保存失败：{e.Message}");
                return false;
            }
        }

        /// <summary>
        /// 清理旧录音文件（保留最近maxSavedFiles个）
        /// </summary>
        private void CleanupOldFiles()
        {
            try
            {
                string dirPath = Path.Combine(Application.persistentDataPath, saveDirectory);
                if (!Directory.Exists(dirPath)) return;

                // 获取所有WAV文件并按修改时间排序
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                FileInfo[] files = dirInfo.GetFiles("*.wav")
                    .OrderByDescending(f => f.LastWriteTime)
                    .ToArray();

                // 删除超过数量的旧文件
                if (files.Length > maxSavedFiles)
                {
                    for (int i = maxSavedFiles; i < files.Length; i++)
                    {
                        files[i].Delete();
                        ULog.Info($"清理旧文件：{files[i].Name}");
                    }
                }
            }
            catch (Exception e)
            {
                ULog.Warning($"[Microphone] 清理旧文件失败：{e.Message}");
            }
        }

        #endregion

        #region 辅助功能

        /// <summary>
        /// 获取当前可用的麦克风设备列表
        /// </summary>
        /// <returns>设备名称数组</returns>
        public string[] GetAvailableDevices()
        {
            return UnityEngine.Microphone.devices;
        }

        /// <summary>
        /// 获取当前录音的实时音量（dB）
        /// </summary>
        /// <returns>当前音量（-80dB到0dB）</returns>
        public float GetCurrentVolume()
        {
            if (!isRecording || recordingBuffer.Count == 0) return -80f;

            // 取最近1024个样本计算音量（平衡实时性和性能）
            int sampleCount = Mathf.Min(1024, recordingBuffer.Count);
            float[] samples = recordingBuffer.GetRange(recordingBuffer.Count - sampleCount, sampleCount).ToArray();

            float sumSquares = 0f;
            foreach (float sample in samples)
            {
                sumSquares += sample * sample;
            }

            float rms = Mathf.Sqrt(sumSquares / sampleCount);
            return 20f * Mathf.Log10(Mathf.Max(rms, 0.00001f)); // 防止对数溢出
        }

        /// <summary>
        /// 获取当前录音的频谱数据（用于可视化）
        /// </summary>
        /// <param name="samples">频谱点数（默认1024）</param>
        /// <returns>频谱数据数组（0-1标准化值）</returns>
        public float[] GetSpectrumData(int samples = 1024)
        {
            if (!isRecording || recordingBuffer.Count == 0)
                return new float[samples];

            // 获取音频数据（取最近的数据）
            int availableSamples = recordingBuffer.Count;
            float[] audioData = recordingBuffer.GetRange(
                Mathf.Max(0, availableSamples - samples * 2),
                Mathf.Min(availableSamples, samples * 2)
            ).ToArray();

            // 执行FFT（这里简化为伪代码，实际需要使用Unity的FFT库或第三方库）
            // 注意：Unity原生不提供FFT，需自行实现或使用插件（如xAudio）
            float[] spectrum = new float[samples];
            // 实际实现需要替换为真实的FFT计算逻辑
            for (int i = 0; i < samples; i++)
            {
                spectrum[i] = Mathf.Sin(i * 0.1f) * 0.5f + 0.5f; // 示例数据
            }

            return spectrum;
        }

        #endregion

        #region 移动端权限处理

        /// <summary>
        /// 检查麦克风权限（移动平台需要）
        /// </summary>
        public void CheckMicrophonePermission()
        {
            // 非移动平台无需检查
#if UNITY_ANDROID || UNITY_IOS
            if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                ULog.Info("[Microphone] 请求麦克风权限");
                StartCoroutine(RequestMicrophonePermission());
            }
#endif
        }

        /// <summary>
        /// 请求麦克风权限（协程）
        /// </summary>
        private IEnumerator RequestMicrophonePermission()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);

            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                ULog.Info("[Microphone] 麦克风权限获取成功");
            }
            else
            {
                ULog.Error("[Microphone] 麦克风权限被拒绝");
                OnError?.Invoke("麦克风权限被拒绝，请在设置中开启");
            }
        }

        #endregion
    }

    /// <summary>
    /// 音频文件格式枚举
    /// </summary>
    public enum AudioFileFormat
    {
        WAV, // 波形音频（无损）
        MP3, // 压缩音频（有损）
        OGG // 开源压缩格式（有损）
    }
}
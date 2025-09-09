using System;
using System.Collections;
using System.Collections.Generic;
using Hotfix.Common;
using Sirenix.OdinInspector;
using Xicheng.Resource;
using UnityEngine;
using Xicheng.Utility;

/*
 * 1.集中控制
     所有音效与背景音乐
   2.资源分组
   （BGM / UI / SFX / 语音）分类管理
   3.可配置性强
    （支持脚本 + 配置表）
    4.支持复用
    （音效池化）
    5.支持淡入淡出、Loop、随机播放等效果
    6.全局音量调节与静音控制
    7.生命周期明确(跨场景播放、自动清理）

   模块划分
    AudioManager : 总控入口，对外提供接口
    AudioChannel : 对应一类声音的管理（如SFX）
    AudioObject / Pool : 播放器对象 + 对象池支持
    AudioConfigTable : 可配置资源表
    AudioSettingsData : 音量开关/偏好存储

音频分组建议：
    BGM-背景音乐
    SFX-游戏内反馈音效
    UI-按钮、界面音效
    Voice-语音播报
    Ambient-环境音

Unity设置建议：
   1.音效不宜过多并发播放（移动平台上限约32个）
   2.短音效（如技能音效、环境互动音效）：设置为「Load In Memory」，减少IO 开销，提升播放响应速度。
   3.长音频（如 BGM、环境音）：设置为「Streaming」（流式加载），即播放时边加载边解码，避免一次性占用大量内存（例如 5 分钟的.ogg BGM 仅需 3-5MB，流式加载时内存占用仅几 KB）。
推荐ogg格式：
   无专利限制（开源），移动端兼容性极佳（覆盖 99% 以上的 Android/iOS 设备），是平衡 “音质、体积、性能” 的最优解。
   ogg 解码速度快，CPU 开销低，且对多声道（如 3D 音效、环绕声）支持优秀。
 
   对比维度	    .wav（Waveform Audio File Format）	.ogg（Ogg Vorbis）	                    .mp3（MPEG-1 Audio Layer 3）
   压缩方式	    无压缩（无损）	                    有损 / 无损（默认有损，开源算法）	        有损压缩（专利算法，2022 年专利到期）
   音质表现	    原始音质，无失真（还原录制细节）	        相同体积下音质优于.mp3，失真低	            中高码率下音质较好，低码率易失真
   文件体积	    极大（如44.1kHz/16bit立体声，1分钟约10MB）较小（同音质下比.mp3 小 20%-50%）	中等（比.wav 小 10-12 倍，比.ogg 大）
   解码效率	    无需解码，加载后直接播放（CPU 开销低）   解码速度快，CPU 开销中等	                解码速度较慢，CPU 开销略高于.ogg
   Unity支持	    完全支持                             支持所有加载模式	                        支持，但需注意：移动端部分设备可能有兼容性问题（如旧 Android 机型）
   多声道支持	    支持立体声、5.1 环绕声等	            支持多声道，压缩效率不衰减	                主要支持立体声，多声道压缩效果差
   元数据支持	    支持（如采样率、位深），但功能简单	    支持丰富元数据（如循环标记、章节）	        支持基础元数据，但扩展性弱
 
 */

namespace Xicheng.Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private readonly Dictionary<AudioType, AudioChannel> _agent = new();
        // 音频缓存字典，用于存储已加载的音频资源
        private readonly Dictionary<string, AudioClip> _audioCache = new();
        private AudioListener _audioListener;
        
        private void Awake()
        {
            _audioListener = gameObject.AddComponent<AudioListener>();
            InitChannels();
        }

        private void InitChannels()
        {
            foreach (AudioType audioType in Enum.GetValues(typeof(AudioType)))
            {
                GameObject go = new GameObject($"AudioChannel_{audioType}")
                {
                    transform =
                    {
                        parent = transform
                    }
                };
                _agent[audioType] = go.AddComponent<AudioChannel>();
            }
        }

        /// <summary>
        /// 播放音效。
        /// </summary>
        /// <param name="type">音效类型</param>
        /// <param name="clipName">音频名称，要带后缀</param>
        /// <param name="loop">循环</param>
        /// <param name="volume">音量</param>
        public void Play(AudioType type, string clipName, bool loop = false, float volume = 1f)
        {
            string location = AssetPath.GetAudio(clipName);
            // 1. 先从缓存中查找
            if (_audioCache.TryGetValue(location, out AudioClip cachedClip))
            {
                _agent[type].Play(cachedClip, loop, volume);
                return;
            }

            // 2. 缓存中没有，开始加载
            var audioClip = Res.LoadAsset<AudioClip>(location);
            _audioCache[clipName] = audioClip; // 存入缓存
            _agent[type].Play(audioClip, loop, volume); // 播放音频
        }

        public void Stop(AudioType type)
        {
            _agent[type].Stop();
        }

        public void SetVolume(AudioType type, float volume)
        {
            _agent[type].SetVolume(volume);
        }
        

        //淡入淡出
        public IEnumerator FadeIn(AudioSource source, float duration)
        {
            source.volume = 0;
            source.Play();
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                source.volume = Mathf.Lerp(0, 1, timer / duration);
                yield return null;
            }
        }

        //随机播放：
        public void PlayRandom(AudioType type, string[] clipNames)
        {
            int index = UnityEngine.Random.Range(0, clipNames.Length);
            Play(type, clipNames[index]);
        }

        /// <summary>
        /// 清理单个音频缓存
        /// </summary>
        /// <param name="clipName"></param>
        public void ClearAudioClip(string clipName)
        {
            string location = AssetPath.GetAudio(clipName);
            if (_audioCache.TryGetValue(location, out AudioClip cachedClip))
            {
                Res.UnloadAsset(cachedClip);
                _audioCache.Remove(location);
            }
        }

        
        /// <summary>
        /// 完整静音，包括系统级音量
        /// </summary>
        public void MuteAll(bool mute)
        {
            _audioListener.enabled = !mute;
        }

        
        /// <summary>
        /// 淡入播放背景音乐
        /// </summary>
        /// <param name="location">音频资源路径</param>
        /// <param name="duration">淡入时长</param>
        // public async UniTask<AudioAgent> FadeInBackground(string location, float duration = 2f)
        // {
        //     var agent = await PlayBackground(location);
        //     if (agent != null)
        //         await agent.FadeIn(duration);
        //     return agent;
        // }
        
        
        /// <summary>
        /// 清理所有缓存的音频
        /// </summary>
        public void ClearCache()
        {
            foreach (var audioClip in _audioCache.Values)
            {
                Res.UnloadAsset(audioClip);
            }
            _audioCache.Clear();
        }

        private void OnDestroy()
        {
            ClearCache();
        }

        [Button]
        public void PlayBtnClick()
        {
            Play(AudioType.UI, "btnClick.wav");
        }
    }
}
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

平台适配与优化建议：
    音效不宜过多并发播放（移动平台上限约32个）
    资源压缩建议使用.ogg 或 .mp3，设置为Streaming加载
    小音效使用LoadInMemory，减少 IO 开销
    大体积BGM使用 AudioClip.LoadAudioData异步加载


 */

namespace Xicheng.Audio
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private readonly Dictionary<AudioType, AudioChannel> _agent = new();
        // 音频缓存字典，用于存储已加载的音频资源
        private readonly Dictionary<string, AudioClip> _audioCache = new();
        
        public bool isMute = false;  // 全局静音
        private void Awake()
        {
            AudioListener.volume = isMute ? 0 : 1;
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
        /// <param name="type"></param>
        /// <param name="clipName">音频名称，要带后缀</param>
        /// <param name="loop"></param>
        /// <param name="volume"></param>
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

        [Button]
        public void PlayBtnClick()
        {
            Play(AudioType.UI, "btnClick.wav");
        }
    }
}
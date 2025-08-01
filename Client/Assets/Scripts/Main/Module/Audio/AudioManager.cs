using System;
using System.Collections;
using System.Collections.Generic;
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
    AudioManager
    总控入口，对外提供接口
    AudioChannel
    对应一类声音的管理（如SFX）
    AudioObject / Pool
    播放器对象 + 对象池支持
    AudioConfigTable
    可配置资源表
    AudioSettingsData
    音量开关/偏好存储

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

namespace Xicheng.log.Audio
{
    public class AudioManager:MonoSingleton<AudioManager>
    {
        private Dictionary<AudioGroup, AudioChannel> channels =new();

        private void Awake()
        {
            // AudioListener.volume = isMute ? 0 : 1; //全局静音
            InitChannels();
        }

        private void InitChannels()
        {
            foreach (AudioGroup group in Enum.GetValues(typeof(AudioGroup)))
            {
                GameObject go = new GameObject($"AudioChannel_{group}");
                go.transform.parent = transform;
                channels[group] = go.AddComponent<AudioChannel>();
            }
        }

        public void Play(AudioGroup group, string clipName, bool loop = false, float volume = 1f)
        {
            //TODO:获取clip
            AudioClip clip = null;
            //AudioClip clip = AudioConfigTable.GetClipByName(clipName);
            channels[group].Play(clip, loop, volume);
        }

        public void Stop(AudioGroup group)
        {
            channels[group].Stop();
        }

        public void SetVolume(AudioGroup group, float volume)
        {
            channels[group].SetVolume(volume);
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
        public void PlayRandom(AudioGroup group, string[] clipNames) 
        {
            int index = UnityEngine.Random.Range(0, clipNames.Length);
            Play(group, clipNames[index]);
        }
    }
}
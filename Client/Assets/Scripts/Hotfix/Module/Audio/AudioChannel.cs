using UnityEngine;

namespace Xicheng.Audio
{
    /// <summary>
    /// 每类声音的播放管理器(对应一类声音的管理（如SFX）)
    /// </summary>
    public class AudioChannel : MonoBehaviour 
    {
        private AudioSource _source;

        void Awake()
        {
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;
        }

        public void Play(AudioClip clip, bool loop, float volume) 
        {
            _source.clip = clip;
            _source.loop = loop;
            _source.volume = volume;
            _source.Play();
        }

        public void Stop() => _source.Stop();

        public void SetVolume(float vol) => _source.volume = vol;
    }
}
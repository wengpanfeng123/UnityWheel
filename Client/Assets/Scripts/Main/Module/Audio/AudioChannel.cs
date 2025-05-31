using UnityEngine;

namespace Main.Module.Audio
{
    /// <summary>
    /// 每类声音的播放管理器(对应一类声音的管理（如SFX）)
    /// </summary>
    public class AudioChannel : MonoBehaviour 
    {
        private AudioSource source;

        void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
        }

        public void Play(AudioClip clip, bool loop, float volume) 
        {
            source.clip = clip;
            source.loop = loop;
            source.volume = volume;
            source.Play();
        }

        public void Stop() => source.Stop();

        public void SetVolume(float vol) => source.volume = vol;
    }
}
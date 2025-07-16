using System.Collections;
using UnityEngine;

namespace xicheng.log.Audio
{
    /* AudioObject池化播放（适合爆发型音效）
     * UI按钮点击、连击命中等 SFX 不能都用一个 AudioSource。
     */
    public class AudioObject : MonoBehaviour 
    {
        private AudioSource source;

        public void Init(AudioClip clip, float volume = 1f) 
        {
            source = GetComponent<AudioSource>();
            source.clip = clip;
            source.volume = volume;
            source.loop = false;
            source.Play();
            StartCoroutine(AutoRecycle(clip.length));
        }

        IEnumerator AutoRecycle(float delay) 
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Xicheng.Audio;
using AudioType = Xicheng.Audio.AudioType;

namespace HsJam
{
    public class ButtonAudioListener : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Header("是否启用点击音效")] public bool isActive = true;
        
        [SerializeField]private string clickAudioName = "btn_click";
        
        private bool _isActive = true;
        //安全校验 
        private void Reset()
        {
            if (!GetComponent<Button>())
            {
                Debug.LogError("这个物体上没有button组件！！！");
                DestroyImmediate(this);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isActive = isActive;
            if (!_isActive)
            {
                return;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isActive)
            {
                return;
            }
            ULog.Info($"{gameObject.name}--Click--{clickAudioName}");
            AudioManager.Inst.Play(AudioType.UI,"btnClick.wav");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_isActive)
            {
                return;
            }
        }

        /// <summary>
        /// 自动加的
        /// </summary>
        private void AutoAddButtonAudioListener()
        {
            foreach (Button b in FindObjectsOfType<Button>())
            {
                if (b.GetComponent<ButtonAudioListener>())
                {
                    Debug.Log("不用加");
                }
                else
                {
                    b.gameObject.AddComponent<ButtonAudioListener>();
                }
            }
        }
    }
}
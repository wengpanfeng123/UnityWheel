using UnityEngine;
using UnityEngine.UI;

namespace HsJam
{
    [ExecuteAlways]
    public class RenderReference : MonoBehaviour
    {
        public Material referenceMaterial;
        private Vector4 m_Diffuse_ST;
        public Vector4 _Diffuse_ST;
        public float m_Intensity;
        public float _Intensity;
        public Vector4 m_Mask_ST;
        public Vector4 _Mask_ST;
        public float m_Vspeed;
        public float _Vspeed;
        public float m_Uspeed;
        public float _Uspeed;
        public Color m_Color;
        public Color _Color;

        void Start()
        {
            referenceMaterial = GetComponent<Image>().material;
            m_Diffuse_ST = referenceMaterial.GetVector("_Diffuse_ST");
            m_Mask_ST = referenceMaterial.GetVector("_Mask_ST");
            m_Intensity = referenceMaterial.GetFloat("_Intensity");
            m_Vspeed = referenceMaterial.GetFloat("_Vspeed");
            m_Uspeed = referenceMaterial.GetFloat("_Uspeed");
            m_Color = referenceMaterial.GetColor("_Color");
            _Diffuse_ST = m_Diffuse_ST;
            _Mask_ST = m_Mask_ST;
            _Intensity = m_Intensity;
            _Vspeed = m_Vspeed;
            _Uspeed = m_Uspeed;
            _Color = m_Color;
        }


        void Update()
        {
            if (m_Diffuse_ST != _Diffuse_ST)
            {
                referenceMaterial.SetVector("_Diffuse_ST", _Diffuse_ST);
                m_Diffuse_ST = _Diffuse_ST;
            }
            if (m_Mask_ST != _Mask_ST)
            {
                referenceMaterial.SetVector("_Mask_ST", _Mask_ST);
                m_Mask_ST = _Mask_ST;
            }
            if (m_Intensity != _Intensity)
            {
                referenceMaterial.SetFloat("_Intensity", _Intensity);
                m_Intensity = _Intensity;
            }
            if (m_Vspeed != _Vspeed)
            {
                referenceMaterial.SetFloat("_Vspeed", _Vspeed);
                m_Vspeed = _Vspeed;
            }
            if (m_Uspeed != _Uspeed)
            {
                referenceMaterial.SetFloat("_Uspeed", _Uspeed);
                m_Uspeed = _Uspeed;
            }
            if (m_Color != _Color)
            {
                referenceMaterial.SetColor("_Color", _Color);
                m_Color = _Color;
            }
        }
    }
}
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StealthGame
{
    public class LightFlickerCustom : MonoBehaviour
    {
        public enum FlickerMode
        {
            Random,
            AnimationCurve
        }
    
        public Light flickeringLight;
        public Renderer flickeringRenderer;
        public FlickerMode flickerMode;
        public float lightIntensityMin = 1.25f;
        public float lightIntensityMax = 2.25f;
        public float flickerDurationMin = 1.0f;
        public float flickerDurationMax = 3.0f;
        public AnimationCurve intensityCurve;

        Material m_FlickeringMaterial;
        Color m_EmissionColor;
        float m_Timer;
        float m_FlickerLightIntensity;
        float m_FlickerDuration;

        static readonly int k_EmissionColorID = Shader.PropertyToID (k_EmissiveColorName);
    
        const string k_EmissiveColorName = "_EmissionColor";
        const string k_EmissionName = "_Emission";
        const float k_LightIntensityToEmission = 2f / 3f;

        void Start()
        {
            m_FlickeringMaterial = flickeringRenderer.material;
            m_FlickeringMaterial.EnableKeyword(k_EmissionName);
            m_EmissionColor = m_FlickeringMaterial.GetColor(k_EmissionColorID);

            ChangeRandomFlickerLightIntensity();
        }

        void Update()
        {
            m_Timer += Time.deltaTime;

            if (flickerMode == FlickerMode.Random)
            {
                if (m_Timer >= m_FlickerDuration)
                {
                    ChangeRandomFlickerLightIntensity ();
                }
            }
            else if(flickerMode == FlickerMode.AnimationCurve)
            {
                ChangeAnimatedFlickerLightIntensity ();
            }
            float ratio = m_Timer / m_FlickerDuration;
            flickeringLight.intensity = Mathf.Lerp(flickeringLight.intensity, m_FlickerLightIntensity, ratio);
            m_FlickeringMaterial.SetColor (k_EmissionColorID, m_EmissionColor * m_FlickerLightIntensity * k_LightIntensityToEmission);
        }

        void ChangeRandomFlickerLightIntensity ()
        {
            m_FlickerLightIntensity = Random.Range(lightIntensityMin, lightIntensityMax);
            m_FlickerDuration =  Random.Range(flickerDurationMin, flickerDurationMax);

            m_Timer = 0f;

        }

        void ChangeAnimatedFlickerLightIntensity ()
        {
            m_FlickerLightIntensity = intensityCurve.Evaluate (m_Timer);

            if (m_Timer >= intensityCurve[intensityCurve.length - 1].time)
                m_Timer = intensityCurve[0].time;
        }
    }
}

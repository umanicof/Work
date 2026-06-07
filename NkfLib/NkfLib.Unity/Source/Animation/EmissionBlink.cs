using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace NkfLib.Unity
{
    public class EmissionBlink : MonoBehaviour
    {
        public bool EnabledBlink { get; set; }
        Color _blinkColor;
        public Color BlinkColor
        {
            get { return _blinkColor; }
            set
            {
                _blinkColor = value;
                _renderer.material.SetColor("_EmissionColor", _blinkColor);
            }
        }

        float MaxIntensity = 2.0f;
        float MinIntensity = 0.0f;
        Renderer _renderer;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _renderer.material.EnableKeyword("_EMISSION"); // キーワードの有効化
            BlinkColor = _renderer.material.GetColor("_EmissionColor");
        }

        void Update()
        {
            if (!EnabledBlink) return;

            float sin = (Mathf.Sin(Time.time * 3.0f) + 1.0f) / 2.0f; // 0.0～1.0f
            float intensity = Mathf.Lerp(MinIntensity, MaxIntensity, sin);
            float factor = Mathf.Pow(2, intensity);
            _renderer.material.SetColor("_EmissionColor", new Color(BlinkColor.r * factor, BlinkColor.g * factor, BlinkColor.b * factor));
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NkfLib.Unity
{
    public class SoundSlider : MonoBehaviour
    {
        [SerializeField]
        Slider _slider;

        [SerializeField]
        float _defaultValue = 0.5f;

        float _maxVolume;

        static float _globalVolume = -1.0f; // ƒŠƒ[ƒh‚Ì‘Îô‚É—pˆÓ

        void Start()
        {
            if (_globalVolume < 0.0f)
            {
                _globalVolume = AudioListener.volume;
            }
 
            _maxVolume = _globalVolume / _defaultValue;
            _slider.value = AudioListener.volume / _maxVolume;
        }

        public void SetValue(float value)
        {
            AudioListener.volume = value * _maxVolume;
        }
    }
}
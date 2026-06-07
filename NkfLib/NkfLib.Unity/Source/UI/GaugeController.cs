using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace NkfLib.Unity
{
    public class GaugeController : MonoBehaviour
    {
        Slider _slider;
        Slider Slider
        {
            get
            {
                if (_slider == null)
                {
                    _slider = GetComponent<Slider>();
                }
                return _slider;
            }
        }

        public float MaxValue { get; private set; }
        public float Value { get; private set; }

        public void SetMaxProperty(ReactiveProperty<float> property)
        {
            property.Subscribe(x =>
            {
                MaxValue = x;
                Refresh();
            });
        }

        public void SetValueProperty(ReactiveProperty<float> property)
        {
            property.Subscribe(x =>
            {
                Value = x;
                Refresh();
            });
        }

        void Refresh()
        {
            if (MaxValue > 0.0f)
            {
                Slider.value = Value / MaxValue;
            }
        }
    }
}
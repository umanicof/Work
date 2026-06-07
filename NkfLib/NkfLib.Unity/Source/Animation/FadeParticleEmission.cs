using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    /// <summary>
    /// パーティクルの生成のフェードインアウト
    /// ・PerticleSystem.Playも制御している
    /// ・もう少しきれいな制御にしたいが保留
    /// </summary>
    public class FadeParticleEmission : MonoBehaviour
    {
        public float FadeRateOverTimeConstant = 1.0f;

        public ParticleSystem Particle { get; private set; }

        float _maxConstant;

        enum State
        {
            Idle,
            FadeIn,
            FadeOut,
        };
        State _state;

        public bool IsFadeIn { get { return _state == State.FadeIn; } }
        public bool IsFadeOut { get { return _state == State.FadeOut; } }
        public bool IsIdle { get { return _state == State.Idle; } }
        public bool IsMaxFade { get { return Particle.emission.rateOverTime.constant == _maxConstant; } }
        public bool IsMinFade { get { return Particle.emission.rateOverTime.constant == 0; } }

        // Start is called before the first frame update
        void Start()
        {
            Particle = GetComponent<ParticleSystem>();
            _maxConstant = Particle.emission.rateOverTime.constant;
            var e = Particle.emission;
            e.rateOverTime = new ParticleSystem.MinMaxCurve(0); // constantの更新
        }

        // Update is called once per frame
        void Update()
        {
            if (_state == State.Idle) return;

            var e = Particle.emission;
            if (_state == State.FadeIn)
            {
                if (!Particle.isPlaying)
                {
                    Particle.Play();
                }
                var constant = e.rateOverTime.constant + FadeRateOverTimeConstant * Time.deltaTime;
                if (constant >= _maxConstant)
                {
                    constant = _maxConstant;
                    _state = State.Idle;
                }
                e.rateOverTime = new ParticleSystem.MinMaxCurve(constant); // constantの更新
            }
            else
            {
                var constant = e.rateOverTime.constant - FadeRateOverTimeConstant * Time.deltaTime;
                if (constant <= 0)
                {
                    constant = 0;
                    _state = State.Idle;
                }
                e.rateOverTime = new ParticleSystem.MinMaxCurve(constant); // constantの更新
            }
        }

        public void FadeIn()
        {
            if (IsMaxFade) return;
            _state = State.FadeIn;
        }

        public void FadeOut()
        {
            if (IsMinFade) return;
            _state = State.FadeOut;
        }
    }
}
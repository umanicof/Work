using System;
using UnityEngine;

namespace NkfLib.Unity
{
    // 出典不明
    public class ParticleEmittionAudio : MonoBehaviour
    {
        public AudioClip Sound;

        // ParticleSystem.emission.rateOverTime.constant の変化によるサウンド調整の設定
        public float MinPitch = 0.4f;
        public float MaxPitch = 2.0f;

        [Serializable]
        public class AdvancedSetttings // A class for storing the advanced options.
        {
            public float MinDistance = 50f;    // The min distance of the engine audio source.
            public float MaxDistance = 1000f;  // The max distance of the engine audio source.
            public float DopplerLevel = 1f;    // The doppler level of the engine audio source.
            [Range(0f, 1f)] public float MasterVolume = 0.5f;
        };
        public AdvancedSetttings Advanced = new AdvancedSetttings();// container to make advanced settings appear as rollout in inspector


        AudioSource _soundSource;  // Reference to the AudioSource for the engine.
        ParticleSystem _particle;
        float _maxConstant;

        void Start()
        {
            _particle = GetComponent<ParticleSystem>();
            _maxConstant = _particle.emission.rateOverTime.constant;

            // オーディオ初期化
            _soundSource = gameObject.AddComponent<AudioSource>();
            _soundSource.playOnAwake = false;
            _soundSource.clip = Sound;
            _soundSource.minDistance = Advanced.MinDistance;
            _soundSource.maxDistance = Advanced.MaxDistance;
            _soundSource.loop = true;
            _soundSource.dopplerLevel = Advanced.DopplerLevel;

            _soundSource.volume = 0.0f;
            _soundSource.Play();
        }

        void Update()
        {
            // Find what proportion of the engine's power is being used.
            var proportion = Mathf.InverseLerp(0, _maxConstant, _particle.emission.rateOverTime.constant);

            // Set the engine's pitch to be proportional to the engine's current power.
            _soundSource.pitch = Mathf.Lerp(MinPitch, MaxPitch, proportion);

            // Increase the engine's pitch by an amount proportional to the aeroplane's forward speed.
            // (this makes the pitch increase when going into a dive!)
            //_soundSource.pitch += m_Plane.ForwardSpeed*Multiplier;

            // Set the engine's volume to be proportional to the engine's current power.
            _soundSource.volume = Mathf.InverseLerp(0, _maxConstant, _particle.emission.rateOverTime.constant) * Advanced.MasterVolume;
        }
    }
}
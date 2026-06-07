using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace NkfLib.Unity
{
    // ■使用例
    //   TimerHelper _timerHelper;
    //   void Awake()
    //   {
    //       _timerHelper = gameObject.AddComponent<TimerHelper>();
    //       _timerHelper.TargetTime = kIntarval;
    //       _timerHelper.CurrentTime = Random.value * kIntarval;
    //   }
    public class TimerHelper : MonoBehaviour
    {
        public bool IsAutoUpdate { get; set; } = true; // タイマ自動更新
        public bool IsLoop { get; set; }
        public float CurrentTime { get; set; } // 現在時間
        public float TargetTime { get; set; }  // 目標時間

        public event Action passed;         // 目標時間経過イベント
        bool _invoked;
        bool _isPassed;
        public bool IsPassed  // 目標時間経過
        {
            get { return _isPassed; }
            set
            {
                _isPassed = value;
                _invoked = value;
            }
        }

        /// <summary>
        /// リセット
        /// </summary>
        public void Reset()
        {
            CurrentTime = 0.0f;
            IsPassed = false;
        }

        /// <summary>
        /// タイマ更新
        /// </summary>
        public void UpdateTimer()
        {
            CurrentTime += Time.deltaTime;
            while (CurrentTime >= TargetTime)
            {
                bool tmp = _invoked;
                IsPassed = true;
                if (!tmp)
                {
                    passed?.Invoke();
                }
                if (!IsLoop || TargetTime <= 0.0f) break;
                CurrentTime -= TargetTime;
                _invoked = false;
            }
        }

        void Update()
        {
            if (!IsAutoUpdate) return;
            UpdateTimer();
        }
    }
}
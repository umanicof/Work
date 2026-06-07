using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace NkfLib.Unity
{
    // 毎フレーム処理を呼び出しつつタイムアウトを得ることが出来るタイマ
    // ・ワンショット
    // ・精度は大雑把
    public class UpdateTimer
    {
        public bool IsRunning { get; private set; }
        public float TimeoutSeconds { get; set; } = -1.0f; // 負の値であればタイムアウト無し
        public float PassedSeconds { get; private set; }
        public float RemainingSeconds 
        {
            get
            {
                if (TimeoutSeconds < 0)
                {
                    return 0f;
                }
                return Mathf.Max(TimeoutSeconds - PassedSeconds, 0.0f); 
            }
        }
        public bool IsPaused { get; private set; }
        public bool UseFixedUpdate { get; set; }

        public IObservable<(float deltaSeconds, float passedSeconds)> OnUpdate { get { return _onUpdate; } }
        public IObservable<(float deltaSeconds, float passedSeconds)> OnFinish { get { return _onFinish; } }

        Subject<(float deltaSeconds, float passedSeconds)> _onUpdate = new Subject<(float, float)>();
        Subject<(float deltaSeconds, float passedSeconds)> _onFinish = new Subject<(float, float)>();
        IDisposable _onUpdateDisposable;

        public void StartTimer()
        {
            StartTimer(TimeoutSeconds);
        }

        public void StartTimer(float timeoutSeconds)
        {
            StopTimer();
            IsRunning = true;
            PassedSeconds = 0.0f;
            TimeoutSeconds = timeoutSeconds;
            var o = UseFixedUpdate ? Observable.EveryFixedUpdate() : Observable.EveryUpdate();
            _onUpdateDisposable = o.Subscribe(_ =>
            {
                if (IsPaused)
                {
                    return;
                }
                var compareSeconds = TimeoutSeconds < 0.0f ? float.MaxValue : TimeoutSeconds;
                PassedSeconds = Mathf.Min(PassedSeconds + Time.deltaTime, compareSeconds);
                _onUpdate.OnNext((Time.deltaTime, PassedSeconds));
                if (PassedSeconds >= compareSeconds)
                {
                    StopTimer(true);
                }
            });
        }

        public async UniTask StartTimerAsync()
        {
            await StartTimerAsync(TimeoutSeconds);
        }

        public async UniTask StartTimerAsync(float timeoutSeconds)
        {
            StartTimer(timeoutSeconds);
            await UniTask.WaitUntil(() => !IsRunning);
        }

        public void StopTimer(bool isFinish = false)
        {
            _onUpdateDisposable?.Dispose();
            _onUpdateDisposable = null;
            IsRunning = false;

            if (isFinish)
            {
                // この呼び出しで再度StartTimerが呼ばれる可能性に注意
                _onFinish.OnNext((Time.deltaTime, PassedSeconds));
            }
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }
    }
}

using DG.Tweening;
using DG.Tweening.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NkfLib.Unity
{
    // DOTween拡張
    // ・DOTweenのAutoPlayやポーズの制御や使い回しで困ったので用意。DOTweenは使い回され、本クラスの破棄と共にKillされる。
    // ・DOTWeenのようにネストで呼び出しができるようにしている（役立つかは不明）
    // ・Pause/Resumeは本クラスの挙動を止める意味であり、アニメーションのPause/Resumeとは微妙に意味が異なる
    public class DOTweenEx<T> where T : Tween
    {
        T _instance;

        bool _isPlayOrPause;

        bool _pauseMode;

        public bool IsValid() => _instance?.IsActive() ?? false;
        public bool IsPlaying() => IsValid() && _isPlayOrPause;
        public bool IsPauseMode() => _pauseMode;


        // コンストラクタ
        public DOTweenEx()
        {
        }
        public DOTweenEx(T instance)
        {
            Set(instance);
        }

        // ファイナライザ
        ~DOTweenEx()
        {
            Kill();
        }

        public T Get()
        {
            return _instance;
        }

        public DOTweenEx<T> Set(T instance)
        {
            _instance?.Kill();

            // 初期化
            _isPlayOrPause = false;
            _pauseMode = false;

            _instance = instance;
            _instance?.SetAutoKill(false); // 破棄を無効
            _instance?.Pause(); // 生成直後はポーズで運用する（AutoPlay対策）
            _instance?.OnComplete(() => 
            { 
                Stop();
            });
            return this;
        }

        // キャスト
        public static implicit operator T(DOTweenEx<T> obj) => obj._instance;
        public static implicit operator DOTweenEx<T>(T obj) => new DOTweenEx<T>(obj);
        public DOTweenEx<T> Reset()
        {
            _instance?.Pause();
            _instance?.Goto(0.0f);
            return this;
        }

        public DOTweenEx<T> Play()
        {
            Debug.Assert(IsValid());

            if (_pauseMode)
            {
                _instance?.Pause();
            }
            else
            {
                _instance?.Play();
            }
            _isPlayOrPause = true;
            return this;
        }

        public DOTweenEx<T> Pause()
        {
            if (_isPlayOrPause)
            {
                _instance?.Pause();
            }
            _pauseMode = true;
            return this;
        }

        public DOTweenEx<T> Resume()
        {
            if (!_pauseMode)
            {
                return this;
            }
            if (_isPlayOrPause)
            {
                _instance?.Play();
            }
            _pauseMode = false;
            return this;
        }

        public DOTweenEx<T> Stop()
        {
            Debug.Assert(IsValid());

            _isPlayOrPause = false;
            _instance?.Pause();
            return this;
        }

        // どうしてもKillしたいのなら
        public DOTweenEx<T> Kill()
        {
            _isPlayOrPause = false;
            _instance?.Kill();
            _instance = null;
            return this;
        }
    }
}

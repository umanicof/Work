using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public static class AnimatorExtensions
{
    /// <summary>
    /// カレントのアニメーションかどうか
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateHash"></param>
    /// <returns></returns>
    public static bool IsCurrentState(this Animator self, int stateHash)
    {
        if (!self)
        {
            return true;
        }
        return self.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash;
    }
    public static bool IsCurrentState(this Animator self, string stateName)
    {
        return self?.IsCurrentState(Animator.StringToHash(stateName)) ?? true;
    }

    /// <summary>
    /// アニメーション再生（ステート）
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateHash"></param>
    /// <returns></returns>
    public static async UniTask PlayStateAsync(this Animator self, int stateHash)
    {
        if (!self)
        {
            return;
        }

        self.Play(stateHash);
        while (!self.IsCurrentState(stateHash))
        {
            await UniTask.Yield();
        }
    }
    public static async UniTask PlayStateAsync(this Animator self, string stateName)
    {
        if (!self)
        {
            return;
        }
        await self.PlayStateAsync(Animator.StringToHash(stateName));
    }

    /// <summary>
    /// Boolパラメータセット（自動リセット有り）
    /// ※遷移に使用する場合はリセット時間は長めに取ること
    /// 　0.1秒程度ではアニメーションが遷移しないままOFFになってしまう場合がある。最低でも１フレーム分は取った方が良いだろう。
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="parameterHash"></param>
    /// <returns></returns>
    public static void SetBool(this Animator self, int parameterHash, bool value, float autoResetSeconds = 0.0f)
    {
        self?.SetBool(parameterHash, value);
        if (autoResetSeconds > 0.0f)
        {
            Observable.Timer(TimeSpan.FromSeconds(autoResetSeconds)).Subscribe(_ =>
            {
                self?.SetBool(parameterHash, !value);
            });
        }
    }
    public static void SetBool(this Animator self, string parameter, bool value, float autoResetSeconds = 0.0f)
    {
        self?.SetBool(Animator.StringToHash(parameter), value, autoResetSeconds);
    }
}

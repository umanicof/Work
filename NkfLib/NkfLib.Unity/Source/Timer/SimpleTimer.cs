using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Audio.GeneratorInstance;

// 簡易タイマ
// ・少し仕様を変えている。以前はUpdateで時間経過後はtrueが返り続けていたが、現状は１度のみとなる
// ・SetupSecondsが0の場合はタイマは自動停止しない
public class SimpleTimer
{
    struct SetupData
    {
        public float seconds;
        public bool one_shot;
        public bool auto_start;
    };
    SetupData _setupData = new SetupData();

    public float SetupSeconds { get { return _setupData.seconds; } set { _setupData.seconds = value; } }
    public float Seconds { get; private set; }

    bool _isFinished = true;

    public bool IsFinished() { return _isFinished; }

    public bool IsRunning() { return !IsFinished(); }

    public SimpleTimer()
    {
    }

    public SimpleTimer(float seconds, bool one_shot = true, bool auto_start = true)
    {
        Setup(seconds, one_shot, auto_start);
    }

    public void Setup(float seconds, bool one_shot = true, bool auto_start = true)
    {
        _setupData.seconds = seconds;
        _setupData.one_shot = one_shot;
        _setupData.auto_start = auto_start;
        Reset();
    }

    public void Reset()
    {
        Seconds = 0.0f;
        _isFinished = !_setupData.auto_start;
    }

    public void Start(bool with_reset)
    {
        if (with_reset)
        {
            Reset();
        }
        _isFinished = false;
    }

    public bool Update(float deltaTime, Action reachedFunc = null)
    {
        if (_isFinished)
        {
            return false;
        }
        Seconds += deltaTime;
        if (SetupSeconds <= 0.0f || Seconds < SetupSeconds)
        {
            return false;
        }

        if (_setupData.one_shot)
        {
            Seconds = SetupSeconds;
            _isFinished = true;
        }
        else
        {
            Seconds -= SetupSeconds;
        }

        reachedFunc?.Invoke();
        return true;
    }
}

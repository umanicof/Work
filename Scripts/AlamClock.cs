using NUnit.Framework;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class AlamClock : MonoBehaviour
{
    public AudioSource _alarm;

    public bool IsBreak;

    public ReactiveProperty<bool> IsAlarmOn { get; } = new ReactiveProperty<bool>(false);

    TimeSpan _alarmHour; 

    void Start()
    {
        
    }

    void Update()
    {
        // 鳴りだしてから10分経過
        if (IsAlarmOn.Value && _alarmHour.Add(TimeSpan.FromMinutes(10.0f)) <  GameMain.Instance.CurrentHour)
        {
            SetOn(false);
        }

    }
    private void OnMouseDown()
    {
        if (IsBreak)
        {
            return;
        }
        SetOn(!IsAlarmOn.Value);
    }

    public void SetOn(bool value)
    {
        IsAlarmOn.Value = value;
        if (value)
        {
            _alarm.Play();
            _alarmHour = GameMain.Instance.CurrentHour;
        }
        else
        {
            _alarm.Stop();
        }

    }

    private void OnMouseEnter()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        if (IsAlarmOn.Value)
        {
            GameUI.Instance.ShowMessageUI("アラームが鳴っている", transform.position);
        }
        else
        {
            if (IsBreak)
            {
                GameUI.Instance.ShowMessageUI("アラームは壊れている", transform.position);
            }
            else
            {
                GameUI.Instance.ShowMessageUI("アラームはセットされていない", transform.position);
            }
        }
    }

    private void OnMouseExit()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        GameUI.Instance.HideMessageUI();
    }
}

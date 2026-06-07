using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using ERoom = PlaceManager.ERoom;
public class GhostTimy : Ghost
{
    public List<ClickableLamp> Lamps = new List<ClickableLamp>();

    enum State { 
        Start,
        ToBathRoom,
        InBathRoom,
        ToBedRoom1,
        Sleeping,
        ToLiving,
        InLiving,
        ToDining,
        InDining,
    };
    State _state = State.Start;

    SimpleTimer _BathTimer = new SimpleTimer();

    bool ReachedWashRoom = false;
    int GetLitCount()
    {
        // ランプの点灯判定
        int lit_count = 0;
        foreach (var lamp in Lamps)
        {
            if (lamp.isLit)
            {
                ++lit_count;
            }
        }
        return lit_count;

    }
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (GameMain.Instance.IsGameEnd)
        {
            return;
        }

        if (GameMain.Instance.IsPause)
        {
            return;
        }

        switch (_state)
        {
            case State.Start:
                UpdateStart();
                break;
            case State.ToBathRoom:
                UpdateToBathRoom();
                break;
            case State.InBathRoom:
                UpdateInBathRoom();
                break;
            case State.ToBedRoom1:
                UpdateToBedRoom1();
                break;
            case State.Sleeping:
                UpdateSleeping();
                break;
            case State.ToLiving:
                UpdateToLiving();
                break;
            case State.InLiving:
                UpdateInLiving();
                break;
            case State.ToDining:
                UpdateToDining();
                break;
            case State.InDining:
                UpdateInDining();
                break;

        }
    }

    void ChangeState(State state)
    {
        _state = state;
    }

    void UpdateStart()
    {
        // ランプの点灯判定
        // ※点いていれば進む
        if (GetLitCount() >= 2)
        {
            GotoDestination("BathRoom");
            ChangeState(State.ToBathRoom);
        }

        if (GameMain.Instance.CurrentHour.TotalHours >= 24.45f) // 00:45～
        {
            GotoDestination("BedRoom1_Left");
            ChangeState(State.ToBedRoom1);
        }
    }
    void UpdateToBathRoom()
    {
        if (IsReached())
        {
            _BathTimer.Setup(45.0f * 60.0f); // 45分
            ChangeState(State.InBathRoom);
            return;
        }

        if (ReachedWashRoom || PlaceManager.Instance.GetRoom(transform.position) == ERoom.WashRoom)
        {
            ReachedWashRoom = true;
            return;
        }

        // ランプの点灯判定
        // ※消えていれば戻る
        if (GetLitCount() < 2)
        {
            GotoDestination("BedRoom1_2");
            ChangeState(State.Start);
        }
    }

    void UpdateInBathRoom()
    {
        if (_BathTimer.Update(Time.deltaTime * GameMain.Instance.GetTimeSpeedScale()))
        {
            GotoDestination("BedRoom1_Left");
            ChangeState(State.ToBedRoom1);
        }
    }
    void UpdateToBedRoom1()
    {
        if (IsReached())
        {
            MyAnimator.speed = 0.0f;
            ChangeState(State.Sleeping);
        }

        // ランプの点灯判定
        // ※廊下にいて、かつ消えていればリビングへ
        var room = PlaceManager.Instance.GetRoom(transform.position);
        if (GetLitCount() < 2 && (room == ERoom.Corridor1 || room == ERoom.Corridor2))
        {
            GotoDestination("Living1");
            ChangeState(State.ToLiving);
        }
    }
    void UpdateSleeping() 
    {
    }
    void UpdateToLiving()
    {
        if (IsReached())
        {
            ChangeState(State.InLiving);
        }
    }
    void UpdateInLiving()
    {
        if (GameMain.Instance.CurrentHour.TotalHours > 25.5f) // 25:30
        {
            GotoDestination("Dining5");
            ChangeState(State.ToDining);
        }
    }
    void UpdateToDining()
    {
        if (IsReached())
        {
            ChangeState(State.ToDining);
        }
    }
    void UpdateInDining()
    {
    }

    void OnMouseEnter()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        switch (_state)
        {
            case State.Start:
                GameUI.Instance.ShowMessageUI("バスルームに行きたいが何かが怖いようだ．．．", transform.position);
                break;
            case State.ToBathRoom:
                GameUI.Instance.ShowMessageUI("バスルームに向かっているようだ", transform.position);
                break;
            case State.InBathRoom:
                GameUI.Instance.ShowMessageUI("気持ち良さそうだ", transform.position);
                break;
            case State.ToBedRoom1:
                GameUI.Instance.ShowMessageUI("自分の部屋に戻るところだ", transform.position);
                break;
            case State.Sleeping:
                GameUI.Instance.ShowMessageUI("寝ている．．．", transform.position);
                break;
            case State.ToLiving:
                GameUI.Instance.ShowMessageUI("明るいリビングに向かっている", transform.position);
                break;
            case State.InLiving:
                GameUI.Instance.ShowMessageUI("リビングでくつろいでいる", transform.position);
                break;
            case State.ToDining:
                GameUI.Instance.ShowMessageUI("食堂に向かっている", transform.position);
                break;
            case State.InDining:
                GameUI.Instance.ShowMessageUI("全員が集まるのを待っている．．．", transform.position);
                break;

        }
    }

    void OnMouseExit()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        GameUI.Instance.HideMessageUI();
    }
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using NkfLib;
using NkfLib.Unity;
using NodeCanvas.BehaviourTrees;
using Redcode.Extensions;
using System;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using ERoom = PlaceManager.ERoom;

public class GhostNary : Ghost
{
    public WallShelf WallShelf;
    public Piano Piano;
    public Desk Desk;
    public GhostQuuKie GhostQuuKie;
    public Transform Cue;
    public Transform Hand;

    public bool InLibrary => PlaceManager.Instance.GetRoom(transform.position) == ERoom.Library;

    public enum State
    {
        Start,
        ToHole,
        InHole,
        ToLibrary,
        InLibrary,
        ToRecreationRoom,
        InRecreationRoom,
        ToDining,
        InDining,
        Surprised,
        PickUpBook,
        MeetHenguu,
    };
    State _state = State.Start;


    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        WallShelf.IsDropped.Subscribe((value) => { if (value) { OnDropBook(); } });
        WallShelf.IsDropped.AddTo(this);
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
            case State.ToHole:
                UpdateToHole();
                break;
            case State.InHole:
                UpdateInHole();
                break;
            case State.ToLibrary:
                UpdateToLibrary();
                break;
            case State.InLibrary:
                UpdateInLibrary();
                break;
            case State.ToRecreationRoom:
                UpdateToRecreationRoom();
                break;
            case State.InRecreationRoom:
                UpdateInRecreationRoom();
                break;
            case State.ToDining:
                UpdateToDining();
                break;
            case State.InDining:
                UpdateInDining();
                break;
            case State.Surprised:
                UpdateSurprised();
                break;
            case State.PickUpBook:
                UpdatePickUpBook();
                break;
            case State.MeetHenguu:
                UpdateMeetHenguu();
                break;
        }
    }


    void ChangeState(State state)
    {
        _state = state;
    }

    public bool IsInHole()
    {
        return _state == State.InHole;
    }

    void UpdateStart()
    {
        if (GameMain.Instance.CurrentHour.TotalHours >= 23.5f) // 23:30
        {
            Desk.CloseBook();
            GotoDestination("Hole2");
            ChangeState(State.ToHole);
        }
    }
    void UpdateToHole()
    {
        if (IsReached())
        {
            Piano.PlayRandomBGM();
            ChangeState(State.InHole);
        }
    }
    void UpdateInHole()
    {
    }

    
    void UpdateToLibrary()
    {
        if (IsReached())
        {
            Desk.OpenBook();
            ChangeState(State.InLibrary);
        }
    }

    void UpdateInLibrary()
    {
        if (GameMain.Instance.CurrentHour.TotalHours >= 25.5f) // 01:30～
        {
            Desk.CloseBook();
            GotoDestination("Dining3");
            ChangeState(State.ToDining);
        }
    }

    void UpdateToRecreationRoom()
    {
        if (IsReached())
        {
            Cue.parent = Hand;
            Cue.localPosition = Vector3.zero;
            Cue.localRotation = Quaternion.identity;

            ChangeState(State.InRecreationRoom);
        }
    }

    void UpdateInRecreationRoom()
    {
    }
    void UpdateToDining()
    {
        if (IsReached())
        {
            ChangeState(State.InDining);
        }
    }

    void UpdateInDining()
    {
    }


    void UpdateSurprised()
    {
    }
    void UpdatePickUpBook()
    { 
    }

    void UpdateMeetHenguu()
    { 
    }

    public async void OnPianoClicked()
    {
        if (_state != State.InHole)
        {
            return;
        }

        ChangeState(State.Surprised);
        Piano.StopBGM();
        await UniTask.Delay(1500); // 少し待つ
        var temp_rotation = transform.rotation;
        transform.DOLocalRotate(new Vector3(0.0f, 90.0f, 0.0f), 0.5f);
        await UniTask.Delay(4000); // 少し待つ
        transform.DOLocalRotateQuaternion(temp_rotation, 0.5f);
        await UniTask.Delay(3000); // 少し待つ
        Piano.PlayRandomBGM();
        ChangeState(State.InHole);
    }

    // イベント発生ごとにシーケンスを分ける方針
    async void OnDropBook()
    {
        // キャンセルは難しいので待ち
        await UniTask.WaitUntil(() => _state != State.Surprised);

        if (_state != State.InHole)
        {
            return;
        }

        if (!GhostQuuKie.CanPickUpBook())
        {
            ChangeState(State.PickUpBook);
            Piano.StopBGM();

            transform.DOLocalRotate(new Vector3(0.0f, 90.0f, 0.0f), 0.5f);
            await UniTask.Delay(5000); // 一瞬待つ
                                       // 本を拾って移動
            WallShelf.PickUpBook();
            GotoDestination("Library1");
            ChangeState(State.ToLibrary);
        }
    }

    public async UniTask<bool> OnMeetHenguu()
    {
        // キャンセルは難しいので待ち
        await UniTask.WaitUntil(() => _state != State.Surprised);

        if (_state != State.InHole)
        {
            return false;
        }

        Piano.StopBGM();
        ChangeState(State.MeetHenguu);
        var temp_rotation = transform.rotation;
        transform.DOLocalRotate(new Vector3(0.0f, 90.0f, 0.0f), 0.5f);
        await UniTask.Delay(5000); //５秒ほど
        GotoDestination("RecreationRoom2");
        ChangeState(State.ToRecreationRoom);
        return true;
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
                GameUI.Instance.ShowMessageUI("読書が日課のようだ", transform.position);
                break;
            case State.ToHole:
                GameUI.Instance.ShowMessageUI("ピアノを弾きにホールに向かっている", transform.position);
                break;
            case State.InHole:
                GameUI.Instance.ShowMessageUI("すばらしい演奏だ", transform.position);
                break;
            case State.ToLibrary:
                GameUI.Instance.ShowMessageUI("拾った本を読みに図書室に向かっている", transform.position);
                break;
            case State.InLibrary:
                GameUI.Instance.ShowMessageUI("読書に夢中だ．．．", transform.position);
                break;
            case State.ToRecreationRoom:
                GameUI.Instance.ShowMessageUI("ビリヤードをしに娯楽室に向かっている", transform.position);
                break;
            case State.InRecreationRoom:
                GameUI.Instance.ShowMessageUI("とても楽しそうだ", transform.position);
                break;
            case State.ToDining:
                GameUI.Instance.ShowMessageUI("食堂に向かっている", transform.position);
                break;
            case State.InDining:
                GameUI.Instance.ShowMessageUI("全員が集まるのを待っている．．．", transform.position);
                break;
            case State.Surprised:
                GameUI.Instance.ShowMessageUI("少し驚いたようだ", transform.position);
                break;
            case State.PickUpBook:
                GameUI.Instance.ShowMessageUI("本に興味を持ったようだ", transform.position);
                break;
            case State.MeetHenguu:
                GameUI.Instance.ShowMessageUI("仲良く話をしている", transform.position);
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

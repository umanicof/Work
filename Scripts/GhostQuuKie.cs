using System;
using System.Security.Cryptography;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using NkfLib;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.VisualScripting;
using System.Threading.Tasks;
using DG.Tweening;

using ERoom = PlaceManager.ERoom;

public class GhostQuuKie : Ghost
{
    public WallShelf WallShelf;
    public GhostNary GhostNary;
    public MeatPlate MeatPlate;

    public bool CanPickUpBook()
    {
        // 部屋の間にいる場合に取れないこともあるかもだが気にしない
        var room = PlaceManager.Instance.GetRoom(transform.position);
        if (room == ERoom.Entrance || room == ERoom.Hole)
        { 
            return true;
        }
        return false;
    }

    public enum State
    {
        Start,
        ToHole,
        ToLibrary,
        InLibrary,
        ToKitchen,
        InKitchen,
        Cooking,
        ToDining,
        InDining,
        DroppedBook,
    };
    State _state = State.Start;
    bool _stateInit = false; // 各ステート開始処理用

    bool _inEvent = false; // イベント処理中

    TimeSpan _startActionHour;

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
            case State.ToLibrary:
                UpdateToLibrary();
                break;
            case State.InLibrary:
                UpdateInLibrary();
                break;
            case State.ToKitchen:
                UpdateToKitchen();
                break;
            case State.InKitchen:
                UpdateInKitchen();
                break;
            case State.Cooking:
                UpdateCooking();
                break;
            case State.ToDining:
                UpdateToDining();
                break;
            case State.InDining:
                UpdateInDining();
                break;
            case State.DroppedBook:
                break;

        }
    }
    void ChangeState(State state)
    {
        _state = state;
        _stateInit = false;
    }

    void UpdateStart()
    {
        if (GameMain.Instance.CurrentHour.TotalHours >= 24.25f)
        {
            GotoDestination("Kitchen1");
            ChangeState(State.ToKitchen);
        }

    }
    void UpdateToHole()
    {
        if (IsReached())
        {
            WallShelf.PickUpBook();
            GotoDestination("Library2");
            ChangeState(State.ToLibrary);
        }
    }

    void UpdateToLibrary()
    {
        if (IsReached())
        {
            ChangeState(State.InLibrary);
        }
    }
    void UpdateInLibrary()
    {
        if (GameMain.Instance.CurrentHour.TotalHours >= 24.5f)
        {
            GotoDestination("Kitchen1");
            ChangeState(State.ToKitchen);
        }
    }
    void UpdateToKitchen()
    {
        if (IsReached())
        {
            ChangeState(State.InKitchen);
        }
    }
    async void UpdateInKitchen()
    {
        if (_inEvent)
        {
            return;
        }

        // りんごが無い
        if (!GimmickManager.Instance.IsCanUseFood(ERoom.Kitchen))
        {
            return;
        }

        _inEvent = true;
        var rotation = transform.rotation;
        transform.DORotate(new Vector3(0.0f, -90.0f, 0.0f), 0.5f);
        await UniTask.Delay(2000);
        if (await GimmickManager.Instance.UseFood(ERoom.Kitchen))
        {
            MeatPlate.Use();
            transform.DORotate(new Vector3(0.0f, 90.0f, 0.0f), 0.5f);
            await UniTask.Delay(500);
            _startActionHour = GameMain.Instance.CurrentHour;
            ChangeState(State.Cooking);
        }
        else
        {
            transform.DORotateQuaternion(rotation, 0.5f);
            await UniTask.Delay(500);
        }
        _inEvent = false;
    }

    void UpdateCooking()
    {
        if ((GameMain.Instance.CurrentHour - _startActionHour).TotalHours > 0.30f) // 30分経過
        {
            GotoDestination("Dining1");
            ChangeState(State.ToDining);
        }
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

    async void OnDropBook()
    {
        if (!CanPickUpBook())
        {
            return;
        }

        ChangeState(State.DroppedBook);
            
        //transform.DOLocalRotate(new Vector3(0.0f, 180.0f, 0.0f), 0.5f);
        await UniTask.Delay(2000); // 少し待つ

        GotoDestination("Hole1");
        ChangeState(State.ToHole);
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
                GameUI.Instance.ShowMessageUI("ゴーストの帰りが遅いのを心配しているようだ．．．", transform.position);
                break;
            case State.ToHole:
                GameUI.Instance.ShowMessageUI("ホールで物音がしたようだ", transform.position);
                break;
            case State.ToLibrary:
                GameUI.Instance.ShowMessageUI("図書室に本を片付けに向かっている", transform.position);
                break;
            case State.ToKitchen:
                GameUI.Instance.ShowMessageUI("キッチンに向かっている", transform.position);
                break;
            case State.InKitchen:
                GameUI.Instance.ShowMessageUI("調理の材料が見当たらず困っているようだ．．．", transform.position);
                break;
            case State.Cooking:
                GameUI.Instance.ShowMessageUI("＊ゴーストシチュー調理中＊", transform.position);
                break;
            case State.ToDining:
                GameUI.Instance.ShowMessageUI("食堂に向かっている", transform.position);
                break;
            case State.InDining:
                GameUI.Instance.ShowMessageUI("全員が集まるのを待っている．．．", transform.position);
                break;
            case State.DroppedBook:
                GameUI.Instance.ShowMessageUI("何かの物音に気づいたようだ", transform.position);
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

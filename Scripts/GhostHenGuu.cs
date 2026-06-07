using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using static PlaceManager;

public class GhostHenGuu : Ghost
{
    public GhostNary GhostNary;
    public GhostRazy GhostRazy;
    public Transform Cue;
    public Transform Hand;

    bool _isTalking = false;
    public enum State
    {
        Start,
        ToHole,
        InHole,
        ToRecreationRoom,
        InRecreationRoom,
        ToBar,
        InBar,
        ToDining,
        InDining,
    };
    State _state = State.Start;
    bool _stateInit = false; // 各ステート開始処理用
    bool _inEvent = false;

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
            case State.ToHole:
                UpdateToHole();
                break;
            case State.InHole:
                UpdateInHole();
                break;
            case State.ToRecreationRoom:
                UpdateToRecreationRoom();
                break;
            case State.InRecreationRoom:
                UpdateInRecreationRoom();
                break;
            case State.ToBar:
                UpdateToBar();
                break;
            case State.InBar:
                UpdateInBar();
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
        _stateInit = false;
    }

    void UpdateStart()
    {
        if (GameMain.Instance.CurrentHour.TotalHours >= 25.0f + (1.0f / 6.0f)) // 25:10
        {
            if (GhostNary.IsInHole())
            {
                GotoDestination("Hole1");
                ChangeState(State.ToHole);
            }
            else
            {
                GotoDestination("Bar2");
                ChangeState(State.ToBar);
            }
        }

    }

    async void UpdateToHole()
    {
        if (IsReached())
        {
            ChangeState(State.InHole);

            if (await GhostNary.OnMeetHenguu())
            {
                GotoDestination("RecreationRoom3");
                ChangeState(State.ToRecreationRoom);
            }
            else
            {   // ナリーがいない
                GotoDestination("Bar2");
                ChangeState(State.ToBar);
            }
        }
    }

    void UpdateInHole()
    {
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
    void UpdateToBar()
    {
        if (IsReached())
        {
            if (!_isTalking && GhostRazy.OnStartTalk())
            {
                //transform.DOLocalRotate(new Vector3(0.0f, 90.0f, 0.0f), 0.5f); // 上手く回転できてない
                _isTalking = true;
            }
            ChangeState(State.InBar);
        }
    }
    void UpdateInBar()
    {
        if (GameMain.Instance.CurrentHour.TotalHours >= (25.5f + 1.0f / 6.0f)) // 01:40～
        {
            GotoDestination("Dining2");
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

    public bool OnStartTalk()
    {
        if (!_isTalking && _state != State.InBar)
        {
            return false;
        }
        //transform.DOLocalRotate(new Vector3(0.0f, 90.0f, 0.0f), 0.5f); // 上手く回転できてない
        _isTalking = true;
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
                GameUI.Instance.ShowMessageUI("外で遊んでいる", transform.position); // ここにはこない
                break;
            case State.ToHole:
                GameUI.Instance.ShowMessageUI("ホールに向かっている", transform.position);
                break;
            case State.InHole:
                GameUI.Instance.ShowMessageUI("仲良く話をしている", transform.position);
                break;
            case State.ToRecreationRoom:
                GameUI.Instance.ShowMessageUI("娯楽室に向かっているようだ", transform.position);
                break;
            case State.InRecreationRoom:
                GameUI.Instance.ShowMessageUI("とても楽しそうだ", transform.position);
                break;
            case State.ToBar:
                GameUI.Instance.ShowMessageUI("バーに向かっている", transform.position);
                break;
            case State.InBar:
                if (!GhostRazy.IsBarTalking)
                {
                    GameUI.Instance.ShowMessageUI("一人で飲んでいる", transform.position);
                }
                else
                {
                    GameUI.Instance.ShowMessageUI("とても楽しそうだ", transform.position);
                }
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

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

public class GhostRazy : Ghost
{
    public GhostHenGuu GhostHenguu;
    public AlamClock AlamClock;

    public bool IsBarTalking => _state == State.InBarTalking;


    public enum State
    {
        Start,
        WakeUp,
        ToBar,
        InBar,
        InBarSleeping,
        InBarTalking,
        ToDining,
        InDining,
    };
    State _state = State.Start;
    bool _stateInit = false; // 各ステート開始処理用
    bool _inEvent = false;

    int _drinkNo = 0;

    SimpleTimer _sleepingTimer = new SimpleTimer();
    SimpleTimer _drinkTimer = new SimpleTimer();

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        AlamClock.IsAlarmOn.Subscribe((value) => { if (value) { OnAlarm(); } });
        AlamClock.IsAlarmOn.AddTo(this);
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
            case State.WakeUp:
                UpdateWakeUp();
                break;
            case State.ToBar:
                UpdateToBar();
                break;
            case State.InBar:
                UpdateInBar();
                break;
            case State.InBarSleeping:
                UpdateInBarSleeping();
                break;
            case State.InBarTalking:
                UpdateInBarTalking();
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
        if (!_stateInit)
        {
            MyAnimator.speed = 0.0f;
            GotoDestination("BedRoom4_Left");
            _stateInit = true;
        }
    }
    void UpdateWakeUp()
    {
        if (IsReached())
        {
            GotoDestination("Bar1");
            ChangeState(State.ToBar);
        }
    }

    void UpdateToBar()
    {
        if (IsReached())
        {
            if (GhostHenguu.OnStartTalk())
            {
                transform.DORotate(new Vector3(0.0f, 180.0f, 0.0f), 0.5f);
                ChangeState(State.InBarTalking);
            }
            else
            {
                _sleepingTimer.Setup(1.0f * 60.0f * 60.0f); // バーに着いてから１時間で寝る
                _drinkTimer.Setup(20.0f * 60.0f); // 20分ごとに飲み物が変わる
                _drinkNo = Random.Range(0, 4);
                ChangeState(State.InBar);
            }
        }

    }

    void UpdateInBar()
    {
        if (_sleepingTimer.Update(Time.deltaTime * GameMain.Instance.GetTimeSpeedScale()))
        {
            MyAnimator.speed = 0.0f;
            GotoDestination("Bar1Sleep");
            ChangeState(State.InBarSleeping);
        }

        if (_drinkTimer.Update(Time.deltaTime * GameMain.Instance.GetTimeSpeedScale()))
        {
            _drinkNo = Random.Range(0, 4);
            _drinkTimer.Reset();
        }
    }
    void UpdateInBarSleeping()
    {
    }
    void UpdateInBarTalking()
    {
        if (GameMain.Instance.CurrentHour.TotalHours >= (25.5f + 1.0f / 6.0f)) // 01:40～
        {
            GotoDestination("Dining4");
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

    public void OnAlarm()
    {
        if (_state != State.Start)
        {
            return;
        }
        MyAnimator.speed = 1.0f;
        GotoDestination("BedRoom4");
        ChangeState(State.WakeUp);
    }

    public bool OnStartTalk()
    {
        if (_state != State.InBar)
        {
            return false;
        }

        transform.DORotate(new Vector3(0.0f, 180.0f, 0.0f), 0.5f);
        ChangeState(State.InBarTalking);

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
                GameUI.Instance.ShowMessageUI("気持ち良さそうに寝ている", transform.position);
                break;
            case State.WakeUp:
                GameUI.Instance.ShowMessageUI("アラームで起きた", transform.position);
                break;
            case State.ToBar:
                GameUI.Instance.ShowMessageUI("バーに向かっている", transform.position);
                break;
            case State.InBar:
                switch (_drinkNo)
                {
                    case 0:
                        GameUI.Instance.ShowMessageUI("ワインを飲んでいる．．．", transform.position);
                    break;
                    case 1:
                        GameUI.Instance.ShowMessageUI("ブランデーを飲んでいる．．．", transform.position);
                        break;
                    case 2:
                        GameUI.Instance.ShowMessageUI("ウイスキーを飲んでいる．．．", transform.position);
                        break;
                    case 3:
                        GameUI.Instance.ShowMessageUI("ビールを飲んでいる．．．", transform.position);
                        break;
                    case 4:
                        GameUI.Instance.ShowMessageUI("ウォッカを飲んでいる．．．", transform.position);
                        break;
                }
                break;
            case State.InBarSleeping:
                GameUI.Instance.ShowMessageUI("酔いつぶれて寝ている．．．", transform.position);
                break;
            case State.InBarTalking:
                GameUI.Instance.ShowMessageUI("とても楽しそうだ", transform.position);
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

using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using static PlaceManager;

public class GhostCheby : Ghost
{
    public enum State
    {
        Start,
        WakeUp,
        ToRecreationRoom,
        InRecreationRoom,
        ToLiving,
        InLiving,
        ToDining,
        InDining,
    };
    State _state = State.Start;
    bool _stateInit = false; // 各ステート開始処理用

    SimpleTimer _eatTimer = new SimpleTimer();

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
            case State.WakeUp:
                UpdateWakeUp();
                break;
            case State.ToRecreationRoom:
                UpdateToRecreationRoom();
                break;
            case State.InRecreationRoom:
                UpdateInRecreationRoom();
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
        _stateInit = false;
    }

    void UpdateStart()
    {
        if (!_stateInit)
        {
            MyAnimator.speed = 0.0f;
            GotoDestination("BedRoom1_Right");
            _stateInit = true;
        }
        else 
        {
            if (GameMain.Instance.CurrentHour.TotalHours >= 23.5f)
            {
                MyAnimator.speed = 1.0f;
                GotoDestination("BedRoom1");
                ChangeState(State.WakeUp);
            }
        }
    }
    void UpdateWakeUp()
    {
        if (IsReached())
        {
            GotoDestination("RecreationRoom1");
            ChangeState(State.ToRecreationRoom);
        }
    }
    void UpdateToRecreationRoom()
    {
        if (IsReached())
        { 
            _eatTimer.Setup(15.0f * 60.0f); // 15分
           ChangeState(State.InRecreationRoom);
        }
    }
    async UniTask UpdateInRecreationRoom()
    {
        if (_eatTimer.Update(Time.deltaTime * GameMain.Instance.GetTimeSpeedScale()))
        {
            if (!await GimmickManager.Instance.EatFood(ERoom.RecreationRoom))
            {                                                         
                Debug.Log("食べれなかった？");
            }
        }

        if (GameMain.Instance.CurrentHour.TotalHours >= 24.25f)
        {
            GotoDestination("Living2");
            ChangeState(State.ToLiving);
        }
    }
    void UpdateToLiving()
    {
        if (IsReached())
        {
            _eatTimer.Setup(15.0f * 60.0f); // 15分
            ChangeState(State.InLiving);
        }
    }
    async UniTask UpdateInLiving()
    {
        if (_eatTimer.Update(Time.deltaTime * GameMain.Instance.GetTimeSpeedScale()))
        {
            if (!await GimmickManager.Instance.EatFood(ERoom.Living))
            {
                Debug.Log("食べれなかった？");
            }
        }

        if (GameMain.Instance.CurrentHour.TotalHours >= 25.5f)
        {
            GotoDestination("Dining6");
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

    void OnMouseEnter()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        switch (_state)
        {
            case State.Start:
                GameUI.Instance.ShowMessageUI("寝ている．．．", transform.position);
                break;
            case State.WakeUp:
                GameUI.Instance.ShowMessageUI("起きたようだ", transform.position);
                break;
            case State.ToRecreationRoom:
                GameUI.Instance.ShowMessageUI("娯楽室のお菓子を狙っているようだ", transform.position);
                break;
            case State.InRecreationRoom:
                {
                    var food = GimmickManager.Instance.FindFood(ERoom.RecreationRoom);
                    if (food)
                    {
                        if (food.IsAte)
                        {
                            GameUI.Instance.ShowMessageUI(food.FoodName + "を食べてしまった！", transform.position);
                        }
                        else
                        {
                            GameUI.Instance.ShowMessageUI(food.FoodName + "を食べている．．．", transform.position);
                        }
                    }
                    else
                    {
                        GameUI.Instance.ShowMessageUI("食べるものがないようだ", transform.position); // ここには来ないはず
                    }
                    break;
                }
            case State.ToLiving:
                GameUI.Instance.ShowMessageUI("リビングの果物を狙っている", transform.position);
                break;
            case State.InLiving:
                {
                    var food = GimmickManager.Instance.FindFood(ERoom.Living);
                    if (food)
                    {
                        if (food.IsAte)
                        {
                            GameUI.Instance.ShowMessageUI(food.FoodName + "を食べてしまった！", transform.position);
                        }
                        else
                        {
                            GameUI.Instance.ShowMessageUI(food.FoodName + "を食べている．．．", transform.position);
                        }
                    }
                    else
                    {
                        GameUI.Instance.ShowMessageUI("食べるものがないようだ", transform.position); // ここには来ないはず
                    }
                    break;
                }
            case State.ToDining:
                GameUI.Instance.ShowMessageUI("お腹が空いたので食堂に向かっている", transform.position);
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

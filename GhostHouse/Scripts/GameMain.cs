using Cysharp.Threading.Tasks;
using Redcode.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

using ERoom = PlaceManager.ERoom;

public class GameMain : MonoBehaviour
{
    public float DefaultTimeSpeedScale = 40.0f;
    public float FastTimeSpeedScale = 70.0f;
    public float StartHour;
    public float EndHour;
    public Transform Dinner;
    public Transform FirePlace;
    public TimeSpan CurrentHour => _currentHour;

    float _timeSpeedScale = 40.0f;

    public bool IsPause { get; private set; }

    public float GetTimeSpeedScale() { return _timeSpeedScale; }

    enum State { Title, HowToPlay, InGame, GameOver, Ending };
    static GameUI UI => GameUI.Instance;
    static PlaceManager PlaceManager => PlaceManager.Instance;

    public bool IsGameEnd => _state == State.GameOver || _state == State.Ending;
    public bool IsInGame => _state == State.InGame;

    bool _gameStarted;
    TimeSpan _startHour;
    TimeSpan _endHour;
    TimeSpan _currentHour;
    State _state = State.Title;
    State _prevState = State.Title;

    int _stat = 0;

    public static GameMain Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Application.targetFrameRate = 25;

        _gameStarted = false;
        _startHour = TimeSpan.FromHours(StartHour);
        _endHour = TimeSpan.FromHours(EndHour);
        _currentHour = _startHour;
        _timeSpeedScale = DefaultTimeSpeedScale;
        Dinner.gameObject.SetActive(false);
        FirePlace.gameObject.SetActive(false);

        ChangeState(State.Title);
    }

    void Update()
    {
        if (_gameStarted)
        {
            Time.timeScale = IsPause ? 0.0f : 1.0f;
        }
        if (IsPause)
        {
            SoundManager.Instance.PauseBGM();
        }
        else
        {
            SoundManager.Instance.UnPauseBGM();
        }

        var current = Keyboard.current;
        if (current.spaceKey.IsPressed() && _state == State.InGame)
        {
            UI.SpeedUpPanel.gameObject.SetActive(true);
            _timeSpeedScale = FastTimeSpeedScale;
        }
        else
        {
            UI.SpeedUpPanel.gameObject.SetActive(false);
            _timeSpeedScale = DefaultTimeSpeedScale;
        }

        switch (_state)
        {
            case State.Title:
                IsPause = true;
                UpdateTitle();
                break;
            case State.HowToPlay:
                IsPause = true;
                UpdateHowToPlay();
                break;
            case State.InGame:
                IsPause = false;
                _gameStarted = true;
                UpdateInGame();
                break;
            case State.GameOver:
                IsPause = false;
                UpdateGameOver();
                break;
            case State.Ending:
                IsPause = false;
                UpdateEnding();
                break;

        }
    }

    void UpdateTitle()
    {
        // キー入力
        var current = Keyboard.current;
        if (current.spaceKey.wasPressedThisFrame)
        {
            ChangeState(State.InGame);
        }
        else if (current.pKey.wasPressedThisFrame)
        {
            ChangeState(State.HowToPlay);
        }

    }
    void UpdateHowToPlay()
    {
        // キー入力
        var current = Keyboard.current;
        if (current.xKey.wasPressedThisFrame)
        {
           BackState();
        }
    }

    void UpdateInGame()
    {
        // キー入力
        var current = Keyboard.current;
        if (current.pKey.wasPressedThisFrame)
        {
            ChangeState(State.HowToPlay);
        }
        else if (current.rKey.wasPressedThisFrame)
        {
            Retry();
            return;
        }

#if UNITY_EDITOR
        // 終了演出確認用
        else if (current.gKey.wasPressedThisFrame)
        {
            ChangeState(State.GameOver);
            return;
        }
        else if (current.eKey.wasPressedThisFrame)
        {
            ChangeState(State.Ending);
            return;
        }
#endif
        if (IsPause)
        {
            return;
        }

        if (current.aKey.wasPressedThisFrame|| current.leftArrowKey.wasPressedThisFrame)
        {
            PlaceManager.PrevCameraTransform();
        }
        if (current.dKey.wasPressedThisFrame || current.rightArrowKey.wasPressedThisFrame)
        {
            PlaceManager.NextCameraTransform();
        }

        if (GhostManager.Instance.IsGameClear())
        {
            ChangeState(State.Ending);
            return;
        }

        _currentHour = _currentHour.Add(TimeSpan.FromSeconds(Time.deltaTime * GetTimeSpeedScale()));
        if (_currentHour >= _endHour)
        {
            _currentHour = _endHour;
            ChangeState(State.GameOver);
            return;
        }
    }

    async void UpdateGameOver()
    {
        if (_stat == 1)
        {
            return;
        }
        else if (_stat == 2)
        {
            // キー入力
            var current = Keyboard.current;
            if (current.rKey.wasPressedThisFrame)
            {
                Retry();
                return;
            }
            return;
        }
        _stat = 1;

        SoundManager.Instance.PlayGameEnd().Forget();

        await GameUI.Instance.FadeOut();
        await UniTask.Delay(4000);
        GameUI.Instance.HideMessageUI();
        PlaceManager.Instance.SetCameraRoom(ERoom.Dining);
        // TODO: 暖炉や光などセット
        GhostManager.Instance.ForceDeactive();
        UI.SetGameOverUI();
        await GameUI.Instance.FadeIn();
        await UniTask.Delay(1000);
        await GameUI.Instance.FadeInGameEndPanel();

        _stat = 2;
    }

    async void UpdateEnding()
    {
        if (_stat == 1)
        {
            return;
        }
        else if (_stat == 2)
        {
            // キー入力
            var current = Keyboard.current;
            if (current.rKey.wasPressedThisFrame)
            {
                Retry();
                return;
            }
            return;
        }
        _stat = 1;

        SoundManager.Instance.PlayGameEnd().Forget();

        await GameUI.Instance.FadeOut();
        await UniTask.Delay(4000);
        GameUI.Instance.HideMessageUI();
        PlaceManager.Instance.SetCameraRoom(ERoom.Dining);
#if UNITY_EDITOR
        GhostManager.Instance.ForceGotoDining();
#endif
        // TODO: 暖炉や光などセット
        Dinner.gameObject.SetActive(true);
        FirePlace.gameObject.SetActive(true);
        GhostManager.Instance.Razy.MyAnimator.speed = 0.0f; // ラジー停止
        UI.SetEndingUI();
        await GameUI.Instance.FadeIn();
        await UniTask.Delay(1000);

        SoundManager.Instance.PlayEndingBGM();

        // 会話
        GameUI.Instance.ShowMessageUI("［クーキー］今月もみんな集まれたね。\n心配したんだから。", GhostManager.Instance.QuuKie.transform.position);
        await UniTask.Delay(4000);
        GameUI.Instance.ShowMessageUI("［ナリー］大げさだよ。\nいつも最後には集まってるし。", GhostManager.Instance.Nary.transform.position);
        await UniTask.Delay(4000);
        GameUI.Instance.ShowMessageUI("［ヘングー］忘れてても何となく\n足が向くんだよね。", GhostManager.Instance.HenGuu.transform.position);
        await UniTask.Delay(4000);
        GameUI.Instance.ShowMessageUI("［ティミー］不思議だね。\nこの屋敷に何かあるのかな？", GhostManager.Instance.Timy.transform.position);
        await UniTask.Delay(4000);
        GameUI.Instance.ShowMessageUI("［チェビー］そろそろ食べようよ！\n冷めちゃうよ！", GhostManager.Instance.Cheby.transform.position);
        await UniTask.Delay(4000);
        GameUI.Instance.ShowMessageUI("［ラジー］Zzzz．．．", GhostManager.Instance.Razy.transform.position);
        await UniTask.Delay(4000);

        await GameUI.Instance.FadeInGameEndPanel();
        _stat = 2;
    }


    void ChangeState(State state)
    {
        // 一部のステートのみ記憶する
        switch (_state)
        {
            case State.Title:
            case State.InGame:
            case State.Ending:
                _prevState = _state;
                break;

        }

        _state = state;

        switch (state)
        {
            case State.Title:
                UI.SetTitleUI();
                break;
            case State.HowToPlay:
                UI.SetHowToPlayUI();
                break;
            case State.InGame:
                UI.SetInGameUI();
                break;                
            case State.GameOver:
            case State.Ending:
                // UI変更は後で行う
                break;

        }
    }

    void BackState()
    {
        ChangeState(_prevState);
    }

    void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

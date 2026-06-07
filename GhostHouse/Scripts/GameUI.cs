using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    static public GameUI Instance;

    public RectTransform CommonUI;
    public RectTransform TitleUI;
    public RectTransform HowToPlayUI;
    public RectTransform CharacterUI;
    public RectTransform InGameUI;
    public RectTransform GameOverUI;
    public RectTransform EndingUI;
    public MessageUI MessageUI;
    public RectTransform FadeUI;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI RoomText;
    public RectTransform SpeedUpPanel;
    public RectTransform TrackingPanel;
    public RectTransform GameOverEndPanel;
    public RectTransform EndingEndPanel;

    RectTransform _gameEndPanel;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (InGameUI.gameObject.activeSelf)
        {
            // 時間表示
            var hour = (float)GameMain.Instance.CurrentHour.TotalHours;
            var hour_floor = Mathf.Floor(hour);
            var hour_decimal = hour - hour_floor;
            //int hour_minites = (int)(Mathf.Floor((hour_decimal * 60.0f) / 5.0f) * 5.0f);
            int hour_minites = (int)(Mathf.Floor((hour_decimal * 60.0f) / 1.0f) * 1.0f);
            hour_floor %= 24;
            TimeText.text = $"{hour_floor:00}:{hour_minites:00}";

            // 追跡中表示
            TrackingPanel.gameObject.SetActive(PlaceManager.Instance.IsTracking);
        }
    }

    public void SetTitleUI()
    {
        TitleUI.gameObject.SetActive(true);
        HowToPlayUI.gameObject.SetActive(false);
        InGameUI.gameObject.SetActive(false);
        GameOverUI.gameObject.SetActive(false);
        EndingUI.gameObject.SetActive(false);
        GameOverEndPanel.gameObject.SetActive(false);
        EndingEndPanel.gameObject.SetActive(false);
    }

    public void SetHowToPlayUI()
    {
        TitleUI.gameObject.SetActive(false);
        HowToPlayUI.gameObject.SetActive(true);
        InGameUI.gameObject.SetActive(false);
        GameOverUI.gameObject.SetActive(false);
        EndingUI.gameObject.SetActive(false);
        GameOverEndPanel.gameObject.SetActive(false);
        EndingEndPanel.gameObject.SetActive(false);
    }

    public void SetInGameUI()
    {
        TitleUI.gameObject.SetActive(false);
        HowToPlayUI.gameObject.SetActive(false);
        InGameUI.gameObject.SetActive(true);
        GameOverUI.gameObject.SetActive(false);
        EndingUI.gameObject.SetActive(false);
        GameOverEndPanel.gameObject.SetActive(false);
        EndingEndPanel.gameObject.SetActive(false);
    }

    public void SetGameOverUI()
    {
        TitleUI.gameObject.SetActive(false);
        HowToPlayUI.gameObject.SetActive(false);
        InGameUI.gameObject.SetActive(false);
        GameOverUI.gameObject.SetActive(true);
        EndingUI.gameObject.SetActive(false);
        GameOverEndPanel.gameObject.SetActive(false);
        EndingEndPanel.gameObject.SetActive(false);
        _gameEndPanel = GameOverEndPanel;
    }

    public void SetEndingUI()
    {
        TitleUI.gameObject.SetActive(false);
        HowToPlayUI.gameObject.SetActive(false);
        InGameUI.gameObject.SetActive(false);
        GameOverUI.gameObject.SetActive(false);
        EndingUI.gameObject.SetActive(true);
        GameOverEndPanel.gameObject.SetActive(false);
        EndingEndPanel.gameObject.SetActive(false);
        _gameEndPanel = EndingEndPanel;
    }

    public void ShowMessageUI(string message, Vector3 position, Vector3 offset_screen = new Vector3())
    {
        MessageUI.gameObject.SetActive(true);
        MessageUI.setText(message);
        MessageUI.setPosition(position, offset_screen);
    }
    public void HideMessageUI()
    {
        MessageUI.gameObject.SetActive(false);
    }

    public void SetRoomText(string name)
    {
        RoomText.text = name;
    }

    public async UniTask FadeOut()
    {
        FadeUI.gameObject.SetActive(true);
        var group = FadeUI.GetComponent<CanvasGroup>();
        group.DOFade(1.0f, 1.0f);
        await UniTask.Delay(1000);
    }

    public async UniTask FadeIn()
    {
        FadeUI.gameObject.SetActive(true);
        var group = FadeUI.GetComponent<CanvasGroup>();
        group.DOFade(0.0f, 1.0f);
        await UniTask.Delay(1000);
        FadeUI.gameObject.SetActive(false);
    }

    public async UniTask FadeInGameEndPanel()
    {
        _gameEndPanel.gameObject.SetActive(true);
        var group = _gameEndPanel.GetComponent<CanvasGroup>();
        group.alpha = 0.0f;
        group.DOFade(1.0f, 2.0f);
        await UniTask.Delay(2000);
    }
}

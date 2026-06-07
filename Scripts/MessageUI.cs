using NodeCanvas.DialogueTrees;
using TMPro;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Screen = UnityEngine.Screen;

public class MessageUI : MonoBehaviour

{
    public TextMeshProUGUI MessageText;
    RectTransform _rectTransform;
    RectTransform _canvasRectTransform;

    int _delayFrame = 0;

    const float cRectMargin = 10.0f; // UI座標のマージン
    const float cUITopScreenMargin = 40.0f; // 上部UIのスクリーン座標のマージン
    const float cUIBottomScreenMargin = 60.0f; // 下部UIのスクリーン座標のマージン
    

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasRectTransform = _rectTransform.parent.GetComponent<RectTransform>();
    }

    void Start()
    {
    }

    void Update()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        // 連続してsetText、setPositionが呼ばれるとダメだが現在は大丈夫そう
        if (_delayFrame > 0)
        {
            --_delayFrame;
            var rect = MessageText.rectTransform.rect;
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width + cRectMargin * 2.0f);
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height + cRectMargin * 2.0f);
        }
        else
        {
            MessageText.alpha = 1.0f;
        }
        AddjustRectAnchoredPosition();
    }

    // UIではrectTransform.positionは使うべきではなく、anchoredPositionを使うべきとのこと
    void AddjustRectAnchoredPosition()
    {       
        var size = RectTransformToScreenSize(_rectTransform);
        var position = WorldToScreenPosition(_rectTransform.position);
        // はみ出し位置調整
        var left = position.x - size.x / 2.0f;
        var right = position.x + size.x / 2.0f;
        var top = position.y - size.y / 2.0f;
        var bottom = position.y + size.y / 2.0f;

        if (left < 0.0f)
        {
            position.x = size.x / 2.0f;
        }
        if (right > Screen.currentResolution.width)
        {
            position.x = Screen.currentResolution.width - size.x / 2.0f;
        }
        if (top < cUITopScreenMargin) // 上部UIを考慮
        {
            position.y = cUITopScreenMargin + size.y / 2.0f;
        }
        if (bottom > (Screen.currentResolution.height - cUIBottomScreenMargin)) // 下部UIを考慮
        {
            position.y = Screen.currentResolution.height - cUIBottomScreenMargin - size.y / 2.0f;
        }
        _rectTransform.anchoredPosition = ScreenToAnchoredPosition(position);
    }

    public void setText(string text)
    {
        MessageText.text = text;
        MessageText.alpha = 0.0f;
        _delayFrame = 2;
    }

    public void setPosition(Vector3 position, Vector3 offset_screen)
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(position);
        _rectTransform.anchoredPosition = ScreenToAnchoredPosition(screenPos + (Vector2)offset_screen);
        MessageText.alpha = 0.0f;
        _delayFrame = 2;

    }

    Vector2 ScreenToAnchoredPosition(Vector2 screen_position)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRectTransform, // _rectTransformではダメ
            screen_position,
            null,
            out var anchoredPosition
        );
        return anchoredPosition;
    }

    Vector2 WorldToScreenPosition(Vector2 world_position)
    {
        return RectTransformUtility.WorldToScreenPoint(null, _rectTransform.position);
    }

    Vector2 RectTransformToScreenSize(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector2 screen0 = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
        Vector2 screen1 = RectTransformUtility.WorldToScreenPoint(null, corners[1]);
        Vector2 screen2 = RectTransformUtility.WorldToScreenPoint(null, corners[2]);
        Vector2 screen3 = RectTransformUtility.WorldToScreenPoint(null, corners[3]);

        return new Vector2(Vector2.Distance(screen0, screen3), Vector2.Distance(screen0, screen1));
    }
}

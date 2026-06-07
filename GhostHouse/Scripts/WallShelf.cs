using DG.Tweening;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

public class WallShelf : MonoBehaviour
{
    public Transform Book;
    public Transform Drop;

    AudioSource _audioSource;

    public ReactiveProperty<bool> IsDropped { get; } = new ReactiveProperty<bool>(false);

    public bool IsPickUped => !Book.gameObject.activeSelf;
    bool _clicked = false;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    void OnMouseDown()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        if (!_clicked)
        {
            var position = transform.position;
            _audioSource.Play();

            transform.DOShakePosition(0.5f, 0.1f).OnComplete(() => {
                transform.position = position;
                Book.position = Drop.position;
                Book.rotation = Drop.rotation;
                IsDropped.Value = true;
            });
        }
        _clicked = true;
    }

    void OnMouseEnter()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        if (IsDropped.Value)
        {
            GameUI.Instance.ShowMessageUI("棚から本が落ちた", transform.position, Vector3.down * 0.2f);
        }
        else
        {
            GameUI.Instance.ShowMessageUI("備え付けの本棚だ", transform.position, Vector3.down * 0.2f);
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

    public void PickUpBook()
    {
        Book.gameObject.SetActive(false);
    }
}

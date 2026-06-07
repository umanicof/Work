using DG.Tweening;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;

public class BookShelf : MonoBehaviour
{
    AudioSource _audioSource;

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
                _clicked = false;
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

        GameUI.Instance.ShowMessageUI("たくさんの本が仕舞ってある", transform.position, Vector3.down * 0.2f);
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

using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Piano : MonoBehaviour
{
    public Transform BGMHolder;
    public Transform SEHolder;

    public GhostNary GhostNary;

    List<AudioSource> BGMs;
    List<AudioSource> SEs;

    void Awake()
    {
    }

    void Start()
    {
        BGMs = BGMHolder.GetComponents<AudioSource>().ToList<AudioSource>();
        SEs = SEHolder.GetComponents<AudioSource>().ToList<AudioSource>();
    }
    void Update()
    {

    }

    public void PlayRandomBGM()
    {
        if (BGMs.Count <= 0)
        {
            return;
        }
        int index = Random.Range(0, BGMs.Count - 2); // 最後のインデックスはエンディングに使用
        BGMs[index].Play();
    }

    public void PlayEndingBGM()
    {
        if (BGMs.Count <= 0)
        {
            return;
        }
        int index = BGMs.Count - 1;
        BGMs[index].Play();
    }

    public void PlayRandomSE()
    {
        if (SEs.Count <= 0)
        {
            return;
        }
        int index = Random.Range(0, SEs.Count - 1);
        SEs[index].Play();
    }
    public void StopBGM()
    {
        // TODO: 生成直後に取れていない間にアクセスされる
        if (BGMs == null)
        {
            return;
        }
        BGMs.ForEach((source) => { source.Stop(); });
    }

    public void SetBGMsVolume(float volume)
    {
        // TODO: 生成直後に取れていない間にアクセスされる
        if (BGMs == null)
        {
            return;
        }
        BGMs.ForEach((source) => { source.volume = volume; });
    }
    public void SetSEsVolume(float volume)
    {
        // TODO: 生成直後に取れていない間にアクセスされる
        if (SEs == null)
        {
            return;
        }
        SEs.ForEach((source) => { source.volume = volume; });
    }

    public void PauseBGM()
    {
        BGMs.ForEach((source) => { source.Pause(); });
    }
    public void UnPauseBGM()
    {
        BGMs.ForEach((source) => { source.UnPause(); });
    }

    void OnMouseDown()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        PlayRandomSE();
        GhostNary.OnPianoClicked();
    }

    void OnMouseEnter()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        GameUI.Instance.ShowMessageUI("この屋敷に古くからあるピアノだ", transform.position, Vector3.down * 30.0f );
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

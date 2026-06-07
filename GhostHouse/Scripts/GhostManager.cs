using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GhostManager : MonoBehaviour
{
    // enumの順番通りにセットする必要あり
    public List<Ghost> Ghosts = new List<Ghost>();
    public List<Transform> DiningDestinations = new List<Transform>(); // デバッグ用

    public enum EGhost
    {
        Cheby,
        HenGuu,
        Nary,
        QuuKie,
        Razy,
        Timy,
        Max,
    }

    public static GhostManager Instance;
    public GhostCheby Cheby => (GhostCheby)GetGhost(EGhost.Cheby);
    public GhostHenGuu HenGuu => (GhostHenGuu)GetGhost(EGhost.HenGuu);
    public GhostNary Nary =>  (GhostNary)GetGhost(EGhost.Nary); 
    public GhostQuuKie QuuKie => (GhostQuuKie)GetGhost(EGhost.QuuKie);
    public GhostRazy Razy => (GhostRazy)GetGhost(EGhost.Razy);
    public GhostTimy Timy => (GhostTimy)GetGhost(EGhost.Timy);


    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        
    }

    public Ghost GetGhost(EGhost ghost)
    {
        return Ghosts[(int)ghost];
    }

    public bool IsGameClear()
    {
        foreach (var ghost in Ghosts)
        {
            if (!ghost.IsReached())
            { 
                return false;
            }
            if (PlaceManager.Instance.GetRoom(ghost.transform.position) != PlaceManager.ERoom.Dining) 
            {
                return false;
            }
        }
        return true;
    }

    // デバッグ用：強制的にゴーストたちを食堂に集める
    public void ForceGotoDining()
    {
        int i = 0;
        foreach (var ghost in Ghosts)
        {
            var dest = DiningDestinations[i];
            ghost.Agent.enabled = false;
            //GotoDestination(dest.gameObject.name);
            ghost.transform.position = dest.transform.position;
            ghost.MyAnimator.speed = 1.0f;
            ++i;
        }

    }

    // 強制的に全員非表示にする
    public void ForceDeactive()
    {
        foreach (var ghost in Ghosts)
        {
            ghost.Agent.enabled = true;
            ghost.gameObject.SetActive(false);
        }

    }
}

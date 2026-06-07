using UnityEngine;

public class WallClock : MonoBehaviour
{
    public AudioSource Bell;

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

        Bell.Play();
    }

    void OnMouseEnter()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        GameUI.Instance.ShowMessageUI("大きな柱時計だ", transform.position);
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

using UnityEngine;

public class Paper : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        GameUI.Instance.ShowMessageUI("「晩餐会のメニュー\nゴーストシチュ―\n材料：肉、玉ねぎ、りんご」\nと書いてある", transform.position);
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

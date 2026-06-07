using UnityEngine;

public class MeatPlate : FoodBase
{
    protected override void Awake()
    {
        base.Awake();
        FoodName = "肉と玉ねぎ";
        CanUse = true;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    void OnMouseEnter()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        if (IsUsed)
        {
            GameUI.Instance.ShowMessageUI("肉と玉ねぎはゴーストシチューの材料になった", transform.position, Vector3.down * 20.0f);
        }
        else
        {
            GameUI.Instance.ShowMessageUI("晩餐会の材料のようだ", transform.position, Vector3.down * 20.0f);
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

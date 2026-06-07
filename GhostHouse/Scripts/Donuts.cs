using UnityEngine;

public class Donuts : FoodBase
{
    protected override void Awake()
    {
        base.Awake();
        FoodName = "ドーナツ";
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    void OnMouseDown()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        if (!IsAte)
        {
            GimmickManager.Instance.ExchangeFoodsPosition(transform.position);
        }
    }

    void OnMouseEnter()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        if (IsAte)
        {
            GameUI.Instance.ShowMessageUI("ドーナツは食べられてしまった．．．", transform.position, Vector3.down * 20.0f);
        }
        else
        {
            GameUI.Instance.ShowMessageUI("とてもおいしそうなドーナツだ", transform.position, Vector3.down * 20.0f);
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

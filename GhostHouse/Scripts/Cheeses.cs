using UnityEngine;

public class Cheeses : FoodBase
{
    protected override void Awake()
    {
        base.Awake();
        FoodName = "チーズ";
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
            GameUI.Instance.ShowMessageUI("チーズは食べられてしまった．．．", transform.position, Vector3.down * 20.0f);
        }
        else
        {
            GameUI.Instance.ShowMessageUI("穴の空いた濃厚なチーズだ", transform.position, Vector3.down * 20.0f);
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

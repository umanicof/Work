using UnityEngine;

public class Apples : FoodBase
{

    protected override void Awake()
    {
        base.Awake();
        FoodName = "りんご";
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

    void OnMouseDown()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        if (!IsAte && !IsUsed)
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
            GameUI.Instance.ShowMessageUI("りんごは食べられてしまった．．．", transform.position, Vector3.down * 20.0f);
        }
        else if(IsUsed)
        {
            GameUI.Instance.ShowMessageUI("りんごはゴーストシチューの材料になった", transform.position, Vector3.down * 20.0f);
        }
        else
        {
            GameUI.Instance.ShowMessageUI("山盛りのりんごだ", transform.position, Vector3.down * 20.0f);
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

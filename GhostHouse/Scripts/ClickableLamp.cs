using UnityEngine;

public class ClickableLamp : MonoBehaviour
{
    public Transform SpotLight;
    public Transform WallLight;
    public AudioSource OnOff;

    public bool isLit => SpotLight.gameObject.activeSelf;

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

        bool active = !SpotLight.gameObject.activeSelf;
        SpotLight.gameObject.SetActive(active);
        WallLight.gameObject.SetActive(active);
        OnOff.Play();
    }

    void OnMouseEnter()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        if (isLit)
        {
            GameUI.Instance.ShowMessageUI("ランプは怪しく光っている", transform.position, new Vector3(0.0f, -5.0f, 0.0f));
        }
        else
        {
            GameUI.Instance.ShowMessageUI("ランプが消えている", transform.position);
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

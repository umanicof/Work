using UnityEngine;

public class Desk : MonoBehaviour
{
    public Transform OpenedBook;

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    public void OpenBook()
    {
        OpenedBook.gameObject.SetActive(true);
    }
    public void CloseBook()
    {
        OpenedBook.gameObject.SetActive(false);
    }

    void OnMouseEnter()
    {
        if (!GameMain.Instance.IsInGame)
        {
            return;
        }

        GameUI.Instance.ShowMessageUI("アンティークでオシャレな机だ", transform.position, Vector3.down * 0.2f);
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

using UnityEngine;

public class CameraTransform : MonoBehaviour
{
    public float Fov = 60.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("メインカメラにこの位置をセット")]
    public void SetMainCamea()
    {
        Camera.main.transform.position = transform.position;
        Camera.main.transform.rotation = transform.rotation;
        Camera.main.fieldOfView = Fov;
    }

    [ContextMenu("メインカメラ位置を取得してこの位置にセット")]
    public void GetMainCamea()
    {
        transform.position = Camera.main.transform.position;
        transform.rotation = Camera.main.transform.rotation;
        Fov = Camera.main.fieldOfView;
    }
}

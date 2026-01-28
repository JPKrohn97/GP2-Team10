using UnityEngine;

public class BillboardController : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCam != null)
        {
            // Yazıyı kameraya çevir
            transform.LookAt(transform.position + mainCam.transform.forward);
        }
    }
}
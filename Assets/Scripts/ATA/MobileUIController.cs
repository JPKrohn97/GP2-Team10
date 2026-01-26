using UnityEngine;

public class MobileUIController : MonoBehaviour
{

    public bool showInEditor = true; 

    void Awake() 
    {

        if (Application.isMobilePlatform)
        {
            gameObject.SetActive(true); 
        }

        else
        {
            if (showInEditor)
            {
                gameObject.SetActive(true); 
            }
            else
            {
                gameObject.SetActive(false); 
            }
        }
    }
}
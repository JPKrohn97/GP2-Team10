using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MobileTouchController : MonoBehaviour
{
    public UnityEvent onPress;
    
    [Header("Renk Ayarları")]
    public Color pressedColor = Color.gray; 
    private Color originalColor;          
    private Image myImage;

    private RectTransform myRect;
    private int pressedFingerId = -1;

    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
        myImage = GetComponent<Image>();

        if (myImage != null)
        {
            originalColor = myImage.color;
        }
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void Update()
    {

        foreach (var touch in Touch.activeTouches)
        {
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
     
                if (pressedFingerId == -1 && RectTransformUtility.RectangleContainsScreenPoint(myRect, touch.screenPosition))
                {
                    pressedFingerId = touch.finger.index; 
                    
         
                    if (myImage != null) myImage.color = pressedColor;

                    onPress.Invoke(); 
                }
            }

            if (touch.finger.index == pressedFingerId && 
                (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended || touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled))
            {
        
                if (myImage != null) myImage.color = originalColor;

                pressedFingerId = -1; 
            }
        }
    }
}
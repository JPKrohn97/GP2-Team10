using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class MobileTouchController : MonoBehaviour
{
    public UnityEvent onPress;

    [Header("Color Setting")]
    public Color pressedColor = Color.gray;

    private Color originalColor;
    private Image myImage;
    private RectTransform myRect;

    private int activeFingerId = -1;

    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
        myImage = GetComponent<Image>();

        if (myImage != null)
            originalColor = myImage.color;
    }

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    private void Update()
    {
        var touches = Touch.activeTouches;

        foreach (var touch in touches)
        {
            if (touch.phase != TouchPhase.Began)
                continue;

            if (activeFingerId != -1)
                continue;

            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    myRect,
                    touch.screenPosition))
                continue;

            activeFingerId = touch.finger.index;

            SetColor(pressedColor);
            onPress?.Invoke();
            
            Invoke(nameof(ResetButton), 0.08f);
            break;
        }
    }

    private void ResetButton()
    {
        activeFingerId = -1;
        SetColor(originalColor);
    }

    private void SetColor(Color c)
    {
        if (myImage != null)
            myImage.color = c;
    }

    
    
    #region old code
    // [Header("Renk Ayarları")]
    // public Color pressedColor = Color.gray; 
    // private Color originalColor;          
    // private Image myImage;
    //
    // private RectTransform myRect;
    // private int pressedFingerId = -1;
    //
    // private void Awake()
    // {
    //     myRect = GetComponent<RectTransform>();
    //     myImage = GetComponent<Image>();
    //
    //     if (myImage != null)
    //     {
    //         originalColor = myImage.color;
    //     }
    // }
    //
    // private void OnEnable()
    // {
    //     EnhancedTouchSupport.Enable();
    // }
    //
    // private void Update()
    // {
    //
    //     foreach (var touch in Touch.activeTouches)
    //     {
    //         if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
    //         {
    //  
    //             if (pressedFingerId == -1 && RectTransformUtility.RectangleContainsScreenPoint(myRect, touch.screenPosition))
    //             {
    //                 pressedFingerId = touch.finger.index; 
    //                 
    //      
    //                 if (myImage != null) myImage.color = pressedColor;
    //
    //                 onPress.Invoke(); 
    //             }
    //         }
    //
    //         if (touch.finger.index == pressedFingerId && 
    //             (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended || touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled))
    //         {
    //     
    //             if (myImage != null) myImage.color = originalColor;
    //
    //             pressedFingerId = -1; 
    //         }
    //     }
    // }
    
    #endregion
}
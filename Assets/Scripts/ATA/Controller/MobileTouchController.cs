using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class MobileTouchController : MonoBehaviour
{
    public UnityEvent onPress;
    
    [Header("Renk Ayarları")]
    public Color pressedColor = Color.gray; 
    private Color originalColor;          
    private Image myImage;

    private RectTransform myRect;
    private int activeFingerId = -1; // Parmak takibi için

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
        // Aktif tüm dokunuşları kontrol et
        var touches = Touch.activeTouches;

        foreach (var touch in touches)
        {
            // 1. BASILMA ANI
            if (touch.phase == TouchPhase.Began)
            {
                // Eğer butona tıklandıysa ve halihazırda bir parmak bu butonu işgal etmiyorsa
                if (activeFingerId == -1 && RectTransformUtility.RectangleContainsScreenPoint(myRect, touch.screenPosition))
                {
                    activeFingerId = touch.finger.index;
                    SetColor(pressedColor);
                    onPress.Invoke();
                }
            }

            // 2. BIRAKILMA ANI (Veya İptal)
            // Sadece bu butona basan parmak bıraktığında sıfırla
            if (touch.finger.index == activeFingerId)
            {
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    ResetButton();
                }
            }
        }

        // 3. GÜVENLİK KONTROLÜ (Takılı kalmayı önler)
        // Eğer kayıtlı parmak artık ekranda değilse butonu serbest bırak
        if (activeFingerId != -1)
        {
            bool fingerStillActive = false;
            foreach (var t in touches)
            {
                if (t.finger.index == activeFingerId)
                {
                    fingerStillActive = true;
                    break;
                }
            }

            if (!fingerStillActive)
            {
                ResetButton();
            }
        }
    }

    private void ResetButton()
    {
        activeFingerId = -1;
        SetColor(originalColor);
    }

    private void SetColor(Color c)
    {
        if (myImage != null) myImage.color = c;
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
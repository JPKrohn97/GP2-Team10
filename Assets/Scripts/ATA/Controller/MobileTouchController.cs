using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MobileTouchController : MonoBehaviour
{
    public UnityEvent onPress;
    
    [Header("Renk Ayarları")]
    public Color pressedColor = Color.gray; // Basıldığında olacak renk
    private Color originalColor;            // Butonun orijinal rengi
    private Image myImage;

    private RectTransform myRect;
    private int pressedFingerId = -1;

    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
        myImage = GetComponent<Image>();

        // Eğer objede bir Image varsa, başlangıç rengini hafızaya al
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
            // --- 1. DOKUNMA BAŞLADI (BEGAN) ---
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                // Eğer butona şu an kimse basmıyorsa (pressedFingerId == -1) 
                // VE dokunma butonun üzerindeyse:
                if (pressedFingerId == -1 && RectTransformUtility.RectangleContainsScreenPoint(myRect, touch.screenPosition))
                {
                    pressedFingerId = touch.finger.index; // Bu parmağı kaydet
                    
                    // Rengi Değiştir (Basıldı Rengi)
                    if (myImage != null) myImage.color = pressedColor;

                    onPress.Invoke(); // Fonksiyonu çalıştır (Zıplama vb.)
                }
            }

            // --- 2. DOKUNMA BİTTİ (ENDED veya CANCELED) ---
            // Sadece bizim butona basan parmak (pressedFingerId) kalktıysa işlem yap
            if (touch.finger.index == pressedFingerId && 
                (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended || touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled))
            {
                // Rengi Eski Haline Getir
                if (myImage != null) myImage.color = originalColor;

                pressedFingerId = -1; // Butonu serbest bırak, artık basılmıyor
            }
        }
    }
}
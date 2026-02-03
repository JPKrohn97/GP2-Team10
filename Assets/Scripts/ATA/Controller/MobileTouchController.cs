using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MobileTouchController : MonoBehaviour
{
    public UnityEvent onPress;

    private RectTransform myRect;

    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
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

                if (RectTransformUtility.RectangleContainsScreenPoint(myRect, touch.screenPosition))
                {
                    onPress.Invoke();
                }
            }
        }
    }
}
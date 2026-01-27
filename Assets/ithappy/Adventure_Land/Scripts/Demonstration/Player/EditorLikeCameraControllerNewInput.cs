using UnityEngine;
using UnityEngine.InputSystem;

namespace ithappy.Adventure_Land
{
    public class EditorLikeCameraControllerNewInput : EditorLikeCameraControllerBase
    {
        private DemonstrationInput _demonstrationInput;
        private bool _isShifted;
        
        protected override void Awake()
        {
            base.Awake();

            _demonstrationInput = new DemonstrationInput();
            _demonstrationInput.Enable();
            
            _demonstrationInput.Camera.RightClick.started += RightClickOnStarted;
            _demonstrationInput.Camera.RightClick.canceled += RightClickOnCanceled;
            _demonstrationInput.Camera.Shifted.started += ShiftedOnstarted;
            _demonstrationInput.Camera.Shifted.canceled += ShiftedOncanceled;
        }

        private void ShiftedOncanceled(InputAction.CallbackContext obj)
        {
            _isShifted = false;
        }

        private void ShiftedOnstarted(InputAction.CallbackContext obj)
        {
            _isShifted = true;
        }

        private void RightClickOnCanceled(InputAction.CallbackContext obj)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _isRotating = false;
        }

        private void RightClickOnStarted(InputAction.CallbackContext obj)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _isRotating = true;
        }

        private void LateUpdate()
        {
            Vector2 inputDirectionMouse = _demonstrationInput.Camera.Mouse.ReadValue<Vector2>();
            Vector3 inputDirectionMove = _demonstrationInput.Camera.Movement.ReadValue<Vector3>();
            Vector2 inputScrollWheel = _demonstrationInput.Camera.Scroll.ReadValue<Vector2>();
            
            HandleMovement(inputDirectionMove, _isShifted);
            HandleRotation(inputDirectionMouse);
            HandleZoom(inputScrollWheel.y);
        }
    }
}
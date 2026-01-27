using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ithappy.Adventure_Land
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerCharacterNewInput : PlayerCharacterInputBase
    {
        private DemonstrationInput demonstrationInput;
        
        protected override void Awake()
        {
            base.Awake();
            
            demonstrationInput =  new DemonstrationInput();
            demonstrationInput.Enable();
            
            demonstrationInput.Character.Jump.performed += JumpOnPerformed;
            demonstrationInput.Character.Emotions.performed += EmotionsOnperformed;
        }

        private void EmotionsOnperformed(InputAction.CallbackContext obj)
        {
            HandleAnimations();
        }

        private void JumpOnPerformed(InputAction.CallbackContext obj)
        {
            HandleJump();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            demonstrationInput.Disable();
        }

        protected override void Update()
        {
            base.Update();
            
            Vector2 inputDirectionMove = demonstrationInput.Character.Movement.ReadValue<Vector2>();
            Vector2 inputDirectionMouse = demonstrationInput.Character.Rotations.ReadValue<Vector2>();
            
            cachedMouseX = inputDirectionMouse.x * rotationSpeed;
            cachedMouseY = inputDirectionMouse.y * rotationSpeed;
            cachedHorizontal = inputDirectionMove.x;
            cachedVertical = inputDirectionMove.y;
        }
    }
}
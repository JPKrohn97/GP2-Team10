using UnityEngine;


public class PlayerRunState : PlayerGroundedState 
{
    private const float DeadZone = 0.01f;
    public PlayerRunState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        if (player.AnimationEvents != null)
            player.AnimationEvents.SetMovingBool(true);
    }

    public override void Exit()
    {
        base.Exit();

        if (player.AnimationEvents != null)
            player.AnimationEvents.SetMovingBool(false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (stateMachine.CurrentState != this) return;

        // Hareket yoksa Idle'a geç
        if (player.CurrentMovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        
        /*
        if (player.InputHandler.Player.Jump.triggered)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        */
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        float inputX = player.CurrentMovementInput.x;


        Vector3 velocity = player.RB.linearVelocity;
        

        velocity.z = inputX * player.moveSpeed; 
        velocity.x = 0f; 
        player.RB.linearVelocity = velocity;
        
        if (Mathf.Abs(inputX) > DeadZone)
        {

            float targetY = (inputX > 0f) ? 0f : 180f; 

            player.transform.rotation = Quaternion.Euler(0f, targetY, 0f);
        }
    }
}
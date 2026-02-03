using UnityEngine;

public class PlayerRunState : PlayerGroundedState 
{

    private const float DeadZone = 0.15f; 
    
    private float acceleration = 50f; 
    private float deceleration = 60f;

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


        if (player.CurrentMovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        float inputX = player.CurrentMovementInput.x;
        if (Mathf.Abs(inputX) < DeadZone)
        {
            inputX = 0f;
        }

        float targetSpeed = inputX * player.moveSpeed;
        Vector3 currentVelocity = player.RB.linearVelocity;

        float speedChangeRate = (Mathf.Abs(targetSpeed) > DeadZone) ? acceleration : deceleration;

        float newSpeedZ = Mathf.MoveTowards(currentVelocity.z, targetSpeed, speedChangeRate * Time.fixedDeltaTime);

        Vector3 finalVelocity = new Vector3(0f, currentVelocity.y, newSpeedZ);
        player.RB.linearVelocity = finalVelocity;

        if (Mathf.Abs(inputX) > DeadZone)
        {
            float targetY = (inputX > 0f) ? 0f : 180f; 
            player.transform.rotation = Quaternion.Euler(0f, targetY, 0f);
        }
    }
}
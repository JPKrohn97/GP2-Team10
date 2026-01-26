using UnityEngine;

public class PlayerRunState : PlayerGroundedState
{

    private const float DeadZone = 0.01f;

    public PlayerRunState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.AnimationEvents.SetMovingBool(true);
    }

    public override void Exit()
    {
        base.Exit();
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


        Vector3 v = player.RB.linearVelocity;
        v.z = inputX * player.moveSpeed;
        v.x = 0f; 
        player.RB.linearVelocity = v;


        if (Mathf.Abs(inputX) > DeadZone)
        {

            float targetY = (inputX > 0f) ? 0f : 180f; 
            player.transform.rotation = Quaternion.Euler(0f, targetY, 0f);
        }
    }
}
using UnityEngine;

public class PlayerAirState : PlayerState
{
    private const float MaxFallSpeed = 30f; 

    public PlayerAirState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.Animator.SetBool("Jump", true);
    }

    public override void Exit()
    {
        base.Exit();
        player.Animator.SetBool("Jump", false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();


        if (player.IsGrounded && player.RB.linearVelocity.y <= 0f)
        {

            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector3 velocity = player.RB.linearVelocity;

        // Gravity shaping
        if (velocity.y < 0f)
        {
            velocity += Vector3.up * Physics.gravity.y * (player.fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (velocity.y > 0f && !player.JumpAction.IsPressed())
        {
            velocity += Vector3.up * Physics.gravity.y * (player.lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }


        if (velocity.y < -MaxFallSpeed)
            velocity.y = -MaxFallSpeed;


        float inputX = player.CurrentMovementInput.x;
        velocity.z = inputX * player.moveSpeed;
        velocity.x = 0f;

        player.RB.linearVelocity = velocity;
        
        if (inputX != 0f)
        {
            float targetY = (inputX > 0f) ? 0f : 180f;
            player.transform.rotation = Quaternion.Euler(0f, targetY, 0f);
        }
    }
}
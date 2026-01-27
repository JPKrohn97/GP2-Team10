using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        Vector3 v = player.RB.linearVelocity;
        v.y = 0f; 
        player.RB.linearVelocity = v;

        float jumpVel = Mathf.Sqrt(player.jumpHeight * -2f * Physics.gravity.y);

        v = player.RB.linearVelocity;
        v.y = jumpVel;         
        player.RB.linearVelocity = v;

        stateMachine.ChangeState(player.AirState);
    }
}
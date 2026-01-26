using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        
        Vector3 currentVel = player.RB.linearVelocity;
        player.RB.linearVelocity = new Vector3(currentVel.x, 0, currentVel.z);
        
        float jumpForce = Mathf.Sqrt(player.jumpHeight * -2f * Physics.gravity.y);
        
        player.RB.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        
        stateMachine.ChangeState(player.AirState);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }
}
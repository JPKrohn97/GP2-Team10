using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

    
        if (player.CheckIfGrounded() && player.RB.linearVelocity.y < 0.1f)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
       
    }
}
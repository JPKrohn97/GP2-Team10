using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.RB.linearVelocity = Vector3.zero; 
        player.AnimationEvents.SetMovingBool(false); 
        player.AnimationEvents.SetAnimationTrigger("Idle"); 
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate(); 

       
        if (player.CurrentMovementInput != Vector2.zero)
        {
            stateMachine.ChangeState(player.RunState);
        }
    }
}
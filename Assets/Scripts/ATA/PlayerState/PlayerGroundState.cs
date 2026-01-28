using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // 1. Mutation 
        if (player.InputHandler.Player.Interact.triggered && player.IsOnDeadEnemy)
        {
            stateMachine.ChangeState(player.MutationState);
            return;
        }

        // 2. Attack
        if (player.InputHandler.Player.Attack.triggered) 
        {
            stateMachine.ChangeState(player.SwordAttackState);
            Debug.Log("saldiri test");
            return;
        }
        
        //3. Range Attack
        if (player.RangeAction.triggered) 
        {
            stateMachine.ChangeState(player.RangeAttackState);
            return;
        }

        // 4. Jump
        if (player.InputHandler.Player.Jump.triggered) 
        {
            stateMachine.ChangeState(player.JumpState);
            return;
        }

        // 5. Fall
        if (!player.CheckIfGrounded())
        {
            stateMachine.ChangeState(player.AirState);
        }
    }
}
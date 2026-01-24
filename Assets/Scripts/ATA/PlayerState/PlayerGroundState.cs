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
            Debug.Log("saldiri");
            return;
        }

        // 3. Jump
        if (player.InputHandler.Player.Jump.triggered) 
        {
            stateMachine.ChangeState(player.JumpState);
            Debug.Log("ziplama");
            return;
        }

        // 4. Fall
        if (!player.CheckIfGrounded())
        {
            stateMachine.ChangeState(player.AirState);
        }
    }
}
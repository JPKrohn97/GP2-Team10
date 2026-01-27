using UnityEngine;

public class PlayerSwordAttackState : PlayerAttackState
{
    private bool shouldCombo;

    public PlayerSwordAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter(); 

        shouldCombo = false; 
        
        player.Combat.Attack();
        
 
        attackDuration = 0.7f; 
        
        player.RB.linearVelocity = Vector3.zero; 
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();


        if (player.InputHandler.Player.Attack.triggered)
        {
            shouldCombo = true;
 
        }


        if (Time.time >= startTime + attackDuration)
        {
            if (shouldCombo) 
            {
      
                stateMachine.ChangeState(player.SwordAttackState);
            }
            else 
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}
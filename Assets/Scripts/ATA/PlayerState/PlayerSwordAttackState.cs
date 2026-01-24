using UnityEngine;

public class PlayerSwordAttackState : PlayerAttackState
{
    public PlayerSwordAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
      
        attackDuration = 0.6f; 
        base.Enter();
        
        player.Combat.Attack(); 
    }
}
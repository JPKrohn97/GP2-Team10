using UnityEngine;

public abstract class PlayerAttackState : PlayerState
{
    protected float attackDuration; 

    public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        // StartTime burada set ediliyor (PlayerState içinde olduğunu varsayıyorum)
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();


        if (Time.time >= startTime + attackDuration)
        {
            FinishAttack();
        }
    }

    protected virtual void FinishAttack()
    {
 
        if (player.CurrentMovementInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(player.RunState);
        }
        else
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
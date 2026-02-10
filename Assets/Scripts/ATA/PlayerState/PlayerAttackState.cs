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

        // Bu süre kontrolü sadece Claw gibi tek vuruşluk saldırılar için geçerli olmalı.
        // Sword gibi combo sistemler bunu override etmeli veya base'i çağırmamalı.
        if (Time.time >= startTime + attackDuration)
        {
            FinishAttack();
        }
    }

    protected virtual void FinishAttack()
    {
        // Vector2.zero yerine sqrMagnitude daha güvenlidir
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
using UnityEngine;

public class PlayerRangeAttackState : PlayerState 
{
    private float attackDuration = 0.3f; 
    private float attackTimer;

    public PlayerRangeAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        
 
        player.RB.linearVelocity = Vector3.zero;
 
        if (player.projectilePrefab != null && player.firePoint != null)
        {
            player.Animator.SetTrigger("RangeAttack");
            GameObject.Instantiate(player.projectilePrefab, player.firePoint.position, player.transform.rotation);
        }

        attackTimer = attackDuration;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        attackTimer -= Time.deltaTime;


        if (attackTimer <= 0)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
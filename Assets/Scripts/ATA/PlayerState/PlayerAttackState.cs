using UnityEngine;


public abstract class PlayerAttackState : PlayerState
{
    protected float attackDuration; 

    public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

 
        Vector3 attackPos = player.transform.position + player.transform.forward * 1.5f;
        float attackRange = 1.5f;

      
        LayerMask enemyLayer = LayerMask.GetMask("Enemy");
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos, attackRange, enemyLayer);

 
        foreach (Collider hitCollider in hitEnemies)
        {
   
            EnemyBehaviour targetEnemy = hitCollider.GetComponent<EnemyBehaviour>();

       
            if (targetEnemy != null)
            {
        
                Vector3 hitPoint = hitCollider.transform.position + Vector3.up;
                
                targetEnemy.TakeDamage(20, hitPoint);

                Debug.Log("Düşmana Vuruldu!");
            }
        }
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Süre doldu mu kontrolü
        if (Time.time >= startTime + attackDuration)
        {
            FinishAttack();
        }
    }


    protected virtual void FinishAttack()
    {

        if (player.CurrentMovementInput != Vector2.zero)
        {
            stateMachine.ChangeState(player.RunState);
        }
   
        else
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
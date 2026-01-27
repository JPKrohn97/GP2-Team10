using UnityEngine;

public class PlayerRangeAttackState : PlayerState 
{
    private float attackDuration = 0.3f; 
    private float attackTimer;
    
    public PlayerRangeAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        player.Animator.ResetTrigger("RangeAttack");

        if (player.projectilePrefab != null && player.firePoint != null)
        {

            player.Animator.SetTrigger("RangeAttack"); 
        }

        if (player.AnimationEvents != null)
        {
            bool isMoving = player.CurrentMovementInput.x != 0;
            player.AnimationEvents.SetMovingBool(isMoving);
        }

        attackTimer = attackDuration;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        attackTimer -= Time.deltaTime;

        if (player.AnimationEvents != null)
        {
            bool isMoving = player.CurrentMovementInput.x != 0;
            player.AnimationEvents.SetMovingBool(isMoving);
        }

        if (attackTimer <= 0)
        {
            
            if (player.CurrentMovementInput.x != 0)
                stateMachine.ChangeState(player.RunState);
            else
                stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        float inputX = player.CurrentMovementInput.x;
        Vector3 currentVel = player.RB.linearVelocity;
        
        currentVel.z = inputX * player.moveSpeed; 
        currentVel.x = 0; 
        

        if (currentVel.y < 0) 
             currentVel += Vector3.up * Physics.gravity.y * (player.fallMultiplier - 1f) * Time.fixedDeltaTime;

        player.RB.linearVelocity = currentVel;


        if (inputX != 0)
        {
            float targetY = (inputX > 0f) ? 0f : 180f; 
            player.transform.rotation = Quaternion.Euler(0f, targetY, 0f);
        }
    }
}
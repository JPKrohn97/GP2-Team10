using UnityEngine;

public class PlayerClawAttackState : PlayerAttackState
{
    private float comboBufferTime; 

    public PlayerClawAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter(); 


        if (Mathf.Abs(player.CurrentMovementInput.x) > 0.1f)
        {
            float targetY = (player.CurrentMovementInput.x > 0f) ? 0f : 180f;
            player.transform.rotation = Quaternion.Euler(0f, targetY, 0f);
        }

        player.RB.linearDamping = 0f;
        player.RB.linearVelocity = Vector3.zero;
        
        player.Combat.Attack();


        comboBufferTime = 0.3f; 
        attackDuration = 0.35f; 
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate(); 
        
        bool hasBufferedInput = (player.LastAttackInputTime > startTime) && 
                                (Time.time - player.LastAttackInputTime <= comboBufferTime);
        
        // attackDuration doldu mu?
        if (Time.time >= startTime + attackDuration)
        {
            if (hasBufferedInput)
            {
                stateMachine.ChangeState(player.ClawAttackState);
            }
            else
            {
                FinishAttack(); 
            }
        }
    }
}
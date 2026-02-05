using UnityEngine;

public class PlayerSwordAttackState : PlayerAttackState
{

    private float comboBufferTime; 

    public PlayerSwordAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        if (player.CurrentMovementInput.sqrMagnitude > 0.01f)
        {
            float targetY = (player.CurrentMovementInput.x > 0f) ? 0f : 180f;
            player.transform.rotation = Quaternion.Euler(0f, targetY, 0f);
        }

        player.RB.linearDamping = 0f;
        player.RB.linearVelocity = Vector3.zero;
        
        player.Combat.Attack();

        if (Application.isMobilePlatform)
        {
            comboBufferTime = 0.3f; 
            attackDuration = 0.35f; 
        }
        else
        {
            comboBufferTime = 0.3f; 
            attackDuration = 0.35f;
        }
    }
    

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        bool hasBufferedInput = (player.LastAttackInputTime > startTime) && 
                                (Time.time - player.LastAttackInputTime <= comboBufferTime);
        
        if (Time.time >= startTime + attackDuration)
        {
            if (hasBufferedInput)
            {
                stateMachine.ChangeState(player.SwordAttackState);
            }
            else
            {

                if (player.CurrentMovementInput.magnitude > 0.1f)
                {
                    stateMachine.ChangeState(player.RunState);
                }
                else
                {
                    stateMachine.ChangeState(player.IdleState);
                }
            }
        }
    }
}
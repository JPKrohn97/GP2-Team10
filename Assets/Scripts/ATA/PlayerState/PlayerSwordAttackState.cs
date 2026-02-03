using UnityEngine;

public class PlayerSwordAttackState : PlayerAttackState
{
    private float lastAttackInputTime = -999f;
    
    private float comboBufferTime; 

    public PlayerSwordAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        player.Combat.Attack();
        player.RB.linearVelocity = Vector3.zero;

        if (Application.isMobilePlatform)
        {

            comboBufferTime = 0.60f; 
            attackDuration = 0.67f; 
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


        if (player.InputHandler.Player.Attack.WasPressedThisFrame())
        {
            lastAttackInputTime = Time.time;
        }

        if (Time.time >= startTime + attackDuration)
        {
            if (Time.time - lastAttackInputTime <= comboBufferTime)
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
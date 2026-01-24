using UnityEngine;

public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.AnimationEvents.SetMovingBool(true);
    }

    public override void Exit()
    {
        base.Exit();
        player.AnimationEvents.SetMovingBool(false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (player.CurrentMovementInput == Vector2.zero)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();


        float inputX = player.CurrentMovementInput.x;

        player.RB.linearVelocity = new Vector3(0, player.RB.linearVelocity.y, inputX * player.moveSpeed);

   
        if (inputX != 0)
        {
            Vector3 direction = new Vector3(0, 0, inputX);
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            float rotationSpeed = 1440f; 
        
            player.transform.rotation = Quaternion.RotateTowards(
                player.transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
        }
    }
}
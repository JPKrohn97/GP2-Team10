using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (player.IsGrounded && player.RB.linearVelocity.y <= 0.01f)
        {
            stateMachine.ChangeState(player.IdleState);
            return;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        
        Vector3 currentVel = player.RB.linearVelocity;


        if (currentVel.y < 0f) 
        {
            currentVel += Vector3.up * Physics.gravity.y * (player.fallMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (currentVel.y > 0f && !player.JumpAction.IsPressed()) // Tuşu bıraktıysa küt diye dur
        {
            currentVel += Vector3.up * Physics.gravity.y * (player.lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
        }
        
        float inputX = player.CurrentMovementInput.x;

        currentVel.z = inputX * player.moveSpeed; 
        currentVel.x = 0; 
        
        player.RB.linearVelocity = currentVel;

        if (inputX != 0)
        {
            Vector3 direction = new Vector3(0, 0, inputX);
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float rotationSpeed = 1440f; 
        
            player.transform.rotation = Quaternion.RotateTowards(
                player.transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }
}
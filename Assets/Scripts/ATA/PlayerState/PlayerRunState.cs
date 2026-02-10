using UnityEngine;

public class PlayerRunState : PlayerGroundedState 
{
    private const float DeadZone = 0.25f;

    private float acceleration = 90f; 
    private float deceleration = 80f; 
    private float turnSpeed = 100f; 
    
    private float stepRate = 0.35f; 
    private float nextStepTime = 0f;

    public PlayerRunState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.AnimationEvents?.SetMovingBool(true);
        
        nextStepTime = Time.time + (stepRate / 2f);
    }

    public override void Exit()
    {
        base.Exit();
        player.AnimationEvents?.SetMovingBool(false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (stateMachine.CurrentState != this) return;
    
        if (player.CurrentMovementInput.sqrMagnitude < 0.01f)
        {
            stateMachine.ChangeState(player.IdleState);
            return; 
        }
        
        bool isActuallyMoving = player.RB.linearVelocity.sqrMagnitude > 0.1f;
        player.AnimationEvents?.SetMovingBool(isActuallyMoving);
        if (isActuallyMoving)
        {

            if (Time.time >= nextStepTime)
            {
                if(SoundManager.Instance != null)
                    SoundManager.Instance.PlaySound(SoundManager.Instance.PlayerFootSteps, player.gameObject);

                nextStepTime = Time.time + stepRate;
            }
        }
        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        float rawInputX = player.CurrentMovementInput.x;

        float direction = 0f;
        if (Mathf.Abs(rawInputX) > DeadZone)
        {
            direction = Mathf.Sign(rawInputX);
        }
        else 
        {
            direction = 0f;
        }
        
        float targetSpeed = direction * player.moveSpeed;
        
        Vector3 currentVelocity = player.RB.linearVelocity;
        
        float currentSpeedZ = currentVelocity.z;
        float speedChangeRate;

        if (Mathf.Abs(targetSpeed) < 0.1f) 
        {
            speedChangeRate = deceleration; 
        }
        else if (Mathf.Sign(targetSpeed) != Mathf.Sign(currentSpeedZ) && Mathf.Abs(currentSpeedZ) > 0.1f)
        {
            speedChangeRate = turnSpeed;
        }
        else
        {
            speedChangeRate = acceleration; 
        }
        
        float newSpeedZ = Mathf.MoveTowards(currentSpeedZ, targetSpeed, speedChangeRate * Time.fixedDeltaTime);
        
        Vector3 finalVelocity = new Vector3(0f, currentVelocity.y, newSpeedZ);
        player.RB.linearVelocity = finalVelocity;

        // Rotation
        if (direction != 0f)
        {
            float targetY = (direction > 0f) ? 0f : 180f; 
            
     
            Quaternion targetRotation = Quaternion.Euler(0f, targetY, 0f);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, 20f * Time.fixedDeltaTime);
        }
    }
}
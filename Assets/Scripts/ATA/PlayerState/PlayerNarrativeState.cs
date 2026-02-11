using UnityEngine;
using DG.Tweening; 

public class PlayerNarrativeState : PlayerState
{
    //private float biteDuration = 2f;     
    //private float mutationDuration = 2.0f; 
    //private Tween mutationTween;

    private bool movementLocked = true; 

    public PlayerNarrativeState(PlayerController player, PlayerStateMachine stateMachine) 
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        movementLocked = true;
        player.RB.linearVelocity = Vector3.zero;

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (movementLocked)
        {
            player.RB.linearVelocity = new Vector3(0, player.RB.linearVelocity.y, 0);
        }
    }

    private void FinishNarrative()
    {
        movementLocked = false; 
        stateMachine.ChangeState(player.IdleState);
    }

    public override void Exit()
    {
        base.Exit();
        movementLocked = false;
    }
}

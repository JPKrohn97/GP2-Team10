using UnityEngine;
using DG.Tweening;

public class PlayerMutationState : PlayerState
{
    public PlayerMutationState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        
   
        player.RB.linearVelocity = Vector3.zero;
        
    
        player.AnimationEvents.SetAnimationTrigger("Bite");
        
    
        DOVirtual.DelayedCall(2f, () =>
        {
 
            if(stateMachine.CurrentState == this)
            {
                player.AnimationEvents.MutationSequence();
           
                FinishMutation();
            }
        });
    }

    private void FinishMutation() => stateMachine.ChangeState(player.IdleState);
    
}
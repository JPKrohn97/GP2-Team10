using UnityEngine;
using DG.Tweening; // DOTween kütüphanesi

public class PlayerMutationState : PlayerState
{
    // AYARLAR
    private float biteDuration = 2f;     
    private float mutationDuration = 2.0f; 

    public PlayerMutationState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        

        player.RB.linearVelocity = Vector3.zero;
        
        player.AnimationEvents.SetAnimationTrigger("Bite");
        

        DOVirtual.DelayedCall(biteDuration, () =>
        {

            if(stateMachine.CurrentState == this)
            {
    
                if (player.CurrentDeadEnemy != null)
                {
                    player.CurrentDeadEnemy.ConsumeBody();
                }

       
                player.AnimationEvents.MutationSequence();

                DOVirtual.DelayedCall(mutationDuration, () => 
                {
                     if(stateMachine.CurrentState == this)
                     {
                         FinishMutation();
                     }
                });
            }
        });
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.RB.linearVelocity = new Vector3(0, player.RB.linearVelocity.y, 0);
    }

    private void FinishMutation() => stateMachine.ChangeState(player.IdleState);
}
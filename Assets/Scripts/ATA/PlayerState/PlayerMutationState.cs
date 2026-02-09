using UnityEngine;
using DG.Tweening; 

public class PlayerMutationState : PlayerState
{
    
    private float biteDuration = 2f;     
    private float mutationDuration = 2.0f; 

    public PlayerMutationState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        

        player.RB.linearVelocity = Vector3.zero;
        
        player.AnimationEvents.SetAnimationTrigger("Bite");
        

        EnemyHealth targetEnemy = player.CurrentDeadEnemy; 

        DOVirtual.DelayedCall(biteDuration, () =>
        {
            if(stateMachine.CurrentState == this)
            {
            
                if (targetEnemy != null)
                {
                    var type = targetEnemy.mutationType;
                    
                    if(player.SkillController != null)
                    {
                        player.SkillController.AbsorbSkill(type);
                    }
                    
                    targetEnemy.ConsumeBody();
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
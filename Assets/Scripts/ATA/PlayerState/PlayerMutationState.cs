using UnityEngine;
using DG.Tweening; 

public class PlayerMutationState : PlayerState
{
    private float biteDuration = 2f;     
    private float mutationDuration = 2.0f; 
    private Tween mutationTween;

    private bool movementLocked = true; 

    public PlayerMutationState(PlayerController player, PlayerStateMachine stateMachine) 
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();


        movementLocked = true;
        player.RB.linearVelocity = Vector3.zero;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(SoundManager.Instance.Eating, player.gameObject);
    
        player.AnimationEvents.SetAnimationTrigger("Bite");

        EnemyHealth targetEnemy = player.CurrentDeadEnemy; 

        DOVirtual.DelayedCall(biteDuration, () =>
        {
            if(stateMachine.CurrentState != this) return;

            if (targetEnemy != null)
            {
                var type = targetEnemy.mutationType;
                
                if(player.SkillController != null)
                    player.SkillController.AbsorbSkill(type);
            
                targetEnemy.ConsumeBody();
                player.AnimationEvents.MutationSequence(type); 
            }
            
        
            DOVirtual.DelayedCall(mutationDuration, () =>
            {
                if(stateMachine.CurrentState != this) return;

                FinishMutation();
            });
        });
       
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (movementLocked)
        {
   
            player.RB.linearVelocity = new Vector3(0, player.RB.linearVelocity.y, 0);
        }
    }

    private void FinishMutation()
    {
        movementLocked = false; 
        stateMachine.ChangeState(player.IdleState);
    }

    public override void Exit()
    {
        base.Exit();
        if (mutationTween != null) mutationTween.Kill();
        movementLocked = false;
    }
}

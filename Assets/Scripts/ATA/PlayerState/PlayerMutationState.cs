using UnityEngine;
using DG.Tweening; 

public class PlayerMutationState : PlayerState
{
    private float biteDuration = 2f;     
    private float mutationDuration = 2.0f; 
    private Tween mutationTween;

    private bool movementLocked = true; // hareket kilidi

    public PlayerMutationState(PlayerController player, PlayerStateMachine stateMachine) 
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        // hareketi bir kez kilitle
        movementLocked = true;
        player.RB.linearVelocity = Vector3.zero;

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(SoundManager.Instance.Eating, player.gameObject);
    
        player.AnimationEvents.SetAnimationTrigger("Bite");

        EnemyHealth targetEnemy = player.CurrentDeadEnemy; 
        
        // Bite animasyonu
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

            // Mutation animasyonu
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
            // Yalnızca z ekseni sıfırlanıyor, x ekseni yok zaten
            player.RB.linearVelocity = new Vector3(0, player.RB.linearVelocity.y, 0);
        }
    }

    private void FinishMutation()
    {
        movementLocked = false; // hareket açıldı
        stateMachine.ChangeState(player.IdleState);
    }

    public override void Exit()
    {
        base.Exit();
        if (mutationTween != null) mutationTween.Kill();
        movementLocked = false;
    }
}

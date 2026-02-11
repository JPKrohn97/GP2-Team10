using DG.Tweening;
using UnityEngine;

public class PlayerMutationState : PlayerState
{
    private float biteDuration = 2f;
    private float mutationDuration = 2f;

    private Tween biteTween;
    private Tween mutationFinishTween;

    public PlayerMutationState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        player.RB.linearVelocity = Vector3.zero;

        SoundManager.Instance?.PlaySound(SoundManager.Instance.Eating, player.gameObject);
        player.AnimationEvents.SetAnimationTrigger("Bite");

        EnemyHealth targetEnemy = player.CurrentDeadEnemy;

        biteTween = DOVirtual.DelayedCall(biteDuration, () =>
        {
            if (stateMachine.CurrentState != this) return;

            if (targetEnemy != null)
            {
                var type = targetEnemy.mutationType;

                player.SkillController?.AbsorbSkill(type);
                targetEnemy.ConsumeBody();

                player.AnimationEvents.MutationSequence(type);
            }

            mutationFinishTween = DOVirtual.DelayedCall(mutationDuration, () =>
            {
                if (stateMachine.CurrentState != this) return;
                FinishMutation();
            });
        });
    }
    
    public override void LogicUpdate()
    {

    }

    public override void PhysicsUpdate()
    {
        player.RB.linearVelocity = Vector3.zero;
    }
    
    private void FinishMutation()
    {
        stateMachine.ChangeState(player.IdleState);
    }

    public override void Exit()
    {
        base.Exit();
        biteTween?.Kill();
        mutationFinishTween?.Kill();
    }
}
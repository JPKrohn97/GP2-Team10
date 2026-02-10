using UnityEngine;

public class PlayerSwordAttackState : PlayerState
{
    private int currentComboStep;
    private int maxSwordLevel;

    private float lastAttackStartTime;
    private bool waitingForNextStep; 

    public PlayerSwordAttackState(PlayerController player, PlayerStateMachine stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        player.RB.linearVelocity = Vector3.zero;
        player.AnimationEvents.SetMovingBool(false);

        currentComboStep = 0;
        maxSwordLevel = player.SkillController.GetSkillLevel(
            EnemyHealth.EnemyMutationType.Sword
        );

        if (maxSwordLevel <= 0)
            maxSwordLevel = 1;

        player.AnimationEvents.ShowSwordVisuals();

        PlayCurrentStep();
    }

    public override void Exit()
    {
        base.Exit();

        player.AnimationEvents.HideSwordVisuals();
        player.CurrentWeapon = PlayerController.ActiveWeaponType.Claw;

        //player.Combat.DisableRightSwordAttackCollider();
    
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();


        if (Time.time < lastAttackStartTime + 0.10f)
            return;

        if (!waitingForNextStep)
            return;

        AnimatorStateInfo info = player.Animator.GetCurrentAnimatorStateInfo(0);
        
        bool animationFinished = info.normalizedTime >= 0.5f; 
        
    
        if (animationFinished) 
        {
            waitingForNextStep = false;

            currentComboStep++;

            if (currentComboStep >= maxSwordLevel)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                PlayCurrentStep();
            }
        }
    }

    private void PlayCurrentStep()
    {
        lastAttackStartTime = Time.time;
        waitingForNextStep = true;

        if (player.CurrentMovementInput != Vector2.zero)
        {
            Vector3 dir = new Vector3(
                player.CurrentMovementInput.x,
                0,
                player.CurrentMovementInput.y
            );
            player.transform.rotation = Quaternion.LookRotation(dir);
        }

        player.AnimationEvents.PlaySwordComboAnimation(
            currentComboStep,
            maxSwordLevel
        );

        player.Combat.PerformAttackStep();
    }
    
    public void OnAttackInput() { }
}

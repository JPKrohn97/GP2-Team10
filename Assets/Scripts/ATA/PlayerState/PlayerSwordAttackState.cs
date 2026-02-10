using UnityEngine;

public class PlayerSwordAttackState : PlayerState
{
    private int currentComboStep;
    private int maxSwordLevel;

    private float lastAttackStartTime;
    private bool waitingForNextStep; // 🔥 KİLİT

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

        player.Combat.DisableLeftAttackCollider();
        player.Combat.DisableRightAttackCollider();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        // Çok hızlı geçişleri önlemek için güvenlik (Buraya dokunma, bu iyi)
        if (Time.time < lastAttackStartTime + 0.15f)
            return;

        if (!waitingForNextStep)
            return;

        AnimatorStateInfo info = player.Animator.GetCurrentAnimatorStateInfo(0);

        // DEĞİŞİKLİK BURADA:
        // 0.95f yerine 0.75f (veya deneme yanılma ile 0.7f) yap.
        // Böylece karakter kılıcı tam indirdiğinde beklemeden diğerine geçer.
        bool animationFinished = info.normalizedTime >= 0.75f; 

        // Not: !player.Animator.IsInTransition(0) kontrolünü kaldırmayı deneyebilirsin
        // eğer geçişlerde hala takılma varsa. Ama şimdilik kalsın, sadece süreyi kısalttık.
    
        if (animationFinished) 
        {
            waitingForNextStep = false;

            currentComboStep++;

            if (currentComboStep >= maxSwordLevel)
            {
                // Combo bitti, Idle'a dön
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                // Beklemeden diğer vuruşa geç
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

    // Input kullanılmıyor (skill otomatik akıyor)
    public void OnAttackInput() { }
}

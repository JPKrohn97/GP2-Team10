using UnityEngine;
using DG.Tweening; // DOTween kütüphanesi

public class PlayerMutationState : PlayerState
{
    // AYARLAR
    private float biteDuration = 2f;     // Isırma animasyonu ne kadar sürüyor?
    private float mutationDuration = 2.0f; // Efektler/Dönüşüm ne kadar sürüyor?

    public PlayerMutationState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        
        
        
        // 1. HIZI SIFIRLA (Girişte kaymayı önle)
        player.RB.linearVelocity = Vector3.zero;
        
        // 2. ISIRMA ANİMASYONU
        player.AnimationEvents.SetAnimationTrigger("Bite");
        
        // --- AŞAMA 1: ISIRMA SÜRESİ KADAR BEKLE ---
        DOVirtual.DelayedCall(biteDuration, () =>
        {
            // State değişmediyse devam et
            if(stateMachine.CurrentState == this)
            {
                // A) CESEDİ YOK ET (Artık yendi)
                if (player.CurrentDeadEnemy != null)
                {
                    player.CurrentDeadEnemy.ConsumeBody();
                }

                // B) MUTASYON EFEKTLERİNİ BAŞLAT
                player.AnimationEvents.MutationSequence();
           
                // --- AŞAMA 2: MUTASYON BİTENE KADAR BEKLE (KİLİT NOKTA BURASI) ---
                // Eskiden burada direkt Finish diyordun, o yüzden kayıyordu.
                // Şimdi efekt süresi kadar daha bekletiyoruz.
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

    // Karakterin işlem bitmeden itilmesini/kaymasını önlemek için
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        // Sadece yerçekimi çalışsın, sağa sola gitmesin
        player.RB.linearVelocity = new Vector3(0, player.RB.linearVelocity.y, 0);
    }

    private void FinishMutation() => stateMachine.ChangeState(player.IdleState);
}
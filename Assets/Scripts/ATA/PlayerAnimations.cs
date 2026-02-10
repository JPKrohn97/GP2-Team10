using System;
using UnityEngine;
using DG.Tweening; 

public class PlayerAnimations : MonoBehaviour
{
    [Header("References")]
    private PlayerController player;
    private Animator animator;
    
    [Header("Visuals")]
    public MeshRenderer weaponMeshRenderer;
    public Material weaponMaterial;

    [Header("BossUpgradeVisuals")]
    public MeshRenderer bossLegsMeshRenderer;
    public Material bossLegsMaterial;
    
    [Header("VFX Spawn Points")]
    [SerializeField] private Transform clawSlashPoint; 
    [SerializeField] private Transform swordSlashPoint;


    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int UpgradeHash = Animator.StringToHash("Upgrade");
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int RunHash = Animator.StringToHash("Run");
    private static readonly int BiteHash = Animator.StringToHash("Bite");
    

    private static readonly int Attack0Hash = Animator.StringToHash("Attack0");
    private static readonly int Attack1Hash = Animator.StringToHash("Attack1");
    private static readonly int Attack2Hash = Animator.StringToHash("Attack2");

    private static readonly int BossMutationHash = Animator.StringToHash("BossMutation");


    void Awake()
    {
        player = GetComponentInParent<PlayerController>();
        animator = GetComponent<Animator>();

        if (weaponMeshRenderer != null)
        {
            weaponMaterial = weaponMeshRenderer.material;
        }

        DisableLeftAttackColliderEvent();
        DisableRightAttackColliderEvent();
    }

    private void Start()
    {
        if (Application.isMobilePlatform)
        {
            animator.speed = 1.0f; 
        }
    }


    public void MutationSequence(EnemyHealth.EnemyMutationType type)
    {
        animator.SetTrigger(UpgradeHash);
    
        if (ManagerCinemachine.Instance != null)
            ManagerCinemachine.Instance.SetMutationCamera();

        if (type == EnemyHealth.EnemyMutationType.Sword)
        {
            ShowSwordVisuals();

            DOVirtual.DelayedCall(2f, () =>
            {
                if (weaponMeshRenderer != null) 
                    weaponMeshRenderer.material = weaponMaterial;
            });
        }

        DOVirtual.DelayedCall(2f, () =>
        {
            animator.SetTrigger(IdleHash);
        
            if (ManagerCinemachine.Instance != null)
                ManagerCinemachine.Instance.SetNormalCamera();
        });
    }
    
    public void ShowSwordVisuals()
    {
        if (weaponMaterial == null) return;
        
        DOTween.To(() => weaponMaterial.GetFloat("_DissolveAmount"),
            x => weaponMaterial.SetFloat("_DissolveAmount", x),
            0f, 1f).SetEase(Ease.OutSine);
    }
    
    public void HideSwordVisuals()
    {
        if (weaponMaterial == null) return;
        
        DOTween.To(() => weaponMaterial.GetFloat("_DissolveAmount"),
            x => weaponMaterial.SetFloat("_DissolveAmount", x),
            1f, 0.5f).SetEase(Ease.InSine);
    }

    public void MutationSequence(bool isBoss)
    {
        animator.SetTrigger(BossMutationHash);

        if (ManagerCinemachine.Instance != null)
            ManagerCinemachine.Instance.SetMutationCamera();

        if (weaponMaterial != null)
        {
            DOTween.To(() => bossLegsMaterial.GetFloat("_DissolveAmount"),
                       x => bossLegsMaterial.SetFloat("_DissolveAmount", x),
                       0f,
                       1.7f).SetEase(Ease.OutSine);
        }

        DOVirtual.DelayedCall(2f, () =>
        {
            if (bossLegsMeshRenderer != null) bossLegsMeshRenderer.material = bossLegsMaterial;

            animator.SetTrigger(IdleHash);

            if (ManagerCinemachine.Instance != null)
                ManagerCinemachine.Instance.SetNormalCamera();
        });
    }

    public void PlayComboAnimation(int step)
    {
        animator.ResetTrigger(Attack0Hash);
        animator.ResetTrigger(Attack1Hash);
        animator.ResetTrigger(Attack2Hash);
        
        if (player != null && player.Combat != null) player.Combat.IsFinalComboActive = false; 
        

        switch (step)
        {
            case 0: 
                animator.SetTrigger(Attack0Hash);
                if(ManagerCinemachine.Instance != null)
                    ManagerCinemachine.Instance.HitImpact(0.05f, 0.1f); 
                break;

            case 1:
                animator.SetTrigger(Attack1Hash);
                if(ManagerCinemachine.Instance != null)
                    ManagerCinemachine.Instance.HitImpact(0.05f, 0.1f);
                break;

            case 2: 
                animator.SetTrigger(Attack2Hash);
                if (player != null && player.Combat != null)
                    player.Combat.IsFinalComboActive = true; 
                break;
        }
    }

    public void PlaySwordComboAnimation(int step, int swordLevel)
    {
        if (step >= swordLevel) return;

        ResetAllAttackTriggers();

        string trigger = $"Sword_Attack{step}";
        animator.SetTrigger(trigger);

        bool isFinal = (step == swordLevel - 1);
        
        if (player != null && player.Combat != null)
            player.Combat.IsFinalComboActive = isFinal;

        if (ManagerCinemachine.Instance != null)
        {
            if (isFinal)
                ManagerCinemachine.Instance.HitImpact(0.12f, 0.25f);
            else
                ManagerCinemachine.Instance.HitImpact(0.05f, 0.1f);
        }
    }
    public void ClawSlashVFXEvent()
    {
        // Ses
        SoundManager.Instance?.PlaySound(SoundManager.Instance.ClawsAttack, gameObject);
        
        // Görsel Efekt
        if (ManagerObjectPool.Instance != null && clawSlashPoint != null)
        {
            Quaternion vfxRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
            GameObject vfx = ManagerObjectPool.Instance.Spawn(ObjectPoolType.ClawSlash, clawSlashPoint.position, vfxRotation);
            vfx.GetComponent<ParticleSystem>().Play();
        }
    }

    public void SwordSlashVFXEvent()
    {
        SoundManager.Instance?.PlaySound(SoundManager.Instance.SwordAttack, gameObject);

        // Görsel Efekt
        if (ManagerObjectPool.Instance != null && swordSlashPoint != null)
        {
            Quaternion vfxRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
            GameObject vfx = ManagerObjectPool.Instance.Spawn(ObjectPoolType.SwordSlash, swordSlashPoint.position, vfxRotation);
            vfx.GetComponent<ParticleSystem>().Play();
        }
    }
    

    public void ResetAllAttackTriggers()
    {
        animator.ResetTrigger("Sword_Attack0");
        animator.ResetTrigger("Sword_Attack1");
        animator.ResetTrigger("Sword_Attack2");
        animator.ResetTrigger(Attack0Hash);
        animator.ResetTrigger(Attack1Hash);
        animator.ResetTrigger(Attack2Hash);
    }

    public void SetAnimationTrigger(string stateName) => animator.SetTrigger(stateName);
    public void SetAnimationTrigger(int stateHash) => animator.SetTrigger(stateHash);
    public void SetMovingBool(bool moving) => animator.SetBool(IsMovingHash, moving);
    public void TriggerStartRun() => animator.SetTrigger(RunHash);

    public void EnableLeftAttackColliderEvent()
    {
        if(player != null && player.Combat != null) 
            player.Combat.EnableLeftAttackCollider();
    }
    
    public void RangeAttackEvent()
    {
        SoundManager.Instance?.PlaySound(SoundManager.Instance.RangedAttack, gameObject);
        
        if (player != null)
        {
            player.SpawnProjectile();
        }
    }
    
    public void DisableLeftAttackColliderEvent()
    {
        if(player != null && player.Combat != null) 
            player.Combat.DisableLeftAttackCollider();
    }

    public void EnableRightAttackColliderEvent()
    {
        if(player != null && player.Combat != null) 
            player.Combat.EnableRightAttackCollider();
    }

    public void DisableRightAttackColliderEvent()
    {
        if(player != null && player.Combat != null) 
            player.Combat.DisableRightAttackCollider();
    }

    public void BiteParticeEvent()
    {
        if(player != null && player.Combat != null) 
            player.Combat.SpawnBiteParticle();
        if(SoundManager.Instance != null)
            SoundManager.Instance.PlaySound(SoundManager.Instance.Eating);
    }
}
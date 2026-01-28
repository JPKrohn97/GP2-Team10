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


    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int UpgradeHash = Animator.StringToHash("Upgrade");
    private static readonly int IdleHash = Animator.StringToHash("Idle");
    private static readonly int RunHash = Animator.StringToHash("Run");
    private static readonly int BiteHash = Animator.StringToHash("Bite");
    

    private static readonly int Attack0Hash = Animator.StringToHash("Attack0");
    private static readonly int Attack1Hash = Animator.StringToHash("Attack1");
    private static readonly int Attack2Hash = Animator.StringToHash("Attack2");

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


    public void MutationSequence()
    {
        animator.SetTrigger(UpgradeHash);
        
   
        if (ManagerCinemachine.Instance != null)
            ManagerCinemachine.Instance.SetMutationCamera();


        if (weaponMaterial != null)
        {
            DOTween.To(() => weaponMaterial.GetFloat("_DissolveAmount"),
                       x => weaponMaterial.SetFloat("_DissolveAmount", x),
                       0f,
                       1.7f).SetEase(Ease.OutSine);
        }

   
        DOVirtual.DelayedCall(2f, () =>
        {
            if (weaponMeshRenderer != null) weaponMeshRenderer.material = weaponMaterial;
            
     
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
        

        if (player != null) player.IsFinalComboActive = false; 

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
                if (player != null)
                    player.IsFinalComboActive = true; 
                break;
        }
    }


    public void SetAnimationTrigger(string stateName)
    {

        animator.SetTrigger(stateName);
    }

    public void SetAnimationTrigger(int stateHash)
    {
        animator.SetTrigger(stateHash);
    }

    public void SetMovingBool(bool moving)
    {
        animator.SetBool(IsMovingHash, moving);
    }

    public void TriggerStartRun()
    {
        animator.SetTrigger(RunHash);
    }


    
    public void EnableLeftAttackColliderEvent()
    {
        if(player != null && player.Combat != null) 
            player.Combat.EnableLeftAttackCollider();
    }
    
    public void RangeAttackEvent()
    {
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
    }
}
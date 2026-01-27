using DG.Tweening;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Player player;
    private Animator animator;
    public Collider attackCollider1;
    public Collider attackCollider2;

    public MeshRenderer weaponMeshRenderer;
    public Material weaponMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weaponMaterial=weaponMeshRenderer.material;
        player = GetComponentInParent<Player>();
        animator = GetComponent<Animator>();
        DisableLeftAttackColliderEvent();
        DisableRightAttackColliderEvent();
    }
    public void MutationSequence ()
    {
        animator.SetTrigger("Upgrade");
        ManagerCinemachine.Instance.SetMutationCamera();
        DOTween.To(() => weaponMaterial.GetFloat("_DissolveAmount"),
                               x => weaponMaterial.SetFloat("_DissolveAmount", x),
                               0f,
                               1.7f).SetEase(Ease.OutSine); 
        DOVirtual.DelayedCall(2f, () =>
        {
            weaponMeshRenderer.material = weaponMaterial;
            player.playerMovement.canMove = true;
            animator.SetTrigger("Idle");
            ManagerCinemachine.Instance.SetNormalCamera();
        });
    }

    public void PlayComboAnimation(int step)
    {

        animator.ResetTrigger("Attack0");
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");

        animator.SetTrigger("Attack" + step);
    }
    public void SetAnimationTrigger(string state)
    {
        animator.SetTrigger(state);
    }
    public void EnableLeftAttackColliderEvent()
    {
        player.playerCombat.EnableLeftAttackCollider();
    }
    public void DisableLeftAttackColliderEvent()
    {
        player.playerCombat.DisableLeftAttackCollider();
    }
    public void BiteParticeEvent()
    {
        player.playerCombat.SpawnBiteParticle();
       
    }

    public void EnableRightAttackColliderEvent()
    {
        player.playerCombat.EnableRightAttackCollider();
    }
    public void DisableRightAttackColliderEvent()
    {
        player.playerCombat.DisableRightAttackCollider();
    }
    public void TriggerStartRun()
    {
        animator.SetTrigger("Run");
    }

    public void SetMovingBool(bool moving)
    {
        animator.SetBool("IsMoving", moving);
    }
}

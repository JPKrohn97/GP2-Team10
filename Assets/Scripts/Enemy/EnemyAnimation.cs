using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamageAnim()
    {
        animator.SetTrigger("TakeDamage");
    }

    public void PlayComboAnimation(int step)
    {

        animator.ResetTrigger("Attack0");
        animator.ResetTrigger("Attack1");
        animator.ResetTrigger("Attack2");

        animator.SetTrigger("Attack" + step);
    }
    public void RangedAnimation()
    {
        animator.SetTrigger("RangedAttack");
    }
    public void SetAnimationTrigger(string state)
    {
        animator.SetTrigger(state);
    }
    public void EnableAttackColliderEvent()
    {
    }
    public void DisableAttackColliderEvent()
    {
    }
    public void TriggerStartRun()
    {
        animator.SetTrigger("Run");
    }

    public void HitReaction(int i)
    {
        animator.SetTrigger("Hit" + i);
    }
    public void SetMovingBool(bool moving)
    {
        animator.SetBool("IsMoving", moving);
    }
}

using System.Collections;
using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class ManagerCinemachine : Singleton<ManagerCinemachine>
{
    public Animator animator;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private float power = 10f;
    public void SetMutationCamera()
    {
        animator.SetTrigger("MutationCamera");
    }
    public void SetNormalCamera()
    {
        animator.SetTrigger("NormalCamera");
    }
    public void SetBossMutationCamera()
    {
        animator.SetTrigger("BossMutationCamera");
    }
    public void FirstBiteCamera()
    {
        animator.SetTrigger("FirstBiteCamera");
    }
    public void SecondBiteCamera()
    {
        animator.SetTrigger("SecondBiteCamera");
    }
    public void Narrative()
    {
        animator.SetTrigger("NarrativeCamera");
    }
    //public void CinemachineShake(float intensity, float time)
    //{
    //    animator.SetFloat("ShakeIntensity", intensity);
    //    animator.SetFloat("ShakeTime", time);
    //    animator.SetTrigger("Shake");
    //}
    public void ShakeOnHit()
    {
        if (impulseSource == null) return;

        impulseSource.GenerateImpulseWithForce(power);
    }
    public void ShakeOnHit(float p)
    {
        if (impulseSource == null) return;

        impulseSource.GenerateImpulseWithForce(p);
    }
    public void HitImpact(float duration, float speed = 0f)
    {
        Time.timeScale = speed;
        DOVirtual.DelayedCall(duration, () => Time.timeScale = 1f, true);
    }
    
    public void TriggerFinisherCamera()
    {
        StartCoroutine(FinisherRoutine());
    }

    IEnumerator FinisherRoutine()
    {
        animator.SetTrigger("HitCamera");

        Time.timeScale = 0.2f;

        //Time.fixedDeltaTime *= 0.02f;
        
        yield return new WaitForSecondsRealtime(0.9f);
        
        //Time.fixedDeltaTime /= 0.02f;

        Time.timeScale = 1f;
        animator.SetTrigger("NormalCamera");
    }
    
    
}

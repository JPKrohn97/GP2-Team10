using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ManagerCinemachine : Singleton<ManagerCinemachine>
{
    public Animator animator;

    public void SetMutationCamera()
    {
        animator.SetTrigger("MutationCamera");
    }
    public void SetNormalCamera()
    {
        animator.SetTrigger("NormalCamera");
    }

    //public void CinemachineShake(float intensity, float time)
    //{
    //    animator.SetFloat("ShakeIntensity", intensity);
    //    animator.SetFloat("ShakeTime", time);
    //    animator.SetTrigger("Shake");
    //}
    
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
        // A. Hit Kamerasına geç (Zoom yapar)
        animator.SetTrigger("HitCamera");

        // B. Zamanı yavaşlat (Matrix efekti)
        Time.timeScale = 0.1f;

        // C. Bekle (Gerçek hayatta 0.2 sn, oyunda çok daha uzun hissettirir)
        yield return new WaitForSecondsRealtime(0.6f);

        // D. Normale dön (Normal kameraya geç ve zamanı düzelt)
        Time.timeScale = 1f;
        animator.SetTrigger("NormalCamera");
    }
    
    
}

using UnityEngine;

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
    public void CinemachineCameraShake()
    {

    }
}

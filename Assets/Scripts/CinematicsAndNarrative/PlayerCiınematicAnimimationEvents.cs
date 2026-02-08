using UnityEngine;
using DG.Tweening;
public class PlayerCiınematicAnimimationEvents : MonoBehaviour
{

    public GameObject fakeBoss;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void FadeIn()
    {
        GameManager.Instance.CinematicBlackFadeIn(0.5f);
        
    }
    public void FadeOut()
    {
        GameManager.Instance.CinematicBlackFadeOut(0.5f);
    }
    public void FirstBite()
    {

    }
    public void Beginning()
    {
        fakeBoss.SetActive(true);
        DOVirtual.DelayedCall(0.5f, () => { ManagerCinemachine.Instance.FirstBiteCamera(); });

    }
    public void End()
    {
        fakeBoss.SetActive(false);
    }
}

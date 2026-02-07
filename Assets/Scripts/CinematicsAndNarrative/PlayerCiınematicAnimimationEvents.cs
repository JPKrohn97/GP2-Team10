using UnityEngine;
using DG.Tweening;
public class PlayerCiınematicAnimimationEvents : MonoBehaviour
{
    public Transform fakeBossBody;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void FirstBite()
    {
        GameManager.Instance.CinematicBlackFadeIn(0.5f);

    }

    public void FadeIn()
    {
        GameManager.Instance.CinematicBlackFadeIn(0.5f);
    }
    public void FadeOut()
    {
        GameManager.Instance.CinematicBlackFadeOut(0.5f);
    }
    public void SecondBite()
    {

    }
    public void End()
    {

    }

}

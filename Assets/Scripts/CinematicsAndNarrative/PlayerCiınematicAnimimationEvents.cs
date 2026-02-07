using UnityEngine;

public class PlayerCiınematicAnimimationEvents : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void FirstBite()
    {
        GameManager.Instance.CinematicBlackFadeIn(0.5f);
    }

    public void FadeIn()
    {
        GameManager.Instance.CinematicBlackFadeIn(0.5f);
    }
    public void FadOut()
    {
        GameManager.Instance.CinematicBlackFadeOut(0.5f);
    }
    public void SecondBite()
    {

    }

}

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CinematicTrigger : MonoBehaviour
{
    public string[] RandomLines1;
    public string[] RandomLines2;
    public string[] RandomLines3;
    public string[] RandomLines4;
    public string[] RandomLines5;
    public string[] RandomLines6;
    private bool wasTriggered = false;
    private Canvas canvas;
    private TextMeshProUGUI uiText;

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;
        uiText = GetComponentInChildren<TextMeshProUGUI>();
        uiText.enabled = false;
        uiText.text = string.Empty;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !wasTriggered)
        {
            StartCoroutine(NarrativeCinematic(other.gameObject));
        }
    }

    IEnumerator NarrativeCinematic(GameObject player)
    {
        wasTriggered = true;
        canvas.enabled = true;
        uiText.enabled = true;

        var text = RandomLines1[Random.Range(0, RandomLines1.Length - 1)] + System.Environment.NewLine + System.Environment.NewLine +
            RandomLines2[Random.Range(0, RandomLines2.Length - 1)] + System.Environment.NewLine + System.Environment.NewLine +
            RandomLines3[Random.Range(0, RandomLines3.Length - 1)] + System.Environment.NewLine + System.Environment.NewLine +
            RandomLines4[Random.Range(0, RandomLines4.Length - 1)] + System.Environment.NewLine + System.Environment.NewLine +
            RandomLines5[Random.Range(0, RandomLines5.Length - 1)] + System.Environment.NewLine + System.Environment.NewLine +
            RandomLines6[Random.Range(0, RandomLines6.Length - 1)];

        uiText.text = text;
        float duration = text.Length / 25 < 5 ? 5 : text.Length / 25;

        Debug.Log($"Message will be destroyed in {duration} seconds.");

        player.GetComponent<PlayerController>()?.TriggerNarrative();
        GetComponent<BoxCollider>().enabled = false;

        yield return new WaitForSeconds(duration);
        player.GetComponent<PlayerController>()?.UntriggerNarrative();
        Destroy(gameObject);
    }
}
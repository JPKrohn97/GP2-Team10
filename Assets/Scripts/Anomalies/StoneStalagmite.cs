using UnityEngine;

public class StoneStalagmite : MonoBehaviour
{
    [Header("References")]
    public Transform spike;
    public GameObject groundHint;

    [Header("Settings")]
    public float riseSpeed = 6f;
    public float activeTime = 1.5f;

    private bool activated = false;
    private Vector3 hiddenPosition;
    private Vector3 visiblePosition;

    void Start()
    {
        hiddenPosition = spike.localPosition;
        visiblePosition = hiddenPosition + Vector3.up * 1.2f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;
            groundHint.SetActive(false);
            StartCoroutine(RiseSpike());
        }
    }

    System.Collections.IEnumerator RiseSpike()
    {
        float t = 0f;
        while (t < 1f)
        {
            spike.localPosition = Vector3.Lerp(hiddenPosition, visiblePosition, t);
            t += Time.deltaTime * riseSpeed;
            yield return null;
        }

        spike.localPosition = visiblePosition;

        yield return new WaitForSeconds(activeTime);

        Destroy(gameObject);
    }
}

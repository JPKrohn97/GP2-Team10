using UnityEngine;
using System.Collections;

public class StoneStalagmite : MonoBehaviour
{
    public Transform spike;
    public float riseHeight = 1.2f;
    public float riseSpeed = 6f;
    public float activeTime = 1.5f;

    private Vector3 hiddenPos;
    private Vector3 visiblePos;
    private bool activated;

    void Start()
    {
        if (spike == null)
        {
            Debug.LogError("StoneStalagmite: spike is NOT assigned!", this);
            enabled = false;
            return;
        }

        hiddenPos = spike.position;
        visiblePos = hiddenPos + Vector3.up * riseHeight;
    }

    public void Activate()
    {
        if (activated) return;
        activated = true;

        Debug.Log("Activate called: rising spike", this);
        StartCoroutine(RiseSpike());
    }

    IEnumerator RiseSpike()
    {
        while (Vector3.Distance(spike.position, visiblePos) > 0.01f)
        {
            spike.position = Vector3.MoveTowards(
                spike.position,
                visiblePos,
                riseSpeed * Time.deltaTime
            );
            yield return null;
        }

        yield return new WaitForSeconds(activeTime);
        Destroy(gameObject);
    }
}

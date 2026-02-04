using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class VolcanicLavaPool : MonoBehaviour
{
    [Header("Movement")]
    public float moveRadius = 3f;
    public float moveSpeed = 2f;

    [Header("Damage Over Time")]
    public int damagePerTick = 5;
    public float damageInterval = 1f;

    private Vector3 startPos;
    private Vector3 targetPos;

    private HashSet<IDamageable> targetsInside = new HashSet<IDamageable>();

    void Start()
    {
        startPos = transform.position;
        PickNewPosition();
        StartCoroutine(DamageRoutine());
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            PickNewPosition();
    }

    void PickNewPosition()
    {
        Vector3 offset = new Vector3(
            Random.Range(-moveRadius, moveRadius),
            0f,
            Random.Range(-moveRadius, moveRadius)
        );

        targetPos = startPos + offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
            targetsInside.Add(dmg);
    }

    private void OnTriggerExit(Collider other)
    {
        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg != null)
            targetsInside.Remove(dmg);
    }

    IEnumerator DamageRoutine()
    {
        while (true)
        {
            foreach (var target in targetsInside)
            {
                if (target != null)
                    target.TakeDamage(damagePerTick);
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(moveRadius, moveRadius, moveRadius));
    }
}

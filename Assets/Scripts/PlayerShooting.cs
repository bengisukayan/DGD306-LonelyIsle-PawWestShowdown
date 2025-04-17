using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Transform gunPoint;
    public float fireRate = 0.2f; // Delay between shots

    public GameObject hitEffect;
    public int damage = 40;
    public float range = 50f;
    public LineRenderer lineRenderer;
    public LayerMask playerLayer;

    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            StartCoroutine(Shoot());
            nextFireTime = Time.time + fireRate; // Set cooldown
        }
    }

    IEnumerator Shoot()
    {
        RaycastHit2D hit = Physics2D.Raycast(gunPoint.position, gunPoint.right, range, ~playerLayer);

        if (hit)
        {
            if (hit.collider.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(damage);
            }

            Instantiate(hitEffect, hit.point, Quaternion.identity);
            lineRenderer.SetPosition(0, gunPoint.position);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(0, gunPoint.position);
            lineRenderer.SetPosition(1, gunPoint.position + gunPoint.right * 100);
        }

        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.02f);
        lineRenderer.enabled = false;
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GodfatherCatBoss : Enemy
{
    public Transform[] balconyPoints;
    public float jumpInterval = 3f;
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float dizzyChance = 0.2f;
    public float dizzyDuration = 2f;
    public int maxHealth = 150;

    private int currentBalcony = 0;
    private float nextJumpTime;
    private bool isDizzy = false;
    private int currentHealth;

    void Start()
    {
        nextJumpTime = Time.time + jumpInterval;
        currentHealth = maxHealth;
        transform.position = balconyPoints[0].position;
    }

    void Update()
    {
        if (isDizzy) return;

        if (Time.time >= nextJumpTime)
        {
            if (Random.value < dizzyChance)
            {
                FallToGround();
                return;
            }

            JumpToNextBalcony();
            nextJumpTime = Time.time + jumpInterval;
            Shoot();
        }
    }

    void JumpToNextBalcony()
    {
        currentBalcony = (currentBalcony + 1) % balconyPoints.Length;
        transform.position = balconyPoints[currentBalcony].position;
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
    }

    void FallToGround()
    {
        isDizzy = true;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Falls to ground
        Invoke("RecoverFromDizzy", dizzyDuration);
    }

    void RecoverFromDizzy()
    {
        isDizzy = false;
        transform.position = balconyPoints[currentBalcony].position;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingStar : Weapon
{
    public float throwingSpeed;
    public bool hasBeenThrown;
    public float distanceFromPlayer = 0.9f; // Distance of the projectile from the player
    public float destroyTime = 20.0f;
    private bool hasCollided = false;
    public float damage = 10;
    public float throwCooldown;
    public float rotationSpeed = 3f;

    public bool rotate;
    

    // Start is called before the first frame update
    void Start()
    {
        type = "ThrowingStar";
        hasBeenThrown = false;
        player = FindAnyObjectByType<PlayerController>();
        playerTransform = player.transform;
        upgradeSystem = player.GetComponentInChildren<UpgradeSystem>();

        // Disable the collision by default to prevent collision with other stars
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        rotate = false;
    }

    // Update is called once per frame
    void Update()
    {
        ThrowingStarRotation();

        if (!hasCollided) // Only reduce destroyTime if arrow hasn't collided
        {
            destroyTime -= Time.deltaTime;

            if (destroyTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void ThrowingStarRotation()
    {
        if (hasBeenThrown) return;
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector2 direction = new Vector2(mousePos.x - playerTransform.position.x, mousePos.y - playerTransform.position.y);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Vector3 StarPosition = playerTransform.position + (Vector3)direction.normalized * distanceFromPlayer;

        // Set the bow's position and rotation
        transform.position = StarPosition;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        rotate = false;

        if (!hasCollided)
        {
            hasCollided = true; // Mark as collided to prevent repeated triggers

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true; 
            }
            Destroy(GetComponent<Rigidbody2D>());

            // Disable the collider to prevent further collisions
            Collider2D collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            transform.parent = collision.transform;

            // Start the timer to destroy the arrow after 5 seconds
            StartCoroutine(DestroyAfterDelay(5.0f));
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Bow bow;
    public Bow bowInstance;
    public GameObject arrowPrefab;
    public ThrowingStar throwingStar;
    public ThrowingStar throwingStarInstance;
    public Bananarang bananarang;
    public Bananarang bananarangInstance;

    public PlayerController player;
    private float holdTime;
    public bool canShoot = true;
    private const float holdTimeToShoot = .2f; // adjust this value as needed
    public Weapon currentWeapon;
    public Weapon currentWeaponInstance;

    [SerializeField] private AudioClip shootBow;

    public void Start()
    {
        currentWeapon = bow;
        bowInstance = Instantiate(bow, player.transform.position, Quaternion.identity);
        currentWeaponInstance = bowInstance;
        currentWeapon.type = "Bow";
    }

    public void Update()
    {
        if (currentWeaponInstance.type == "Bananarang")
        {
            if (bananarangInstance.rotate == true) 
            {
                bananarangInstance.transform.Rotate(Vector3.forward * bananarang.rotationSpeed); 
            }
            if (bananarangInstance.beingThrown && !bananarangInstance.returningToPlayer) ReduceReturnTime(); 

            if (bananarangInstance.returningToPlayer)
            {
                ReturnToPlayer();
            }
        }
    }

    public void CheckWeaponInput(bool canDash)
    {
        if (currentWeapon.type == "Bow")
        {
            BowInput(canDash);
        }
        else if (currentWeapon.type == "ThrowingStar")
        {
            ThrowingStarInput();
        }
        else if (currentWeapon.type == "Bananarang")
        {
            BananarangInput();
        }
    }

    private void BananarangInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && canShoot)
        {
            if (!bananarangInstance.beingThrown) ThrowBanana();
        }
    }

    private void ThrowBanana()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the z-coordinate is 0 since we're working in 2D

        // Calculate the direction from the player to the mouse position
        Vector3 direction = (mousePosition - player.transform.position).normalized;

        // Calculate the rotation angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Set the velocity
        Vector2 velocity = new Vector2(direction.x, direction.y);

        bananarangInstance.beingThrown = true;

        bananarangInstance.GetComponent<Rigidbody2D>().velocity = velocity * bananarang.throwingSpeed;

        bananarangInstance.rotate = true;

    }

    public void ReduceReturnTime()
    {
        if (bananarangInstance.returnTime <= 0)
        {
            bananarangInstance.returningToPlayer = true;
            bananarangInstance.returnTime = 1f;
        } 
        else bananarangInstance.returnTime -= Time.deltaTime;
    }

    public void ReturnToPlayer()
    {
        // Check distance and move the Bananarang towards the player
        if (Vector2.Distance(bananarangInstance.transform.position, player.transform.position) > 0.1f)
        {
            // Calculate direction towards the player
            Vector2 directionToPlayer = (player.transform.position - bananarangInstance.transform.position).normalized;
            bananarangInstance.rb.velocity = directionToPlayer * bananarang.returnSpeed;
        }
        else
        {
            // Stop movement when close to the player
            bananarangInstance.rb.velocity = Vector2.zero;
            bananarangInstance.returningToPlayer = false; // Stop the return process
            bananarangInstance.beingThrown = false;
            bananarangInstance.rotate = false;
        }
    }

    private void ThrowingStarInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && canShoot)
        {
            ThrowStar();
        }
    }
    private void ThrowStar()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the z-coordinate is 0 since we're working in 2D

        // Calculate the direction from the player to the mouse position
        Vector3 direction = (mousePosition - player.transform.position).normalized;

        // Calculate the rotation angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Set the velocity
        Vector2 velocity = new Vector2(direction.x, direction.y);

        throwingStarInstance.hasBeenThrown = true;

        throwingStarInstance.GetComponent<Rigidbody2D>().velocity = velocity * throwingStar.throwingSpeed;

        throwingStarInstance.GetComponent<Collider2D>().enabled = true;

        throwingStarInstance = Instantiate(throwingStar, player.transform.position, Quaternion.identity);
        currentWeaponInstance = throwingStarInstance;
    }

    private void BowInput(bool canDash)
    {
        // Hold Left click
        if (Input.GetKey(KeyCode.Mouse0) && canShoot && canDash)
        {
            holdTime += Time.deltaTime;
            currentWeaponInstance.animator.SetBool("Shooting", true);
        }

        // Release Left click
        if (Input.GetKeyUp(KeyCode.Mouse0) && canShoot && canDash)
        {
            // Bow can be shot
            if (holdTime >= holdTimeToShoot)
            {
                ShootArrow();
                currentWeaponInstance.animator.SetBool("Shooting", false);
                holdTime = 0.0f; // Reset hold time after shooting
            }
            else
            {
                currentWeaponInstance.animator.SetBool("Shooting", false);
                holdTime = 0.0f; // Reset hold time if not held long enough
            }
        }
    }

    void ShootArrow()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Ensure the z-coordinate is 0 since we're working in 2D

        // Calculate the direction from the player to the mouse position
        Vector3 direction = (mousePosition - player.transform.position).normalized;

        // Calculate the rotation angle
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // Set the velocity
        Vector2 velocity = new Vector2(direction.x, direction.y);

        // Set the offset (if needed, based on the direction)
        Vector3 offset = direction * 0.15f; // Adjust this value if needed

        // Calculate the spawn position with the offset
        Vector3 spawnPosition = player.transform.position + offset;

        // Instantiate the arrow at the spawn position with the calculated rotation
        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, rotation);
        arrow.GetComponent<Rigidbody2D>().velocity = velocity * arrow.GetComponent<Arrow>().speed; 
        SoundFXManager.instance.PlaySoundFXClip(shootBow, transform, 1f, false);
    }

    public void SwitchToWeapon(String weaponName)
    {
        currentWeapon.type = weaponName;
        
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance.gameObject);
        }

        if (weaponName == "Bow")
        {
            bowInstance = Instantiate(bow, player.transform.position, Quaternion.identity);
            currentWeaponInstance = bowInstance;
        }
        else if (weaponName == "ThrowingStar")
        {
            throwingStarInstance = Instantiate(throwingStar, player.transform.position, Quaternion.identity);
            currentWeaponInstance = throwingStarInstance;
        }
        else if (weaponName == "Bananarang")
        {
            bananarangInstance = Instantiate(bananarang, player.transform.position, Quaternion.identity);
            currentWeaponInstance = bananarangInstance;
        }


    }
}

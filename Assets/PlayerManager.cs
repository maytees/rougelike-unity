using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PlayerState {
    Dead,
    Alive
}

public class PlayerManager : MonoBehaviour
{
    public int experience = 0;
     
    public float searchRadius = 5f;
    public float pickupSpeed = 5f;
    public float xpMinSpawnRadi = 10f;
    public float xpMaxSpawnRadi = 50f;
    public int xpSpawnLimit = 100;
    public GameObject xpObject;
    
    public HealthBar healthBar;
    public float playerSpeed;
        
    public PlayerState playerState = PlayerState.Alive;

    private bool canDash = true;
    private float currentDashTime;

    public float startDashTime = 1f;
    public float dashSpeed = 2f;
    public float dashCooldown = 5f;
    
    private Rigidbody2D rb;
    private TrailRenderer trailRenderer; 
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        
        healthBar.setMaxHealth(100);
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBar.slider.value <= 0)
        {
            playerState = PlayerState.Dead;
            return;
        }
        
        playerMovement(); 
        checkPickupXP();
        spawnRandomXP();
    }

    private void playerMovement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Vector2 movement = new Vector3(playerSpeed * inputX, playerSpeed * inputY, 0);

        if (canDash && Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(dash(movement));
        }

        transform.Translate(movement * Time.deltaTime);
    }

    private IEnumerator dash(Vector2 direction)
    {
        canDash = false;
        currentDashTime = startDashTime;
        trailRenderer.emitting = true; 
        
        while (currentDashTime > 0f)
        {
            currentDashTime -= Time.deltaTime;

            rb.velocity = direction * dashSpeed;

            yield return null;
        }
        
        rb.velocity = new Vector2(0f, 0f);
        
        trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    
    private void spawnRandomXP()
    {
        int xps = GameObject.FindGameObjectsWithTag("XP").Length;
        
        if ((Random.Range(0f, 1f) < 0.4 ) && xps <= xpSpawnLimit)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(xpMinSpawnRadi, xpMaxSpawnRadi);
            Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0f);
            Instantiate(xpObject, spawnPosition, Quaternion.identity);
        }

        foreach (GameObject xp in GameObject.FindGameObjectsWithTag("XP"))
        {
            if (Vector3.Distance(xp.transform.position, transform.position) > xpMaxSpawnRadi)
            {
                Destroy(xp);
            }
        }
    }
    private void checkPickupXP()
    { 
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.gameObject;

            if (hitObject.tag.Equals("XP"))
            {
                Vector3 newPos = new Vector3(transform.position.x, transform.position.y, -10f);
                hitObject.transform.position =
                    Vector3.Lerp(
                        hitObject.transform.position,
                        newPos,
                        pickupSpeed * Time.deltaTime
                        );
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("XP"))
        {
            experience++;
            Destroy(other.gameObject);
            return;
        }
        
        if (other.gameObject.name.Equals("Enemy")) 
        {
            healthBar.takeDamage(20);
        }
    }
}

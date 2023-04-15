using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PlayerState {
    Dead,
    Alive
}

public class PlayerManager : MonoBehaviour
{
    public int experience = 0;
    
    public float searchRadius = 5f;
    public float pickupSpeed = 5f;
    
    public HealthBar healthBar;

    public PlayerState playerState = PlayerState.Alive;
    
    private void Start()
    {
        healthBar.setMaxHealth(100);
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBar.slider.value <= 0)
        {
            playerState = PlayerState.Dead;
        }
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            GameObject hitObject = hitCollider.gameObject;

            if (hitObject.name.Equals("XPEntity"))
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
        if (other.gameObject.name.Equals("XPEntity"))
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

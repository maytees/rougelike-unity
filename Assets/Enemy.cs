using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public PlayerManager playerManager; 
    public GameObject player;
    
    public float speed = 2f;

    public float searchRadius = 1f;

    private bool _isCollidingWithPlayer = false;
    
    // Update is called once per frame
    void Update()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, searchRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            GameObject hit = hitCollider.gameObject;

            if (hit.gameObject.Equals(player)) 
            {
                
                if ( playerManager.playerState == PlayerState.Dead) return;
                transform.LookAt(player.gameObject.transform.position);
                transform.Rotate(new Vector3(0, -90, 0), Space.Self);

                if (_isCollidingWithPlayer) return;
                transform.position = Vector3.MoveTowards(transform.position, hit.gameObject.transform.position,
                    speed * Time.deltaTime);
            }
        }
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.Equals(player))
        {
            _isCollidingWithPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.Equals(player))
        {
            _isCollidingWithPlayer = false;
        }
    }
}

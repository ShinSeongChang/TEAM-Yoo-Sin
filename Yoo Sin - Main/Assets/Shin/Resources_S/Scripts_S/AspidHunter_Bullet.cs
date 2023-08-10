using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspidHunter_Bullet : MonoBehaviour
{
    public GameObject player = default;
    private Rigidbody2D bulletRigid = default;
    private float bulletSpeed = 5.0f;
    
    void Start()
    {
        bulletRigid = GetComponent<Rigidbody2D>();

        Vector2 offset = player.transform.position - transform.position;

        bulletRigid.velocity = offset.normalized * bulletSpeed;
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Player") || collision.tag.Equals("Platform") || collision.tag.Equals("Wall"))
        {
            transform.position = Vector2.zero;
        }
    }

}

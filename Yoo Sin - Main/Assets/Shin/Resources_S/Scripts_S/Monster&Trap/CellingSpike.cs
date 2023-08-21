using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellingSpike : MonoBehaviour
{
    private Rigidbody2D spikeRigid = default;
    private PolygonCollider2D spikeCollider = default;
    private BoxCollider2D detectedCollider = default;   

    // Start is called before the first frame update
    void Start()
    {
        spikeRigid = GetComponent<Rigidbody2D>();
        spikeCollider = GetComponent<PolygonCollider2D>();
        detectedCollider = transform.GetComponentInChildren<BoxCollider2D>(); 

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            spikeRigid.gravityScale = 2f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag.Equals("Platform"))
        {
            spikeRigid.simulated = false;
            spikeCollider.enabled = false;
            detectedCollider.enabled = false;
        }
    }
}

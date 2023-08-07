using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnighBehavior : MonoBehaviour
{
    private Animator falseKnightAni;
    private BoxCollider2D detectRange;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        detectRange = gameObject.GetComponent<BoxCollider2D>();
        player = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            player = other.gameObject;
            detectRange.enabled = false;
        }
    }

    private void ComparePosition()
    {
        if(transform.position.x - player.transform.position.x <= 0)
        {

        }
    }
}

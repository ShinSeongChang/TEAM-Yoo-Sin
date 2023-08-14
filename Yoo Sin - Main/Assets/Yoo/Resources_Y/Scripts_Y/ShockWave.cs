using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    private float speed = 10f;
    private Quaternion left = Quaternion.Euler(0, -180, 0);
    // Start is called before the first frame update
    void Start()
    {
        if(transform.rotation == left)
        {
            speed *= -1;
        }
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Wall"))
        {
            StartCoroutine(BreakObject());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Platform"))
        {
            StartCoroutine(BreakObject());
        }
    }

    IEnumerator BreakObject()
    {
        yield return null;
        Destroy(gameObject);
        yield break;
    }
}

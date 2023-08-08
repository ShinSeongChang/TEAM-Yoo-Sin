using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VengeFly : MonoBehaviour
{
    private Rigidbody2D vengeflyRigid = default;
    private Transform vengeflyTransform = default;
    private CircleCollider2D detectedArea = default;
    private SpriteRenderer vengeflySprite = default;
    private Animator vengeflyAnimator = default;

    private Vector2 moveArea = default;

    private float areaSpeed = 0.5f;
    private float chaseSpeed = 1.5f;

    public bool isIdle = false;
    public bool isTurn = false;

    // Start is called before the first frame update
    void Awake()
    {
        vengeflyRigid = GetComponent<Rigidbody2D>();
        vengeflyTransform = GetComponent<Transform>();
        detectedArea = transform.GetComponentInChildren<CircleCollider2D>();      
        vengeflySprite = transform.GetComponent<SpriteRenderer>();
        vengeflyAnimator = GetComponent<Animator>();
        
        moveArea = vengeflyTransform.position;
    }


    private void FixedUpdate()
    {        
        if(isIdle == false && isTurn == false)
        {
            isIdle = true;
            StartCoroutine(MoveArea());
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag.Equals("Player"))
        {
            vengeflyAnimator.SetBool("Detected", true);

            Vector2 offset = collision.transform.position - transform.position;

            if(offset.normalized.x > 0)
            {
                vengeflySprite.flipX = true;
            }
            else
            {
                vengeflySprite.flipX=false;
            }

            Debug.LogFormat("플레이어의 위치 {0}", offset.normalized);
            Debug.LogFormat("속력값 : {0}", offset.normalized * chaseSpeed);

            vengeflyRigid.velocity = offset.normalized * chaseSpeed;
            Debug.Log("플레이어 탐지");
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag.Equals("Player"))
        {
            vengeflyAnimator.SetBool("Detected", false);
        }
    }


    IEnumerator MoveArea()
    {
        float xPos = Random.Range(-1f, 1.01f);
        float yPos = Random.Range(-1f, 1.01f);

        Vector2 ray = new Vector2(xPos, yPos);

        Vector2 randomPos = moveArea - ray;

        Vector2 myPos = transform.position;       

        Vector2 offset = randomPos - myPos;

        //Vector2 vector =


        vengeflyRigid.velocity = offset.normalized * areaSpeed;

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(Turn());

        isIdle = false;

    }

    IEnumerator Turn()
    {
        isTurn = true;
        vengeflyRigid.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        isTurn = false;
    }

}

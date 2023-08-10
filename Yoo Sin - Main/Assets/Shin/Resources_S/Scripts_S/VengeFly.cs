using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vengefly : MonoBehaviour
{
    private Rigidbody2D vengeflyRigid = default;
    private Transform vengeflyTransform = default;
    private CircleCollider2D vengeflyCollider = default;
    private CircleCollider2D vengeflyDetectedArea = default;
    private SpriteRenderer vengeflySprite = default;
    private Animator vengeflyAnimator = default;

    private Vector2 firstPos = default;

    private float areaSpeed = 0.5f;
    private float chaseSpeed = 3.0f;
    private int lifeCount = 3;

    public float xPos = default;
    public float yPos = default;

    public bool isDead = false;
    public bool isIdle = false;
    public bool isStay = false;

    
    void Awake()
    {
        vengeflyRigid = GetComponent<Rigidbody2D>();
        vengeflyTransform = GetComponent<Transform>();
        vengeflyCollider = GetComponent<CircleCollider2D>();
        vengeflyDetectedArea = transform.GetComponentInChildren<CircleCollider2D>();
        vengeflySprite = GetComponent<SpriteRenderer>();
        vengeflyAnimator = GetComponent<Animator>();

        firstPos = vengeflyTransform.position;
    }

    private void Start()
    {
        xPos = Random.Range(-1f, 1.01f);
        yPos = Random.Range(-1f, 1.01f);
    }

    private void FixedUpdate()
    {
        if (isDead.Equals(false))
        {
            if (isIdle == false && isStay == false)
            {
                isIdle = true;
                StartCoroutine(MoveArea());
            }
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead.Equals(false) && collision.tag.Equals("Player"))
        {
            vengeflyAnimator.SetBool("Detected", true);

            Vector2 offset = collision.transform.position - transform.position;

            if (offset.normalized.x > 0)
            {
                vengeflySprite.flipX = true;
            }
            else
            {
                vengeflySprite.flipX = false;
            }

            vengeflyRigid.velocity = offset.normalized * chaseSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("PlayerAttack"))
        {           
            lifeCount -= 1;

            vengeflyAnimator.SetTrigger("isHit");

            if (lifeCount <= 0)
            {
                vengeflyAnimator.SetTrigger("isDead");
                isDead = true;                                
                vengeflyRigid.velocity = Vector2.zero;
                vengeflyCollider.isTrigger = true;
                vengeflyRigid.gravityScale = 1.0f;                

            }

        }

        if (isDead.Equals(true) && collision.tag.Equals("Platform"))
        {
            vengeflyRigid.simulated = false;
            vengeflyCollider.enabled = false;            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            vengeflyAnimator.SetBool("Detected", false);
        }

    }


    IEnumerator MoveArea()
    {
        
        // 최초 위치에서 움직일 구역 정하기
        Vector2 randomPos = new Vector2(firstPos.x + xPos, firstPos.y + yPos);
        Vector2 myPos = transform.position;

        // 이번 방향 지시값
        Vector2 offset = randomPos - myPos;

        vengeflyRigid.velocity = offset.normalized * areaSpeed;

        xPos = Random.Range(-1f, 1.01f);
        yPos = Random.Range(-1f, 1.01f);

        //Debug.LogFormat("이번 방향 지시값 : {0}", offset.normalized);

        if (offset.normalized.x < 0f)
        {
            vengeflySprite.flipX = false;
        }
        else
        {
            vengeflySprite.flipX = true;
        }

        // 이동을 진행할 시간
        yield return new WaitForSeconds(3.0f);

        // 다음 방향 지시값
        Vector2 randomPos2 = new Vector2(firstPos.x + xPos, firstPos.y + yPos);
        Vector2 myPos2 = transform.position;
        Vector2 offset2 = randomPos2 - myPos2;

        float isTurn = offset.normalized.x * offset2.normalized.x;


        /* 
        =========== Legacy : VengeFly의 Turn 기준값 및 애니메이션 집어넣기 ================

        Debug.LogFormat("다음 방향 지시값 : {0}", offset2.normalized);
        Debug.LogFormat("이번 방향 X 다음 방향 = {0} 음수면 턴, 양수면 턴X", isTurn);

        if (isTurn < 0f)
        {
            vengeflyAnimator.SetBool("isTurn", false);
        }
        else
        {
            vengeflyAnimator.SetBool("isTurn", true);
        }
        
        =========== Legacy : VengeFly의 Turn 기준값 및 애니메이션 집어넣기 ================
        */

        StartCoroutine(Stay());

        isIdle = false;

    }

    IEnumerator Stay()
    {
        isStay = true;

        vengeflyRigid.velocity = Vector2.zero;

        yield return new WaitForSeconds(1.0f);

        isStay = false;

    }

}

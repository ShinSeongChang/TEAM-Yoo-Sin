using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Crawlid : MonoBehaviour
{   
    private AudioSource myAudio = default;
    private SkillGauge skillGauge;
    private Transform player = default;
    private Rigidbody2D crawlidRigid = default;
    private CapsuleCollider2D crawlidCollider = default;
    private Animator crawlidAnimator = default;
    private SpriteRenderer crawlidSprite = default;

    private Vector2 myPos = default;    
    private float vector = -1;
    private float speed = 2.0f;
    private int lifeCount = 3;

    private Color firstColor = default;
    private Color hitColor = default;

    public bool isDie = false;
    public bool isTurn = false;
    public bool isGround = false;
    public bool isHit = false;

    void Awake()
    {
        myAudio = GetComponent<AudioSource>();
        player =GameObject.FindWithTag("Player").GetComponent<Transform>();
        crawlidRigid = GetComponent<Rigidbody2D>();
        crawlidCollider = GetComponent<CapsuleCollider2D>();
        crawlidAnimator  = GetComponent<Animator>();
        crawlidSprite = GetComponent<SpriteRenderer>();

        firstColor = new Color(1f, 1f, 1f, 1f);
        hitColor = new Color(0.75f, 0.25f, 0.25f, 0.75f);

        isGround = true;        
    }

    void Start()
    {
        skillGauge = GameObject.Find("GaugeImg").GetComponent<SkillGauge>();
        
    }

    private void FixedUpdate()
    {
        Debug.Log("Crawlid 컬러 체크" + crawlidSprite.color);

        myPos = transform.position;
        
        // crawlid의 update 행동들은 살아있는 동안만 동작한다.
        if(isDie.Equals(false))
        {

            // Crawlid의 이동 로직 == 죽지않았을 때, 돌고있는 상태가 아닐 때, 맞고있을때가 아닐 때 한 방향으로 움직인다.
            if (isTurn == false && isHit == false)
            {
                crawlidRigid.velocity = new Vector2(speed * vector, crawlidRigid.velocity.y);        
            }


            // *{ 레이캐스트로 절벽 탐지하는 로직

            // 레이캐스트의 시작점 == crawlid보다 x값이 0.6앞선 위치 * vector로 왼쪽인지 오른쪽인지 판별한다.
            Vector2 crawlidFront = new Vector2(transform.position.x + 0.6f * vector, transform.position.y);

            // 레이캐스트의 끝점 == 시작점에의 y값의 -1f한 지점까지 찍을예정
            Vector2 crawlidFrontdown = new Vector2(0, -1f);

            // 레이캐스트가 찍히는곳을 직접 봐보기
            //Debug.DrawRay(new Vector2(myPos.x + 1f, myPos.y), new Vector2(-2f, 0f), Color.green);
 
            RaycastHit2D hit = Physics2D.Raycast(crawlidFront, crawlidFrontdown, 1, LayerMask.GetMask("Platform"));            

            // 레이캐스트가 찍은곳에 플랫폼이 없으며 현재 플랫폼 위에 있는 상태라면
            if (isGround == true && hit.collider == null) 
            {
                StartCoroutine(Turning());
            }

           

            // *} 레이캐스트로 절벽 탐지하는 로직

        }


    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 충돌 지점의 좌표를 받아내는 crashedPoint
        Vector2 crashedPoint = collision.contacts[0].point;
        Vector2 crashedNormalize = crashedPoint - myPos;

        // 충돌한것이 플레이어가 아니며
        if(collision.collider.tag != "Player")
        {
            // crawlid의 아랫방향에 있는것이 아니라면 (땅바닥을 제외하려고) 방향전환을 한다.
            if (crashedNormalize.y > -0.2f && isHit == false)
            {
                StartCoroutine(Turning());
            }
        }

        // 충돌한것이 플랫폼이라면 isGround = true
        if(collision.collider.tag == "Platform")
        {
            isGround = true;
        }

    }

    // 플랫폼에서 벗어나는 순간 isGround = false
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.tag == "Platform")
        {
            isGround = false;
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.tag == "MainCamera")
        {
            Debug.Log("카메라에 닿았나");
            myAudio.Play();
        }

        if(isDie.Equals(true))
        {
            crawlidRigid.velocity = crawlidRigid.velocity * 0f;

            if(collision.tag.Equals("Platform"))
            {
                crawlidAnimator.SetTrigger("Die_Land");

                // Rigidbody 비활성화
                crawlidRigid.simulated = false;

                // 충돌을 제외하기 위한 Collider 비활성화
                crawlidCollider.enabled = false;
            }

        }

        // 플레이어의 공격에 닿게 되면
        if(collision.tag.Equals("PlayerAttack"))
        {
            StartCoroutine(Hit());
            // 라이프 카운트가 1씩 깎이며
            lifeCount -= 1;

            skillGauge.GaugePlus();

            // 현재 플레이어와의 거리를 계산하고
            Vector2 offset = new Vector2(player.position.x - myPos.x, player.position.y - myPos.y);


            // 라이프 카운트가 0 이하가 되면
            if(lifeCount <= 0)
            {
                // isDie = true, 날아가는 도중 충돌력을 제외하기 위해 isTrigger = true
                crawlidCollider.isTrigger = true;
                isDie = true;
                crawlidRigid.velocity = Vector2.zero;
                crawlidAnimator.SetTrigger("Die");

                // 만약 플레이어가 왼쪽방향에서 가격하여 사망하게 된경우
                if (offset.normalized.x < 0f && (offset.normalized.y > -0.5f && offset.normalized.y < 0.5f))
                {
                    // 오른쪽 윗 대각선으로 날아간다.
                    Vector2 hitForce = new Vector2(10f, 2.5f);
                    crawlidRigid.AddForce(hitForce, ForceMode2D.Impulse);
                }
                else if (offset.normalized.x > 0f && (offset.normalized.y > -0.5f && offset.normalized.y < 0.5f))
                {
                    // 플레이어가 오른쪽 방향에서 가격하여 사망하게 된 경우 왼쪽 윗 대각선으로 날아간다.
                    Vector2 hitForce = new Vector2(-10f, 2.5f);
                    crawlidRigid.AddForce(hitForce, ForceMode2D.Impulse);
                }



            }
            else if (offset.normalized.x < 0f)
            {
                // 살아있는 상태, 플레이어가 왼쪽에서 가격한 경우 오른쪽으로 타격하는 힘을 줌
                crawlidRigid.velocity = Vector2.zero;
                crawlidRigid.AddForce(transform.right * 5f, ForceMode2D.Impulse);
                //StartCoroutine(Hit());
            }
            else if(offset.normalized.x > 0f)
            {
                // 살아있는 상태, 플레이어가 오른쪽에서 가격한 경우 오른쪽으로 타격하는 힘을 줌
                crawlidRigid.velocity = Vector2.zero;
                crawlidRigid.AddForce(transform.right * -5f, ForceMode2D.Impulse);
                //StartCoroutine(Hit());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "MainCamera")
        {
            myAudio.Stop();
        }
    }


    // crawlid의 방향 전환을 하는 상태
    // FixedUpdate와 OnCollsionEnter2D 함수 내부 존재
    IEnumerator Turning()
    {        
        crawlidRigid.velocity = Vector2.zero;
        isTurn = true;
        vector *= -1;

        crawlidAnimator.SetTrigger("isTurn");
        yield return new WaitForSeconds(0.5f);

        HorizontalFlip();
        isTurn = false;
        

    }

    // crawlid가 공격받는 상태
    // OnTriggerEnter2D 함수 내부 존재
    IEnumerator Hit()
    {
        isHit = true;

        crawlidSprite.color = hitColor;

        yield return new WaitForSeconds(0.25f);

        crawlidSprite.color = firstColor;

        isHit = false;
    }

    // crawlid의 방향 방향 전환
    // IEnumerator Turning 함수 내부존재
    void HorizontalFlip()
    {
        if(vector > 0)
        {
            crawlidSprite.flipX = true;
        }
        else if(vector < 0)
        {
            crawlidSprite.flipX = false;
        }
    }

}

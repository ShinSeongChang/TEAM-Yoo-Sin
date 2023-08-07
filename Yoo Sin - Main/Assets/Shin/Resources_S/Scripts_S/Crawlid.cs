using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Crawlid : MonoBehaviour
{
    public Rigidbody2D crawlidRigid = default;
    public SpriteRenderer crawlidSprite = default;
    public Animator crawlidAnimator = default;

    private float speed = 1.5f;

    public int vector = default;

    public bool isTurn = false;
    public bool isGround = false;
    public bool isCliff = false;
    public bool isCrash = false;

    // Start is called before the first frame update
    void Start()
    {
        crawlidRigid = GetComponent<Rigidbody2D>();
        crawlidSprite = GetComponent<SpriteRenderer>();
        crawlidAnimator = GetComponent<Animator>();

        // Crawlid 의 전진방향, 기본 스프라이트가 왼쪽을 바라보는 방향이기에 -1 값을 기본으로 주었음.
        vector = -1;
    }

    private void FixedUpdate()
    {
        // 플랫폼 위에 있으면서 방향전환을 하고 있는것이 아니라면 해당 방향으로 전진한다.
        if(isGround.Equals(true) && isTurn.Equals(false) )
        {
            Vector2 crawlidPos = new Vector2(speed * vector, crawlidRigid.velocity.y);
            crawlidRigid.velocity = crawlidPos;            
        }

        // 몬스터 앞 찍힐 레이위치 조정하기
        Vector2 front = new Vector2(transform.position.x + 0.5f * vector, transform.position.y);
        Vector2 frontDown = new Vector3(0f, -0.6f);
        // 몬스터 앞 찍힐 레이위치 조정하기


        // 몬스터 앞 레이 찍어보기
        Debug.DrawRay(front, frontDown, Color.yellow);

        // 플랫폼 위에 있을때만 (공중에서는 레이를 찍지 않는다.) 자신의 전진방향 아래를 레이로 절벽인지 체크한다.    
        if (isGround.Equals(true))
        {
            RaycastHit2D hit = Physics2D.Raycast(front, frontDown, 1, LayerMask.GetMask("Platform"));

            // 레이가 찍히는 지점에 콜라이더가 없다면 (절벽이라면) 절벽은 참이다.
            if(hit.collider == null)
            {
                isCliff = true;
                
            }
            else if(hit.collider != null)
            {
                isCliff = false;
            }

        }

        // 절벽이거나 충돌이 일어난다면 방향전환을 참으로 바꿔준다.
        if (isCliff.Equals(true) || isCrash.Equals(true))
        {            
            // 무한 방향전환을 막기위해 충돌의 참 값 초기화, 방향전환동안 crawlid의 이동값을 막기위해 isTurn = true.
            isCrash = false;
            isTurn = true;

            crawlidAnimator.SetTrigger("isTurn");

            // 방향 전환애니메이션 클립이 재생되는 시간만큼 crawlid를 멈춰둔다.
            crawlidRigid.velocity = Vector2.zero;

            // crawlid를 멈춰둘 시간, 스프라이트 뒤집을 함수 CrawlidFlip 호출
            Invoke("CrawlidFlip", 0.5f);

            // 전진방향 전환을 위해 -1을 곱하여 방향을 다시 잡아준다.
            vector *= -1;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // crawlid에게 충돌이 발생한 지점의 좌표를 받아내는 contact
        ContactPoint2D contact = collision.contacts[0];

        // crawlid의 자기 포지션 가져오기
        Vector2 myPos = new Vector2(transform.position.x, transform.position.y);

        // 충돌체와의 거리 계산
        Vector2 offset = collision.contacts[0].point - myPos;

        // Legacy : 좌표값, 방향값 확인용 Log
        // Debug.LogFormat("충돌체 값 : {0}", collision.contacts[0].point.normalized);
        // Debug.LogFormat("충돌체와의 방향값 : {0}", offset.normalized);


        // 충돌 지점이 일정높이 이상이라면 ( 전방에서 충돌이 일어났다면 ) 충돌값을 참으로 바꿔준다.
        if(offset.normalized.y >= -0.2f)
        {
            isCrash = true;
        }

        // 공중에서 무한절벽 탐지를 막기위한 isGround
        if (collision.collider.tag == "Platform")
        {
            isGround = true;
        }

    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        // 공중에서 무한절벽 탐지를 막기위한 isGround
        if (collision.collider.tag == "Platform")
        {
            isGround = false;
        }
    }


    public void CrawlidFlip()
    {
        // 기본 스프라이트가 왼쪽방향 (-1 방향) 을 바라보기에 vector 값이 음수면 플립 false, 양수면 플립 true.
        if (vector <= 0)
        {
            crawlidSprite.flipX = false;
        }
        else
        {
            crawlidSprite.flipX = true;
        }

        isTurn = false;
    }

}

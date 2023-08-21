using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VengeFly : MonoBehaviour
{
    private Transform player = default;
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
    private float hitForce = 10f;

    public float xPos = default;
    public float yPos = default;

    public bool isDead = false;
    public bool isHit = false;

    
    void Awake()
    {
        // 플레이어의 좌표를 받아내기 위한 Findtag
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        vengeflyRigid = GetComponent<Rigidbody2D>();
        vengeflyTransform = GetComponent<Transform>();
        vengeflyCollider = GetComponent<CircleCollider2D>();

        // 플레이어를 탐지하기 위한 자식 오브젝트의 콜라이더 받아오기
        vengeflyDetectedArea = transform.GetComponentInChildren<CircleCollider2D>();
        vengeflySprite = GetComponent<SpriteRenderer>();
        vengeflyAnimator = GetComponent<Animator>();

        // 생성된 최초의 좌표 받아내기 ( 최초 좌표로 랜덤 움직임 구역 정할것 )
        firstPos = vengeflyTransform.position;
    }

    private void Start()
    {
        // 가장 처음 랜덤으로 이동할 좌표 받아오기
        xPos = Random.Range(-1f, 1.01f);
        yPos = Random.Range(-1f, 1.01f);

        StartCoroutine(MoveArea());
    }

    // vengefly 기초 상태 ( 자신의 일정 구역 내에서 랜덤 위치로 이동 )
    IEnumerator MoveArea()
    {
        
        // 받아온 랜덤값 벡터 환산 = 최초 좌표값 + 랜덤값
        Vector2 randomPos = new Vector2(firstPos.x + xPos, firstPos.y + yPos);
        Vector2 myPos = transform.position;

        // 이번 방향 지시값
        Vector2 offset = randomPos - myPos;

        // vengefly는 지정받은 방향으로 설정한 속도로 이동한다.
        vengeflyRigid.velocity = offset.normalized * areaSpeed;

        // 다음 방향 이동값 미리 받아오기 ( 터닝 애니메이션 재생 여부 판별하기 위함 )
        xPos = Random.Range(-1f, 1.01f);
        yPos = Random.Range(-1f, 1.01f);

        //Debug.LogFormat("이번 방향 지시값 : {0}", offset.normalized);


        // { vengefly의 스프라이트는 기본 왼쪽방향, 방향값에 따라 이미지 플립
        if (offset.normalized.x < 0f)
        {
            vengeflySprite.flipX = false;
        }
        else
        {
            vengeflySprite.flipX = true;
        }
        // } vengefly의 스프라이트는 기본 왼쪽방향, 방향값에 따라 이미지 플립


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


        // 이동을 진행한 후 잠시 멈춰 있을 코루틴
        StartCoroutine(Stay());

    }

    // IEnumerator MoveArea 함수 내부 존재
    IEnumerator Stay()
    {
        // 1초동안 속력을 잃었다가 다시 움직인다.
        vengeflyRigid.velocity = Vector2.zero;
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(MoveArea());

    }


    // 탐지범위내에 플레이어가 들어오면 추적하는 로직
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(isDead.Equals(false) && isHit.Equals(false))
        {
            // 살아있으며, 피격상태가 아니라면 매 프레임마다 플레이어를 향해 추적한다.
            if (collision.tag.Equals("Player"))
            {
                vengeflyAnimator.SetBool("Detected", true);
                Vector2 offset = player.transform.position - transform.position;

                // { vengefly의 스프라이트는 기본 왼쪽방향, 방향값에 따라 이미지 플립
                if (offset.normalized.x > 0)
                {
                    vengeflySprite.flipX = true;
                }
                else
                {
                    vengeflySprite.flipX = false;
                }
                // } vengefly의 스프라이트는 기본 왼쪽방향, 방향값에 따라 이미지 플립

                vengeflyRigid.velocity = offset.normalized * chaseSpeed;
            }
        }
    }


    // 플레이어에게 공격 받았을때 로직
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // vengefly는 죽으면 포물선으로 날아가게 된다. 이후 땅에 닿게되면 이미지가 사라짐.
        if (isDead.Equals(true))
        {
            if(collision.tag.Equals("Platform") || collision.tag.Equals("Wall"))
            {
                gameObject.SetActive(false);
            }
        }
        else if (collision.tag.Equals("PlayerAttack"))
        {           
            // 살아있는 동안 플레이어 공격에 맞은경우
            Vector2 offset = player.transform.position - transform.position;
            lifeCount -= 1;
            vengeflyAnimator.SetTrigger("isHit");

            // 라이프카운트가 다 떨어지게 되면
            if (lifeCount <= 0)
            {
                vengeflyAnimator.SetTrigger("isDead");
                isDead = true;

                // 이동중이던 속력을 잃고 날아가는 모션 방해받지 않기위해 트리거를 켜준다.
                vengeflyRigid.velocity = Vector2.zero;
                vengeflyCollider.isTrigger = true;

                // 비행체라 gravityScale 의 기존값이 0, 떨어지는 물리력이 필요하기에 gravityScale 값을 올려준다.
                vengeflyRigid.gravityScale = 1.0f;       
                
                // { 플레이어가 공격한 방향을 계산하여 날아갈 방향 구분하기
                if(offset.normalized.x < 0f)
                {
                    vengeflyRigid.AddForce(new Vector2(7f, 3f), ForceMode2D.Impulse);
                }
                else
                {
                    vengeflyRigid.AddForce(new Vector2(-7f, 3f), ForceMode2D.Impulse);
                }
                // } 플레이어가 공격한 방향을 계산하여 날아갈 방향 구분하기

            }
            // { 플레이어에게 맞은 후 밀려날 방향 계산하기
            else if(offset.normalized.x < 0f)
            {
                vengeflyRigid.AddForce(Vector2.right * hitForce, ForceMode2D.Impulse);
                StartCoroutine(Hit());
            }
            else if (offset.normalized.x > 0f)
            {
                vengeflyRigid.AddForce(Vector2.left * hitForce, ForceMode2D.Impulse);
                StartCoroutine(Hit());
            }
            // } 플레이어에게 맞은 후 밀려날 방향 계산하기
        }

    }

    // 플레이어가 탐색범위를 벗어나게 되면 추적 애니메이션을 끈다.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            vengeflyAnimator.SetBool("Detected", false);
        }

    }


    // 맞는 순간 매프레임당 쫓아오는 속력을 순간적으로 없애기 위한 Hit 딜레이
    IEnumerator Hit()
    {
        isHit = true;

        yield return new WaitForSeconds(0.25f);

        isHit = false;
    }
}

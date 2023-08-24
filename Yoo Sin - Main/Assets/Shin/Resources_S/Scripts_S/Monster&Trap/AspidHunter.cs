using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class AspidHunter : MonoBehaviour
{
    private WaitForSeconds stayTime = default;
    private WaitForSeconds moveStay = default;
    private WaitForSeconds hitTime = default;
    private WaitForSeconds turnStay = default;

    private Transform player = default;
    private Rigidbody2D aspidRigid = default;
    private Transform aspidTransform = default;
    private CircleCollider2D aspidCollider = default;
    private CircleCollider2D aspidDetectedArea = default;
    private SpriteRenderer aspidSprite = default;
    private Animator aspidAnimator = default;

    public GameObject bulletPrefab = default;

    private Vector2 firstPos = default;   

    public float xPos = default;
    public float yPos = default;

    public bool isDead = false;
    public bool isIdle = false;
    public bool isStay = false;
    public bool chaseLimit = false;
    public bool isFire = false;
    public bool isHit = false;
    public bool isArea = false;

    private float areaSpeed = 1.0f;
    private float chaseSpeed = 2.0f;
    private int lifeCount = 4;
    private float hitForce = 5f;

    private Color firstColor = default;
    private Color hitColor = default;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        aspidRigid = GetComponent<Rigidbody2D>();
        aspidTransform = GetComponent<Transform>();
        aspidCollider = GetComponent<CircleCollider2D>();
        aspidDetectedArea = transform.GetComponentInChildren<CircleCollider2D>();
        aspidSprite = GetComponent<SpriteRenderer>();
        aspidAnimator = GetComponent<Animator>();

        firstColor = new Color(1f, 1f, 1f, 1f);
        hitColor = new Color(0.75f, 0.25f, 0.25f, 0.75f);

        // AspidHunter 의 최초 포지션 받아오기 == 랜덤 이동할 구역크기 정하기 위함.
        firstPos = aspidTransform.position;
    }

    private void Start()
    {
        // 최초로 이동할 랜덤 좌표 받기.
        xPos = Random.Range(-1f, 1.01f);
        yPos = Random.Range(-1f, 1.01f);

        stayTime = new WaitForSeconds(1.0f);
        moveStay = new WaitForSeconds(3.0f);
        hitTime = new WaitForSeconds(0.25f);
        turnStay = new WaitForSeconds(0.1f);

        // 이후 정해진 코루틴에 따라 움직인다.
        StartCoroutine(MoveArea());
    }

    IEnumerator MoveArea()
    {
        while(true)
        {
            // 최초 위치에서 움직일 구역 정하기
            Vector2 randomPos = new Vector2(firstPos.x + xPos, firstPos.y + yPos);
            Vector2 myPos = transform.position;

            // 이번 방향 지시값
            Vector2 offset = randomPos - myPos;
            
            // 이번 방향으로 주어진 크기만큼의 속도로 이동한다.
            aspidRigid.velocity = offset.normalized * areaSpeed;

            // 다음 방향 지시 받기.
            xPos = Random.Range(-1f, 1.01f);
            yPos = Random.Range(-1f, 1.01f);

            // 기본 스프라이트 이미지가 왼쪽방향이어서 음수방향이면 플립 false, 양수방향이면 플립 true
            if (offset.normalized.x < 0f)
            {
                aspidSprite.flipX = false;
            }
            else
            {
                aspidSprite.flipX = true;
            }

            // 이동을 진행할 시간
            yield return moveStay;

            // 다음 방향 지시값
            Vector2 randomPos2 = new Vector2(firstPos.x + xPos, firstPos.y + yPos);
            Vector2 myPos2 = transform.position;
            Vector2 offset2 = randomPos2 - myPos2;

            float isTurn = offset.normalized.x * offset2.normalized.x;

            aspidRigid.velocity = Vector2.zero;

            yield return stayTime;


            if (isTurn < 0f)
            {
                aspidAnimator.SetTrigger("isTurn");
            }


            if (offset.normalized.x < 0f)
            {
                aspidSprite.flipX = false;
            }
            else
            {
                aspidSprite.flipX = true;
            }

            yield return turnStay;

        }

    }


    public void OnTriggerStay2D(Collider2D collision)
    {
        // 살아있는 상태에서 탐지범위 안에 들어온것이 플레이어라면
        if (isDead.Equals(false) && collision.tag.Equals("Player") && isHit == false && isArea == false)
        {
            Vector2 chasedArea = default;
            Vector2 chasedNoramalize = default;
            Vector2 chasedPos = default;


            // 실시간으로 바뀌는 자신의 위치 받기
            Vector2 myPos = new Vector2(transform.position.x, transform.position.y);

            // 자신으로부터 플레이어 거리 계산하기
            Vector2 offset = collision.transform.position - transform.position;

            // x의 음수값, 양수값을 비교해서 왼쪽인지 오른쪽인지 판단하여 이미지 플립 시키기.
            if (offset.normalized.x > 0)
            {
                aspidSprite.flipX = true;
            }
            else
            {
                aspidSprite.flipX = false;
            }

            // aspidhunter는 플레이어의 방향으로 추격속도로 이동한다.
            aspidRigid.velocity = offset.normalized * chaseSpeed;



            // { aspidhunter가 플레이어의 일정 거리만 쫓아오게 만들 로직

            // 플레이어로부터 aspidhunter 까지의 거리
            chasedArea = transform.position - collision.transform.position;

            // chasedArea를 노멀라이즈로 자른 후 (반지름 크기 1의 원) 이후 임의의 크기로 만들 숫자를 곱해준다.
            chasedNoramalize = chasedArea.normalized * 5f;

            // 월드좌표를 받기위해 플레이어의 위치에 cahsedNormalize의 크기만큼 더한곳의 좌표를 저장한다. 
            chasedPos = new Vector2(collision.transform.position.x + chasedNoramalize.x, collision.transform.position.y + chasedNoramalize.y);            


            // 방향이 각 4분면으로 존재하기에 해당 분면마다 방향을 비교한다.

            // 플레이어가 aspidhunter 왼쪽 아래 있을 때.
            if(offset.normalized.x < 0 && offset.normalized.y < 0)
            {
                // aspidhunter가 chasedPos 를 넘어가려 한다면
                if (aspidRigid.position.x <= chasedPos.x && aspidRigid.position.y <= chasedPos.y)
                {
                    // 진행중인 속력의 반대방향으로 일정시간만큼 부여한다.
                    StartCoroutine(OppositeVector());
                }

                // 이후 나머지 3방향에 관한 로직은 같은 내용이다.
            }

            // 플레이어가 aspidhunter 왼쪽 위에 있을 때.
            if (offset.normalized.x < 0 && offset.normalized.y > 0)
            {
                if (aspidRigid.position.x <= chasedPos.x && aspidRigid.position.y >= chasedPos.y)
                {
                    StartCoroutine(OppositeVector());
                }
            }

            // 플레이어가 aspidhunter 오른쪽 아래 있을 때.
            if (offset.normalized.x > 0 && offset.normalized.y < 0)
            {
                if (aspidRigid.position.x >= chasedPos.x && aspidRigid.position.y <= chasedPos.y)
                {
                    StartCoroutine(OppositeVector());
                }
            }

            // 플레이어가 aspidhunter 오른쪽 위에 있을 때.
            if (offset.normalized.x > 0 && offset.normalized.y > 0)
            {
                if (aspidRigid.position.x >= chasedPos.x && aspidRigid.position.y >= chasedPos.y)
                {
                    StartCoroutine(OppositeVector());
                }
            }

            // } aspidhunter가 플레이어의 일정 거리만 쫓아오게 만들 로직


            // aspidhunter의 탐지범위 안에 플레이어가 존재하는동안 일정 주기마다 원거리 공격을 할 로직
            if (isFire.Equals(false))
            {
                isFire = true;
                StartCoroutine(FireBullet());
            }

        }        
        
    }

    // 플레이어에게 공격 받을시
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("PlayerAttack"))
        {
            Vector2 offset = player.transform.position - transform.position;

            // 공격을 받을시 라이프카운트를 1씩 잃으며
            lifeCount -= 1;



            if (lifeCount <= 0)
            {
                // 라이프 카운트가 0이하가 되면 죽는 애니메이션과 함께 속도를 잃게 하기위해 Rigidbody의 simulated를 false 한다.
                aspidAnimator.SetTrigger("isDead");
                aspidRigid.simulated = false;
                StartCoroutine(Hit());

                // 죽음 애니메이션이 나온 후 오브젝트를 비활성화 시킬 Die 함수
                Invoke("Die", 0.6f);

            }
            else if (offset.normalized.x < 0f)
            {
                aspidRigid.AddForce(Vector2.right * hitForce, ForceMode2D.Impulse);
                StartCoroutine(Hit());
            }
            else if (offset.normalized.x > 0f)
            {
                aspidRigid.AddForce(Vector2.left * hitForce, ForceMode2D.Impulse);
                StartCoroutine(Hit());
            }

        }

    }


    // OnTriggerEnter2D 함수 내부 존재
    private void Die()
    {
        gameObject.SetActive(false);
    }


    // OnTriggerStay2D 함수 내부 존재
    IEnumerator OppositeVector()
    {
        isArea = true;

        aspidRigid.velocity *= -1 * 0.2f;

        yield return new WaitForSeconds(0.5f);

        isArea = false;

        yield break;
    }

    // OnTriggerStay2D 함수 내부 존재
    IEnumerator FireBullet()
    {
        // 공격 애니메이션을 재생한 이후
        aspidAnimator.SetBool("isFire", true);

        yield return new WaitForSeconds(0.85f);

        // Bullet 프리팹을 생성
        Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // 공격을 마친 aspidhunter는 Idle 애니메이션으로 복귀한다.
        aspidAnimator.SetBool("isFire", false);

        // OnTriggerStay2D 로 플레이어를 프레임마다 탐색중이므로 bool값을 변화 시키며 bullet 인스턴스화의 주기를 맞춰준다.
        yield return moveStay;

        isFire = false;
    }

    IEnumerator Hit()
    {
        isHit = true;
        aspidSprite.color = hitColor;

        yield return hitTime;

        aspidSprite.color = firstColor;
        isHit = false;

        yield break;
    }

}

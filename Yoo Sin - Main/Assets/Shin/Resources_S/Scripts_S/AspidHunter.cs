using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class AspidHunter : MonoBehaviour
{
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

    private float areaSpeed = 1.0f;
    private float chaseSpeed = 2.0f;
    private int lifeCount = 4;

    void Awake()
    {
        aspidRigid = GetComponent<Rigidbody2D>();
        aspidTransform = GetComponent<Transform>();
        aspidCollider = GetComponent<CircleCollider2D>();
        aspidDetectedArea = transform.GetComponentInChildren<CircleCollider2D>();
        aspidSprite = GetComponent<SpriteRenderer>();
        aspidAnimator = GetComponent<Animator>();

        firstPos = aspidTransform.position;
    }

    private void Start()
    {
        xPos = Random.Range(-1f, 1.01f);
        yPos = Random.Range(-1f, 1.01f);

        StartCoroutine(MoveArea());
    }

    IEnumerator MoveArea()
    {


            // 최초 위치에서 움직일 구역 정하기
            Vector2 randomPos = new Vector2(firstPos.x + xPos, firstPos.y + yPos);
            Vector2 myPos = transform.position;

            // 이번 방향 지시값
            Vector2 offset = randomPos - myPos;

            aspidRigid.velocity = offset.normalized * areaSpeed;

            xPos = Random.Range(-1f, 1.01f);
            yPos = Random.Range(-1f, 1.01f);

            if (offset.normalized.x < 0f)
            {
                aspidSprite.flipX = false;
            }
            else
            {
                aspidSprite.flipX = true;
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

   

    }

    IEnumerator Stay()
    {
        aspidRigid.velocity = Vector2.zero;

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(MoveArea());

    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        
        if (isDead.Equals(false) && collision.tag.Equals("Player"))
        {
            Vector2 chasedArea = default;
            Vector2 chasedNoramalize = default;
            Vector2 chasedPos = default;

            Vector2 myPos = new Vector2(transform.position.x, transform.position.y);
            // 플레이어 위치 추적
            Debug.DrawRay(transform.position, (transform.position - collision.transform.position) * - 1 , Color.green);

            Vector2 offset = collision.transform.position - transform.position;

            if (offset.normalized.x > 0)
            {
                aspidSprite.flipX = true;
            }
            else
            {
                aspidSprite.flipX = false;
            }

            aspidRigid.velocity = offset.normalized * chaseSpeed;

            chasedArea = transform.position - collision.transform.position;
            chasedNoramalize = chasedArea.normalized * 5f;
            chasedPos = new Vector2(collision.transform.position.x + chasedNoramalize.x, collision.transform.position.y + chasedNoramalize.y);

            //Debug.LogFormat("도달해야할 위치 {0}", chasedPos);

            if(offset.normalized.x < 0 && offset.normalized.y < 0)
            {                
                if(aspidRigid.position.x <= chasedPos.x && aspidRigid.position.y <= chasedPos.y)
                {
                    Debug.Log("왼쪽 아래");
                    //aspidRigid.velocity = aspidRigid.velocity * -1 * 1f;
                    Invoke("OppositeVector", 0.1f);
                }
            }

            if (offset.normalized.x < 0 && offset.normalized.y > 0)
            {

                if (aspidRigid.position.x <= chasedPos.x && aspidRigid.position.y >= chasedPos.y)
                {
                    Debug.Log("왼쪽 위");
                    //aspidRigid.velocity = Vector2.zero;
                    Invoke("OppositeVector", 0.1f);
                }
            }

            if (offset.normalized.x > 0 && offset.normalized.y < 0)
            {

                if (aspidRigid.position.x >= chasedPos.x && aspidRigid.position.y <= chasedPos.y)
                {
                    Debug.Log("오른쪽 아래");
                    //aspidRigid.velocity = Vector2.zero;
                    Invoke("OppositeVector", 0.1f);
                }
            }

            if (offset.normalized.x > 0 && offset.normalized.y > 0)
            {

                if (aspidRigid.position.x >= chasedPos.x && aspidRigid.position.y >= chasedPos.y)
                {
                    Debug.Log("오른쪽 위");
                    //aspidRigid.velocity = Vector2.zero;
                    Invoke("OppositeVector", 0.1f);
                }
            }

            if(isFire.Equals(false))
            {
                isFire = true;
                StartCoroutine(FireBullet());
            }

        }        
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("PlayerAttack"))
        {           
            lifeCount -= 1;

            if (lifeCount <= 0)
            {
                aspidAnimator.SetTrigger("isDead");

                aspidRigid.simulated = false;

                //StopCoroutine(MoveArea());

                Invoke("Die", 0.6f);

            }

        }

    }

    private void Die()
    {
        gameObject.SetActive(false);
    }

    private void OppositeVector()
    {
        aspidRigid.velocity *= -1 * 0.2f;
    }

    IEnumerator FireBullet()
    {
        yield return new WaitForSeconds(3.0f);

        isFire = false;
        Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    }
}

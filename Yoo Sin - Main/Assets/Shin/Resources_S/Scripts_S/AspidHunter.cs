using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspidHunter : MonoBehaviour
{
    private Rigidbody2D aspidRigid = default;
    private Transform aspidTransform = default;
    private CircleCollider2D aspidCollider = default;
    private CircleCollider2D aspidDetectedArea = default;
    private SpriteRenderer aspidSprite = default;
    private Animator aspidAnimator = default;

    private Vector2 firstPos = default;

    public float xPos = default;
    public float yPos = default;

    public bool isDead = false;
    public bool isIdle = false;
    public bool isStay = false;

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


            // ���� ��ġ���� ������ ���� ���ϱ�
            Vector2 randomPos = new Vector2(firstPos.x + xPos, firstPos.y + yPos);
            Vector2 myPos = transform.position;

            // �̹� ���� ���ð�
            Vector2 offset = randomPos - myPos;

            aspidRigid.velocity = offset.normalized * areaSpeed;

            xPos = Random.Range(-1f, 1.01f);
            yPos = Random.Range(-1f, 1.01f);

            Debug.LogFormat("�̹� ���� ���ð� : {0}", offset.normalized);

            if (offset.normalized.x < 0f)
            {
                aspidSprite.flipX = false;
            }
            else
            {
                aspidSprite.flipX = true;
            }

            // �̵��� ������ �ð�
            yield return new WaitForSeconds(3.0f);

            // ���� ���� ���ð�
            Vector2 randomPos2 = new Vector2(firstPos.x + xPos, firstPos.y + yPos);
            Vector2 myPos2 = transform.position;
            Vector2 offset2 = randomPos2 - myPos2;

            float isTurn = offset.normalized.x * offset2.normalized.x;


            /* 
            =========== Legacy : VengeFly�� Turn ���ذ� �� �ִϸ��̼� ����ֱ� ================

            Debug.LogFormat("���� ���� ���ð� : {0}", offset2.normalized);
            Debug.LogFormat("�̹� ���� X ���� ���� = {0} ������ ��, ����� ��X", isTurn);

            if (isTurn < 0f)
            {
                vengeflyAnimator.SetBool("isTurn", false);
            }
            else
            {
                vengeflyAnimator.SetBool("isTurn", true);
            }
        
            =========== Legacy : VengeFly�� Turn ���ذ� �� �ִϸ��̼� ����ֱ� ================
            */

            StartCoroutine(Stay());

   

    }

    IEnumerator Stay()
    {
        aspidRigid.velocity = Vector2.zero;

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(MoveArea());

    }

    private void OnTriggerStay2D(Collider2D collision)
    {


        if (isDead.Equals(false) && collision.tag.Equals("Player"))
        {
            // �÷��̾� ��ġ ����
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
}

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
        // �÷��̾��� ��ǥ�� �޾Ƴ��� ���� Findtag
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        vengeflyRigid = GetComponent<Rigidbody2D>();
        vengeflyTransform = GetComponent<Transform>();
        vengeflyCollider = GetComponent<CircleCollider2D>();

        // �÷��̾ Ž���ϱ� ���� �ڽ� ������Ʈ�� �ݶ��̴� �޾ƿ���
        vengeflyDetectedArea = transform.GetComponentInChildren<CircleCollider2D>();
        vengeflySprite = GetComponent<SpriteRenderer>();
        vengeflyAnimator = GetComponent<Animator>();

        // ������ ������ ��ǥ �޾Ƴ��� ( ���� ��ǥ�� ���� ������ ���� ���Ұ� )
        firstPos = vengeflyTransform.position;
    }

    private void Start()
    {
        // ���� ó�� �������� �̵��� ��ǥ �޾ƿ���
        xPos = Random.Range(-1f, 1.01f);
        yPos = Random.Range(-1f, 1.01f);

        StartCoroutine(MoveArea());
    }

    // vengefly ���� ���� ( �ڽ��� ���� ���� ������ ���� ��ġ�� �̵� )
    IEnumerator MoveArea()
    {
        
        // �޾ƿ� ������ ���� ȯ�� = ���� ��ǥ�� + ������
        Vector2 randomPos = new Vector2(firstPos.x + xPos, firstPos.y + yPos);
        Vector2 myPos = transform.position;

        // �̹� ���� ���ð�
        Vector2 offset = randomPos - myPos;

        // vengefly�� �������� �������� ������ �ӵ��� �̵��Ѵ�.
        vengeflyRigid.velocity = offset.normalized * areaSpeed;

        // ���� ���� �̵��� �̸� �޾ƿ��� ( �ʹ� �ִϸ��̼� ��� ���� �Ǻ��ϱ� ���� )
        xPos = Random.Range(-1f, 1.01f);
        yPos = Random.Range(-1f, 1.01f);

        //Debug.LogFormat("�̹� ���� ���ð� : {0}", offset.normalized);


        // { vengefly�� ��������Ʈ�� �⺻ ���ʹ���, ���Ⱚ�� ���� �̹��� �ø�
        if (offset.normalized.x < 0f)
        {
            vengeflySprite.flipX = false;
        }
        else
        {
            vengeflySprite.flipX = true;
        }
        // } vengefly�� ��������Ʈ�� �⺻ ���ʹ���, ���Ⱚ�� ���� �̹��� �ø�


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


        // �̵��� ������ �� ��� ���� ���� �ڷ�ƾ
        StartCoroutine(Stay());

    }

    // IEnumerator MoveArea �Լ� ���� ����
    IEnumerator Stay()
    {
        // 1�ʵ��� �ӷ��� �Ҿ��ٰ� �ٽ� �����δ�.
        vengeflyRigid.velocity = Vector2.zero;
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(MoveArea());

    }


    // Ž���������� �÷��̾ ������ �����ϴ� ����
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(isDead.Equals(false) && isHit.Equals(false))
        {
            // ���������, �ǰݻ��°� �ƴ϶�� �� �����Ӹ��� �÷��̾ ���� �����Ѵ�.
            if (collision.tag.Equals("Player"))
            {
                vengeflyAnimator.SetBool("Detected", true);
                Vector2 offset = player.transform.position - transform.position;

                // { vengefly�� ��������Ʈ�� �⺻ ���ʹ���, ���Ⱚ�� ���� �̹��� �ø�
                if (offset.normalized.x > 0)
                {
                    vengeflySprite.flipX = true;
                }
                else
                {
                    vengeflySprite.flipX = false;
                }
                // } vengefly�� ��������Ʈ�� �⺻ ���ʹ���, ���Ⱚ�� ���� �̹��� �ø�

                vengeflyRigid.velocity = offset.normalized * chaseSpeed;
            }
        }
    }


    // �÷��̾�� ���� �޾����� ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // vengefly�� ������ ���������� ���ư��� �ȴ�. ���� ���� ��ԵǸ� �̹����� �����.
        if (isDead.Equals(true))
        {
            if(collision.tag.Equals("Platform") || collision.tag.Equals("Wall"))
            {
                gameObject.SetActive(false);
            }
        }
        else if (collision.tag.Equals("PlayerAttack"))
        {           
            // ����ִ� ���� �÷��̾� ���ݿ� �������
            Vector2 offset = player.transform.position - transform.position;
            lifeCount -= 1;
            vengeflyAnimator.SetTrigger("isHit");

            // ������ī��Ʈ�� �� �������� �Ǹ�
            if (lifeCount <= 0)
            {
                vengeflyAnimator.SetTrigger("isDead");
                isDead = true;

                // �̵����̴� �ӷ��� �Ұ� ���ư��� ��� ���ع��� �ʱ����� Ʈ���Ÿ� ���ش�.
                vengeflyRigid.velocity = Vector2.zero;
                vengeflyCollider.isTrigger = true;

                // ����ü�� gravityScale �� �������� 0, �������� �������� �ʿ��ϱ⿡ gravityScale ���� �÷��ش�.
                vengeflyRigid.gravityScale = 1.0f;       
                
                // { �÷��̾ ������ ������ ����Ͽ� ���ư� ���� �����ϱ�
                if(offset.normalized.x < 0f)
                {
                    vengeflyRigid.AddForce(new Vector2(7f, 3f), ForceMode2D.Impulse);
                }
                else
                {
                    vengeflyRigid.AddForce(new Vector2(-7f, 3f), ForceMode2D.Impulse);
                }
                // } �÷��̾ ������ ������ ����Ͽ� ���ư� ���� �����ϱ�

            }
            // { �÷��̾�� ���� �� �з��� ���� ����ϱ�
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
            // } �÷��̾�� ���� �� �з��� ���� ����ϱ�
        }

    }

    // �÷��̾ Ž�������� ����� �Ǹ� ���� �ִϸ��̼��� ����.
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Equals("Player"))
        {
            vengeflyAnimator.SetBool("Detected", false);
        }

    }


    // �´� ���� �������Ӵ� �Ѿƿ��� �ӷ��� ���������� ���ֱ� ���� Hit ������
    IEnumerator Hit()
    {
        isHit = true;

        yield return new WaitForSeconds(0.25f);

        isHit = false;
    }
}

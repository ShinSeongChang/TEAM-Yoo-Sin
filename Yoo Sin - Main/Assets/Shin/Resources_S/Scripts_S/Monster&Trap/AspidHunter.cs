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

        // AspidHunter �� ���� ������ �޾ƿ��� == ���� �̵��� ����ũ�� ���ϱ� ����.
        firstPos = aspidTransform.position;
    }

    private void Start()
    {
        // ���ʷ� �̵��� ���� ��ǥ �ޱ�.
        xPos = Random.Range(-1f, 1.01f);
        yPos = Random.Range(-1f, 1.01f);

        stayTime = new WaitForSeconds(1.0f);
        moveStay = new WaitForSeconds(3.0f);
        hitTime = new WaitForSeconds(0.25f);
        turnStay = new WaitForSeconds(0.1f);

        // ���� ������ �ڷ�ƾ�� ���� �����δ�.
        StartCoroutine(MoveArea());
    }

    IEnumerator MoveArea()
    {
        while(true)
        {
            // ���� ��ġ���� ������ ���� ���ϱ�
            Vector2 randomPos = new Vector2(firstPos.x + xPos, firstPos.y + yPos);
            Vector2 myPos = transform.position;

            // �̹� ���� ���ð�
            Vector2 offset = randomPos - myPos;
            
            // �̹� �������� �־��� ũ�⸸ŭ�� �ӵ��� �̵��Ѵ�.
            aspidRigid.velocity = offset.normalized * areaSpeed;

            // ���� ���� ���� �ޱ�.
            xPos = Random.Range(-1f, 1.01f);
            yPos = Random.Range(-1f, 1.01f);

            // �⺻ ��������Ʈ �̹����� ���ʹ����̾ ���������̸� �ø� false, ��������̸� �ø� true
            if (offset.normalized.x < 0f)
            {
                aspidSprite.flipX = false;
            }
            else
            {
                aspidSprite.flipX = true;
            }

            // �̵��� ������ �ð�
            yield return moveStay;

            // ���� ���� ���ð�
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
        // ����ִ� ���¿��� Ž������ �ȿ� ���°��� �÷��̾���
        if (isDead.Equals(false) && collision.tag.Equals("Player") && isHit == false && isArea == false)
        {
            Vector2 chasedArea = default;
            Vector2 chasedNoramalize = default;
            Vector2 chasedPos = default;


            // �ǽð����� �ٲ�� �ڽ��� ��ġ �ޱ�
            Vector2 myPos = new Vector2(transform.position.x, transform.position.y);

            // �ڽ����κ��� �÷��̾� �Ÿ� ����ϱ�
            Vector2 offset = collision.transform.position - transform.position;

            // x�� ������, ������� ���ؼ� �������� ���������� �Ǵ��Ͽ� �̹��� �ø� ��Ű��.
            if (offset.normalized.x > 0)
            {
                aspidSprite.flipX = true;
            }
            else
            {
                aspidSprite.flipX = false;
            }

            // aspidhunter�� �÷��̾��� �������� �߰ݼӵ��� �̵��Ѵ�.
            aspidRigid.velocity = offset.normalized * chaseSpeed;



            // { aspidhunter�� �÷��̾��� ���� �Ÿ��� �Ѿƿ��� ���� ����

            // �÷��̾�κ��� aspidhunter ������ �Ÿ�
            chasedArea = transform.position - collision.transform.position;

            // chasedArea�� ��ֶ������ �ڸ� �� (������ ũ�� 1�� ��) ���� ������ ũ��� ���� ���ڸ� �����ش�.
            chasedNoramalize = chasedArea.normalized * 5f;

            // ������ǥ�� �ޱ����� �÷��̾��� ��ġ�� cahsedNormalize�� ũ�⸸ŭ ���Ѱ��� ��ǥ�� �����Ѵ�. 
            chasedPos = new Vector2(collision.transform.position.x + chasedNoramalize.x, collision.transform.position.y + chasedNoramalize.y);            


            // ������ �� 4�и����� �����ϱ⿡ �ش� �и鸶�� ������ ���Ѵ�.

            // �÷��̾ aspidhunter ���� �Ʒ� ���� ��.
            if(offset.normalized.x < 0 && offset.normalized.y < 0)
            {
                // aspidhunter�� chasedPos �� �Ѿ�� �Ѵٸ�
                if (aspidRigid.position.x <= chasedPos.x && aspidRigid.position.y <= chasedPos.y)
                {
                    // �������� �ӷ��� �ݴ�������� �����ð���ŭ �ο��Ѵ�.
                    StartCoroutine(OppositeVector());
                }

                // ���� ������ 3���⿡ ���� ������ ���� �����̴�.
            }

            // �÷��̾ aspidhunter ���� ���� ���� ��.
            if (offset.normalized.x < 0 && offset.normalized.y > 0)
            {
                if (aspidRigid.position.x <= chasedPos.x && aspidRigid.position.y >= chasedPos.y)
                {
                    StartCoroutine(OppositeVector());
                }
            }

            // �÷��̾ aspidhunter ������ �Ʒ� ���� ��.
            if (offset.normalized.x > 0 && offset.normalized.y < 0)
            {
                if (aspidRigid.position.x >= chasedPos.x && aspidRigid.position.y <= chasedPos.y)
                {
                    StartCoroutine(OppositeVector());
                }
            }

            // �÷��̾ aspidhunter ������ ���� ���� ��.
            if (offset.normalized.x > 0 && offset.normalized.y > 0)
            {
                if (aspidRigid.position.x >= chasedPos.x && aspidRigid.position.y >= chasedPos.y)
                {
                    StartCoroutine(OppositeVector());
                }
            }

            // } aspidhunter�� �÷��̾��� ���� �Ÿ��� �Ѿƿ��� ���� ����


            // aspidhunter�� Ž������ �ȿ� �÷��̾ �����ϴµ��� ���� �ֱ⸶�� ���Ÿ� ������ �� ����
            if (isFire.Equals(false))
            {
                isFire = true;
                StartCoroutine(FireBullet());
            }

        }        
        
    }

    // �÷��̾�� ���� ������
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("PlayerAttack"))
        {
            Vector2 offset = player.transform.position - transform.position;

            // ������ ������ ������ī��Ʈ�� 1�� ������
            lifeCount -= 1;



            if (lifeCount <= 0)
            {
                // ������ ī��Ʈ�� 0���ϰ� �Ǹ� �״� �ִϸ��̼ǰ� �Բ� �ӵ��� �Ұ� �ϱ����� Rigidbody�� simulated�� false �Ѵ�.
                aspidAnimator.SetTrigger("isDead");
                aspidRigid.simulated = false;
                StartCoroutine(Hit());

                // ���� �ִϸ��̼��� ���� �� ������Ʈ�� ��Ȱ��ȭ ��ų Die �Լ�
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


    // OnTriggerEnter2D �Լ� ���� ����
    private void Die()
    {
        gameObject.SetActive(false);
    }


    // OnTriggerStay2D �Լ� ���� ����
    IEnumerator OppositeVector()
    {
        isArea = true;

        aspidRigid.velocity *= -1 * 0.2f;

        yield return new WaitForSeconds(0.5f);

        isArea = false;

        yield break;
    }

    // OnTriggerStay2D �Լ� ���� ����
    IEnumerator FireBullet()
    {
        // ���� �ִϸ��̼��� ����� ����
        aspidAnimator.SetBool("isFire", true);

        yield return new WaitForSeconds(0.85f);

        // Bullet �������� ����
        Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // ������ ��ģ aspidhunter�� Idle �ִϸ��̼����� �����Ѵ�.
        aspidAnimator.SetBool("isFire", false);

        // OnTriggerStay2D �� �÷��̾ �����Ӹ��� Ž�����̹Ƿ� bool���� ��ȭ ��Ű�� bullet �ν��Ͻ�ȭ�� �ֱ⸦ �����ش�.
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

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
        Debug.Log("Crawlid �÷� üũ" + crawlidSprite.color);

        myPos = transform.position;
        
        // crawlid�� update �ൿ���� ����ִ� ���ȸ� �����Ѵ�.
        if(isDie.Equals(false))
        {

            // Crawlid�� �̵� ���� == �����ʾ��� ��, �����ִ� ���°� �ƴ� ��, �°��������� �ƴ� �� �� �������� �����δ�.
            if (isTurn == false && isHit == false)
            {
                crawlidRigid.velocity = new Vector2(speed * vector, crawlidRigid.velocity.y);        
            }


            // *{ ����ĳ��Ʈ�� ���� Ž���ϴ� ����

            // ����ĳ��Ʈ�� ������ == crawlid���� x���� 0.6�ռ� ��ġ * vector�� �������� ���������� �Ǻ��Ѵ�.
            Vector2 crawlidFront = new Vector2(transform.position.x + 0.6f * vector, transform.position.y);

            // ����ĳ��Ʈ�� ���� == ���������� y���� -1f�� �������� ��������
            Vector2 crawlidFrontdown = new Vector2(0, -1f);

            // ����ĳ��Ʈ�� �����°��� ���� ������
            //Debug.DrawRay(new Vector2(myPos.x + 1f, myPos.y), new Vector2(-2f, 0f), Color.green);
 
            RaycastHit2D hit = Physics2D.Raycast(crawlidFront, crawlidFrontdown, 1, LayerMask.GetMask("Platform"));            

            // ����ĳ��Ʈ�� �������� �÷����� ������ ���� �÷��� ���� �ִ� ���¶��
            if (isGround == true && hit.collider == null) 
            {
                StartCoroutine(Turning());
            }

           

            // *} ����ĳ��Ʈ�� ���� Ž���ϴ� ����

        }


    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �浹 ������ ��ǥ�� �޾Ƴ��� crashedPoint
        Vector2 crashedPoint = collision.contacts[0].point;
        Vector2 crashedNormalize = crashedPoint - myPos;

        // �浹�Ѱ��� �÷��̾ �ƴϸ�
        if(collision.collider.tag != "Player")
        {
            // crawlid�� �Ʒ����⿡ �ִ°��� �ƴ϶�� (���ٴ��� �����Ϸ���) ������ȯ�� �Ѵ�.
            if (crashedNormalize.y > -0.2f && isHit == false)
            {
                StartCoroutine(Turning());
            }
        }

        // �浹�Ѱ��� �÷����̶�� isGround = true
        if(collision.collider.tag == "Platform")
        {
            isGround = true;
        }

    }

    // �÷������� ����� ���� isGround = false
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
            Debug.Log("ī�޶� ��ҳ�");
            myAudio.Play();
        }

        if(isDie.Equals(true))
        {
            crawlidRigid.velocity = crawlidRigid.velocity * 0f;

            if(collision.tag.Equals("Platform"))
            {
                crawlidAnimator.SetTrigger("Die_Land");

                // Rigidbody ��Ȱ��ȭ
                crawlidRigid.simulated = false;

                // �浹�� �����ϱ� ���� Collider ��Ȱ��ȭ
                crawlidCollider.enabled = false;
            }

        }

        // �÷��̾��� ���ݿ� ��� �Ǹ�
        if(collision.tag.Equals("PlayerAttack"))
        {
            StartCoroutine(Hit());
            // ������ ī��Ʈ�� 1�� ���̸�
            lifeCount -= 1;

            skillGauge.GaugePlus();

            // ���� �÷��̾���� �Ÿ��� ����ϰ�
            Vector2 offset = new Vector2(player.position.x - myPos.x, player.position.y - myPos.y);


            // ������ ī��Ʈ�� 0 ���ϰ� �Ǹ�
            if(lifeCount <= 0)
            {
                // isDie = true, ���ư��� ���� �浹���� �����ϱ� ���� isTrigger = true
                crawlidCollider.isTrigger = true;
                isDie = true;
                crawlidRigid.velocity = Vector2.zero;
                crawlidAnimator.SetTrigger("Die");

                // ���� �÷��̾ ���ʹ��⿡�� �����Ͽ� ����ϰ� �Ȱ��
                if (offset.normalized.x < 0f && (offset.normalized.y > -0.5f && offset.normalized.y < 0.5f))
                {
                    // ������ �� �밢������ ���ư���.
                    Vector2 hitForce = new Vector2(10f, 2.5f);
                    crawlidRigid.AddForce(hitForce, ForceMode2D.Impulse);
                }
                else if (offset.normalized.x > 0f && (offset.normalized.y > -0.5f && offset.normalized.y < 0.5f))
                {
                    // �÷��̾ ������ ���⿡�� �����Ͽ� ����ϰ� �� ��� ���� �� �밢������ ���ư���.
                    Vector2 hitForce = new Vector2(-10f, 2.5f);
                    crawlidRigid.AddForce(hitForce, ForceMode2D.Impulse);
                }



            }
            else if (offset.normalized.x < 0f)
            {
                // ����ִ� ����, �÷��̾ ���ʿ��� ������ ��� ���������� Ÿ���ϴ� ���� ��
                crawlidRigid.velocity = Vector2.zero;
                crawlidRigid.AddForce(transform.right * 5f, ForceMode2D.Impulse);
                //StartCoroutine(Hit());
            }
            else if(offset.normalized.x > 0f)
            {
                // ����ִ� ����, �÷��̾ �����ʿ��� ������ ��� ���������� Ÿ���ϴ� ���� ��
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


    // crawlid�� ���� ��ȯ�� �ϴ� ����
    // FixedUpdate�� OnCollsionEnter2D �Լ� ���� ����
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

    // crawlid�� ���ݹ޴� ����
    // OnTriggerEnter2D �Լ� ���� ����
    IEnumerator Hit()
    {
        isHit = true;

        crawlidSprite.color = hitColor;

        yield return new WaitForSeconds(0.25f);

        crawlidSprite.color = firstColor;

        isHit = false;
    }

    // crawlid�� ���� ���� ��ȯ
    // IEnumerator Turning �Լ� ��������
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

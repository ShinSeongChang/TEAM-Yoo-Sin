using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Crawlid : MonoBehaviour
{
    public Transform playerTransform = default;
    private Rigidbody2D crawlidRigid = default;
    private SpriteRenderer crawlidSprite = default;
    private Animator crawlidAnimator = default;
    private CapsuleCollider2D crawlidCollider = default;

    private float speed = 3.0f;
    private int lifeCount = 3;

    public int vector = default;

    public bool isTurn = false;
    public bool isGround = false;
    public bool isCliff = false;
    public bool isCrash = false;
    public bool isDead = false;
    public bool isHit = false;

    // Start is called before the first frame update
    void Awake()
    {        
        crawlidRigid = GetComponent<Rigidbody2D>();
        crawlidSprite = GetComponent<SpriteRenderer>();
        crawlidAnimator = GetComponent<Animator>();
        crawlidCollider = GetComponent<CapsuleCollider2D>();

        // Crawlid �� ��������, �⺻ ��������Ʈ�� ������ �ٶ󺸴� �����̱⿡ -1 ���� �⺻���� �־���.
        vector = -1;
    }

    private void FixedUpdate()
    {

        // crawlid �� �ൿ�� ������� ���� �����Ѵ�.
        if(isDead.Equals(false))
        {
            // �÷��� ���� �����鼭 ������ȯ�� �ϰ� �ִ°��� �ƴ϶�� �ش� �������� �����Ѵ�.
            if (isGround.Equals(true) && isTurn.Equals(false))
            {
                Vector2 crawlidPos = new Vector2(speed * vector, crawlidRigid.velocity.y);
                crawlidRigid.velocity = crawlidPos;            
            }


            // { ���� �� ���� ������ġ �����ϱ�
            Vector2 front = new Vector2(transform.position.x + 0.6f * vector, transform.position.y);
            Vector2 frontDown = new Vector3(0f, -0.6f);

            // ���� �� ���� ����
            Debug.DrawRay(front, frontDown, Color.yellow);

            // } ���� �� ���� ������ġ �����ϱ�

            
            // �÷��� ���� �������� (���߿����� ���̸� ���� �ʴ´�.) �ڽ��� �������� �Ʒ��� ���̷� �������� üũ�Ѵ�.    
            if (isGround.Equals(true))
            {
                RaycastHit2D hit = Physics2D.Raycast(front, frontDown, 1, LayerMask.GetMask("Platform"));

                // ���̰� ������ ������ �ݶ��̴��� ���ٸ� (�����̶��) ������ ���̴�.
                if(hit.collider == null)
                {
                    isCliff = true;
                
                }
                else if(hit.collider != null)
                {
                    isCliff = false;
                }

            }


            // �����̰ų� �浹�� �Ͼ�ٸ� ������ȯ�� ������ �ٲ��ش�.
            if (isCliff.Equals(true) || isCrash.Equals(true))
            {            
                // ���� ������ȯ�� �������� �浹�� �� �� �ʱ�ȭ, ������ȯ���� crawlid�� �̵����� �������� isTurn = true.
                isCrash = false;
                isTurn = true;

                crawlidAnimator.SetTrigger("isTurn");

                // ���� ��ȯ�ִϸ��̼� Ŭ���� ����Ǵ� �ð���ŭ crawlid�� ����д�.
                crawlidRigid.velocity = Vector2.zero;

                // crawlid�� ����� �ð�, ��������Ʈ ������ �Լ� CrawlidFlip ȣ��
                Invoke("CrawlidFlip", 0.5f);

                // �������� ��ȯ�� ���� -1�� ���Ͽ� ������ �ٽ� ����ش�.
                vector *= -1;
            }

        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isDead.Equals(true))
        {            
            if(collision.collider.tag.Equals("Platform"))
            {
                crawlidRigid.simulated = false;
                crawlidCollider.enabled = false;

                crawlidAnimator.SetTrigger("Die_Land");
            }

        }
        else
        {
            // crawlid���� �浹�� �߻��� ������ ��ǥ�� �޾Ƴ��� contact
            ContactPoint2D contact = collision.contacts[0];

            // crawlid�� �ڱ� ������ ��������
            Vector2 myPos = new Vector2(transform.position.x, transform.position.y);

            // �浹ü���� �Ÿ� ���
            Vector2 offset = collision.contacts[0].point - myPos;

            // Legacy : ��ǥ��, ���Ⱚ Ȯ�ο� Log
            // Debug.LogFormat("�浹ü �� : {0}", collision.contacts[0].point.normalized);
            // Debug.LogFormat("�浹ü���� ���Ⱚ : {0}", offset.normalized);

            // ���߿��� �������� Ž���� �������� isGround
            if (collision.collider.tag == "Platform")
            {
                isGround = true;
            }

            // �浹 ������ �������� �̻��̶�� ( ���濡�� �浹�� �Ͼ�ٸ� ) �浹���� ������ �ٲ��ش�.
            if(collision.collider.tag.Equals("Player"))
            {
                // �浹�Ѱ��� �÷��̾���
                /* Pass */
            }
            else if(offset.normalized.y >= -0.2f)
            {
                isCrash = true;
            }

        }

    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        // ���߿��� �������� Ž���� �������� isGround
        if (collision.collider.tag == "Platform")
        {
            isGround = false;
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {    
        
        // crawlid �� ����ִ� ���� ������ �޴´ٸ�
        if (isDead.Equals(false) && collision.tag.Equals("Attack"))
        {                                    
            lifeCount -= 1;            

            Vector3 myPos = new Vector2(transform.position.x, transform.position.y);
            Vector3 offset = playerTransform.position - myPos;

            Debug.LogFormat("���ͷκ��� �÷��̾� ���� : {0}", offset.normalized);
            

            if (offset.normalized.x < 0)
            {
                crawlidRigid.AddForce(transform.right * 5f, ForceMode2D.Impulse);
            }
            else if (offset.normalized.x > 0)
            {
                
            }

            if (lifeCount <= 0)
            {
                crawlidRigid.velocity = Vector2.zero;
                isDead = true;

                crawlidAnimator.SetTrigger("Die");                          

                if(offset.normalized.x < 0 && (offset.normalized.y > -0.3f && offset.normalized.y < 0.3f))
                {                    
                    Vector2 attackForce = new Vector2(5f, 5f);
                    crawlidRigid.AddForce(attackForce, ForceMode2D.Impulse);
                }
                else if(offset.normalized.x > 0 && (offset.normalized.y > -0.3f && offset.normalized.y < 0.3f))
                {
                    Vector2 attackForce = new Vector2(5f *-1, 5f);
                    crawlidRigid.AddForce(attackForce, ForceMode2D.Impulse);
                }
                else
                {
                    crawlidRigid.simulated = false;
                    crawlidCollider.enabled = false;                              
                }
                

            }

            isHit = false;

        }
    }

    public void CrawlidFlip()
    {
        // �⺻ ��������Ʈ�� ���ʹ��� (-1 ����) �� �ٶ󺸱⿡ vector ���� ������ �ø� false, ����� �ø� true.
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawlid : MonoBehaviour
{
    public Rigidbody2D crawlidRigid = default;
    public SpriteRenderer crawlidSprite = default;
    public Animator crawlidAnimator = default;
    private float speed = 3.0f;
    public int vector = default;
    public bool isTurn = false;
    public bool isGround = false;


    // Start is called before the first frame update
    void Start()
    {
        crawlidRigid = GetComponent<Rigidbody2D>();
        crawlidSprite = GetComponent<SpriteRenderer>();
        crawlidAnimator = GetComponent<Animator>();

        vector = -1;
    }

    private void FixedUpdate()
    {
        if(!isTurn && isGround)
        {
            Vector2 crawlidPos = new Vector2(speed * vector, crawlidRigid.velocity.y);
            crawlidRigid.velocity = crawlidPos;

        }
        
        // 몬스터 앞 찍힐 레이위치 조정하기
        Vector2 front = new Vector2(transform.position.x + 0.5f * vector, transform.position.y);
        Vector2 frontDown = new Vector3(0f, -0.7f);
        // 몬스터 앞 찍힐 레이위치 조정하기


        // 몬스터 앞 레이 찍어보기
        Debug.DrawRay(front, frontDown, Color.yellow);

        // 찍히는 레이 탐지        

        if(isGround)
        {
            RaycastHit2D hit = Physics2D.Raycast(front, frontDown, 1, LayerMask.GetMask("Platform"));
            if(hit.collider == null)
            {
                isTurn = true;
                crawlidAnimator.SetTrigger("isTurn");

                crawlidRigid.velocity = Vector2.zero;

                Invoke("CrawlidFlip", 0.5f);

                vector *= -1;
            }

        }

        
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Platform")
        {
            isGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Platform")
        {
            isGround = false;
        }
    }

    public void CrawlidFlip()
    {

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


    #region  Legacy  Crawlid 절벽이 아닌 일반지형에서 벽 충돌시 방향전환 코드

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    ContactPoint2D contact = collision.contacts[0];
    //    Debug.LogFormat("충돌한 물체의 좌표 {0}", collision.contacts[0].point);

    //    Vector2 myPos = new Vector2(transform.position.x, transform.position.y);

    //    Vector2 offset = collision.contacts[0].point - myPos;

    //    Debug.LogFormat("충돌한 물체와의 방향값 : {0}", offset.normalized);

    //    if (offset.normalized.x < 0)
    //    {
    //        isTurn = true;
    //        crawlidAnimator.SetTrigger("isTurn");

    //        crawlidRigid.velocity = Vector2.zero;

    //        Invoke("CrawlidFlip", 0.5f);

    //        vector *= -1;
    //    }

    //}
    #endregion

}

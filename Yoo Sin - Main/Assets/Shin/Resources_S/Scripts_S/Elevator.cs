using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    private BoxCollider2D myCollider = default;
    private Rigidbody2D myRigid = default;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
        myRigid = GetComponent<Rigidbody2D>();

    }

    private void Update()
    {
        // 플레이어 낙하시 충격으로 엘레베이터 밀림방지
        if(GameManager.instance.boss1Die == true)
        {
            // 보스전 진행중 엘레베이터 충돌에 의해 플레이어 애니메이션 오류, 보스 애니메이션 오류, 기술 사라짐 등 발생
            // => 엘레베이터 충돌작용을 없애기위해 콜라이더 offset값을 다른위치에 보내두었다가 보스가 죽으면 지정값에 복귀하도록 설정
            myCollider.offset = new Vector2(0f, -0.1f);
        }

        // 목표높이까지 올라오면 포지션 고정
        if(transform.position.y >= 28.7f)
        {
            myRigid.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // 플레이어가 엘레베이터에서 벗어난 이후 엘레베이터가 일정위치까지 내려가면 멈추기
        if (transform.position.y < 2.65f)
        {
            myRigid.velocity = Vector2.zero;
            transform.position = new Vector2(transform.position.x, 2.66f);
        }
    }

    // 엘레베이터 상승
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 엘레베이터가 목표 위치보다 낮고
        if (transform.position.y < 28.7f)
        {            
            // 보스를 잡은 상태에서 플레이어가 접촉했다면
            if (GameManager.instance.boss1Die == true && collision.collider.tag == "Player")
            {
                // 엘레베이터는 위로 상승하는 속도를 가진다.
                if (transform.position.y < 28.7f)
                {
                    myRigid.velocity = Vector2.up * 4f;            
                }
            }            
        }
    }

    // 엘레베이터 하강
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 보스를 잡은상태에서 플레이어가 엘레베이터를 벗어나면
        if (GameManager.instance.boss1Die == true && collision.collider.tag == "Player")
        {
            // 엘레베이터는 하강한다.
            if (transform.position.y < 28.7f)
            {
                myRigid.velocity = Vector2.down * 4f;
            }
        }
    }

}

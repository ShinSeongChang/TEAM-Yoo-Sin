using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnightStunBody: MonoBehaviour
{
    private int hp = 13;
    private Animator bodyAni;
    private Collider2D bodyCollider;
    // Start is called before the first frame update
    void Start()
    {
        bodyAni = GetComponent<Animator>();
        bodyCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // 바디의 체력이 0이고, 바디콜라이더 트리거가 비활성화 이면
        if (hp == 0 && bodyCollider.isTrigger == false) 
        {
            // 바디콜라이더 트리거 활성화
            bodyCollider.isTrigger = true;
        }
        // 바디의 체력이 0이고, 스턴 관리 오브젝트의 스케일이 0이라면
        if (hp == 0 && transform.parent.transform.localScale == Vector3.zero)
        {
            // 바디의 체력 13으로 초기화, 바디애니메이터, 바디콜라이더 트리거 비활성화
            hp = 13;
            bodyAni.enabled = false;
            bodyCollider.isTrigger = false;
        }
        // 바디의 애니메이터가 비활성화 상태이고, 스턴관리 오브젝트의 스케일이 1이라면
        if (bodyAni.enabled == false && transform.parent.transform.localScale == Vector3.one)
        {
            // 바디의 애니메이터 활성화
            bodyAni.enabled = true;
        }
    }

    // 트리거 접촉시
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 바디의 체력이 0보다 크고, 접촉한 트리거가 플레이어의 공격태그를 가지고 있다면
        if (hp > 0 && collision.CompareTag("PlayerAttack"))
        {
            StartCoroutine(Hit());
        }
    }

    // 피격시 바디의 체력이 감소하도록하고, 애니메이터를 관리하는 Hit코루틴
    IEnumerator Hit()
    {
        // 바디의 애니메이터의 IsHit을 true로 초기화함
        bodyAni.SetBool("IsHit", true);
        // Hit애니메이션 지속시간만큼 기다린 후
        yield return new WaitForSeconds(0.22f);
        // 바디의 체력을 1 감소시키고 바디의 애니메이터의 IsHit을 false로 초기화함
        hp -= 1;
        bodyAni.SetBool("IsHit", false);
    }
}

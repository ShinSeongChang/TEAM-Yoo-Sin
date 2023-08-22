using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnightStunHead : MonoBehaviour
{
    private const int BODY = 1;

    public int hp = 8;
    private SkillGauge_Y skillGauge;
    private Animator headAni;
    private Animator bodyAni;
    // Start is called before the first frame update
    void Start()
    {
        skillGauge = GameObject.Find("GaugeImg").GetComponent<SkillGauge_Y>();
        headAni = GetComponent<Animator>();
        bodyAni = transform.parent.transform.GetChild(BODY).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 헤드의 체력이 0이고, 스턴 관리 오브젝트의 스케일이 0이라면
        if(hp == 0 && transform.parent.transform.localScale == Vector3.zero) 
        {
            // 헤드의 체력 8로 초기화, 헤드애니메이터 비활성화
            hp = 8;
            headAni.enabled = false;
        }
        // 헤드의 애니메이터가 비활성화 상태이고, 스턴관리 오브젝트의 스케일이 1이라면
        if (headAni.enabled == false && transform.parent.transform.localScale == Vector3.one)
        {
            // 헤드의 애니메이터 활성화
            headAni.enabled = true;
        }
    }

    // 트리거 접촉시
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 헤드의 체력이 0보다 크고, 접촉한 트리거가 플레이어의 공격태그를 가지고 있다면
        if(hp > 0 && collision.CompareTag("PlayerAttack")) 
        {
            StartCoroutine(Hit());
        }
    }

    // 피격시 헤드의 체력이 감소하도록하고, 애니메이터를 관리하는 Hit코루틴
    IEnumerator Hit()
    {
        // 헤드의 애니메이터와 바디의 애니메이터의 IsHit을 true로 초기화함
        headAni.SetBool("IsHit", true);
        bodyAni.SetBool("IsHit", true);
        // Hit애니메이션 지속시간만큼 기다린 후
        yield return new WaitForSeconds(0.22f);
        // 헤드의 체력을 1 감소시키고 헤드의 애니메이터와 바디의 애니메이터의 IsHit을 false로 초기화함
        hp -= 1;
        headAni.SetBool("IsHit", false);
        bodyAni.SetBool("IsHit", false);
        skillGauge.GaugePlus();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalseKnightStun : MonoBehaviour
{
    public FalseKnightStunHead head;
    private Animator falseKnightAni;
    private SpriteRenderer falseKnightSprite;
    // Start is called before the first frame update
    void Start()
    {
        falseKnightAni = transform.parent.GetComponent<Animator>();
        falseKnightSprite = transform.parent.GetComponent<SpriteRenderer>();
        head = transform.GetComponentInChildren<FalseKnightStunHead>();
    }

    // Update is called once per frame
    void Update()
    {
        // 스턴 관리 하는 오브젝트의 스케일이 0이고, 기사의 애니메이터의 IsHeadOpen이 true라면
        if(transform.localScale == Vector3.zero && falseKnightAni.GetBool("IsHeadOpen") == true)
        {
            // 기사의 스프라이트를 끄고, 스턴 관리 오브젝트의 스케일을 1로 초기화함
            falseKnightSprite.enabled = false;
            transform.localScale = Vector3.one;
        }

        // 스턴 관리 하는 오브젝트의 스케일이 1이고, 기사의 애니메이터의 IsHeadOpen이 false라면
        if(transform.localScale == Vector3.one && falseKnightAni.GetBool("IsHeadOpen") == false)
        {
            // 기사의 스프라이트를 키고, 스턴 관리 오브젝트의 스케일을 0으로 초기화함
            falseKnightSprite.enabled = true;
            transform.localScale = Vector3.zero;
        }
    }
}

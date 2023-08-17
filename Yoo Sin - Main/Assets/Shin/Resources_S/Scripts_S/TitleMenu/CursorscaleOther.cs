using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorscaleOther : MonoBehaviour
{
    // 메인 커서 위치 받아오기
    [SerializeField] 
    RectTransform mainCursor = default;

    // 내 커서 위치 받아오기 ( 내 커서 = 부모 )
    private RectTransform myCursor = default;

    // 자식 커서들 컴포넌트 받아오고 배열로 저장해두기
    private Transform childCursor = default;
    private RectTransform[] childs = new RectTransform[2];

    private Vector2 minusScale = default;
    private Vector2 plusScale = default;

    // 업데이트 부분에서 각 코루틴을 매프레임마다 호출할것이기에 호출 제약을 위한 불값들
    private bool isEnlargement = false;
    private bool isRedution = false;
    private bool isLeave = false;

    // Start is called before the first frame update
    void Start()
    {
        // 내 커서 위치 받아오기
        myCursor = GetComponent<RectTransform>();

        // 자식커서들 컴포넌트 받아오기
        childCursor = transform.GetComponentInChildren<RectTransform>();

        for(int i = 0; i < childCursor.childCount; i++)
        {
            // 받아온 자식들 컴포넌트 배열변수로 저장해두고, 사이즈 줄여두기 (시작위치 커서 빼고는 가려두는 목적)
            childs[i] = childCursor.GetChild(i).GetComponent<RectTransform>();
            childs[i].sizeDelta = new Vector2(0f, 0f);
        }

        // 각각 자식커서들 이미지를 순차적으로 확대, 축소 시킬 값
        plusScale = new Vector2(40f, 0f);
        minusScale = new Vector2(-40f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // 메인커서 위치가 같아지면
        if(isEnlargement == false && mainCursor.anchoredPosition.y == myCursor.anchoredPosition.y)
        {            
            // 자식커서들 이미지를 확대시키는 코루틴 호출
            StartCoroutine(Enlargement());
        }
  

        // 무분별한 호출 막기위해 자기자신을 제약
        if(isLeave == false)
        {
            // 메인커서가 내 위치를 벗어난다면
            if(mainCursor.anchoredPosition.y != myCursor.anchoredPosition.y)
            {
                // 메인커서가 떠났다는 조건값 저장
                isLeave = true;
            }
        }
        

        // 메인커서가 내 위치에서 벗어나는 상태라면
        if(isRedution == false && isLeave == true)
        {
            // 자식커서들 이미지를 축소시키는 코루틴 호출
            StartCoroutine(Reduction());
        }

    }


    // 자식커서들 이미지 확대 코루틴
    IEnumerator Enlargement()
    {
        // Update에서 프레임마다 코루틴을 호출하기에 조건값으로 끊어준다.
        isEnlargement = true;

        // 코루틴 stop 조건 == 이미지 width 값이 100을 넘어가려 하면
        if (childs[0].sizeDelta.x >= 100f)
        {
            // 자식커서 모두 width, height 값을 100으로 초기화 ( 원본 크기 )
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta = new Vector2(100f, 100f);
            }

            // 확대 기능이 끝났다면 자신을 호출 못하게 조건값 설정
            isEnlargement = true;

            // 커서가 확대될때는 메인커서가 자신의 위치에 온것이니 isLeave값 false로 초기화
            isLeave = false;

            // 메인커서가 떠날 때 축소시키기 위한 조건값 초기화
            isRedution = false;

            yield break;
        }


        // 자식커서들 지정값만큼 widht 키우기 반복 == 1프레임마다
        for (int i = 0; i < childCursor.childCount; i++)
        {
            childs[i].sizeDelta += plusScale;

            yield return null;
        }

        // 확대작업이 끝나면 탈출 조건 달성까지 반복호출 하기위해 조건값 초기화
        isEnlargement = false;

    }

    IEnumerator Reduction()
    {
        // Update에서 프레임마다 코루틴을 호출하기에 조건값으로 끊어준다.
        isRedution = true;

        // 코루틴 stop 조건 == 이미지 width 값이 0보다 작아지려하면
        if (childs[0].sizeDelta.x <= 0f)
        {
            // 자식커서 모두 width값을 0으로 초기화 ( 이미지상 안보이게 )
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta = new Vector2(0f, 100f);
            }

            // 축소 기능이 끝났다면 자신을 호출 못하게 조건값 설정
            isRedution = true;

            // 메인커서가 도착할 때 확대시키기 위한 조건값 초기화
            isEnlargement = false;

            yield break;
        }

        // 자식커서들 지정값만큼 widht 줄이기 반복 == 1프레임마다
        for (int i = 0; i < childCursor.childCount; i++)
        {
            childs[i].sizeDelta += minusScale;

            yield return null;
        }


        // 축소작업이 끝나면 탈출 조건 달성까지 반복호출 하기위해 조건값 초기화
        isRedution = false;

    }
}

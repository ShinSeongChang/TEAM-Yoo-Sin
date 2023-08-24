using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorscaleOther : MonoBehaviour
{
    // 메인 커서 위치 받아오기
    [SerializeField] 
    RectTransform mainCursor = default;

    //WaitForSeconds term = default;

    // 내 커서 위치 받아오기 ( 내 커서 = 부모 )
    private RectTransform myCursor = default;

    // 자식 커서들 컴포넌트 받아오고 배열로 저장해두기
    private Transform childCursor = default;
    private RectTransform[] childs = new RectTransform[2];

    private Vector2 maxSize = default;
    private Vector2 plusScale = default;
    private Vector2 minusScale = default;
    private Vector2 minSize = default;
    

    // 업데이트 부분에서 각 코루틴을 매프레임마다 호출할것이기에 호출 제약을 위한 불값들
    private bool isEnlargement = false;    
    private bool isLeave = false;

    // Start is called before the first frame update
    void Awake()
    {      
        // 내 커서 위치 받아오기
        myCursor = GetComponent<RectTransform>();

        // 자식커서들 컴포넌트 받아오기
        childCursor = transform.GetComponentInChildren<RectTransform>();

        // 각각 자식커서들 이미지를 순차적으로 확대, 축소 시킬 값
        maxSize = new Vector2(100f, 100f);
        plusScale = new Vector2(4000f, 0f);
        minSize = new Vector2(0f, 100f);
        minusScale = new Vector2(-4000f, 0f);

        for(int i = 0; i < childCursor.childCount; i++)
        {
            // 받아온 자식들 컴포넌트 배열변수로 저장해두고, 사이즈 줄여두기 (시작위치 커서 빼고는 가려두는 목적)
            childs[i] = childCursor.GetChild(i).GetComponent<RectTransform>();
            childs[i].sizeDelta = minSize;
        }


        isLeave = true;
        //isEnlargement = true;
    }

    // Update is called once per frame
    void Update()
    {
        // 메인커서 위치가 같아지면
        if(isEnlargement == false && mainCursor.anchoredPosition.y == myCursor.anchoredPosition.y)
        {
            StartCoroutine(Enlargement());
        }


        if(mainCursor.anchoredPosition.y == myCursor.anchoredPosition.y)
        {
            isLeave = false;
        }    


        if(isLeave == false)
        {
            if(mainCursor.anchoredPosition.y != myCursor.anchoredPosition.y)
            {
                isLeave = true;
            }
        }

        // 메인커서가 내 위치에서 벗어나는 상태라면
        if (isLeave == true)
        {
            StartCoroutine(Reduction());
        }

    }


    // 자식커서들 이미지 확대 코루틴
    IEnumerator Enlargement()
    {
        // Update에서 프레임마다 코루틴을 호출하기에 조건값으로 끊어준다.
        isEnlargement = true;

        if (childs[1].sizeDelta.x < 100f)
        {
            for(int i = 0;i < childCursor.childCount;i++)
            {
                // 일시정지 메뉴 ( 타임스케일 0 상태 ) 에서도 동작하게 unscaledDeltaTime을 붙여준다.
                childs[i].sizeDelta += plusScale * Time.unscaledDeltaTime;
                yield return null;
            }
        }

        if (childs[1].sizeDelta.x >= 100f)
        {
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta = maxSize;
            }
        }

        //isLeave = false;
        //isRedution = false;

        // 확대작업이 끝나면 탈출 조건 달성까지 반복호출 하기위해 조건값 초기화
        isEnlargement = false;

        yield break;

    }

    IEnumerator Reduction()
    {        
        // Update에서 프레임마다 코루틴을 호출하기에 조건값으로 끊어준다.
        if (childs[1].sizeDelta.x > 0f)
        {
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta += minusScale * Time.unscaledDeltaTime;

                yield return null;
            }
        }

        if (childs[1].sizeDelta.x <= 0f)
        {
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta = minSize;
            }
        }

        //isLeave = false;
        yield break;

    }
}

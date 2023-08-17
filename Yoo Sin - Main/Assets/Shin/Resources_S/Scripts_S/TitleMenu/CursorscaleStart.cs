using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorscaleStart : MonoBehaviour
{
    // 메인 커서 위치 받아오기
    [SerializeField]
    RectTransform mainCursor = default;

    // 내 커서 위치 받아오기
    private RectTransform myCursor = default;

    // 자식 커서들 컴포넌트 받아오고 배열로 저장해두기
    private Transform childCursor = default;
    private RectTransform[] childs = new RectTransform[2];

    private Vector2 minusScale = default;
    private Vector2 plusScale = default;

    private bool isEnlargement = false;
    private bool isRedution = false;
    private bool isLeave = false;

    private float wiatTime = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        myCursor = GetComponent<RectTransform>();
        childCursor = transform.GetComponentInChildren<RectTransform>();

        for(int i = 0; i < childCursor.childCount; i++)
        {
            childs[i] = childCursor.GetChild(i).GetComponent<RectTransform>();
        }

        isEnlargement = true;

        minusScale = new Vector2(-40f, 0f);
        plusScale = new Vector2(40f, 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isEnlargement == false && mainCursor.anchoredPosition.y == myCursor.anchoredPosition.y)
        {
            StartCoroutine(Enlargement());
        }

        if(isLeave == false)
        {
            if(mainCursor.anchoredPosition.y != myCursor.anchoredPosition.y)
            {
                isLeave = true;
            }

        }
        

        if(isRedution == false && isLeave == true)
        {
            StartCoroutine(Reduction());
        }

    }

    IEnumerator Enlargement()
    {
        isEnlargement = true;

        if (childs[0].sizeDelta.x >= 100f)
        {
            for (int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta = new Vector2(100f, 100f);
            }

            isEnlargement = true;
            isLeave = false;
            isRedution = false;
            yield break;
        }

        for (int i = 0; i < childCursor.childCount; i++)
        {
            childs[i].sizeDelta += plusScale;

            yield return null;
        }

        isEnlargement = false;

    }

    IEnumerator Reduction()
    {
        isRedution = true;

        if(childs[0].sizeDelta.x <= 0f)
        {
            for(int i = 0; i < childCursor.childCount; i++)
            {
                childs[i].sizeDelta = new Vector2(0f, 100f);
            }

            isRedution = true;
            isEnlargement = false;
            yield break;
        }

        for(int i = 0; i < childCursor.childCount; i++)
        {
            childs[i].sizeDelta += minusScale;

            yield return null;
        }

        isRedution = false;
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCursor : MonoBehaviour
{
    private Transform cursor = default;
    private Vector2 cursorScale = default;      

    // Start is called before the first frame update
    void Start()
    {
        cursor = gameObject.GetComponentInChildren<RectTransform>();

        for(int i = 0; i < cursor.childCount; i++)
        {
            Debug.Log(cursor.GetChild(i).name);

        }
        cursorScale = new Vector2(-40f, 0f);

        // 방명록 : 경민이 왔다감 ㅅㄱ
        // 방명록 : 옆에있던 성철이 왔다감 ㅅㄱ
        // 제주감귤 한박스만 주셈 ^ㅅ^        

        StartCoroutine(CursorScale());
    }

    IEnumerator CursorScale()
    {
        float waitTime = 0.1f;

        for (int i = 0; i < cursor.childCount; ++i)
        {
            cursor.GetChild(i).GetComponent<RectTransform>().sizeDelta += cursorScale;
        }

        yield return null;

        if(cursor.GetChild(1).GetComponent<RectTransform>().sizeDelta.x < 0f)
        {
            cursor.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 100f);
            Debug.Log("코루틴 종료");
            yield break;
        }

        StartCoroutine(CursorScale());
    }

}

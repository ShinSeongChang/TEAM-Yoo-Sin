using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCursor : MonoBehaviour
{
    private RectTransform cursorTransform = default;
    private MainManager mainManager = default;

    private Vector2 downLimit = default;
    private Vector2 upLimit = default;

    // Start is called before the first frame update
    void Start()
    {
        cursorTransform = GetComponent<RectTransform>();

        mainManager = GameObject.Find("Title_MainUi").GetComponent<MainManager>();

        downLimit = new Vector2(0f, -95f);
        upLimit = new Vector2(0f, -295f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 tempPos = cursorTransform.anchoredPosition;

        bool upKey = Input.GetKeyDown(KeyCode.UpArrow);
        bool downKey = Input.GetKeyDown(KeyCode.DownArrow);
        bool enter = Input.GetKeyDown(KeyCode.Return);

        if (downKey.Equals(true))
        {
            tempPos.y -= 100f;
            cursorTransform.anchoredPosition = tempPos;

            if (cursorTransform.anchoredPosition.y < -295f)
            {
                cursorTransform.anchoredPosition = downLimit;
            }
        }
        else if (upKey.Equals(true))
        {
            tempPos.y += 100f;
            cursorTransform.anchoredPosition = tempPos;

            if (cursorTransform.anchoredPosition.y > -95f)
            {
                cursorTransform.anchoredPosition = upLimit;
            }
        }

        // 메뉴커서가 게임시작을 선택했을 때
        if (cursorTransform.anchoredPosition.y == -95f && enter == true)
        {
            TitleManager.instance.GameStart();
        }
        // 메뉴커서가 옵션창을 선택했을 때
        else if(cursorTransform.anchoredPosition.y == -195f && enter == true)
        {
            mainManager.OptionInit();
        }
        // 메뉴커서가 게임종료를 선택했을 때
        else if (cursorTransform.anchoredPosition.y == -295f && enter == true)
        {
            TitleManager.instance.GameExit();
        }

    }
}

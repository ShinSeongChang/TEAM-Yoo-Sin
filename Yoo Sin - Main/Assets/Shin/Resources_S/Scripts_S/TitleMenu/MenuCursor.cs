using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCursor : MonoBehaviour
{
    private RectTransform cursorTransform = default;
    private MainManager mainManager = default;

    // Start is called before the first frame update
    void Start()
    {
        cursorTransform = GetComponent<RectTransform>();
        mainManager = GameObject.Find("Main").GetComponent<MainManager>();
    }

    // Update is called once per frame
    void Update()
    {
        bool upKey = Input.GetKeyDown(KeyCode.UpArrow);
        bool downKey = Input.GetKeyDown(KeyCode.DownArrow);
        bool enter = Input.GetKeyDown(KeyCode.Return);

        if (downKey.Equals(true))
        {
            cursorTransform.anchoredPosition = new Vector2(cursorTransform.anchoredPosition.x, cursorTransform.anchoredPosition.y - 100f);

            if (cursorTransform.anchoredPosition.y < -295f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, -95f);
            }
        }
        else if (upKey.Equals(true))
        {
            cursorTransform.anchoredPosition = new Vector2(cursorTransform.anchoredPosition.x, cursorTransform.anchoredPosition.y + 100f);

            if (cursorTransform.anchoredPosition.y > -95f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, -295f);
            }
        }


        if (cursorTransform.anchoredPosition.y == -95f && enter == true)
        {
            TitleManager.instance.GameStart();
        }
        else if(cursorTransform.anchoredPosition.y == -195f && enter == true)
        {
            mainManager.OptionInit();
        }
        else if (cursorTransform.anchoredPosition.y == -295f && enter == true)
        {
            TitleManager.instance.GameExit();
        }

    }
}

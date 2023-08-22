using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitMenuCursor : MonoBehaviour
{
    private RectTransform cursorTransform = default;

    // Start is called before the first frame update
    void Start()
    {
        cursorTransform = GetComponent<RectTransform>();
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

            if (cursorTransform.anchoredPosition.y < -100f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, 100f);
            }
        }
        else if (upKey.Equals(true))
        {
            cursorTransform.anchoredPosition = new Vector2(cursorTransform.anchoredPosition.x, cursorTransform.anchoredPosition.y + 100f);

            if (cursorTransform.anchoredPosition.y > 100f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, -100f);
            }
        }

        // Resume 선택시
        if (cursorTransform.anchoredPosition.y == 100f && enter == true)
        {
            GameManager.instance.MenuClose();
        }

        // GameExit 선택시
        if (cursorTransform.anchoredPosition.y == -100f && enter == true)
        {
            GameManager.instance.GameExit();
        }

    }
}

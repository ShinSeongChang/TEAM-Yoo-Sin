using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCursor : MonoBehaviour
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

        if (downKey.Equals(true))
        {
            cursorTransform.anchoredPosition = new Vector2(cursorTransform.anchoredPosition.x, cursorTransform.anchoredPosition.y - 100f);

            if (cursorTransform.anchoredPosition.y < -295f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, -195f);
            }
        }
        else if (upKey.Equals(true))
        {
            cursorTransform.anchoredPosition = new Vector2(cursorTransform.anchoredPosition.x, cursorTransform.anchoredPosition.y + 100f);

            if (cursorTransform.anchoredPosition.y > -195f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, -295f);
            }
        }

    }
}

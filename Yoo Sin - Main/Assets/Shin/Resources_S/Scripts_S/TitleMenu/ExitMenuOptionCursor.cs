using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitMenuOptionCursor : MonoBehaviour
{
    [SerializeField] Slider Mastrer_SoundBar;
    [SerializeField] Slider BGM_SoundBar;
    [SerializeField] Slider Effect_SoundBar;


    private RectTransform cursorTransform = default;


    private Vector2 tempCursor = default;
    private Vector2 downLimit = default;
    private Vector2 downSpace = default;
    private Vector2 upSpace = default;


    // Start is called before the first frame update
    void Start()
    {
        cursorTransform = GetComponent<RectTransform>();

        downLimit = new Vector2(0f, 75f);
        downSpace = new Vector2(0f, -200f);
        upSpace = new Vector2(0f, -75f);

    }

    // Update is called once per frame
    void Update()
    {
        bool upKey = Input.GetKeyDown(KeyCode.UpArrow);
        bool downKey = Input.GetKeyDown(KeyCode.DownArrow);
        bool leftKey = Input.GetKeyDown(KeyCode.LeftArrow);
        bool rightKey = Input.GetKeyDown(KeyCode.RightArrow);
        bool enter = Input.GetKeyDown(KeyCode.Return);

        tempCursor = cursorTransform.anchoredPosition;

        // 커서 아랫방향 이동
        if (downKey.Equals(true))
        {
            tempCursor.y -= 75f;
            cursorTransform.anchoredPosition = tempCursor;

            if (cursorTransform.anchoredPosition.y < -200f)
            {
                cursorTransform.anchoredPosition = downLimit;
            }
            else if (cursorTransform.anchoredPosition.y == -150f)
            {
                cursorTransform.anchoredPosition = downSpace;
            }

        }

        // 커서 윗방향 이동
        if (upKey.Equals(true))
        {
            tempCursor.y += 75f;
            cursorTransform.anchoredPosition = tempCursor;

            if (cursorTransform.anchoredPosition.y > 75f)
            {
                cursorTransform.anchoredPosition = downSpace;
            }
            else if (cursorTransform.anchoredPosition.y == -125f)
            {
                cursorTransform.anchoredPosition = upSpace;
            }

        }

        // Mastrer_SoundBar 조절
        if (cursorTransform.anchoredPosition.y == 75f && leftKey == true)
        {
            Mastrer_SoundBar.value -= 5f;

            if(Mastrer_SoundBar.value <= Mastrer_SoundBar.minValue)
            {
                Mastrer_SoundBar.value = Mastrer_SoundBar.minValue;
            }
        }
        else if(cursorTransform.anchoredPosition.y == 75f && rightKey == true)
        {
            Mastrer_SoundBar.value += 5f;

            if(Mastrer_SoundBar.value >= Mastrer_SoundBar.maxValue)
            {
                Mastrer_SoundBar.value = Mastrer_SoundBar.maxValue;
            }
        }

        // BGM 조절;
        if(cursorTransform.anchoredPosition.y == 0f && leftKey == true)
        {
            BGM_SoundBar.value -= 5f;

            if(BGM_SoundBar.value <= BGM_SoundBar.minValue)
            {
                BGM_SoundBar.value = BGM_SoundBar.minValue;
            }
        }
        else if(cursorTransform.anchoredPosition.y == 0f && rightKey == true)
        {
            BGM_SoundBar.value += 5f;

            if(BGM_SoundBar.value >= BGM_SoundBar.maxValue)
            {
                BGM_SoundBar.value = BGM_SoundBar.maxValue;
            }
        }

        // Effect 조절
        if (cursorTransform.anchoredPosition.y == -75f && leftKey == true)
        {
            Effect_SoundBar.value -= 5f;

            if (Effect_SoundBar.value <= Effect_SoundBar.minValue)
            {
                Effect_SoundBar.value = Effect_SoundBar.minValue;
            }
        }
        else if (cursorTransform.anchoredPosition.y == -75f && rightKey == true)
        {
            Effect_SoundBar.value += 5f;

            if (Effect_SoundBar.value >= Effect_SoundBar.maxValue)
            {
                Effect_SoundBar.value = Effect_SoundBar.maxValue;
            }
        }


        // 옵션창 나가기
        if (cursorTransform.anchoredPosition.y == -200f && enter == true)
        {
            GameManager.instance.OptionOut();
        }

    }
}

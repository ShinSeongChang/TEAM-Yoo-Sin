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

        Mastrer_SoundBar.value = AudioManager.audioInstance.masterValue;
        BGM_SoundBar.value = AudioManager.audioInstance.bgmValue;
        Effect_SoundBar.value = AudioManager.audioInstance.effectValue;
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
            Mastrer_SoundBar.value += AudioManager.audioInstance.downValue;

            if(Mastrer_SoundBar.value <= AudioManager.audioInstance.minValue)
            {
                Mastrer_SoundBar.value = AudioManager.audioInstance.minValue;
            }

            AudioManager.audioInstance.masterValue = Mastrer_SoundBar.value;
        }
        else if(cursorTransform.anchoredPosition.y == 75f && rightKey == true)
        {
            Mastrer_SoundBar.value += AudioManager.audioInstance.upValue;

            if(Mastrer_SoundBar.value >= AudioManager.audioInstance.maxValue)
            {
                Mastrer_SoundBar.value = AudioManager.audioInstance.maxValue;
            }

            AudioManager.audioInstance.masterValue = Mastrer_SoundBar.value;
        }

        // BGM 조절;
        if (cursorTransform.anchoredPosition.y == 0f && leftKey == true)
        {
            BGM_SoundBar.value += AudioManager.audioInstance.downValue;

            if (BGM_SoundBar.value <= AudioManager.audioInstance.minValue)
            {
                BGM_SoundBar.value = AudioManager.audioInstance.minValue;
            }

            AudioManager.audioInstance.bgmValue = BGM_SoundBar.value;
        }
        else if(cursorTransform.anchoredPosition.y == 0f && rightKey == true)
        {
            BGM_SoundBar.value += AudioManager.audioInstance.upValue;

            if (BGM_SoundBar.value >= AudioManager.audioInstance.maxValue)
            {
                BGM_SoundBar.value = AudioManager.audioInstance.maxValue;
            }

            AudioManager.audioInstance.bgmValue = BGM_SoundBar.value;
        }

        // Effect 조절
        if (cursorTransform.anchoredPosition.y == -75f && leftKey == true)
        {
            Effect_SoundBar.value += AudioManager.audioInstance.downValue;

            if (Effect_SoundBar.value <= AudioManager.audioInstance.minValue)
            {
                Effect_SoundBar.value = AudioManager.audioInstance.minValue;
            }

            AudioManager.audioInstance.effectValue = Effect_SoundBar.value;
        }
        else if (cursorTransform.anchoredPosition.y == -75f && rightKey == true)
        {
            Effect_SoundBar.value += AudioManager.audioInstance.upValue;

            if (Effect_SoundBar.value >= AudioManager.audioInstance.maxValue)
            {
                Effect_SoundBar.value = AudioManager.audioInstance.maxValue;
            }

            AudioManager.audioInstance.effectValue = Effect_SoundBar.value;
        }


        // 옵션창 나가기
        if (cursorTransform.anchoredPosition.y == -200f && enter == true)
        {
            GameManager.instance.OptionOut();
        }

    }
}

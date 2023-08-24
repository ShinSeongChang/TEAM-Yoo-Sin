using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionCursor : MonoBehaviour
{
    private RectTransform cursorTransform = default;
    private OptionManager optionManager;

    [SerializeField] Slider Master_SoundBar;
    [SerializeField] Slider BGM_SoundBar;
    [SerializeField] Slider Effect_SoundBar;

    // Start is called before the first frame update
    void Start()
    {
        cursorTransform = GetComponent<RectTransform>();
        optionManager = GameObject.Find("Title_OptionUi").GetComponent<OptionManager>();

        Master_SoundBar.value = AudioManager.audioInstance.masterValue;
        BGM_SoundBar.value = AudioManager.audioInstance.bgmValue;
        Effect_SoundBar.value = AudioManager.audioInstance.effectValue;
    }

    // Update is called once per frame
    void Update()
    {
        bool upKey = Input.GetKeyDown(KeyCode.UpArrow);
        bool downKey = Input.GetKeyDown(KeyCode.DownArrow);
        bool enter = Input.GetKeyDown(KeyCode.Return);

        bool leftKey = Input.GetKeyDown(KeyCode.LeftArrow);
        bool rightKey = Input.GetKeyDown(KeyCode.RightArrow);

        if (downKey.Equals(true))
        {
            cursorTransform.anchoredPosition = new Vector2(cursorTransform.anchoredPosition.x, cursorTransform.anchoredPosition.y - 75f);

            if(cursorTransform.anchoredPosition.y == -225f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, -370f);
            }
            else if (cursorTransform.anchoredPosition.y < -370f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, 0f);
            }
        }
        else if (upKey.Equals(true))
        {
            cursorTransform.anchoredPosition = new Vector2(cursorTransform.anchoredPosition.x, cursorTransform.anchoredPosition.y + 75f);

            if (cursorTransform.anchoredPosition.y > 0f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, -370f);
            }
            else if (cursorTransform.anchoredPosition.y == -295f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, -150f);
            }
        }



        // Master_SoundBar 조절
        if (cursorTransform.anchoredPosition.y == 0f && leftKey == true)
        {
            Master_SoundBar.value += AudioManager.audioInstance.downValue;

            if (Master_SoundBar.value <= AudioManager.audioInstance.minValue)
            {
                Master_SoundBar.value = AudioManager.audioInstance.minValue;
            }
            
            AudioManager.audioInstance.masterValue = Master_SoundBar.value;
        }
        else if(cursorTransform.anchoredPosition.y == 0f && rightKey == true)
        {
            Master_SoundBar.value += AudioManager.audioInstance.upValue;

            if (Master_SoundBar.value >= AudioManager.audioInstance.maxValue)
            {
                Master_SoundBar.value = AudioManager.audioInstance.maxValue;
            }

            AudioManager.audioInstance.masterValue = Master_SoundBar.value;
        }

        // BGM_SoundBar 조절
        if (cursorTransform.anchoredPosition.y == -75f && leftKey == true)
        {
            BGM_SoundBar.value += AudioManager.audioInstance.downValue;

            if (BGM_SoundBar.value <= AudioManager.audioInstance.minValue)
            {
                BGM_SoundBar.value = AudioManager.audioInstance.minValue;
            }

            AudioManager.audioInstance.bgmValue = BGM_SoundBar.value;
        }
        else if (cursorTransform.anchoredPosition.y == -75f && rightKey == true)
        {
            BGM_SoundBar.value += AudioManager.audioInstance.upValue;

            if (BGM_SoundBar.value >= AudioManager.audioInstance.maxValue)
            {
                BGM_SoundBar.value = AudioManager.audioInstance.maxValue;
            }

            AudioManager.audioInstance.bgmValue = BGM_SoundBar.value;
        }

        // Effect_SoundBar 조절
        if (cursorTransform.anchoredPosition.y == -150f && leftKey == true)
        {
            Effect_SoundBar.value += AudioManager.audioInstance.downValue;

            if (Effect_SoundBar.value <= AudioManager.audioInstance.minValue)
            {
                Effect_SoundBar.value = AudioManager.audioInstance.minValue;
            }

            AudioManager.audioInstance.effectValue = Effect_SoundBar.value;

        }
        else if (cursorTransform.anchoredPosition.y == -150f && rightKey == true)
        {
            Effect_SoundBar.value += AudioManager.audioInstance.upValue;

            if (Effect_SoundBar.value >= AudioManager.audioInstance.maxValue)
            {
                Effect_SoundBar.value = AudioManager.audioInstance.maxValue;
            }

            AudioManager.audioInstance.effectValue = Effect_SoundBar.value;
        }



        else if (cursorTransform.anchoredPosition.y == -370f && enter == true)
        {
            optionManager.OptionOut();
        }

    }
}

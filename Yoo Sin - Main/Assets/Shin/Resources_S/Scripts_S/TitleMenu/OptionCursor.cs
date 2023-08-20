using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionCursor : MonoBehaviour
{
    private RectTransform cursorTransform = default;
    private OptionManager optionManager;

    [SerializeField] Image soundBar = default;

    // Start is called before the first frame update
    void Start()
    {
        cursorTransform = GetComponent<RectTransform>();
        optionManager = GameObject.Find("Option").GetComponent<OptionManager>();
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
            cursorTransform.anchoredPosition = new Vector2(cursorTransform.anchoredPosition.x, cursorTransform.anchoredPosition.y - 370f);

            if (cursorTransform.anchoredPosition.y < -370f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, 0f);
            }
        }
        else if (upKey.Equals(true))
        {
            cursorTransform.anchoredPosition = new Vector2(cursorTransform.anchoredPosition.x, cursorTransform.anchoredPosition.y + 370f);

            if (cursorTransform.anchoredPosition.y > 0f)
            {
                cursorTransform.anchoredPosition = new Vector2(0f, -370f);
            }
        }


        if (cursorTransform.anchoredPosition.y == 0f && leftKey == true)
        {
            soundBar.fillAmount -= 0.1f;

            if(soundBar.fillAmount < 0f)
            {
                soundBar.fillAmount = 0f;
            }
        }
        else if(cursorTransform.anchoredPosition.y == 0f && rightKey == true)
        {
            soundBar.fillAmount += 0.1f;

            if( soundBar.fillAmount > 1.0f)
            {
                soundBar.fillAmount = 1.0f;
            }
        }
        else if (cursorTransform.anchoredPosition.y == -370f && enter == true)
        {
            optionManager.OptionOut();            
        }

    }
}

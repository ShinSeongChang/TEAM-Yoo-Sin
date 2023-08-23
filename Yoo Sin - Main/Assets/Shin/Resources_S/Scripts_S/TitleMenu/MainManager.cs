using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI gamestartText = default;
    [SerializeField] TextMeshProUGUI optionText = default;
    [SerializeField] TextMeshProUGUI gameexitText = default;

    float invisibleSpeed = 2.5f;

    public void OptionInit()
    {
        StartCoroutine(InvisibilityTrue());
    }
    IEnumerator InvisibilityTrue()
    {
        float tempTextcolor = gameexitText.alpha;

        while(gameexitText.alpha > 0f)
        {
            tempTextcolor -= invisibleSpeed * Time.deltaTime;

            gamestartText.alpha = tempTextcolor;
            optionText.alpha = tempTextcolor;
            gameexitText.alpha = tempTextcolor;

            yield return null;
        }

        if (gameexitText.alpha <= 0f)
        {
            gamestartText.alpha = 0f;
            optionText.alpha = 0f;
            gameexitText.alpha = 0f;
        }

        TitleManager.instance.OptionInit();

        yield break;
    }


    public void OptionOut()
    {
        StartCoroutine(InvisibilityFalse());
    }

    IEnumerator InvisibilityFalse()
    {
        float tempTextcolor = gameexitText.alpha;


        while(gameexitText.alpha < 1f)
        {
            tempTextcolor += invisibleSpeed * Time.deltaTime;

            gamestartText.alpha += tempTextcolor;
            optionText.alpha += tempTextcolor;
            gameexitText.alpha = tempTextcolor;

            yield return null;

        }

        if (gameexitText.alpha >= 1f)
        {
            gamestartText.alpha = 1f;
            optionText.alpha = 1f;
            gameexitText.alpha = 1f;

        }

        yield break;
    }


}

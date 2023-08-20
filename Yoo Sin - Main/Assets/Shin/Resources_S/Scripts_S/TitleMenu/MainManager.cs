using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainManager : MonoBehaviour
{

    //[SerializeField] Image soundbarEmpty = default;
    //[SerializeField] Image soundbar = default;

    [SerializeField] TextMeshProUGUI gametartText = default;
    [SerializeField] TextMeshProUGUI optionText = default;
    [SerializeField] TextMeshProUGUI gameexitText = default;


    public void OptionInit()
    {
        StartCoroutine(InvisibilityTrue());
    }
    IEnumerator InvisibilityTrue()
    {
        //soundbarEmpty.color = new Color(soundbarEmpty.color.r, soundbarEmpty.color.g, soundbarEmpty.color.b, soundbarEmpty.color.a - 0.04f);
        //soundbar.color = new Color(soundbar.color.r, soundbar.color.g, soundbar.color.b, soundbar.color.a - 0.04f);
        gametartText.alpha -= 0.04f;
        optionText.alpha -= 0.04f;
        gameexitText.alpha -= 0.04f;

        if (gameexitText.alpha <= 0f)
        {
            //soundbarEmpty.color = new Color(soundbarEmpty.color.r, soundbarEmpty.color.g, soundbarEmpty.color.b, 0f);
            //soundbar.color = new Color(soundbar.color.r, soundbar.color.g, soundbar.color.b, 0f);
            gametartText.alpha = 0f;
            optionText.alpha = 0f;
            gameexitText.alpha = 0f;

            TitleManager.instance.OptionInit();
            yield break;

        }

        yield return new WaitForSeconds(0.016f);

        StartCoroutine(InvisibilityTrue());
    }
    public void OptionOut()
    {
        StartCoroutine(InvisibilityFalse());
    }

    IEnumerator InvisibilityFalse()
    {
        //soundbarEmpty.color = new Color(soundbarEmpty.color.r, soundbarEmpty.color.g, soundbarEmpty.color.b, soundbarEmpty.color.a + 0.04f);
        //soundbar.color = new Color(soundbar.color.r, soundbar.color.g, soundbar.color.b, soundbar.color.a + 0.04f);
        gametartText.alpha += 0.04f;
        optionText.alpha += 0.04f;
        gameexitText.alpha += 0.04f;


        if (gameexitText.alpha >= 1f)
        {
            //soundbarEmpty.color = new Color(soundbarEmpty.color.r, soundbarEmpty.color.g, soundbarEmpty.color.b, 1f);
            //soundbar.color = new Color(soundbar.color.r, soundbar.color.g, soundbar.color.b, 1f);
            gametartText.alpha = 1f;
            optionText.alpha = 1f;
            gameexitText.alpha = 1f;


            yield break;

        }

        yield return new WaitForSeconds(0.016f);

        StartCoroutine(InvisibilityFalse());
    }


}

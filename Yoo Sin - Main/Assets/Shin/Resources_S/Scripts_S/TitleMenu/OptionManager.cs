using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionManager : MonoBehaviour
{

    [SerializeField] Image soundbarEmpty = default;
    [SerializeField] Image soundbar = default;
    [SerializeField] TextMeshProUGUI soundText = default;
    [SerializeField] TextMeshProUGUI resumeText = default;

    public void OptionInit()
    {
        StartCoroutine(InvisibilityFalse());
    }

    IEnumerator InvisibilityFalse()
    {
        soundbarEmpty.color = new Color(soundbarEmpty.color.r, soundbarEmpty.color.g, soundbarEmpty.color.b, soundbarEmpty.color.a + 0.04f);
        soundbar.color = new Color(soundbar.color.r, soundbar.color.g, soundbar.color.b, soundbar.color.a + 0.04f);
        soundText.alpha += 0.04f;
        resumeText.alpha += 0.04f;

        if (resumeText.alpha >= 1f)
        {
            soundbarEmpty.color = new Color(soundbarEmpty.color.r, soundbarEmpty.color.g, soundbarEmpty.color.b, 1f);
            soundbar.color = new Color(soundbar.color.r, soundbar.color.g, soundbar.color.b, 1f);
            soundText.alpha = 1f;
            resumeText.alpha = 1f;

            yield break;

        }

        yield return new WaitForSeconds(0.016f);

        StartCoroutine(InvisibilityFalse());
    }

    public void OptionOut()
    {
        StartCoroutine(InvisibilityTrue());
    }

    IEnumerator InvisibilityTrue()
    {
        soundbarEmpty.color = new Color(soundbarEmpty.color.r, soundbarEmpty.color.g, soundbarEmpty.color.b, soundbarEmpty.color.a - 0.04f);
        soundbar.color = new Color(soundbar.color.r, soundbar.color.g, soundbar.color.b, soundbar.color.a - 0.04f);
        soundText.alpha -= 0.04f;
        resumeText.alpha -= 0.04f;

        if (resumeText.alpha <= 0f)
        {
            soundbarEmpty.color = new Color(soundbarEmpty.color.r, soundbarEmpty.color.g, soundbarEmpty.color.b, 0f);
            soundbar.color = new Color(soundbar.color.r, soundbar.color.g, soundbar.color.b, 0f);
            soundText.alpha = 0f;
            resumeText.alpha = 0f;


            TitleManager.instance.OptionExit();
            yield break;

        }

        yield return new WaitForSeconds(0.016f);

        StartCoroutine(InvisibilityTrue());
    }
}

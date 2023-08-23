using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class OptionManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI MasterText = default;
    [SerializeField] TextMeshProUGUI BGMText = default;
    [SerializeField] TextMeshProUGUI EffectText = default;
    [SerializeField] TextMeshProUGUI resumeText = default;

    float invisibleSpeed = 2.5f;

    private Image Master_SoundBar_BackGround = default;
    private Image Master_SoundBar_Fill = default;
    private Image BGM_SoundBar_BackGround = default;
    private Image BGM_SoundBar_Fill = default;
    private Image Effect_SoundBar_BackGround = default;
    private Image Effect_SoundBar_Fill = default;

    // Active False인 상태에서 다른매니저에서 참조하려하니 null Refernce 발생, Start => Awake 변경
    void Awake()
    {
        Master_SoundBar_BackGround = transform.GetChild(4).transform.GetChild(0).GetComponent<Image>();
        Master_SoundBar_Fill = transform.GetChild(4).transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();

        BGM_SoundBar_BackGround = transform.GetChild(5).transform.GetChild(0).GetComponent<Image>();
        BGM_SoundBar_Fill = transform.GetChild(5).transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();

        Effect_SoundBar_BackGround = transform.GetChild(6).transform.GetChild(0).GetComponent<Image>();
        Effect_SoundBar_Fill = transform.GetChild(6).transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
    }

    public void OptionInit()
    {
        StartCoroutine(InvisibilityFalse());
    }

    IEnumerator InvisibilityFalse()
    {
        Color tempImagecolor = Effect_SoundBar_BackGround.color;
        Color tempImagecolor2 = Effect_SoundBar_Fill.color;
        float tempTextcolor = resumeText.alpha;

        while(resumeText.alpha < 1f || Master_SoundBar_Fill.color.a < 1f)
        {
            tempImagecolor.a += invisibleSpeed * Time.deltaTime;
            tempImagecolor2.a += invisibleSpeed * Time.deltaTime;
            tempTextcolor += invisibleSpeed * Time.deltaTime;

            Master_SoundBar_BackGround.color = tempImagecolor;
            Master_SoundBar_Fill.color = tempImagecolor2;
            BGM_SoundBar_BackGround.color = tempImagecolor;
            BGM_SoundBar_Fill.color = tempImagecolor2;
            Effect_SoundBar_BackGround.color = tempImagecolor;
            Effect_SoundBar_Fill.color = tempImagecolor2;

            MasterText.alpha = tempTextcolor;
            BGMText.alpha = tempTextcolor;
            EffectText.alpha = tempTextcolor;
            resumeText.alpha = tempTextcolor;

            yield return null;

        }       
        
        yield break;
    }

    public void OptionOut()
    {
        StartCoroutine(InvisibilityTrue());
    }

    IEnumerator InvisibilityTrue()
    {
        //Color tempImagecolor = Master_SoundBar_BackGround.color;
        //Color tempImagecolor2 = Master_SoundBar_Fill.color;

        Color tempImagecolor = Master_SoundBar_BackGround.color;
        Color tempImagecolor2 = Master_SoundBar_Fill.color;
        float tempTextcolor = resumeText.alpha;

        while (resumeText.alpha >= 0f || Master_SoundBar_Fill.color.a >= 0f)
        {
            tempImagecolor.a -= invisibleSpeed * Time.deltaTime;
            tempImagecolor2.a -= invisibleSpeed * Time.deltaTime;
            tempTextcolor -= invisibleSpeed * Time.deltaTime;

            Master_SoundBar_BackGround.color = tempImagecolor;
            Master_SoundBar_Fill.color = tempImagecolor2;
            BGM_SoundBar_BackGround.color = tempImagecolor;
            BGM_SoundBar_Fill.color = tempImagecolor2;
            Effect_SoundBar_BackGround.color = tempImagecolor;
            Effect_SoundBar_Fill.color = tempImagecolor2;

            MasterText.alpha = tempTextcolor;
            BGMText.alpha = tempTextcolor;
            EffectText.alpha = tempTextcolor;
            resumeText.alpha = tempTextcolor;

            yield return null;
        }

  

        TitleManager.instance.OptionExit();
        yield break;
    }
}

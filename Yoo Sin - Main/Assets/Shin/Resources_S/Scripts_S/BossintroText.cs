using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossintroText : MonoBehaviour
{
    private TextMeshProUGUI myText = default;

    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<TextMeshProUGUI>();

        myText.alpha = 0f;

        StartCoroutine(IntroStart());
    }

    
    IEnumerator IntroStart()
    {

        while(myText.alpha < 1f)
        {
            myText.alpha += 0.1f;

            yield return new WaitForSeconds(0.032f);

        }
        

        yield return new WaitForSeconds(2.5f);

        while(myText.alpha >= 0f) 
        {
            myText.alpha -= 0.1f;

            yield return new WaitForSeconds(0.032f);
        }

        yield break;
    }
}

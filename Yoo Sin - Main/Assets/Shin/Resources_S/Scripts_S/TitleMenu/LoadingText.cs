using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingText : MonoBehaviour
{
    private TextMeshProUGUI myText = default;

    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<TextMeshProUGUI>();

        StartCoroutine(Invisible());
    }


    IEnumerator Invisible()
    {
        while (true)
        {
            if(myText.alpha >= 1.0f)
            {
                while(myText.alpha >= 0.3f)
                {
                    myText.alpha -= 0.02f;
                    yield return new WaitForSeconds(0.032f);
                }

            }
            else if(myText.alpha < 0.3f)
            {
                while (myText.alpha <= 1.0f)
                {
                    myText.alpha += 0.02f;
                    yield return new WaitForSeconds(0.032f);
                }
            }


            
        }
    }
}

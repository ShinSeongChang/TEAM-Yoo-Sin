using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvisibleScreen : MonoBehaviour
{    
    private Image myImage = default;

    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<Image>();

        myImage.color = new Color(0f, 0f, 0f, 1f);

        StartCoroutine(Invisible());
    }
    
    IEnumerator Invisible()
    {
        Color color = myImage.color;

        yield return new WaitForSeconds(1.5f);

        while(myImage.color.a > 0f)
        {
            color.a -= 0.6f * Time.deltaTime;
            myImage.color = color;

            yield return null;
        }

        yield break;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InvisibleImage : MonoBehaviour
{
    private Image myImage = default;    

    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<Image>();        
        StartCoroutine(BackgroundInvisible());
    }

    IEnumerator BackgroundInvisible()
    {
        yield return new WaitForSeconds(0.35f);

        Color tempColor = myImage.color;

        while (myImage.color.a >= 0f)
        {
            tempColor.a -= 0.6f * Time.deltaTime;
            myImage.color = tempColor;

            yield return null;
        }

        yield break;
    }
}

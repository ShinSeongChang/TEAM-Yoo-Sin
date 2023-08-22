using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DieBackGround : MonoBehaviour
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
        while(myImage.color.a < 1.0f)
        {
            myImage.color = new Color(myImage.color.r, myImage.color.g, myImage.color.b, myImage.color.a + 0.025f);

            yield return new WaitForSeconds(0.032f);
        }

        yield break;
    }
}

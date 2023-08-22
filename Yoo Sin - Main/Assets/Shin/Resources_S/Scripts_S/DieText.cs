using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DieText : MonoBehaviour
{
    private TextMeshProUGUI myText = default;

    // Start is called before the first frame update
    void Start()
    {
        myText = GetComponent<TextMeshProUGUI>();

        StartCoroutine(BackgroundInvisible());
    }

    IEnumerator BackgroundInvisible()
    {
        while (myText.alpha < 1.0f)
        {
            myText.alpha += 0.025f;

            yield return new WaitForSeconds(0.032f);
        }

        yield break;
    }
}

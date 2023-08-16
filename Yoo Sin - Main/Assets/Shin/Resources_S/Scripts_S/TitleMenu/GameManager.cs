using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public RectTransform menuCursor = default;


    // Update is called once per frame
    void Update()
    {
        bool enter = Input.GetKeyDown(KeyCode.Return);

        if (menuCursor.anchoredPosition.y > -201f && enter == true)
        {
            SceneManager.LoadScene("Room001");
        }
        else if(menuCursor.anchoredPosition.y < -299f && enter == true)
        {
            //UnityEditor.EditorApplication.isPlaying = false;
            Application.Quit();
        }
    }
}

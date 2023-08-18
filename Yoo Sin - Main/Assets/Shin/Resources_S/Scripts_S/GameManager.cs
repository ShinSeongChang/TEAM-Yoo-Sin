using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = default;

    [SerializeField]
    GameObject exitMenu = default;

    public int playerLife = 5;

    private bool escape = false;
    private bool menuOpen = false;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }


    private void Update()
    {
        escape = Input.GetKeyDown(KeyCode.Escape);

        if(menuOpen && escape == true)
        {
            exitMenu.SetActive(false);
            Time.timeScale = 1.0f;
            PlayerBehavior_S.playermove = true;


            menuOpen = false;
            escape = false;
        }

        if (menuOpen == false && escape == true)
        {
            Time.timeScale = 0f;
            PlayerBehavior_S.playermove = false;
            exitMenu.SetActive(true);      
            
            menuOpen = true;

        }

    }

}

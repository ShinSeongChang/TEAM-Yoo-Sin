using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = default;
    
    [SerializeField]
    GameObject exitMenu = default;

    [SerializeField]
    GameObject exitText = default;

    public int playerLife = 5;    

    private bool escape = false;
    private bool menuOpen = false;


    private void Awake()
    {
        // 싱글턴 패턴
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        // 싱글턴 패턴

        menuOpen = false;
    }


    private void Update()
    {
        // 메뉴 열고 닫기는 esc키 입력을 받아서 동작한다.
        escape = Input.GetKeyDown(KeyCode.Escape);

        // 일시정지 메뉴 닫기
        if(menuOpen && escape == true)
        {
            exitText.SetActive(false);
            exitMenu.SetActive(false);
            Time.timeScale = 1.0f;
            PlayerBehavior_S.playermove = true;

            menuOpen = false;
            escape = false;
        }

        // 일시정지 메뉴 열기
        if (menuOpen == false && escape == true)
        {
            Time.timeScale = 0f;
            PlayerBehavior_S.playermove = false;
            exitMenu.SetActive(true);


            StartCoroutine(Delay());            
            menuOpen = true;

        }

    }

    IEnumerator Delay()
    {
        // 타임스케일에 영향받지않는 코루틴 : WaitForSecondsRealtime
        yield return new WaitForSecondsRealtime(0.5f);

        // 텍스트들은 스크롤 애니메이션이 먼저 나온 이후에 출력되게 했음.
        exitText.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = default;

    private WaitForSeconds titleDelay = default;
    private WaitForSeconds endingDelay = default;
    private WaitForSecondsRealtime menuDelay = default;
    
    [SerializeField] GameObject ExitUi = default;
    [SerializeField] GameObject MenuMain = default;
    [SerializeField] GameObject MenuOption = default;
    [SerializeField] GameObject FalseKnight_Intro = default;
    [SerializeField] GameObject Hornet_Intro = default;
    [SerializeField] GameObject EndingUi = default;
    [SerializeField] GameObject DieUi = default;

    public int playerLife = 5;

    public bool boss1Die = false;
    public bool boss2Die = false;

    private bool escape = false;
    private bool menuOpen = false;

    private void Awake()
    {
        // 싱글턴 패턴
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        // 싱글턴 패턴

        titleDelay = new WaitForSeconds(2.0f);
        endingDelay = new WaitForSeconds(0.75f);
        menuDelay = new WaitForSecondsRealtime(0.5f);

        menuOpen = false;
    }


    private void Update()
    {
        // 메뉴 열고 닫기는 esc키 입력을 받아서 동작한다.
        escape = Input.GetKeyDown(KeyCode.Escape);

        // 일시정지 메뉴 열기
        if (menuOpen == false && escape == true)
        {
            Time.timeScale = 0f;
            PlayerBehavior_F.playermove = false;
            ExitUi.SetActive(true);

            StartCoroutine(Delay());            
            menuOpen = true;

        }

    }

    IEnumerator Delay()
    {
        // 타임스케일에 영향받지않는 코루틴 : WaitForSecondsRealtime
        yield return menuDelay;

        // 텍스트들은 스크롤 애니메이션이 먼저 나온 이후에 출력되게 했음.
        MenuMain.SetActive(true);

        yield break;
    }

    public void FalseKngiht_Intro()
    {
        FalseKnight_Intro.SetActive(true);
        
    }

    public void HornetIntro()
    {
        Hornet_Intro.SetActive(true);
    }

    // { 게임 클리어시
    public void EndingScene()
    {
        EndingUi.SetActive(true);

        StartCoroutine(LoadTitle());
    }
    // } 게임 클리어시


    // { 플레이어 사망시
    public void PlayerDie()
    {
        DieUi.SetActive(true);

        StartCoroutine(LoadTitle());
    }

    IEnumerator LoadTitle()
    {
        yield return titleDelay;

        SceneManager.LoadScene("TilteScene_Main");

        yield break;
    }
    // } 플레이어 사망시


    // Exit 메뉴 중 Resume을 선택할 때
    public void MenuClose()
    {        
        // 메뉴창 닫기, 일시정지 풀기, 플레이어 행동제한 풀기
        if (menuOpen == true)
        {
            MenuMain.SetActive(false);
            ExitUi.SetActive(false);
            Time.timeScale = 1.0f;
            PlayerBehavior_F.playermove = true;

            menuOpen = false;
            escape = false;
        }
    }


    // Exit 메뉴 중 게임종료를 선택할 때
    public void GameExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TilteScene_Main");
    }


    // Exit 메뉴 중 옵션창을 열 때
    public void OptionInit()
    {
        MenuMain.SetActive(false);
        MenuOption.SetActive(true);

    }

    
    // Exit 메뉴 중 옵션창에서 나올 때
    public void OptionOut()
    {
        MenuOption.SetActive(false);
        MenuMain.SetActive(true);
    }

}

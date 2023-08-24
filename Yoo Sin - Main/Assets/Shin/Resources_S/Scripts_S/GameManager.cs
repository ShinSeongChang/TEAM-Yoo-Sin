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
        // �̱��� ����
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
        // �̱��� ����

        titleDelay = new WaitForSeconds(2.0f);
        endingDelay = new WaitForSeconds(0.75f);
        menuDelay = new WaitForSecondsRealtime(0.5f);

        menuOpen = false;
    }


    private void Update()
    {
        // �޴� ���� �ݱ�� escŰ �Է��� �޾Ƽ� �����Ѵ�.
        escape = Input.GetKeyDown(KeyCode.Escape);

        // �Ͻ����� �޴� ����
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
        // Ÿ�ӽ����Ͽ� ��������ʴ� �ڷ�ƾ : WaitForSecondsRealtime
        yield return menuDelay;

        // �ؽ�Ʈ���� ��ũ�� �ִϸ��̼��� ���� ���� ���Ŀ� ��µǰ� ����.
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

    // { ���� Ŭ�����
    public void EndingScene()
    {
        EndingUi.SetActive(true);

        StartCoroutine(LoadTitle());
    }
    // } ���� Ŭ�����


    // { �÷��̾� �����
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
    // } �÷��̾� �����


    // Exit �޴� �� Resume�� ������ ��
    public void MenuClose()
    {        
        // �޴�â �ݱ�, �Ͻ����� Ǯ��, �÷��̾� �ൿ���� Ǯ��
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


    // Exit �޴� �� �������Ḧ ������ ��
    public void GameExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TilteScene_Main");
    }


    // Exit �޴� �� �ɼ�â�� �� ��
    public void OptionInit()
    {
        MenuMain.SetActive(false);
        MenuOption.SetActive(true);

    }

    
    // Exit �޴� �� �ɼ�â���� ���� ��
    public void OptionOut()
    {
        MenuOption.SetActive(false);
        MenuMain.SetActive(true);
    }

}

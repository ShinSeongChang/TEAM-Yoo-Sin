using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = default;
    //WaitForSecondsRealtime 
    
    [SerializeField]
    GameObject exitMenu = default;

    [SerializeField]
    GameObject exitText = default;

    [SerializeField]
    GameObject FalseKnight_Intro = default;

    [SerializeField]
    GameObject Hornet_Intro = default;

    [SerializeField] 
    GameObject EndingCanvas = default;

    [SerializeField]
    GameObject DieUi = default;

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
            PlayerBehavior_S.playermove = false;
            exitMenu.SetActive(true);

            StartCoroutine(Delay());            
            menuOpen = true;

        }

    }

    IEnumerator Delay()
    {
        // Ÿ�ӽ����Ͽ� ��������ʴ� �ڷ�ƾ : WaitForSecondsRealtime
        yield return new WaitForSecondsRealtime(0.5f);

        // �ؽ�Ʈ���� ��ũ�� �ִϸ��̼��� ���� ���� ���Ŀ� ��µǰ� ����.
        exitText.SetActive(true);

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

    public void EndingScene()
    {
        StartCoroutine(EndingDelay());
    }
    IEnumerator EndingDelay()
    {

        yield return new WaitForSeconds(0.75f);

        EndingCanvas.SetActive(true);

        yield break;
    }


    public void PlayerDie()
    {
        DieUi.SetActive(true);

        StartCoroutine(LoadTitle());
    }

    IEnumerator LoadTitle()
    {
        yield return new WaitForSeconds(2.0f);

        SceneManager.LoadScene("TilteScene");

        yield break;
    }

    public void MenuClose()
    {
        // �Ͻ����� �޴� �ݱ�
        if (menuOpen == true)
        {
            exitText.SetActive(false);
            exitMenu.SetActive(false);
            Time.timeScale = 1.0f;
            PlayerBehavior_S.playermove = true;

            menuOpen = false;
            escape = false;
        }
    }

    public void GameExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TilteScene");
    }



}

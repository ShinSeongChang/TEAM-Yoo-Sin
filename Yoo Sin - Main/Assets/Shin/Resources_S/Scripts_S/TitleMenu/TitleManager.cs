using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public static TitleManager instance;
    MainManager mainManager;
    OptionManager optionManager;

    Transform child = default;

    [SerializeField] GameObject option = default;
    [SerializeField] GameObject main = default;

    [SerializeField] Image titleBackground = default;
    [SerializeField] Image titleText = default;

    [SerializeField] TextMeshProUGUI gamestartText = default;
    [SerializeField] TextMeshProUGUI optionText = default;
    [SerializeField] TextMeshProUGUI gameexitText = default;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("매니저가 둘입니다.");
            Destroy(gameObject);
        }

        child = GameObject.Find("TitleBackground").GetComponentInChildren<Transform>();

        mainManager = GameObject.Find("Main").GetComponent<MainManager>();
        optionManager = child.GetChild(2).GetComponentInChildren<OptionManager>();

    }

    // 게임 스타트
    public void GameStart()
    {
        StartCoroutine(GameInit());
    }
    IEnumerator GameInit()
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Room001");
    }
    // 게임 스타트

    // 게임 종료
    public void GameExit()
    {
        StartCoroutine(GameOut());
    }


    IEnumerator GameOut()
    {
        yield return new WaitForSeconds (1f);

        Application.Quit();
    }
    // 게임 종료

    // 옵션창 나올 때
    public void OptionExit()
    {
        option.SetActive(false);

        main.SetActive(true);

        mainManager.OptionOut();

    }
    // 옵션창 나올 때



    // 옵션창 들어갈 때
    public void OptionInit()
    {
        main.SetActive(false);

        option.SetActive(true);

        optionManager.OptionInit();
    }
    // 옵션창 들어갈 때

}

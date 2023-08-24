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

    [SerializeField] GameObject main = default;
    [SerializeField] GameObject option = default;
    [SerializeField] GameObject loading = default;

    [SerializeField] Image titleBackground = default;
    [SerializeField] Image titleText = default;

    [SerializeField] TextMeshProUGUI gamestartText = default;
    [SerializeField] TextMeshProUGUI optionText = default;
    [SerializeField] TextMeshProUGUI gameexitText = default;

    private float invisibleSpeed = 2.5f;

    // 로딩씬 구현
    private AsyncOperation operation;

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

        mainManager = GameObject.Find("Title_MainUi").GetComponent<MainManager>();

        // 메인캔버스 내 옵션 ui가 비활성화 된 상태이기에 부모인 메인캔버스내 자식 인덱스로 접근한 것.
        optionManager = GameObject.Find("TitleCanvas").transform.GetChild(3).GetComponent<OptionManager>();

    }

    // 게임 스타트
    public void GameStart()
    {
        StartCoroutine(GameInit());
    }
    IEnumerator GameInit()
    {
        Color tempImagecolor = titleBackground.color;
        float tempTextcolor = gameexitText.alpha;

        // 해당 씬을 로딩하기 시작
        // 해당씬에 필요한 리소스들이 메모리상에 모두 준비 되면 씬이 넘어가게 됨.
        operation = SceneManager.LoadSceneAsync("Room001");

        // 참값을 이용하여 로딩씬이 넘어가는 구간을 임의로 지정할수 있다.        
        operation.allowSceneActivation = false;        

        while (gameexitText.alpha > 0f || titleText.color.a > 0f)
        {
            tempImagecolor.a -= invisibleSpeed * Time.deltaTime;
            tempTextcolor -= invisibleSpeed * Time.deltaTime;

            titleBackground.color = tempImagecolor;
            titleText.color = tempImagecolor;

            gamestartText.alpha -= tempTextcolor;
            optionText.alpha -= tempTextcolor;
            gameexitText.alpha -= tempTextcolor;

            yield return null;
        }

        main.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        loading.SetActive(true);

        // 로딩에 걸리는 시간을 재기위한 timer
        float timer = 0f;

        // 로딩이 완료되지 않았다면
        while(operation.isDone == false)
        {
            // 매 프레임마다 진행되는 시간을 잰다.
            yield return null;
            timer += Time.deltaTime;

            // 로딩값의 최대는 0.9f 라고 하는 듯 함?
            // 즉 로딩값이 0.9f보다 작은 상태 = 아직 씬을 준비하는 로딩시간인 상태
            if(operation.progress < 0.9f)
            {
                Debug.Log("로딩중 진행 시간 : " +  timer);
                Debug.Log("로딩중");
                Debug.Log(operation.progress);
            }
            else
            {
                // 로딩값이 0.9f가 넘으면 씬을 넘긴다.
                // 임의로 로딩 시간 조절해보기                 
                if(timer < 5f)
                {
                    operation.allowSceneActivation = false;
                }
                else
                {
                    Debug.Log("로딩 완료까지 걸린 시간 : " + timer);
                    Debug.Log("로딩 완료");
                    Debug.Log(operation.progress);
                    operation.allowSceneActivation = true;

                }

            }
        }
        
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

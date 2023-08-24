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

    // �ε��� ����
    private AsyncOperation operation;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("�Ŵ����� ���Դϴ�.");
            Destroy(gameObject);
        }

        child = GameObject.Find("TitleBackground").GetComponentInChildren<Transform>();

        mainManager = GameObject.Find("Title_MainUi").GetComponent<MainManager>();

        // ����ĵ���� �� �ɼ� ui�� ��Ȱ��ȭ �� �����̱⿡ �θ��� ����ĵ������ �ڽ� �ε����� ������ ��.
        optionManager = GameObject.Find("TitleCanvas").transform.GetChild(3).GetComponent<OptionManager>();

    }

    // ���� ��ŸƮ
    public void GameStart()
    {
        StartCoroutine(GameInit());
    }
    IEnumerator GameInit()
    {
        Color tempImagecolor = titleBackground.color;
        float tempTextcolor = gameexitText.alpha;

        // �ش� ���� �ε��ϱ� ����
        // �ش���� �ʿ��� ���ҽ����� �޸𸮻� ��� �غ� �Ǹ� ���� �Ѿ�� ��.
        operation = SceneManager.LoadSceneAsync("Room001");

        // ������ �̿��Ͽ� �ε����� �Ѿ�� ������ ���Ƿ� �����Ҽ� �ִ�.        
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

        // �ε��� �ɸ��� �ð��� ������� timer
        float timer = 0f;

        // �ε��� �Ϸ���� �ʾҴٸ�
        while(operation.isDone == false)
        {
            // �� �����Ӹ��� ����Ǵ� �ð��� ���.
            yield return null;
            timer += Time.deltaTime;

            // �ε����� �ִ�� 0.9f ��� �ϴ� �� ��?
            // �� �ε����� 0.9f���� ���� ���� = ���� ���� �غ��ϴ� �ε��ð��� ����
            if(operation.progress < 0.9f)
            {
                Debug.Log("�ε��� ���� �ð� : " +  timer);
                Debug.Log("�ε���");
                Debug.Log(operation.progress);
            }
            else
            {
                // �ε����� 0.9f�� ������ ���� �ѱ��.
                // ���Ƿ� �ε� �ð� �����غ���                 
                if(timer < 5f)
                {
                    operation.allowSceneActivation = false;
                }
                else
                {
                    Debug.Log("�ε� �Ϸ���� �ɸ� �ð� : " + timer);
                    Debug.Log("�ε� �Ϸ�");
                    Debug.Log(operation.progress);
                    operation.allowSceneActivation = true;

                }

            }
        }
        
    }
    // ���� ��ŸƮ

    // ���� ����
    public void GameExit()
    {
        StartCoroutine(GameOut());
    }


    IEnumerator GameOut()
    {
        yield return new WaitForSeconds (1f);

        Application.Quit();
    }
    // ���� ����

    // �ɼ�â ���� ��
    public void OptionExit()
    {
        option.SetActive(false);

        main.SetActive(true);

        mainManager.OptionOut();

    }
    // �ɼ�â ���� ��



    // �ɼ�â �� ��
    public void OptionInit()
    {
        main.SetActive(false);

        option.SetActive(true);

        optionManager.OptionInit();
    }
    // �ɼ�â �� ��

}

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
        // �̱��� ����
        if(instance == null)
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

        // �Ͻ����� �޴� �ݱ�
        if(menuOpen && escape == true)
        {
            exitText.SetActive(false);
            exitMenu.SetActive(false);
            Time.timeScale = 1.0f;
            PlayerBehavior_S.playermove = true;

            menuOpen = false;
            escape = false;
        }

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
    }
}

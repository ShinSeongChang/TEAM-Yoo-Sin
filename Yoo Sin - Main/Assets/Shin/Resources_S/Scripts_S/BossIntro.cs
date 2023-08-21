using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIntro : MonoBehaviour
{
    private Transform childText = default;
    [SerializeField] SpriteRenderer BossIntro_Ani = default;
    // Start is called before the first frame update
    void Start()
    {
        childText = transform.GetComponentInChildren<Transform>();

        for(int i = 0; i < childText.childCount; i++)
        {
            childText.GetChild(i).gameObject.SetActive(true);
        }

        StartCoroutine(IntroStart());
        StartCoroutine(AniInvisible());
    }

    IEnumerator IntroStart()
    {
        float timer = 0;        

        for(int i = 0;i < childText.childCount; i++)
        {
            childText.GetChild(i).gameObject.SetActive(true);

            timer += Time.deltaTime;

            // ���ڰ� ���� ������ �ȵȴ� ����??

            //Debug.Log("Ƚ�� : " + i);
            //Debug.Log("�ɸ� �ð� : " + timer);

            yield return new WaitForSeconds(0.25f);
        }


        yield break;

    }

    IEnumerator AniInvisible()
    {
        // �ؽ�Ʈ�� ���������� Ÿ�̹� ���߱�
        yield return new WaitForSeconds(1.2f);


        while (BossIntro_Ani.color.a > 0f)
        {
            BossIntro_Ani.color = new Color(BossIntro_Ani.color.r, BossIntro_Ani.color.g, BossIntro_Ani.color.b, BossIntro_Ani.color.a - 0.05f);

            yield return new WaitForSeconds(0.016f);

        }

        yield break;
    }
}

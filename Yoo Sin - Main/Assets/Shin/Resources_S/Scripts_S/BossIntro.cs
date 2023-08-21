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

            // 글자간 간격 조절이 안된다 왜지??

            //Debug.Log("횟수 : " + i);
            //Debug.Log("걸린 시간 : " + timer);

            yield return new WaitForSeconds(0.25f);
        }


        yield break;

    }

    IEnumerator AniInvisible()
    {
        // 텍스트들 투명해지는 타이밍 맞추기
        yield return new WaitForSeconds(1.2f);


        while (BossIntro_Ani.color.a > 0f)
        {
            BossIntro_Ani.color = new Color(BossIntro_Ani.color.r, BossIntro_Ani.color.g, BossIntro_Ani.color.b, BossIntro_Ani.color.a - 0.05f);

            yield return new WaitForSeconds(0.016f);

        }

        yield break;
    }
}

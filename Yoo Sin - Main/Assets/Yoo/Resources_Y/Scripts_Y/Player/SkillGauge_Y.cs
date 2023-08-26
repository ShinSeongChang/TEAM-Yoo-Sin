using Mono.CompilerServices.SymbolWriter;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkillGauge_Y : MonoBehaviour
{
    private Animator skillAnimator = default;

    private int skillCount = default;
    private int maxCount = 4;
    private int minCount = 0;    

    private void Awake()
    {
        skillAnimator = GetComponent<Animator>();

        skillCount = minCount;
    }

    public void GaugePlus()
    {
        skillCount += 1;

        if(skillCount >= maxCount)
        {
            skillCount = maxCount;
        }

        AnimatorSelector();
    }

    public void UseHeal()
    {
        skillCount -= 4;

        AnimatorSelector();
    }

    public int GetGauge()
    {
        return skillCount;
    }

    void AnimatorSelector()
    {
        // 애니메이터의 파라미터들 중에 불값을 가진 파라미터들의 불값을 false로 초기화
        for (int i = 0; i < skillAnimator.parameters.Length; i++)
        {
            if (skillAnimator.parameters[i].type == AnimatorControllerParameterType.Bool)
            {
                skillAnimator.SetBool(skillAnimator.parameters[i].name, false);
            }
        }

        switch (skillCount)
        {
            case 0:
                skillAnimator.SetBool("isEmpty", true);
                break;

            case 1:
                skillAnimator.SetBool("is25", true);
                break;

            case 2:
                skillAnimator.SetBool("is50", true);
                break;

            case 3:
                skillAnimator.SetBool("is75", true);
                break;

            case 4:
                skillAnimator.SetBool("isFull", true);
                break;

        }

    }

}

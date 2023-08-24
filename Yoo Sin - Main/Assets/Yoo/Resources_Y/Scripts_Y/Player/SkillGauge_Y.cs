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
        switch(skillCount)
        {
            case 0:
                skillAnimator.SetTrigger("isEmpty");
                break;

            case 1:
                skillAnimator.SetTrigger("is25");
                break;

            case 2:
                skillAnimator.SetTrigger("is50");
                break;

            case 3:
                skillAnimator.SetTrigger("is75");
                break;

            case 4:
                skillAnimator.SetTrigger("isFull");
                break;

        }

    }

}

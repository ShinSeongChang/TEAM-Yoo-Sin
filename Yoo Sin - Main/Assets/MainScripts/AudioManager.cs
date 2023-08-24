using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioInstance = default;

    [SerializeField] AudioMixer audioMixer;

    public float maxValue = default;
    public float minValue = default;
    public float upValue = 5f;
    public float downValue = -5f;

    public float masterValue = default;
    public float bgmValue = default;
    public float effectValue = default;

    private void Awake()
    {
        if (audioInstance == null)
        {
            audioInstance = this;
        }
        else
        {
            Debug.LogWarning("오디오 매니저 두개 존재");
            Destroy(gameObject);
        }

        maxValue = 10f;
        minValue = -40f;

        masterValue = maxValue;
        bgmValue = maxValue;
        effectValue = maxValue;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        Debug.Log("Master Value : " + masterValue);

        audioMixer.SetFloat("Master", masterValue);
        audioMixer.SetFloat("Bgm", bgmValue);
        audioMixer.SetFloat("Effect", effectValue);
    }


}

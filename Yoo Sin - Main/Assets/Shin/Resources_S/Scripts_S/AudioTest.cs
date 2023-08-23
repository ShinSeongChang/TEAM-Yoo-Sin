using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioTest : MonoBehaviour
{
    [SerializeField] private AudioMixer testAudioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider monsterSlider;

    // Update is called once per frame
    void Update()
    {
        testAudioMixer.SetFloat("MasterVolume", masterSlider.value);
        //testAudioMixer.SetFloat("TestMaster", masterSlider.value);
        testAudioMixer.SetFloat("MonsterVolume", monsterSlider.value);
    }
}

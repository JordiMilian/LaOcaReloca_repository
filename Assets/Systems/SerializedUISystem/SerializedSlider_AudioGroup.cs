using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SerializedSlider_AudioGroup : MonoBehaviour
{   
    Slider slider;
    [SerializeField] AudioMixer mixer;
    [SerializeField] string paramName;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        
    }
    private void OnEnable()
    {
        slider.onValueChanged.AddListener(OnValueChanged);
    }
    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnValueChanged);
    }

    private void OnValueChanged(float value)
    {
        mixer.SetFloat(paramName, LinearToDecibel(value));
    }

    float LinearToDecibel(float linear)
    {
        float dB;

        if (linear != 0)
            dB = 20.0f * Mathf.Log10(linear);
        else
            dB = -144.0f;

        return dB;
    }
    float DecibelToLinear(float decibel)
    {
        return Mathf.Pow(10f, decibel / 20f);
    }
}

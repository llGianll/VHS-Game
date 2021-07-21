using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UISliderValue : MonoBehaviour
{
    [SerializeField] float inputFieldMaxValue = 10f;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] Slider _slider;

    public Action<float> OnSliderChanged = delegate { }; 
    public Action<float> OnInputValueChanged = delegate { }; 

    private void Start()
    {
        _inputField.text = (inputFieldMaxValue * _slider.value).ToString();
    }

    public void SliderValueChanged()
    {
        float value = inputFieldMaxValue * _slider.value;
        _inputField.text = value.ToString("0.00");
        OnSliderChanged(value);
    }

    public void InputFieldValueChanged()
    {
        //input validation
        if (float.Parse(_inputField.text) < 0)
            _inputField.text = (0).ToString();
        else if (float.Parse(_inputField.text) > inputFieldMaxValue)
            _inputField.text = inputFieldMaxValue.ToString();

        _slider.value = float.Parse(_inputField.text) / inputFieldMaxValue;
        OnInputValueChanged(float.Parse(_inputField.text));
    }

    public void InitializeUIValues(float value)
    {
        _inputField.text = value.ToString("0.00");
        _slider.value = float.Parse(_inputField.text) / inputFieldMaxValue;
    }
}

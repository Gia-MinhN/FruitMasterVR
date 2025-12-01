using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RangeSlider : MonoBehaviour
{
    public TextMeshProUGUI rangeText;

    private const int MIN_RANGE = 60;
    private const int MAX_RANGE = 360;
    private const int STEP      = 30;

    void Start()
    {
        Scrollbar scrollbar = GetComponent<Scrollbar>();
        int stepsCount = (MAX_RANGE - MIN_RANGE) / STEP;

        int savedRange = Mathf.Clamp(VariableHolder.range, MIN_RANGE, MAX_RANGE);

        int index = (savedRange - MIN_RANGE) / STEP;
        float normalized = (float)index / stepsCount; 

        scrollbar.value = normalized;
        SliderValueChanged(normalized);
    }

    public void SliderValueChanged(float value)
    {
        int stepsCount = (MAX_RANGE - MIN_RANGE) / STEP;
        int index = Mathf.RoundToInt(value * stepsCount);

        int final_range = MIN_RANGE + index * STEP;

        VariableHolder.range = final_range;
        rangeText.text = "" + final_range;
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HighscoreUpdate : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(VariableHolder.highscore <= 0) {
            gameObject.SetActive(false);
        } else {
            scoreText.text = "" + VariableHolder.highscore;
        }
    }

}

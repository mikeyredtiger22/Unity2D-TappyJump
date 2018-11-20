using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HighscoreText : MonoBehaviour
{
    Text highscore;

    private void OnEnable()
    {
        highscore = GetComponent<Text>();
        highscore.text = "Highscore: " + PlayerPrefs.GetInt("HIGHSCORE").ToString();
    }
}

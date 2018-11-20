using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour
{
    Text scoreText;
    void Start()
    {
        scoreText = GetComponent<Text>();
        scoreText.text = GameManager.Instance.Score.ToString();
    }
}

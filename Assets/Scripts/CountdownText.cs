using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class CountdownText : MonoBehaviour {

    public delegate void CountDownFinished();
    public static event CountDownFinished OnCountDownFinished;

    Text countDown;

    private void OnEnable()
    {
        countDown = GetComponent<Text>();
        countDown.text = "3";
        StartCoroutine("CountDown");
    }

    IEnumerator CountDown()
    {
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            countDown.text = (count - i).ToString();
            yield return new WaitForSeconds(1);
        }

        OnCountDownFinished();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

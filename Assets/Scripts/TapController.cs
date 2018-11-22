using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TapController : MonoBehaviour {

    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float tapForce = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    Rigidbody2D rigidbody;
    Quaternion downRotation;
    Quaternion forwardRotation;

    GameManager gameManager;


	void Start () {
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, 0);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        rigidbody.simulated = false;
        gameManager = GameManager.Instance;

	}
	
	void Update () {
        if (gameManager.GameOver)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0)) 
        {
            //Time.timeScale += 1;
            transform.rotation = forwardRotation;
            rigidbody.AddForce(Vector2.up * tapForce, ForceMode2D.Force);

        }
        transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "ScoreZone")
        {
            OnPlayerScored(); // event sent to GameManager

        }

        if (collision.tag == "DeadZone")
        {
            rigidbody.simulated = false; // Freeze Player
            OnPlayerDied(); // event sent to GameManager
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameStarted += GameManager_OnGameStarted;
        GameManager.OnGameOverConfirmed += GameManager_OnGameOverConfirmed;;
    }

    private void OnDisable()
    {
        GameManager.OnGameStarted -= GameManager_OnGameStarted;
        GameManager.OnGameOverConfirmed -= GameManager_OnGameOverConfirmed;;
    }

    void GameManager_OnGameStarted()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true;
    }

    void GameManager_OnGameOverConfirmed()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{

    class PoolObject
    {
        public Transform transform;
        public bool inUse;
        public bool consumed = false; // Used for consumable items to be removed from scene
        public PoolObject(Transform transform) { this.transform = transform; }
        public void Use() { inUse = true; }
        public void Dispose() { inUse = false; }
    }

    [System.Serializable]
    public struct YSpawnRange
    {
        public float min;
        public float max;
    }

    public GameObject Prefab;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;
    public float firstSpawnRate;
    public bool useSpawnRange;
    public YSpawnRange ySpawnRange;
    public Vector3 defaultSpawnPos;
    public bool spawnImmediate; // Should be on screen at start
    public Vector3 spawnImmediatePos;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;
    float aspectModifier;
    PoolObject[] poolObjects;
    GameManager gameManager;

    float playerPosX;

    private void Awake()
    {
        Configure();
    }

    void Start()
    {
        gameManager = GameManager.Instance;
        spawnTimer = spawnRate - firstSpawnRate;
    }

    void Update()
    {
        if (gameManager.GameOver) return;
        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameOverConfirmed += GameManager_OnGameOverConfirmed;
    }

    private void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= GameManager_OnGameOverConfirmed;

    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        aspectModifier = Camera.main.aspect / targetAspect;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject gameObject = Instantiate(Prefab) as GameObject;
            Transform transform = gameObject.transform;
            transform.SetParent(transform);
            transform.position = Vector3.one * 10;
            poolObjects[i] = new PoolObject(transform);
            //int a = IndexOf(poolObjects, gameObject);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
            //Spawn();
        }
    }

    void GameManager_OnGameOverConfirmed()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 10;
        }
        if (spawnImmediate)
        {
            SpawnImmediate();
            //Spawn();
        }
    }

    void Spawn() // Changes position of object - Simulating spawning
    {
        Transform transform = GetPoolObject();
        if (transform == null) return;
        Vector3 pos = Vector3.zero;
        pos.x = defaultSpawnPos.x * aspectModifier;
        pos.y = useSpawnRange ? Random.Range(ySpawnRange.min, ySpawnRange.max) : defaultSpawnPos.y;
        transform.position = pos;

    }

    void SpawnImmediate()
    {
        Transform transform = GetPoolObject();
        if (transform == null) return;
        Vector3 pos = Vector3.zero;
        pos.x = spawnImmediatePos.x * aspectModifier;
        pos.y = useSpawnRange ? Random.Range(ySpawnRange.min, ySpawnRange.max) : spawnImmediatePos.y;
        transform.position = pos;
    }

    void Shift()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }

    void CheckDisposeObject(PoolObject poolObject)
    {
        if (poolObject.consumed || poolObject.transform.position.x < -defaultSpawnPos.x * aspectModifier)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 10;
            if (poolObject.consumed)
            {
                poolObject.consumed = false;
            }
        }
    }

    Transform GetPoolObject()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;

            }
        }
        return null;
    }

    public void OnCollisionWithPlayer()
    {
        //PoolObject closestToPlayer = null;
        int count = 0;
        int count2 = 0;
        float[] posXArr = new float[4];
        // Find consumable closest to player/center
        for (int i = 0; i < poolObjects.Length; i++)
        {
            count2++;
            PoolObject currentObject = poolObjects[i];
            //float closestObjectDistance = float.MaxValue;
            if (currentObject.inUse)
            {
                // 0.4 to -0.25
                float xPos = currentObject.transform.position.x;
                posXArr[i] = xPos;

                if (xPos < -0 && xPos > -3)
                {
                    count++;
                    currentObject.consumed = true;
                }

                //if (closestToPlayer == null)
                //{
                //    closestToPlayer = currentObject;
                //    closestObjectDistance = Mathf.Abs((float)-1.5 - (currentObject.transform.position.x));
                //}
                //else
                //{
                //    float currentObjectDistance = Mathf.Abs((float)-1.5 - (currentObject.transform.position.x));
                //    if (currentObjectDistance < closestObjectDistance)
                //    {
                //        closestToPlayer = currentObject;
                //        closestObjectDistance = currentObjectDistance;
                //    }
                //}
            }
        }
        Debug.Log("Count: " + count);
        Debug.Log("In Use: " + count2); // !!! 4 - should be 2
        string posXArrString = "";
        foreach (var item in posXArr)
        {
            posXArrString += " : " + item;
        }

        Debug.Log("PosXArr: " + posXArrString); 
        //closestToPlayer.Dispose();
        //Debug.Log(Mathf.Abs((float)-1.5 - (closestToPlayer.transform.position.x)));
        //closestToPlayer.consumed = true;
    }
}

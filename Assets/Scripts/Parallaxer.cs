using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallaxer : MonoBehaviour
{

    class PoolObject
    {
        public Transform transform;
        public bool inUse;
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
        if (transform == null) return; // Pool size is too small
        Vector3 pos = Vector3.zero;
        pos.x = defaultSpawnPos.x * aspectModifier; // edit later
        pos.y = defaultSpawnPos.y; //Random.Range(ySpawnRange.min, ySpawnRange.max);
        transform.position = pos;

    }

    void SpawnImmediate()
    {
        Transform transform = GetPoolObject();
        if (transform == null) return; // Pool size is too small
        Vector3 pos = Vector3.zero;
        pos.x = spawnImmediatePos.x * aspectModifier; // edit later
        pos.y = spawnImmediatePos.y; ; //Random.Range(ySpawnRange.min, ySpawnRange.max);
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
        if (poolObject.transform.position.x < -defaultSpawnPos.x * aspectModifier)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 10; // do we need this?
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
}

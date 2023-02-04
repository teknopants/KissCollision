using UnityEngine;

public class MarbleRunEngine : MonoBehaviour
{
    public GameObject marblePrefab;
    public Transform spawnPosition;
    public float spawnFrequency = 1f;
    public float randomStartPositionVariation = 0f;

    private float time = 0;

    public void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void Run(float dt)
    {
        time += dt;

        if (time > spawnFrequency)
        {
            time -= spawnFrequency;
            var random = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)) * randomStartPositionVariation;
            Instantiate(marblePrefab, spawnPosition.position + random, Quaternion.identity);
        }
    }
}

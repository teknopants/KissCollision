using UnityEngine;

public class PachinkoEngine: MonoBehaviour
{
    public Transform killFloorYPosition;
    public Transform ballSpawnPosition;
    public GameObject ballPrefab;
    public float ballSpawnFrequency = 1f / 10;
    public float ballSpawnSpeedMin = 33;
    public float ballSpawnSpeedMax = 50;
    public float bounceFriction = 2f;

    private float timer = 0;

    public void Run(float dt)
    {
        // Error checking
        if (!killFloorYPosition)
        {
            Debug.LogError("must assign an object to killFloorYPosition so balls that fall below this point are destroyed");
        }

        if (!ballSpawnPosition)
        {
            Debug.LogError("Must assign a ballSpawnPosition so we know where to spawn balls");
        }

        if (!ballPrefab)
        {
            Debug.LogError("Must assign a ball prefab so we know what to spawn");
        }

        if (!killFloorYPosition || !ballSpawnPosition || !ballPrefab)
        {
            return;
        }

        // Ball Spawning
        timer += dt;
        var spawnTimeAndVariation = ballSpawnFrequency + Mathf.Sin(Time.realtimeSinceStartup) * .15f;
        if (timer >= spawnTimeAndVariation )
        {
            timer -= spawnTimeAndVariation;
            var newBall = Instantiate(ballPrefab, ballSpawnPosition.transform.position, Quaternion.identity);
            var velocity = newBall.AddComponent<Velocity>();
            velocity.velocity.y = Random.Range(ballSpawnSpeedMin, ballSpawnSpeedMax);

            // Override BounceOffWalls default friction with this engine's friction value
            var bounce = newBall.GetComponent<BounceOffWalls>();
            if (!bounce)
            {
                bounce = newBall.AddComponent<BounceOffWalls>();
            }
            bounce.friction = bounceFriction;
        }
    }
}

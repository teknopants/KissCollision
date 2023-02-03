using UnityEngine;

public class PachinkoUpdate: MonoBehaviour
{
    public Transform killFloorYPosition;
    public Transform ballSpawnPosition;
    public GameObject ballPrefab;
    public float ballSpawnFrequency = 1f / 10;
    public float ballSpawnSpeedMin = 33;
    public float ballSpawnSpeedMax = 50;
    public float fallForce = .1f;
    public float bounceFriction = 2f;

    private float timer = 0;

    public void Start()
    {
        Application.targetFrameRate = 60;
    }

    public void Update()
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
        timer += Time.deltaTime;
        var spawnTimeAndVariation = ballSpawnFrequency + Mathf.Sin(Time.realtimeSinceStartup) * .15f;
        if (timer >= spawnTimeAndVariation )
        {
            timer -= spawnTimeAndVariation;
            var newBall = Instantiate(ballPrefab, ballSpawnPosition.transform.position, Quaternion.identity);
            var velocity = newBall.AddComponent<Velocity>();
            velocity.velocity.y = Random.Range(ballSpawnSpeedMin, ballSpawnSpeedMax);
        }

        // Check all entities with a Velocity Component Attatched
        foreach (Velocity velocity in Object.FindObjectsOfType<Velocity>())
        {
            var entity = velocity.gameObject;
            var collideComponent = entity.GetComponent<Collide>();

            if (!collideComponent)
            {
                Debug.LogError(entity.ToString() + " needs a Collide component to know what layer we're colliding against");
                continue;
            }

            // Project motion
            var previousVelocity = velocity.velocity;
            KissCollision.MotionPath motionPath = KissCollision.ProjectCapsule(entity.GetComponent<CapsuleCollider>(), velocity.velocity, collideComponent.layerMask, 3);
            motionPath.Draw();

            // Bounce off of walls
            foreach(RaycastHit hit in motionPath.Collisions)
            {
                if (hit.collider)
                {
                    // friction based on how different old velocity direction and new velocity direction is
                    var angleDif = Vector3.Dot(previousVelocity.normalized, Vector3.ProjectOnPlane(previousVelocity.normalized, hit.normal));
                    var friction = Mathf.Lerp(bounceFriction, 1f, Mathf.Clamp01(angleDif));
                    velocity.velocity = Vector3.Reflect(velocity.velocity / friction, hit.normal);
                    Debug.DrawRay(hit.point, velocity.velocity.normalized * friction, Color.green, 1 / 60);
                }
            }

            entity.transform.position = motionPath.EndPosition;

            // add gravity
            velocity.velocity.y += fallForce;

            // Destroy balls that fall below our killFloorYPosition object's y position
            if (entity.transform.position.y < killFloorYPosition.transform.position.y)
            {
                Destroy(entity);
            }
        }
    }
}

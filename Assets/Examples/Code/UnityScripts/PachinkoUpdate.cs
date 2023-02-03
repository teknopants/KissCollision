using UnityEngine;
using QuickInput;

public class PachinkoUpdate: MonoBehaviour
{
    public float timer = 0;
    public float spawnBallTime = 1f / 10;
    public GameObject ballPrefab;

    public void Update()
    {
        timer += Time.DeltaTime;
        if (timer > spawnBallTime)
        {
            timer -= spawnBallTime;
            var newBall = Instantiate(ballPrefab);
            var velocity = newBall.AddComponent<Velocity>();
            velocity.y = Random.Range(0.9, 2);
        }

        // Check all entities with a Velocity Component Attatched
        foreach (Velocity velocity in Object.FindObjectsOfType<Velocity>())
        {
            var entity = player.gameObject;
            var collide = entity.GetComponent<Collide>();

            if (!collide)
            {
                Debug.LogError(entity.ToString() + " needs a Collide component to know what layer we're colliding against");
                continue;
            }

            // Project motion
            KissCollision.MotionPath motionPath = KissCollision.ProjectCapsule(player.GetComponent<CapsuleCollider>(), motion, collide.layerMask);

            entity.gameObject.transform.position = motionPath.EndPosition;
        }
    }
}

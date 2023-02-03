using UnityEngine;
using QuickInput;

public class PachinkoUpdate: MonoBehaviour
{
    public float timer = 0;
    public float spawnBallTime = 1f / 10;
    public GameObject ballPrefab;

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer > spawnBallTime)
        {
            timer -= spawnBallTime;
            var newBall = Instantiate(ballPrefab);
            var velocity = newBall.AddComponent<Velocity>();
            velocity.velocity.y = Random.Range(0.9f, 2);
        }

        // Check all entities with a Velocity Component Attatched
        foreach (Velocity velocity in Object.FindObjectsOfType<Velocity>())
        {
            var entity = velocity.gameObject;
            var collide = entity.GetComponent<Collide>();

            if (!collide)
            {
                Debug.LogError(entity.ToString() + " needs a Collide component to know what layer we're colliding against");
                continue;
            }

            // Project motion
            KissCollision.MotionPath motionPath = KissCollision.ProjectCapsule(entity.GetComponent<CapsuleCollider>(), velocity.velocity, collide.layerMask);

            foreach(RaycastHit hit in motionPath.Collisions)
            {
                if (hit.normal.sqrMagnitude == 0) {continue;}

                velocity.velocity = Vector3.Reflect(velocity.velocity, hit.normal);
                Debug.DrawRay(hit.point, velocity.velocity * 10, Color.green, 4);
            }

            entity.gameObject.transform.position = motionPath.EndPosition;
        }
    }
}

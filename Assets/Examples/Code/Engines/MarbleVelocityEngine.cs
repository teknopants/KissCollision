using UnityEngine;

public class MarbleVelocityEngine : MonoBehaviour
{
    void Run(float dt)
    {
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
            var bounceOffWalls = entity.GetComponent<BounceOffWalls>();
            if (bounceOffWalls)
            {
                foreach (RaycastHit hit in motionPath.Collisions)
                {
                    if (hit.collider)
                    {
                        // friction based on how different old velocity direction and new velocity direction is
                        var angleDif = Vector3.Dot(previousVelocity.normalized, Vector3.ProjectOnPlane(previousVelocity.normalized, hit.normal));
                        var friction = Mathf.Lerp(bounceOffWalls.friction, 1f, Mathf.Clamp01(angleDif));
                        velocity.velocity = Vector3.Reflect(velocity.velocity / friction, hit.normal);
                        Debug.DrawRay(hit.point, velocity.velocity.normalized * friction, Color.green, 1 / 60);
                    }
                }
            }

            entity.transform.position = motionPath.EndPosition;
        }
    }
}

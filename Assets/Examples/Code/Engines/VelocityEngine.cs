using UnityEngine;

public class VelocityEngine : Engine
{
    public override void Run(float dt)
    {
        // Fall Force
        foreach (FallForce fallForce in Object.FindObjectsOfType<FallForce>())
        {
            GameObject entity = fallForce.gameObject;

            if (entity.GetComponent<Grounded>()) continue;

            Velocity velocity = entity.GetComponent<Velocity>();

            if (!velocity)
            {
                velocity = entity.AddComponent<Velocity>();
            }

            if (velocity)
            {
                velocity.velocity.y += fallForce.force * dt;
            }
        }

        foreach(Velocity velocity in Object.FindObjectsOfType<Velocity>())
        {
            var entity = velocity.gameObject;
            var collide = entity.GetComponent<Collide>();
            var capsuleCollider = entity.GetComponent<CapsuleCollider>();
            var grounded = entity.GetComponent<Grounded>();

            var secondaryCapsulePoint = entity.GetComponent<SecondaryCapsulePoint>();
            Vector3 capsulePoint1 = entity.transform.position;
            if (secondaryCapsulePoint)
            {
                capsulePoint1 = secondaryCapsulePoint.entity.transform.position;
            }

            if (collide)
            {
                KissCollision.MotionPath motionPath = KissCollision.MoveCapsule(capsuleCollider, velocity.velocity, collide.layerMask, out Vector3 velocityResult);
                velocity.velocity = velocityResult;

                foreach (RaycastHit hit in motionPath.Collisions)
                {
                    Debug.DrawRay(hit.point, velocity.velocity, Color.green, 2);
                }
            }
        }

        // Check if on ground
        foreach (CheckGrounded checkGrounded in Object.FindObjectsOfType<CheckGrounded>())
        {
            GameObject entity = checkGrounded.gameObject;
            RaycastHit hit;
            var ray = new Ray(entity.transform.position, Vector3.down);

            var collide = entity.GetComponent<Collide>();
            if (!collide)
            {
                Debug.LogError(entity.ToString() + " is trying to check for ground but they do not have a Collide component specifying which layerMask to check");
                continue;
            }

            var grounded = entity.GetComponent<Grounded>();

            if (Physics.SphereCast(ray, checkGrounded.radius, out hit, checkGrounded.distance, collide.layerMask))
            {
                if (!grounded)
                {
                    grounded = entity.AddComponent<Grounded>();
                }

                grounded.normal = hit.normal;
                entity.transform.Translate(Vector3.down * (hit.distance - .01f));
            }
            else if (entity.GetComponent<Grounded>())
            {
                // Allow player to jump right after walking off a ledge
                grounded.coyoteTime -= dt;
                if (grounded.coyoteTime <= 0)
                {
                    Object.Destroy(grounded);
                }
            }
        }
    }
}

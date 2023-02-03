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
            var radius = .1f;
            var grounded = entity.GetComponent<Grounded>();
            var collisionRadius = entity.GetComponent<CollisionRadius>();
            if (collisionRadius)
            {
                radius = collisionRadius.radius;
            }

            var secondaryCapsulePoint = entity.GetComponent<SecondaryCapsulePoint>();
            Vector3 capsulePoint1 = entity.transform.position;
            if (secondaryCapsulePoint)
            {
                capsulePoint1 = secondaryCapsulePoint.entity.transform.position;
            }

            if (collide)
            {
                /*if (entity.GetComponent<CharacterControllerPlatformer3D>())
                {
                    RaycastHit[] groundPlaneHits = { };
                    Vector3 groundPlaneNormal = new Vector3(0, 1, 0);
                    if (grounded)
                    {
                        groundPlaneNormal = grounded.normal;
                    }
                    Vector3 groundPlaneVelocity = Vector3.ProjectOnPlane(velocity.velocity, groundPlaneNormal);
                    (entity.transform.position, groundPlaneHits) = KissCollision.MoveCapsule(entity.transform.position, capsulePoint1, groundPlaneVelocity, radius, collide.layerMask, 2);

                    foreach (RaycastHit hit in groundPlaneHits)
                    {
                        if (!hit.transform) continue;
                        velocity.velocity = Vector3.ProjectOnPlane(velocity.velocity, hit.normal);
                        Debug.DrawRay(hit.point, velocity.velocity, Color.green, 10);
                    }

                    RaycastHit[] verticalHits = { };
                    Vector3 verticalVelocity = new Vector3(0, velocity.velocity.y, 0);
                    (entity.transform.position, verticalHits) = KissCollision.MoveCapsule(entity.transform.position, capsulePoint1, verticalVelocity, radius, collide.layerMask, 2);

                    foreach (RaycastHit hit in verticalHits)
                    {
                        if (!hit.transform) continue;
                        velocity.velocity = Vector3.ProjectOnPlane(velocity.velocity, hit.normal);
                        Debug.DrawRay(hit.point, velocity.velocity, Color.white, 10);
                    }
                }
                else
                {
                    RaycastHit[] hits = { };
                    (entity.transform.position, hits) = KissCollision.MoveCapsule(entity.transform.position, capsulePoint1, velocity.velocity, radius, collide.layerMask, 3);

                    foreach (RaycastHit hit in hits)
                    {
                        if (!hit.transform) continue;

                        velocity.velocity = Vector3.ProjectOnPlane(velocity.velocity, hit.normal);
                        Debug.DrawRay(hit.point, velocity.velocity, Color.yellow, 10);
                    }
                }*/
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

using System;
using UnityEngine;

public class VelocityEngine : MonoBehaviour
{
    public void Run(float dt)
    {
        // Motion
        foreach (Velocity velocity in FindObjectsOfType<Velocity>())
        {
            var velocityResult = velocity.velocity;
            var entity = velocity.gameObject;
            var collide = entity.GetComponent<Collide>();
            var capsuleCollider = entity.GetComponent<CapsuleCollider>();
            KissCollision.MotionPath motionPath = new KissCollision.MotionPath();

            if (capsuleCollider)
            {
                motionPath = KissCollision.MoveCapsule(entity.GetComponent<CapsuleCollider>(), velocity.velocity, collide.layerMask, out velocityResult);
            }
            else
            {
                motionPath = KissCollision.MovePoint(entity, velocity.velocity, collide.layerMask, out velocityResult);
            }

            velocity.velocity = velocityResult;
            Debug.DrawRay(entity.transform.position, velocityResult * 10, Color.blue, 1);

            motionPath.Draw();
        }

        // Check if on ground
        foreach (CheckGrounded checkGrounded in FindObjectsOfType<CheckGrounded>())
        {
            GameObject entity = checkGrounded.gameObject;
            var ray = new Ray(entity.transform.position, Vector3.down);

            var velocity = entity.GetComponent<Velocity>();
            if (!velocity)
            {
                Debug.LogError(entity.ToString() + " needs a velocity component in order to check ground");
            }

            var collide = entity.GetComponent<Collide>();
            if (!collide)
            {
                Debug.LogError(entity.ToString() + " is trying to check for ground but they do not have a Collide component specifying which layerMask to check");
                continue;
            }

            var grounded = entity.GetComponent<Grounded>();

            if (Physics.Raycast(ray, out RaycastHit hit, checkGrounded.distance,  collide.layerMask))
            {
                if (velocity.velocity.y <= 0)
                {
                    if (!grounded)
                    {
                        grounded = entity.AddComponent<Grounded>();
                    }

                    grounded.normal = hit.normal;
                }
            }
            else if (entity.GetComponent<Grounded>())
            {
                // Allow player to jump right after walking off a ledge
                grounded.coyoteTime -= dt;
                if (grounded.coyoteTime <= 0)
                {
                    Destroy(grounded);
                }
            }
        }
    }
}

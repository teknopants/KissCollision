using UnityEngine;

public class FallForceEngine : MonoBehaviour
{
    void Run(float dt)
    {
        foreach(FallForce fallForce in Object.FindObjectsOfType<FallForce>())
        {
            var entity = fallForce.gameObject;
            var velocity = entity.GetComponent<Velocity>();
            
            if (!velocity)
            {
                velocity = entity.AddComponent<Velocity>();
            }

            velocity.velocity.y += fallForce.force;
        }
    }
}

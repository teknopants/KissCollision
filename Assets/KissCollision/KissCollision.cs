using UnityEngine;

// Keep it short and simple with KISS COLLISION
// by Beau Blyth

public static class KissCollision
{
    private static readonly float skin = .025f;

    public static MotionPath MoveSphere(SphereCollider moveMe, Vector3 motion, LayerMask layerMask, int steps = 2)
    {
        RaycastHit[] collisions = new RaycastHit[steps];
        Vector3[] motionSteps = new Vector3[steps];
        Vector3 startPosition = moveMe.transform.position;

        Vector3 position = startPosition;


        for (var i = 0; i < steps; i++)
        {
            var stepDistance = motion.magnitude;

            if (Physics.SphereCast(
                    position,
                    moveMe.radius,
                    motion,
                    out RaycastHit hit,
                    motion.magnitude,
                    layerMask)
                )
            {
                stepDistance = hit.distance - skin;
                collisions[i] = hit;
                Debug.DrawRay(hit.point, hit.normal, Color.yellow, .25f);
            }

            motionSteps[i] = motion.normalized * stepDistance;
            position += motion.normalized * stepDistance;

            float remainingDistance = motion.magnitude - stepDistance;
            motion = Vector3.ProjectOnPlane(motion.normalized, hit.normal) * remainingDistance;

            
            // if in wall, try to push out
            foreach (Collider collider in Physics.OverlapSphere(position, moveMe.radius, layerMask))
            {
                Physics.ComputePenetration(moveMe, position, moveMe.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out Vector3 pushOutDirection, out float pushOutDistance);
                position += pushOutDirection * pushOutDistance;
            }

            // if still in wall, undo this motion
            if (Physics.OverlapSphere(position, moveMe.radius, layerMask).Length > 0)
            {
                return new MotionPath(startPosition, startPosition, new Vector3[0], new RaycastHit[0]);
            }

            if (remainingDistance <= 0)
            {
                break;
            }
        }

        return new MotionPath(startPosition, position, motionSteps, collisions);
    }

    public struct MotionPath
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public Vector3[] MotionSteps;
        public RaycastHit[] Collisions;

        public MotionPath(Vector3 startPosition, Vector3 endPosition, Vector3[] motionSteps, RaycastHit[] collisions)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            MotionSteps = motionSteps;
            Collisions = collisions;
        }

        public void Draw()
        {
            var pos = StartPosition;
            for (var i = 0; i < MotionSteps.Length; i++)
            {
                Debug.DrawLine(pos, pos + MotionSteps[i], Color.red, .25f);
                pos += MotionSteps[i];
            }
        }
    }
}

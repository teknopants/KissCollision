using UnityEngine;

// Keep it short and simple with KISS COLLISION
// by Beau Blyth

public static class KissCollision
{
    private static readonly float skin = .01f;
    private static readonly int pushOutAttempts = 10;

    /// <summary>
    /// Projects a capsule in a motion, sliding against any collider in layerMask.
    /// Returns a MotionPath, which contains the end position, the motion vectors it took to get there, and any collisions along the way
    /// </summary>
    public static MotionPath ProjectCapsule(CapsuleCollider capsuleCollider, Vector3 motion, LayerMask layerMask, int steps = 3)
    {
        RaycastHit[] collisions = new RaycastHit[steps];
        Vector3[] motionSteps = new Vector3[steps];
        Vector3 startPosition = capsuleCollider.transform.position;

        Vector3 point0 = startPosition - capsuleCollider.height * capsuleCollider.transform.up / 4;
        Vector3 point1 = point0 + capsuleCollider.height * capsuleCollider.transform.up / 2;

        for (var i = 0; i < steps; i++)
        {
            var stepDistance = motion.magnitude;

            if (Physics.CapsuleCast(
                    point0,
                    point1,
                    capsuleCollider.radius,
                    motion,
                    out RaycastHit hit,
                    motion.magnitude,
                    layerMask)
                )
            {
                stepDistance = hit.distance - skin;
                collisions[i] = hit;
                Debug.DrawRay(hit.point, hit.normal / 2, Color.yellow, .25f);
            }

            motionSteps[i] = motion.normalized * stepDistance;
            point0 += motion.normalized * stepDistance;
            point1 = point0 + capsuleCollider.height * capsuleCollider.transform.up / 2;

            float remainingDistance = motion.magnitude - stepDistance;
            motion = Vector3.ProjectOnPlane(motion.normalized, hit.normal) * remainingDistance;

            // if in wall, try to push out
            for (var j = 0; j < pushOutAttempts; j++)
            {
                foreach (Collider collider in Physics.OverlapCapsule(point0, point1, capsuleCollider.radius, layerMask))
                {
                    Physics.ComputePenetration(capsuleCollider, point0, capsuleCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out Vector3 pushOutDirection, out float pushOutDistance);
                    point0 += pushOutDirection * pushOutDistance;
                }
            }

            // if still in wall, undo this motion
            if (Physics.OverlapCapsule(point0, point1, capsuleCollider.radius, layerMask).Length > 0)
            {
                return new MotionPath(startPosition, startPosition, new Vector3[0], new RaycastHit[0]);
            }

            if (remainingDistance <= 0)
            {
                break;
            }
        }

        return new MotionPath(startPosition, point0 + capsuleCollider.height * capsuleCollider.transform.up / 4, motionSteps, collisions);
    }

    /// <summary>
    /// Struct containing information about a projected motion path.
    /// StartPosition is where the projection started. EndPosition is the end of the projection.
    /// MotionSteps[] are all the motion vectors it took to get from StartPosition to EndPosition.
    /// Collisions[] contain RayCastHit information about anything collided with along the way
    /// </summary>
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

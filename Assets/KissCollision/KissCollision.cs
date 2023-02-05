using UnityEngine;
using System;

// Keep it short and simple with KISS COLLISION
// by Beau Blyth

public static class KissCollision
{
    private static readonly float skin = .001f;
    private static readonly int pushOutAttempts = 2;

    /// <summary>
    /// The simplest way to move a point in space. It's basically ProjectPoint() but it also moves the transform to MotionPath.endPosition.
    /// For sliding physics set the gameobject's velocity to velocityResult
    /// </summary>
    public static MotionPath MovePoint(GameObject gameObject, Vector3 motion, LayerMask layerMask, out Vector3 velocityResult)
    {
        MotionPath motionPath = ProjectPoint(gameObject.transform.position, motion, layerMask);
        gameObject.transform.position = motionPath.EndPosition;

        Vector3 totalNormal = Vector3.zero;
        foreach (var hit in motionPath.Collisions)
        {
            totalNormal += hit.normal;
        }
        velocityResult = Vector3.ProjectOnPlane(velocityResult.normalized, hit.normal) * motion.magnitude;

        return motionPath;
    }

    /// <summary>
    /// The simplest way to move a capsule collider. It's basically ProjectCapsule but it also moves the transform to MotionPath.endPosition.
    /// For sliding physics set the gameobject's velocity to velocityResult
    /// </summary>
    public static MotionPath MoveCapsule(CapsuleCollider capsuleCollider, Vector3 motion, LayerMask layerMask, out Vector3 velocityResult)
    {
        MotionPath motionPath = ProjectCapsule(capsuleCollider, motion, layerMask);
        capsuleCollider.transform.position = motionPath.EndPosition;

        velocityResult = motion;
        foreach(var hit in motionPath.Collisions)
        {
            velocityResult = Vector3.ProjectOnPlane(velocityResult.normalized, hit.normal).normalized * motion.magnitude;
        }

        return motionPath;
    }

    /// <summary>
    /// Projects a point in a motion, sliding against any collider in layerMask
    /// </summary>
    public static MotionPath ProjectPoint(Vector3 point, Vector3 motion, LayerMask layerMask, int steps = 3)
    {
        Vector3 initialMovement = motion;

        RaycastHit[] collisions = new RaycastHit[0];
        Vector3[] motionSteps = new Vector3[1];
        Vector3 startPosition = point;

        for (var i = 0; i < steps; i++)
        {
            var stepDistance = motion.magnitude;

            if (Physics.Raycast(
                    point,
                    motion,
                    out RaycastHit hit,
                    motion.magnitude,
                    layerMask)
                )
            {
                stepDistance = hit.distance - skin;
                Array.Resize<RaycastHit>(ref collisions, i + 1);
                collisions[i] = hit;
                Debug.DrawRay(hit.point, hit.normal / 2, Color.yellow, .25f);
            }

            Array.Resize<Vector3>(ref motionSteps, i + 1);
            motionSteps[i] = motion.normalized * stepDistance;
            point += motion.normalized * stepDistance;
            Debug.DrawRay(point, motion.normalized * stepDistance, Color.green, 1);

            float remainingDistance = motion.magnitude;
            motion = Vector3.ProjectOnPlane(motion.normalized, hit.normal) * remainingDistance;

            // if motion goes back on inital vector, project onto initial vector plane
            if (Vector3.Dot(initialMovement, motion) < 0)
            {
                motion = Vector3.ProjectOnPlane(motion, initialMovement.normalized);
            }

            // if in wall, try to push out
            /*for (var j = 0; j < pushOutAttempts; j++)
            {
                var radius = skin;
                Collider[] colliders = Physics.OverlapSphere(point, radius, layerMask);
                if (colliders.Length == 0) { break; }

                var pushOutVector = Vector3.zero;
                foreach (Collider collider in colliders)
                {
                    var closestPointOnCollider = collider.ClosestPoint(point);
                    var difference = closestPointOnCollider - point;
                    var depth = skin - difference.magnitude;
                    pushOutVector += difference.normalized * depth;
                    Debug.DrawRay(closestPointOnCollider, -pushOutVector, Color.magenta, 1);
                }

                point += pushOutVector;
            }*/
        }

        return new MotionPath(startPosition, point, motionSteps, collisions, motion);
    }

    /// <summary>
    /// Projects a capsule in a motion, sliding against any collider in layerMask.
    /// Returns a MotionPath, which contains the end position, the motion vectors it took to get there, and any collisions along the way
    /// </summary>
    public static MotionPath ProjectCapsule(CapsuleCollider capsuleCollider, Vector3 motion, LayerMask layerMask, int steps = 3)
    {
        Vector3 initialMovement = motion;

        RaycastHit[] collisions = new RaycastHit[0];
        Vector3[] motionSteps = new Vector3[1];
        Vector3 startPosition = capsuleCollider.transform.position;

        float height = Mathf.Max(0, capsuleCollider.height - 1);
        Vector3 point0 = startPosition - height * capsuleCollider.transform.up / 2;
        Vector3 point1 = point0 + height * capsuleCollider.transform.up;

        for (var i = 0; i < steps; i++)
        {
            var stepDistance = motion.magnitude;

            if (Physics.CapsuleCast(
                    point0 - motion.normalized * skin,
                    point1 - motion.normalized * skin,
                    capsuleCollider.radius,
                    motion,
                    out RaycastHit hit,
                    motion.magnitude,
                    layerMask)
                )
            {
                stepDistance = hit.distance - skin;
                Array.Resize<RaycastHit>(ref collisions, i + 1);
                collisions[i] = hit;
                Debug.DrawRay(hit.point, hit.normal / 2, Color.yellow, .25f);
            }

            Array.Resize<Vector3>(ref motionSteps, i + 1);
            motionSteps[i] = motion.normalized * stepDistance;
            point0 += motion.normalized * stepDistance;
            point1 = point0 + height * capsuleCollider.transform.up;

            float remainingDistance = motion.magnitude - (stepDistance + skin);
            motion = Vector3.ProjectOnPlane(motion.normalized, hit.normal) * remainingDistance;

            // if motion goes back on inital vector, project onto initial vector plane
            if (Vector3.Dot(initialMovement, motion) < 0)
            {
                motion = Vector3.ProjectOnPlane(motion, initialMovement.normalized);
            }

            // if in wall, try to push out
            for (var j = 0; j < pushOutAttempts; j++)
            {
                Collider[] colliders = Physics.OverlapCapsule(point0, point1, capsuleCollider.radius + skin, layerMask);
                if (colliders.Length == 0) { break; }

                foreach (Collider collider in colliders)
                {
                    Vector3 projectedPosition = point0 + height * capsuleCollider.transform.up / 2;
                    Physics.ComputePenetration(capsuleCollider, projectedPosition, capsuleCollider.transform.rotation, collider, collider.transform.position, collider.transform.rotation, out Vector3 pushOutDirection, out float pushOutDistance);
                    point0 += pushOutDirection * pushOutDistance;
                    point1 += pushOutDirection * pushOutDistance;
                }
            }

            // if still in wall, undo this motion
            if (Physics.OverlapCapsule(point0, point1, capsuleCollider.radius - skin, layerMask).Length > 0)
            {
                Debug.DrawLine(startPosition, point0, Color.magenta, 1);
                Debug.DrawRay(startPosition, (point0 - startPosition) * 10, Color.magenta, 1);
                return new MotionPath(startPosition, startPosition, new Vector3[0], new RaycastHit[0], Vector3.zero);
            }
        }

        return new MotionPath(startPosition, point0 + height * capsuleCollider.transform.up / 2, motionSteps, collisions, motion);
    }

    /// <summary>
    /// Struct containing information about a projected motion path.
    /// StartPosition is where the projection started. EndPosition is the end of the projection.
    /// MotionSteps[] are all the motion vectors it took to get from StartPosition to EndPosition.
    /// Collisions[] contain RayCastHit information about anything collided with along the way.
    /// EndVelocity is the resulting velocity after the motion was projected onto all collisions.
    /// </summary>
    public struct MotionPath
    {
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public Vector3[] MotionSteps;
        public RaycastHit[] Collisions;
        public Vector3 EndVelocity;

        public MotionPath(Vector3 startPosition, Vector3 endPosition, Vector3[] motionSteps, RaycastHit[] collisions, Vector3 endVelocity)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            MotionSteps = motionSteps;
            Collisions = collisions;
            EndVelocity = endVelocity;
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


// FAQ

// My entity won't move

// are you using a PROJECTcapsule() or PROJECTpoint() method? if so you have to set the transform position to the returned MotionPath's EndPosition.
// That or use MOVEcapsule() or MOVEpoint() which essentially does that for you
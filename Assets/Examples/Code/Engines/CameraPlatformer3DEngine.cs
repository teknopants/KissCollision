using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraPlatformer3DEngine : Engine
{
    public override void Run(float dt)
    {
        foreach(TargetTracker targetTracker in Object.FindObjectsOfType<TargetTracker>())
        {
            var entity = targetTracker.gameObject;
            var target = targetTracker.target;
            var newPosition = entity.transform.position;

            if (!target)
            {
                Debug.LogError(targetTracker.ToString() + " does not have a target to track");
                continue;
            }

            newPosition.x = target.transform.position.x;
            newPosition.z = target.transform.position.z;

            var facingDirection = target.GetComponent<FacingDirection>();
            var characterController = target.GetComponent<CharacterControllerPlatformer3D>();
            if (facingDirection)
            {
                targetTracker.lookAheadPosition = Vector3.Lerp(targetTracker.lookAheadPosition, facingDirection.facingDirection, targetTracker.lookAheadLerpTime);
                var dot = Mathf.Clamp01(Vector3.Dot(targetTracker.lookAheadPosition, characterController.cameraTransform.forward) + 1);
                Debug.Log(dot);
                newPosition += targetTracker.lookAheadPosition * dot;
            }

            var groundY = entity.transform.position.y;
            Debug.DrawRay(new Vector3(newPosition.x, groundY, newPosition.z), Vector3.up, Color.green, .25f);

            // if ground youre jumping over is higher than the old ground, treat that as new ground height
            if (Physics.Raycast(
                new Ray(target.transform.position, Vector3.down),
                out RaycastHit hit,
                200,
                1 << LayerMask.NameToLayer("Level"))
                )
            {
                Debug.DrawRay(hit.point, Vector3.up, Color.red, .25f);
                groundY = Mathf.Max(groundY, hit.point.y + targetTracker.verticalOffset);
            }

            newPosition.y = Mathf.Lerp(newPosition.y, Mathf.Clamp(groundY, target.transform.position.y - targetTracker.heightLimit + targetTracker.verticalOffset, target.transform.position.y + targetTracker.fallLimit + targetTracker.verticalOffset), targetTracker.verticalLerpTime);

            entity.transform.position = newPosition;
        }

        foreach(CameraPlatformer3D cameraPlatformer3D in Object.FindObjectsOfType<CameraPlatformer3D>())
        {
            var entity = cameraPlatformer3D.gameObject;

            var lookTarget = cameraPlatformer3D.target.transform.position;

            cameraPlatformer3D.lookAtPosition = Vector3.Lerp(cameraPlatformer3D.lookAtPosition, lookTarget, cameraPlatformer3D.lookAtLerpTime);
            entity.transform.forward = cameraPlatformer3D.lookAtPosition - cameraPlatformer3D.virtualPosition;

            var groundDistance = cameraPlatformer3D.virtualPosition - cameraPlatformer3D.target.transform.position;
            groundDistance.y = 0;

            var groundPosition = cameraPlatformer3D.target.transform.position + groundDistance.normalized * cameraPlatformer3D.groundDistance;
            cameraPlatformer3D.virtualPosition = new Vector3(groundPosition.x, Mathf.Lerp(cameraPlatformer3D.virtualPosition.y, cameraPlatformer3D.target.transform.position.y + cameraPlatformer3D.heightOffset, cameraPlatformer3D.verticalPositionLerpTime), groundPosition.z);

            var distance = cameraPlatformer3D.groundDistance;

            // Dont go through walls
            /*if (Physics.Raycast(
                cameraPlatformer3D.lookAtPosition,
                cameraPlatformer3D.virtualPosition - cameraPlatformer3D.lookAtPosition,
                out RaycastHit hit,
                cameraPlatformer3D.groundDistance,
                1 << LayerMask.NameToLayer("Level"))
                )
            {
                distance = hit.distance;
            }*/

            entity.transform.position = cameraPlatformer3D.target.transform.position - (cameraPlatformer3D.lookAtPosition - cameraPlatformer3D.virtualPosition).normalized * distance;
        }
    }
}


using UnityEngine;

public class CameraPlatformer3D : MonoBehaviour
{
    public GameObject target;
    public Vector3 virtualPosition = Vector3.zero;
    public float groundDistance = 5f;
    public float heightOffset = 4f;
    public float lookAtLerpTime = .1f;
    public float verticalPositionLerpTime = .1f;
    public Vector3 lookAtPosition = Vector3.zero;
}

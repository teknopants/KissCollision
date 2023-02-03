using UnityEngine;

public class TargetTracker : MonoBehaviour
{
    public GameObject target;
    public float heightLimit = 4f;
    public float fallLimit = 1f;
    public float verticalOffset = 2f;
    public float verticalLerpTime = .1f;
    public Vector3 lookAheadPosition = Vector3.zero;
    public float lookAheadLerpTime = .05f;
}

using UnityEngine;

public class CharacterControllerPlatformer3D : MonoBehaviour
{
    public float accelGround = .25f;
    public float accelAir = .25f;
    public float dampingGround = .75f;
    public float dampingAir = .8f;
    public float maxSpeed = 1f;
    public float jumpSpeed = 1f;
    public Transform cameraTransform;
}

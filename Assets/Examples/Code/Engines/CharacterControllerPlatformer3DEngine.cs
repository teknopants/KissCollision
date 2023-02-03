using UnityEngine;
using QuickInput;

public class CharacterControllerPlatformer3DEngine : Engine
{
    private Button Up = new();
    private Button Down = new();
    private Button Left = new();
    private Button Right = new();
    private Button Jump = new();
    private Axis Horizontal = new();
    private Axis Vertical = new();

    public override void Run(float dt)
    {

        Up.CheckForInput(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W));
        Down.CheckForInput(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S));
        Left.CheckForInput(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A));
        Right.CheckForInput(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D));

        Jump.CheckForInput(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0));

        Horizontal.CheckForInput(Right, Left);
        Vertical.CheckForInput(Up, Down);

        var inputVector = new Vector2(Horizontal.position, Vertical.position);
        var inputDistance = new Vector2(Horizontal.position, Vertical.position).normalized.magnitude;

        // Timers
        foreach (var c in Object.FindObjectsOfType<JumpBufferTimer>())
        {
            c.time -= dt;
            if (c.time <= 0)
            {
                Object.Destroy(c);
            }
        }

        // Character Controller
        foreach (var c in Object.FindObjectsOfType<CharacterControllerPlatformer3D>())
        {
            var entity = c.gameObject;
            var velocity = entity.GetComponent<Velocity>();
            var grounded = entity.GetComponent<Grounded>();

            var localInputVectorFlat = c.cameraTransform.forward * inputVector.y + c.cameraTransform.right * inputVector.x;
            localInputVectorFlat.y = 0;
            localInputVectorFlat.Normalize();

            var facingDirection = entity.GetComponent<FacingDirection>();
            if (facingDirection && inputDistance > 0)
            {
                facingDirection.facingDirection = localInputVectorFlat;
            }

            // Movement
            if (inputDistance > 0)
            {
                var groundMotionVector = localInputVectorFlat;
                if (grounded)
                {
                    groundMotionVector = Vector3.ProjectOnPlane(groundMotionVector, grounded.normal.normalized);
                    groundMotionVector.y = Mathf.Min(groundMotionVector.y, 0);
                }
                Debug.DrawRay(entity.transform.position, groundMotionVector, Color.red, 1 / 60f);

                var accel = grounded ? c.accelGround : c.accelAir;

                if (velocity)
                {
                    velocity.velocity += groundMotionVector * inputDistance * accel;

                    var flatVelocity = new Vector3(velocity.velocity.x, 0, velocity.velocity.z);
                    if (flatVelocity.magnitude > c.maxSpeed)
                    {
                        flatVelocity = flatVelocity.normalized * c.maxSpeed;
                        velocity.velocity = new Vector3(flatVelocity.x, velocity.velocity.y, flatVelocity.z);
                    }
                }
            }
            else
            {
                // Slow down
                if (grounded)
                {
                    velocity.velocity *= c.dampingGround;

                    if (velocity.velocity.magnitude < .1f)
                    {
                        velocity.velocity = Vector3.zero;
                    }
                }
                else
                {
                    velocity.velocity.x *= c.dampingAir;
                    velocity.velocity.z *= c.dampingAir;
                }
            }

            // Jump
            if (Jump.IsPressed() && !entity.GetComponent<JumpBufferTimer>())
            {
                Debug.Log("Jumped!");
                entity.AddComponent<JumpBufferTimer>();
            }

            if (entity.GetComponent<JumpBufferTimer>() && grounded)
            {
                Object.Destroy(grounded);
                Object.Destroy(entity.GetComponent<JumpBufferTimer>());
                velocity.velocity.y = c.jumpSpeed;
            }
            if (Jump.IsReleased())
            {
                velocity.velocity.y /= 2;
            }
        }
    }
}

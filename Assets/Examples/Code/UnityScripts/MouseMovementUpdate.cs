using UnityEngine;
using QuickInput;

public class MouseMovementUpdate: MonoBehaviour
{
    public Button ScanThenMove = new();
    public Button Move = new();
    public Button ScanAndMoveRandomly = new();

    private Button Up = new();
    private Button Down = new();
    private Button Left = new();
    private Button Right = new();
    private Axis Horizontal = new();
    private Axis Vertical = new();

    private float arrowKeyMovementSpeed = 15f;

    public void Run(float dt)
    {
        // Input for StepMotion Engine Example
        ScanThenMove.CheckForInput(Input.GetKey(KeyCode.Space));
        ScanAndMoveRandomly.CheckForInput(Input.GetKey(KeyCode.R));
        Move.CheckForInput(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftShift));

        // Arrows / WASD
        Up.CheckForInput(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W));
        Down.CheckForInput(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S));
        Left.CheckForInput(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A));
        Right.CheckForInput(Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D));

        Horizontal.CheckForInput(Right, Left);
        Vertical.CheckForInput(Up, Down);

        var inputVector = new Vector2(Horizontal.position, Vertical.position);
        var inputDistance = new Vector2(Horizontal.position, Vertical.position).normalized.magnitude;

        // Find mouse cursor
        var mouseCursorEntity = Object.FindObjectOfType<MouseCursor>();
        if (!mouseCursorEntity) return;

        var goalPosition = mouseCursorEntity.transform.position;

        if (ScanAndMoveRandomly.IsDown() || inputDistance > 0)
        {
            // make mouse line invisible
            Object.FindObjectOfType<RenderLineFromOneEntityToAnotherBehaviour>().GetComponent<LineRenderer>().enabled = false;
        }
        else
        {
            // make mouse line visible
            Object.FindObjectOfType<RenderLineFromOneEntityToAnotherBehaviour>().GetComponent<LineRenderer>().enabled = true;
        }

        // Check all entities with a StepMotion Component Attatched
        foreach (IsPlayer player in Object.FindObjectsOfType<IsPlayer>())
        {
            var entity = player.gameObject;
            var collide = entity.GetComponent<Collide>();

            if (!collide)
            {
                Debug.LogError(entity.ToString() + " needs a Collide component to know what layer we're colliding against");
                continue;
            }

            if (Move.IsDown() || ScanThenMove.IsDown() || ScanThenMove.IsReleased() || ScanAndMoveRandomly.IsDown() || inputDistance > 0)
            {
                Vector3 entityPosition = player.gameObject.transform.position;

                // if 'R' held, jump around randomly instead of moving toward cursor
                if (ScanAndMoveRandomly.IsDown())
                {
                    goalPosition = entityPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * 4;
                }

                Vector3 motion = goalPosition - entityPosition;

                // if arrows or wasd pressed, move with them instead
                if (inputDistance > 0)
                {
                    motion = new Vector3(inputVector.x, inputVector.y, 0) * arrowKeyMovementSpeed * dt;
                }

                // Project motion
                KissCollision.MotionPath motionPath = KissCollision.ProjectCapsule(player.GetComponent<CapsuleCollider>(), motion, collide.layerMask);
                motionPath.Draw();

                // Move to projected motion
                if (Move.IsDown() || ScanThenMove.IsReleased() || ScanAndMoveRandomly.IsDown() || inputDistance > 0)
                {
                    player.gameObject.transform.position = motionPath.EndPosition;
                }
            }
        }
    }
}

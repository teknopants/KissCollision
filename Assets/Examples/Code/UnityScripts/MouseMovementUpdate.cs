using UnityEngine;
using QuickInput;

public class MouseMovementUpdate: MonoBehaviour
{
    public Button ScanThenMove = new();
    public Button Move = new();
    public Button ScanAndMoveRandomly = new();

    public void Update()
    {
        // Input for StepMotion Engine Example
        ScanThenMove.CheckForInput(Input.GetKey(KeyCode.Space));
        ScanAndMoveRandomly.CheckForInput(Input.GetKey(KeyCode.R));
        Move.CheckForInput(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftShift));

        // Find mouse cursor
        var mouseCursorEntity = Object.FindObjectOfType<MouseCursor>();
        if (!mouseCursorEntity) return;

        var goalPosition = mouseCursorEntity.transform.position;

        if (ScanAndMoveRandomly.IsDown())
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

            if (Move.IsDown() || ScanThenMove.IsDown() || ScanThenMove.IsReleased() || ScanAndMoveRandomly.IsDown())
            {
                Vector3 entityPosition = player.gameObject.transform.position;

                // if 'R' held, jump around randomly instead of moving toward cursor
                if (ScanAndMoveRandomly.IsDown())
                {
                    goalPosition = entityPosition + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * 4;
                }

                // Project motion
                KissCollision.MotionPath motionPath = KissCollision.MoveSphere(player.GetComponent<SphereCollider>(), goalPosition - entityPosition, collide.layerMask);
                motionPath.Draw();

                // Move to projected motion
                if (Move.IsDown() || ScanThenMove.IsReleased() || ScanAndMoveRandomly.IsDown())
                {
                    player.gameObject.transform.position = motionPath.EndPosition;
                }
            }
        }
    }
}

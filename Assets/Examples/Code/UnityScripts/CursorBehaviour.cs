using UnityEngine;

public class CursorBehaviour : MonoBehaviour
{
    public float motionFactor = 1.0f;

    public void Update()
    {
        transform.position += new Vector3(Input.GetAxis("Mouse X") * motionFactor, Input.GetAxis("Mouse Y") * motionFactor, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}

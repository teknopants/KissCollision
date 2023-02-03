using UnityEngine;

// This script keeps the entity it's attatched to within the bounding box of a referenced collider component
// keep in mind Bounds isn't the shape of the collider but an invisible cube that contains it
public class ConstrainToBoundingBoxBehaviour : MonoBehaviour
{
    public Collider constraint;

    void Update()
    {
        transform.position = constraint.ClosestPointOnBounds(transform.position);
    }
}
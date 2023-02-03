using UnityEngine;

public class RenderLineFromOneEntityToAnotherBehaviour : MonoBehaviour
{
    public GameObject Entity1;
    public GameObject Entity2;

    void Update()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        if (Entity1)
        {
            lineRenderer.SetPosition(0, Entity1.transform.position);
        }

        if (Entity2)
        {
            lineRenderer.SetPosition(1, Entity2.transform.position);
        }
    }
}

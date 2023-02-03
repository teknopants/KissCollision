using UnityEngine;

public class TraceLineEngine : Engine
{
    public override void  Run(float dt)
    {
        foreach (PreviousPositions previousPositions in Object.FindObjectsOfType<PreviousPositions>())
        {
            var entity = previousPositions.gameObject;

            for (var i = previousPositions.positions.Length - 1; i > 0; i--)
            {
                previousPositions.positions[i] = previousPositions.positions[i - 1];
            }
            previousPositions.positions[0] = previousPositions.gameObject.transform.position;

            var lineRenderer = entity.GetComponent<LineRenderer>();
            if (lineRenderer)
            {
                lineRenderer.positionCount = previousPositions.positions.Length;
                lineRenderer.SetPositions(previousPositions.positions);
                lineRenderer.startColor = Color.green;
                lineRenderer.endColor = Color.red;
                lineRenderer.startWidth = .1f;
                lineRenderer.endWidth = .1f;
            }
            else
            {
                entity.AddComponent<LineRenderer>();
            }
        }
    }
}

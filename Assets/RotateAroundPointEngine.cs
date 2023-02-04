using UnityEngine;

public class RotateAroundPointEngine : MonoBehaviour
{
    void Run(float dt)
    {
        foreach(RotateAroundObject rotateAround in Object.FindObjectsOfType<RotateAroundObject>())
        {
            var entity = rotateAround.gameObject;
            var difference = rotateAround.toRotateAround.transform.position - entity.transform.position;

            difference = Quaternion.Euler(rotateAround.rotationPerSecond * dt) * difference;

            entity.transform.forward = Quaternion.Euler(rotateAround.rotationPerSecond * dt) * entity.transform.forward;

            entity.transform.position = rotateAround.toRotateAround.transform.position - difference;
        }
    }
}

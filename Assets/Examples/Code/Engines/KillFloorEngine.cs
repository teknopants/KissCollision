using UnityEngine;

public class KillFloorEngine : MonoBehaviour
{
    GameObject killFloorObj;

    private void Start()
    {
        killFloorObj = Object.FindObjectOfType<KillFloor>().gameObject;
    }

    void Run(float dt)
    {
        if (!killFloorObj)
        {
            Debug.LogError("KillFloorEngine can't find any entity with a killFloor component on it");
            return;
        }
        
        foreach (DestroyWhenBelowKillFloor destroyWhenBelowKillFloor in FindObjectsOfType<DestroyWhenBelowKillFloor>())
        {
            var entity = destroyWhenBelowKillFloor.gameObject;
            if (entity.transform.position.y < killFloorObj.transform.position.y)
            {
                Destroy(entity);
            }
        }
    }
}

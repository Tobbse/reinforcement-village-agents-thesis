using UnityEngine;

public class BuildingDistanceNavPoint : MonoBehaviour
{
    private void Start()
    {
        enabled = false;
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }
}

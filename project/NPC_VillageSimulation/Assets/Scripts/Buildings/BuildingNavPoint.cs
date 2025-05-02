using UnityEngine;

public class BuildingNavPoint : MonoBehaviour
{
    private bool _isOccupied;
    private int _agentId;

    public bool IsOccupied { get => _isOccupied; }
    public int AgentId { get => _agentId; }

    private void Start()
    {
        enabled = false;
    }

    public Transform getPosition()
    {
        return transform;
    }

    public void occupy(int agentId)
    {
        _agentId = agentId;
        _isOccupied = true;
    }

    public void vacate()
    {
        _agentId = -1;
        _isOccupied = false;
    }
}

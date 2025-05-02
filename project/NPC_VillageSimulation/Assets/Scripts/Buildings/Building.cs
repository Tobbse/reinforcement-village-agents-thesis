using UnityEngine;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    private BuildingNavPoint[] _buildingNavPoints;
    private Vector3 _distanceNavPointPosition;
    private int _numSlots;
    private Dictionary<int, BuildingNavPoint> _occupiedSlots;

    public Vector3 DistanceNavPointPosition { get => _distanceNavPointPosition; }
    public bool IsBuildingOccupied { get => _occupiedSlots.Count >= _numSlots; }
    public int NumSlots { get => _numSlots; }
    public int NumOccupiedSlots { get => _occupiedSlots.Count; }

    protected virtual void Awake()
    {
        BuildingDistanceNavPoint distancePoint = gameObject.GetComponentInChildren<BuildingDistanceNavPoint>();
        if (distancePoint == null) Debug.LogError("Did not find any distance nav point in building with name " + name);
        else _distanceNavPointPosition = distancePoint.transform.position;

        _buildingNavPoints = gameObject.GetComponentsInChildren<BuildingNavPoint>();
        if (_buildingNavPoints == null || _buildingNavPoints.Length == 0) Debug.LogError("Did not find any NavPoints in building with name " + name);

        _numSlots = _buildingNavPoints.Length;
        _occupiedSlots = new Dictionary<int, BuildingNavPoint>();

        enabled = false;
    }

    public bool isAgentOccupying(int agentId)
    {
        return _occupiedSlots.ContainsKey(agentId);
    }

    public Transform occupySlot(int agentId)
    {
        if (_occupiedSlots.ContainsKey(agentId))
        {
            Debug.LogWarning("Agent is already occupying a slot in building " + name + "!");
            return null;
        }
        foreach (BuildingNavPoint slot in _buildingNavPoints)
        {
            if (!slot.IsOccupied)
            {
                slot.occupy(agentId);
                _occupiedSlots[agentId] = slot;
                return slot.getPosition();
            }
        }
        // This can happen due to some race conditions when multiple agents occupy slots.
        Debug.Log("Attempted to occupy a free slot for " + agentId.ToString() + " in building " + name + " but there is none.");
        return null;
    }

    public Transform getRandomSlotPosition()
    {
        List<int> freeSpotIndices = new List<int>();
        for (int i = 0; i < _buildingNavPoints.Length; i++)
        {
            if (!_buildingNavPoints[i].IsOccupied) freeSpotIndices.Add(i);
        }
        if (freeSpotIndices.Count != 0)
        {
            int randomIndex = freeSpotIndices[Mathf.RoundToInt(Random.value * (freeSpotIndices.Count - 1))];
            return _buildingNavPoints[randomIndex].transform;
        }
        return null;
    }

    public Transform occupyRandomSlot(int agentId)
    {
        if (_occupiedSlots.ContainsKey(agentId))
        {
            Debug.LogWarning("Agent is already occupying a slot in building " + name + "!");
            return null;
        }
        List<int> freeSpotIndices = new List<int>();
        for (int i = 0; i < _buildingNavPoints.Length; i++)
        {
            BuildingNavPoint slot = _buildingNavPoints[i];
            if (!slot.IsOccupied) freeSpotIndices.Add(i);
        }
        if (freeSpotIndices.Count != 0)
        {
            int randomIndex = freeSpotIndices[Mathf.RoundToInt(UnityEngine.Random.value * (freeSpotIndices.Count - 1))];
            BuildingNavPoint pickedSlot = _occupiedSlots[randomIndex];
            pickedSlot.occupy(agentId);
            _occupiedSlots[agentId] = pickedSlot;
            return pickedSlot.getPosition();
        }
        // This can happen due to some race conditions when multiple agents occupy slots.
        Debug.Log("Attempted to occupy a free slot for " + agentId.ToString() + " in building " + name + " but there is none.");
        return null;
    }

    public Transform getAgentSlotPosition(int agentId)
    {
        if (!isAgentOccupying(agentId)) {
            Debug.Log("Trying to get the slot position for agent " + agentId.ToString() + " but no such slot was found in building " + name + ".");
            return null;
        } else
        {
            return _occupiedSlots[agentId].getPosition();
        }
    }

    public void vacateSlot(int agentId)
    {
        if (!isAgentOccupying(agentId))
        {
            Debug.LogWarning("Trying to vacate a slot for agent " + agentId.ToString() + " but no such slot was not found in building " + name + ".");
        } else
        {
            BuildingNavPoint slot = _occupiedSlots[agentId];
            slot.vacate();
            _occupiedSlots.Remove(agentId);
        }
    }

    public void vacateAllSlots()
    {
        foreach (BuildingNavPoint slot in _buildingNavPoints) slot.vacate();
        _occupiedSlots = new Dictionary<int, BuildingNavPoint>();
    }
}

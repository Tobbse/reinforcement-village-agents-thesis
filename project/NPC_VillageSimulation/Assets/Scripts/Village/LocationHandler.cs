using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using CielaSpike;
using System.Collections;
using System.Collections.Concurrent;

public class LocationHandler : MonoBehaviour
{
    public Village npcSpawner;

    private Dictionary<string, List<Building>> _locations;
    private Dictionary<string, int> _availableLocationTypeSlots;
    private Dictionary<int, Building> _agentsInBuildings;
    private Building[] _rawBuildings;
    private Dictionary<string, List<Building>> _buildingsByTags;
    private Dictionary<int, float> _calculatedPathLengths;
    private Dictionary<int, Transform> _calculatedPositions;
    private Dictionary<string, Transform> _goapPositions;
    private ConcurrentBag<BaseNpcGoapAction> _pendingTargetGoapActions; // A concurrent collection is needed to assure Thread safety.

    private void Awake()
    {
        _locations = new Dictionary<string, List<Building>>();
        _calculatedPositions = new Dictionary<int, Transform>();
        _calculatedPathLengths = new Dictionary<int, float>();
        _goapPositions = new Dictionary<string, Transform>();
        _availableLocationTypeSlots = new Dictionary<string, int>();
        _agentsInBuildings = new Dictionary<int, Building>();
        _pendingTargetGoapActions = new ConcurrentBag<BaseNpcGoapAction>();
    }

    // Checks for goap actions that are pending and have no target yet.
    /*void Update()
    {
        if (_pendingTargetGoapActions.Count == 0) return;

        foreach (BaseNpcGoapAction pendingAction in _pendingTargetGoapActions)
        {
            if (GameAcademy.VERBOSE) Debug.Log("Starting Pending Target Coroutine for GOAP action: "
                + pendingAction.GetType().ToString() + ".");
            this.StartCoroutineAsync(pendingAction.addTarget());
        }
        _pendingTargetGoapActions = new ConcurrentBag<BaseNpcGoapAction>();
    }*/

    /*public void addPendingTargetGoapAction(BaseNpcGoapAction action)
    {
        if (GameAcademy.VERBOSE) Debug.Log("Added pending target action.");
        _pendingTargetGoapActions.Add(action);
    }*/

    public void init(Building[] rawBuildings, BuildingHome[] rawHomes, List<NpcAgent> npcs)
    {
        _rawBuildings = rawBuildings;
        _setBuildingsByTags();
        _setupLocations();
        _setupAgentHomeLocations(rawHomes, npcs);
    }

    public static bool isValidLocation(Vector3 location)
    {
        return location != Vector3.negativeInfinity && !float.IsNegativeInfinity(location.x) &&
            !float.IsNegativeInfinity(location.y) && !float.IsNegativeInfinity(location.z);
    }

    public int getTotalNumSlotsForLocations(string locationType)
    {
        return _availableLocationTypeSlots[locationType];
    }

    public void vacateSlot(string locationType, int agentId)
    {
        if (_agentsInBuildings.ContainsKey(agentId))
        {
            _agentsInBuildings[agentId].vacateSlot(agentId);
            _agentsInBuildings.Remove(agentId);
            _availableLocationTypeSlots[locationType]++;
        } else
        {
            Debug.LogWarning("Trying to vacate building for locationtype " + locationType + " and agent " + agentId.ToString() + ", but no occupied slot was found.");
        }
    }

    public Transform getCalculatedPosition(int agentId)
    {
        return _calculatedPositions[agentId];
    }

    public Transform getcalculatedGoapPosition(string guid)
    {
        Transform res = _goapPositions[guid];
        _goapPositions.Remove(guid);
        return res;
    }

    public IEnumerator occupyFreeSlotInFirstFoundBuilding(string locationType, int agentId)
    {
        _calculatedPositions[agentId] = null;
        List<Building> locationTypeBuildings = _locations[locationType];
        int len = locationTypeBuildings.Count;
        for (int i = 0; i < len; i++)
        {
            Building building = locationTypeBuildings[i];
            if (!building.IsBuildingOccupied)
            {
                Transform slotLocation = building.occupySlot(agentId);
                if (slotLocation == null) continue;
                _agentsInBuildings[agentId] = building;
                _calculatedPositions[agentId] = slotLocation;
                _availableLocationTypeSlots[locationType]--;
                yield break;
            }
        }
        Debug.LogWarning("Attempted to occupy the first slot for locationtype " + locationType + ", but there is none.");
        yield break;
    }

    public IEnumerator getRandomGoapPosition(string locationType, int agentId, string guid)
    {
        yield return Ninja.JumpToUnity;
        List<Building> locationTypeBuildings = _locations[locationType];
        List<int> freeBuildingIndices = new List<int>();
        System.Random rand = new System.Random();
        for (int i = 0; i < locationTypeBuildings.Count; i++)
        {
            if (!locationTypeBuildings[i].IsBuildingOccupied) freeBuildingIndices.Add(i);
        }
        if (freeBuildingIndices.Count != 0)
        {
            int randomIndex = freeBuildingIndices[Mathf.RoundToInt(rand.Next(freeBuildingIndices.Count - 1))];
            _goapPositions[guid] = locationTypeBuildings[randomIndex].getRandomSlotPosition();
        }
        yield return null;
    }

    public IEnumerator occupyFreeSlotInRandomBuilding(string locationType, int agentId, bool forceRandomSlot = false, bool getPositionOnly = false)
    {
        _calculatedPositions[agentId] = null;
        List<Building> locationTypeBuildings = _locations[locationType];
        List<int> freeBuildingIndices = new List<int>();
        int len = locationTypeBuildings.Count;
        for (int i = 0; i < len; i++)
        {
            if (!locationTypeBuildings[i].IsBuildingOccupied) freeBuildingIndices.Add(i);
        }
        if (freeBuildingIndices.Count == 0)
        {
            Debug.LogWarning("Attempted to occupy a free building for locationtype " + locationType + ", but there is none.");
            yield break;
        }
        int randomIndex = freeBuildingIndices[Mathf.RoundToInt(UnityEngine.Random.value * (freeBuildingIndices.Count - 1))];
        Building chosenBuilding = locationTypeBuildings[randomIndex];
        _agentsInBuildings[agentId] = chosenBuilding;

        Transform slotLocation = forceRandomSlot ? chosenBuilding.occupyRandomSlot(agentId) : chosenBuilding.occupySlot(agentId);
        if (slotLocation == null)
        {
            Debug.LogWarning("Failed to occupy free slot in random building for type " + locationType + ".");
            yield break;
        }
        _availableLocationTypeSlots[locationType]--;
        _calculatedPositions[agentId] = slotLocation;
        yield break;
    }

    public IEnumerator occupyFreeSlotInClosestBuilding(string locationType, int agentId, Vector3 agentPosition, NavMeshAgent navMeshAgent)
    {
        _calculatedPositions[agentId] = null;
        List<Building> locationTypeBuildings = _locations[locationType];
        float shortestPathLength = Mathf.Infinity;
        Building closestBuilding = null;
        foreach (Building building in locationTypeBuildings)
        {
            if (building.IsBuildingOccupied) continue;
            yield return StartCoroutine(calculatePathLength(agentPosition, building.DistanceNavPointPosition, navMeshAgent, agentId));
            float buildingPathLength = _calculatedPathLengths[agentId];
            if (buildingPathLength < shortestPathLength)
            {
                shortestPathLength = buildingPathLength;
                closestBuilding = building;
            }
        }
        if (closestBuilding == null)
        {
            Debug.LogWarning("Attempted to occupy a slot for locationtype " + locationType + ", but there is none.");
            yield break;
        }
        _availableLocationTypeSlots[locationType]--;
        _agentsInBuildings[agentId] = closestBuilding;
        _calculatedPositions[agentId] = closestBuilding.occupySlot(agentId);
        yield break;
    }

    public IEnumerator occupyFreeSlotInPartiallyOccupiedBuilding(string locationType, int agentId, Building agentHomeBuilding)
    {
        _calculatedPositions[agentId] = null;
        List<Building> locationTypeBuildings = _locations[locationType];
        List<int> partiallyOccupiedIndices = new List<int>();
        for (int i = 0; i < locationTypeBuildings.Count; i++)
        {
            Building building = locationTypeBuildings[i];
            if (!building.IsBuildingOccupied && building.NumOccupiedSlots > 0)
            {
                partiallyOccupiedIndices.Add(i);
            }
        }
        if (partiallyOccupiedIndices.Count == 0)
        {
            if (agentHomeBuilding != null && !agentHomeBuilding.IsBuildingOccupied)
            {
                if (GameAcademy.VERBOSE) Debug.Log("Attempted to find any partially occupied home for " + locationType + ", but there is none. Choosing the agents home instead.");
                _agentsInBuildings[agentId] = agentHomeBuilding;
                _calculatedPositions[agentId] = agentHomeBuilding.occupySlot(agentId);
                yield break;
            } else
            {
                if (GameAcademy.VERBOSE) Debug.Log("Attempted to find partially occupied building for " + locationType + ", but there is none. Choosing a random spot instead.");
                int attempts = 1;
                while (attempts < 5)
                {
                    yield return occupyFreeSlotInRandomBuilding(locationType, agentId);
                    if (_calculatedPositions[agentId] == null)
                    {
                        if (GameAcademy.VERBOSE) Debug.Log("Attempted to occupy random slot failed for locationType " + locationType + "." +
                        " This was attempt No. " + attempts.ToString() + ".");
                    } else
                    {
                        _calculatedPositions[agentId] = _calculatedPositions[agentId];
                        yield break;
                    }
                    attempts++;
                }
                Debug.LogWarning("Multiple attempts to occupy random slot for type " + locationType + " have failed.");
                yield break;
            }
        }
        int randomIndex = partiallyOccupiedIndices[Mathf.RoundToInt(UnityEngine.Random.value * (partiallyOccupiedIndices.Count - 1))];
        Building chosenBuilding = locationTypeBuildings[randomIndex];
        _agentsInBuildings[agentId] = chosenBuilding;
        _calculatedPositions[agentId] = chosenBuilding.occupySlot(agentId);
        _availableLocationTypeSlots[locationType]--;
        yield break;
    }

    public bool hasFreeSpots(string locationType)
    {
        return _availableLocationTypeSlots[locationType] > 0;
    }

    public IEnumerator calculatePathLength(Vector3 origin, Vector3 destination, NavMeshAgent navMeshAgent, int agentId)
    {
        yield return Ninja.JumpToUnity;
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(destination, path);
        yield return Ninja.JumpBack;

        float pathLength = 0f;
        Vector3[] wayPoints = new Vector3[path.corners.Length + 2];
        wayPoints[0] = origin;
        wayPoints[wayPoints.Length - 1] = destination;

        for (int i = 0; i < path.corners.Length; i++)
        {
            wayPoints[i + 1] = path.corners[i];
        }
        for (int i = 0; i < wayPoints.Length - 1; i++)
        {
            pathLength += Vector3.Distance(wayPoints[i], wayPoints[i + 1]);
        }
        _calculatedPathLengths[agentId] = pathLength;
        yield return null;
    }

    public float getCalculatedPathLength(int agentId)
    {
        return _calculatedPathLengths[agentId];
    }

    private void _setupAgentHomeLocations(BuildingHome[] rawHomes, List<NpcAgent> npcs)
    {
        int homeId = 1;
        int numHomes = rawHomes.Length;
        int numBeds = 0;
        BuildingHome[] sortedHomes = new BuildingHome[numHomes];
        foreach (BuildingHome building in rawHomes)
        {
            if (building == null)
            {
                Debug.LogError("Did not find VillageBuildingHome for home " + building.name + ".");
                continue;
            }
            numBeds += building.NumSlots;
            sortedHomes[building.HomeId - 1] = building;
        }
        Debug.Log("There are " + numHomes + " homes, " + numBeds + " beds and " + npcs.Count + " agents.");
        if (numBeds < npcs.Count) Debug.LogWarning("There are more agents then beds! Multiple agents will be assigned to the same bed.");

        foreach (NpcAgent npc in npcs)
        {
            int triedBuildings = 0;
            while (true)
            {
                if (homeId > numHomes) homeId = 1; // Too many npcs, start over from the beginning.

                BuildingHome potentialHome = sortedHomes[homeId - 1];
                if (!potentialHome.IsBuildingOccupied)
                {
                    Transform bedPos = potentialHome.occupySlot(npc.AgentId);
                    if (bedPos != null)
                    {
                        npc.HomePosition = bedPos;
                        npc.HomeBuilding = potentialHome;
                        break;
                    }
                }
                if (triedBuildings >= numHomes)
                {
                    Debug.LogWarning("Could not find a free home spot for agent " + npc.AgentId + "! Resetting homes now. Multi agent-bed assigning will occur.");
                    foreach (BuildingHome occupiedHome in sortedHomes) occupiedHome.vacateAllSlots();
                    homeId = 1;
                    triedBuildings = 0;
                }
                triedBuildings++;
                homeId++;
            }
        }
        foreach (BuildingHome occupiedHome in sortedHomes)
        {
            occupiedHome.vacateAllSlots(); // Removing npcs from slots agents. Their home positions were saved in each Npc.
        }
    }

    private void _setupLocations()
    {
        _locations = new Dictionary<string, List<Building>>();

        _addLocationType(LocationTypes.HOME, GameTags.TAG_HOME);

        _addLocationType(LocationTypes.MARKET, GameTags.TAG_HUNGER);
        _addLocationType(LocationTypes.WELL, GameTags.TAG_THIRST);

        _addLocationType(LocationTypes.LIBRARY, GameTags.TAG_EDUCATION);
        _addLocationType(LocationTypes.SCHOOL, GameTags.TAG_EDUCATION);
        _addLocationType(LocationTypes.COLLEGE, GameTags.TAG_EDUCATION);

        _addLocationType(LocationTypes.MEETING_POINT, GameTags.TAG_COMMUNICATION);

        _addLocationType(LocationTypes.CHURCH, GameTags.TAG_RELIGION);

        _addLocationType(LocationTypes.FIELD_HOPS, GameTags.TAG_WORK);
        _addLocationType(LocationTypes.BREWERY, GameTags.TAG_WORK);
        _addLocationType(LocationTypes.FIELD_GRAIN, GameTags.TAG_WORK);
        _addLocationType(LocationTypes.WINDMILL, GameTags.TAG_WORK);
        _addLocationType(LocationTypes.BAKERY, GameTags.TAG_WORK);
        _addLocationType(LocationTypes.HUNTING_SPOT, GameTags.TAG_WORK);
        _addLocationType(LocationTypes.BUTCHER, GameTags.TAG_WORK);
        _addLocationType(LocationTypes.WOOD_CUTTING_SPOT, GameTags.TAG_WORK);
        _addLocationType(LocationTypes.CARPENTER, GameTags.TAG_WORK);
        _addLocationType(LocationTypes.MINE, GameTags.TAG_WORK);
        _addLocationType(LocationTypes.BLACKSMITH, GameTags.TAG_WORK);

        _addLocationType(LocationTypes.STORAGE_GRAIN, GameTags.TAG_STORAGE);
        _addLocationType(LocationTypes.STORAGE_HOPS, GameTags.TAG_STORAGE);
        _addLocationType(LocationTypes.STORAGE_MAIN, GameTags.TAG_STORAGE);
        _addLocationType(LocationTypes.STORAGE_FOREST, GameTags.TAG_STORAGE);
        _addLocationType(LocationTypes.STORAGE_MINE, GameTags.TAG_STORAGE);

        _addLocationType(LocationTypes.TAVERN, GameTags.TAG_RECREATION);
        _addLocationType(LocationTypes.SOCCER_FIELD, GameTags.TAG_RECREATION);
        _addLocationType(LocationTypes.CAFE, GameTags.TAG_RECREATION);
        _addLocationType(LocationTypes.THEATER, GameTags.TAG_RECREATION);
        _addLocationType(LocationTypes.SHOP, GameTags.TAG_RECREATION);
        _addLocationType(LocationTypes.SHOOTING_RANGE, GameTags.TAG_RECREATION);
        _addLocationType(LocationTypes.BARBECUE, GameTags.TAG_RECREATION);
    }

    private void _addLocationType(string locationType, string tag)
    {
        List<Building> typeBuildings = _getBuildingsForType(locationType, tag);
        if (typeBuildings.Count == 0)
        {
            Debug.LogError("Did not find any building containing " + locationType + " with tag " + tag + ".");
            return;
        }

        int numSlots = 0;
        foreach (Building building in typeBuildings) numSlots += building.NumSlots;

        _locations[locationType] = typeBuildings;
        _availableLocationTypeSlots[locationType] = numSlots;
    }

    private List<Building> _getBuildingsForType(string partOfBuildingName, string tag)
    {
        List<Building> buildings = new List<Building>();
        foreach (Building building in _buildingsByTags[tag])
        {
            if (building.name.Contains(partOfBuildingName)) buildings.Add(building);
        }
        return buildings;
    }

    private void _setBuildingsByTags()
    {
        _buildingsByTags = new Dictionary<string, List<Building>>();
        foreach (Building building in _rawBuildings)
        {
            string tag = building.tag;
            if (!_buildingsByTags.ContainsKey(tag))
            {
                _buildingsByTags[tag] = new List<Building>();
            }
            _buildingsByTags[tag].Add(building);
        }
    }
}

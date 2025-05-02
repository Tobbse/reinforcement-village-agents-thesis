using UnityEngine;
using System;
using CielaSpike;
using System.Collections;

public class MoveAction : BaseAction
{
    public const int PICK_BUILDING_STRATEGY_RANDOM = 0;
    public const int PICK_BUILDING_STRATEGY_CLOSEST = 1;
    public const int PICK_BUILDING_STRATEGY_HOME = 2;
    public const int PICK_BUILDING_STRATEGY_NEXT_OCCUPIED = 3;

    public Transform destinationResult;

    private const float PATH_LENGTH_DURATION_MULTIPLIER = 1.1f;

    private MoveHandler _moveHandler;
    private LocationHandler _locationHandler;
    private Action _callback;
    private Action _cancelCallback;
    private float _startMovingTime;
    private float _duration;
    private int _pickBuildingStrategy;
    private Building _agentHomeBuilding;
    private string _locationType;
    private bool _force;
    private Vector3 _npcPosition;

    public MoveAction(string actionType, NpcAgent npc, string locationType,
        bool force = false, int pickBuildingStrategy = PICK_BUILDING_STRATEGY_CLOSEST,
        Building agentHomeBuilding = null)
        : base(actionType, npc)
    {
        _locationType = locationType;
        _pickBuildingStrategy = pickBuildingStrategy;
        _agentHomeBuilding = agentHomeBuilding;
        _moveHandler = npc.MoveHandler;
        _locationHandler = _npc.LocationHandler;
        _force = force;
    }

    public override IEnumerator execute(Action callback, Action cancelCallback)
    {
        _callback = callback;
        _cancelCallback = cancelCallback;
        _npcPosition = _npc.transform.position;

        if (!_locationHandler.hasFreeSpots(_locationType) && _force == false)
        {
            Debug.Log("No free spot available for locationType " + _locationType + " in action " + _actionType + " for agent " + _npc.AgentId + ".");
            cancelCallback(); // Will request new action to execute.
            yield return null;
        }
        Task task;
        _npc.StartCoroutineAsync(_destinationTask(), out task);        yield return _npc.StartCoroutine(task.Wait());
        Transform destination = destinationResult;

        if (destination == null || !LocationHandler.isValidLocation(destination.position))
        {
            if (_force)
            {
                destination = _npc.HomePosition;
            }
            else
            {
                Debug.Log("Failed to get destination for moving action in " + _actionType);
                _duration = -1;
                _cancelCallback();
                yield break;
            }
        }
        _npc.VillageInfo.changeAmountMoving(true);

        switch (GameAcademy.GAME_MODE)
        {
            case GameAcademy.GAME_MODE_REGULAR_INFERENCE:
                _handleInference(destination.position);
                break;

            case GameAcademy.GAME_MODE_SINGLE_AGENT_TRAINING:
                yield return _handleSingleAgentTraining(destination.position);
                break;

            case GameAcademy.GAME_MODE_FAKE_INFERENCE:
            case GameAcademy.GAME_MODE_MULTI_AGENT_TRAINING:
                yield return _handleFakeInference(destination.position);
                break;
            default:
                Debug.LogError("Unknown Game Mode " + GameAcademy.GAME_MODE.ToString() + ".");
                cancelCallback();
                break;
        }
        yield return null;
    }

    private IEnumerator _destinationTask()
    {
        yield return Ninja.JumpBack;
        bool home = _pickBuildingStrategy == PICK_BUILDING_STRATEGY_HOME;
        destinationResult = null;
        if (GameAcademy.IS_TRAINING && _pickBuildingStrategy != PICK_BUILDING_STRATEGY_HOME)
        {
            yield return _locationHandler.occupyFreeSlotInFirstFoundBuilding(_locationType, _npc.AgentId);
        }
        else
        {
            switch (_pickBuildingStrategy)
            {
                case PICK_BUILDING_STRATEGY_RANDOM:
                    yield return _locationHandler.occupyFreeSlotInRandomBuilding(_locationType, _npc.AgentId);
                    break;

                case PICK_BUILDING_STRATEGY_CLOSEST:
                    yield return _locationHandler.occupyFreeSlotInClosestBuilding(_locationType, _npc.AgentId, _npcPosition, _moveHandler.NavAgent);
                    break;

                case PICK_BUILDING_STRATEGY_HOME:
                    destinationResult = _npc.HomePosition;
                    break;

                case PICK_BUILDING_STRATEGY_NEXT_OCCUPIED:
                    yield return _locationHandler.occupyFreeSlotInPartiallyOccupiedBuilding(_locationType, _npc.AgentId, _agentHomeBuilding);
                    break;
            }
        }
        if (!home) destinationResult = _locationHandler.getCalculatedPosition(_npc.AgentId);
        yield return destinationResult;
    }

    private void _finishedMoving()
    {
        if (_duration == -1)
        {
            _duration = _startMovingTime - Time.realtimeSinceStartup; // Only necessary during inference.
        }

        float exhaustion = _npc.NpcState.Exhaustion;
        // TODO check values for exhaustion.
        _npc.NpcState.updateExhaustion(-0.004f * _duration);
        _duration = -1;
        _npc.VillageInfo.changeAmountMoving(false);
        _callback();
    }

    // Move the agent to the destination using the NavMeshAgent.
    private void _handleInference(Vector3 destination)
    {
        _startMovingTime = Time.realtimeSinceStartup;
        _moveHandler.moveAgent(destination, _actionType, _finishedMoving);
    }

    // Calculates the duration of the path that the agent would take during inference.
    // Uses that to update the needs accordingly and teleports the agent to the destination directly.
    private IEnumerator _handleSingleAgentTraining(Vector3 destination)
    {
        yield return _moveHandler.StartCoroutine(_locationHandler.calculatePathLength(
            _npc.transform.position, destination, _moveHandler.NavAgent, _npc.AgentId));
        float pathLength = _locationHandler.getCalculatedPathLength(_npc.AgentId);
        float speed = _moveHandler.NavAgent.speed;
        if (pathLength < 1f) _duration = 1f;
        else _duration = (pathLength / speed) * PATH_LENGTH_DURATION_MULTIPLIER; // Increasing duration a bit as the agent will take a little time to accelerate, turn etc.

        _timeController.advanceTime(_duration);
        _npc.transform.position = destination;
        _finishedMoving();
    }

    private IEnumerator _handleFakeInference(Vector3 destination)
    {
        yield return _moveHandler.StartCoroutine(_locationHandler.calculatePathLength(
            _npc.transform.position, destination, _moveHandler.NavAgent, _npc.AgentId));
        float pathLength = _locationHandler.getCalculatedPathLength(_npc.AgentId);
        float speed = _moveHandler.NavAgent.speed;
        if (pathLength < 1f) _duration = 1f;
        else _duration = (pathLength / speed) * PATH_LENGTH_DURATION_MULTIPLIER; // Increasing duration a bit as the agent will take a little time to accelerate, turn etc.

        _npc.transform.position = destination;

        WaitAction waitAction = new WaitAction(_actionType, _npc, _duration);
        _npc.StartCoroutine(waitAction.execute(_finishedMoving, _cancelCallback));
    }
}

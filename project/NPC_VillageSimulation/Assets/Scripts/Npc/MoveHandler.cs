using UnityEngine;
using UnityEngine.AI;
using System;

public class MoveHandler : MonoBehaviour
{
    private const int MOVE_TYPE_PATH_PENDING = 0;
    private const int MOVE_TYPE_MOVING = 1;
    private const int MOVE_STATUS_STOPPED = 2;
    private const string ACTION_STOPPED = "";

    private NavMeshAgent _navAgent;
    private Action _moveCallback;
    private string _moveAction = ACTION_STOPPED;
    private float _startTime;

    public NavMeshAgent NavAgent { get => _navAgent; }
    public bool IsMoving { get => _moveAction != ACTION_STOPPED; }

    void Awake()
    {
        _navAgent = gameObject.GetComponent<NavMeshAgent>();
        _navAgent.stoppingDistance = GameAcademy.AGENT_STOPPING_DISTANCE;
        reset();
    }

    public void moveAgent(Vector3 destination, string type, Action moveCallback)
    {
        _startTime = Time.unscaledTime;
        if (_moveAction != ACTION_STOPPED)
        {
            Debug.Log("Moving agent while agent has not reached its destination yet for action: "
                + _moveAction + ". Overriding destination for new action " + _moveAction + ".");
        }
        _navAgent.isStopped = false;
        _navAgent.SetDestination(destination);
        _moveAction = type;
        _moveCallback = moveCallback;
        enabled = true;
    }

    public void reset()
    {
        _moveAction = ACTION_STOPPED;
        _navAgent.isStopped = true;
        enabled = false;
    }

    void Update()
    {
        if (_moveAction == ACTION_STOPPED) return; // No update necessary, the agent has no destination.
        if (_getMoveStatus() == MOVE_STATUS_STOPPED) _endMove();
        else if (Time.unscaledTime - _startTime > 100)
        {
            Debug.LogWarning(name + " took longer then 100 seconds to move. It is probably stuck.");
            _startTime = Time.unscaledTime;
        }
    }

    private int _getMoveStatus()
    {
        if (_navAgent.pathPending)
        {
            return MOVE_TYPE_PATH_PENDING;
        } else if (_navAgent.remainingDistance <= _navAgent.stoppingDistance &&
          (!_navAgent.hasPath || _navAgent.velocity.sqrMagnitude < 0.01f) &&
          _navAgent.remainingDistance != Mathf.Infinity)
        {
            return MOVE_STATUS_STOPPED;
        } else
        {
            return MOVE_TYPE_MOVING;
        }
    }

    private void _endMove()
    {
        reset();
        _moveCallback?.Invoke();
    }
}

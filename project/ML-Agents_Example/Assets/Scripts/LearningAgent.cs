using UnityEngine;
using MLAgents;

public class LearningAgent : Agent
{
    public float spawnAreaX;    // X width of spawning/positioning area.
    public float spawnAreaZ;    // Z width of spawning/positioning area.
    public Target target;       // The target for the ball to navigate to.     
    public Transform zero;      // Positioning offset (multiple levels are used).

    private const float SPEED_MULT = 5f;        // Arbitrary speed multiplier.
    private const float CLOSE_DISTANCE = 2f; // Arbitrary distance, depends on size.

    private GameAcademy _gameAcademy;   // Connection to Tensorflow backend.
    private Rigidbody _agentRigid;      // Handles agent physics.
    private float _lastDistance;        // Last distance between agent and target.
    private float _agentStartYPos;      // Initial agent Y-position needed for reset.
    private float _targetStartYPos;     // Initial target Y-position needed for reset.

    public override void InitializeAgent()
    {
        _agentRigid = GetComponent<Rigidbody>();
        _gameAcademy = FindObjectOfType<GameAcademy>();
        _agentStartYPos = transform.position.y;
        _targetStartYPos = target.transform.position.y;
    }

    public override void CollectObservations()
    {
        Vector3 adjustedAgentPosition = zero.position - transform.position;
        Vector3 adjustedTargetPosition = zero.position - target.transform.position;

        AddVectorObs(adjustedAgentPosition.x);
        AddVectorObs(adjustedAgentPosition.z);
        AddVectorObs(_isAgentFalling());

        AddVectorObs(_agentRigid.velocity.x);
        AddVectorObs(_agentRigid.velocity.z);

        AddVectorObs(adjustedTargetPosition.x);
        AddVectorObs(adjustedTargetPosition.z);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // This is necessary due to a ML-Agents bug.
        if (IsDone()) return;

        // The agent fell off the edge. Finish episode with a punishment.
        if (_isAgentFalling()) 
        {
            _finish(-1f);
            return;
        }

        // The agent has reached the target. Finish the episode with a reward.
        float currentDistance = Vector3.Distance(transform.position, target.transform.position);
        if (currentDistance < CLOSE_DISTANCE)
        {
            _finish(1f);
            return;
        }

        // Set the agents velocity to what the model thinks is a good value.
        float moveY = vectorAction[0] * SPEED_MULT;
        float moveX = vectorAction[1] * SPEED_MULT;
        _agentRigid.velocity = new Vector3(moveX, 0f, moveY);

        // In case the agent moved in the wrong direction, punish it.
        if (currentDistance > _lastDistance) AddReward(-0.1f); 
        _lastDistance = currentDistance;
    }

    // Will be called in the beginning and when an episode is completed.
    public override void AgentReset()
    {
        _agentRigid.velocity = _agentRigid.angularVelocity = Vector3.zero;
        if (_isAgentFalling()) _setRandomPosition(transform, _agentStartYPos);
        _setRandomPosition(target.transform, _targetStartYPos);
        _lastDistance = Vector3.Distance(transform.position, target.transform.position);
    }

    private void _finish(float reward)
    {
        AddReward(reward);
        if (_gameAcademy.verbose) Debug.Log("Reward: " + reward.ToString());
        Done();
    }

    // Sets random position to a transform until target and agent are far enough away from each other.
    private void _setRandomPosition(Transform pTransform, float y)
    {
        float distance = 0f;
        while (distance < CLOSE_DISTANCE * 1.5f)
        {
            float newTargetX = (Random.value * spawnAreaX) - (spawnAreaX / 2);
            float newTargetZ = (Random.value * spawnAreaZ) - (spawnAreaZ / 2);
            pTransform.position = new Vector3(zero.position.x + newTargetX, y, zero.position.z + newTargetZ);
            distance = Vector3.Distance(transform.position, target.transform.position);
        }
    }

    private bool _isAgentFalling()
    {
        // Sometimes the float comparison is weird, so 0.01f is added to prevent mistakes.
        return (transform.position.y + 0.01f) < _agentStartYPos;
    }
}

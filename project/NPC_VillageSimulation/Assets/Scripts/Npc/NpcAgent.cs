using UnityEngine;
using MLAgents;
using System.Collections.Generic;
using SwordGC.AI.Goap;

public class NpcAgent : Agent
{
    public bool shouldMonitorNpc;

    private Transform _homePosition;
    private Building _homeBuilding;
    private ActionHandler _actionHandler;
    private NpcState _npcState;
    private LocationHandler _locationHandler;
    private ActionMaskingHandler _actionMaskingHandler;
    private MoveHandler _moveHandler;
    private WaitHandler _waitHandler;
    private MonitorHandler _monitorHandler;
    private InteractionHandler _interactionHandler;
    private ManualDecisionMaker _manualDecisionMaker;
    private Dictionary<string, int> _decisionLog;
    private ResourceHandler _resourceHandler;
    private JobHandler _jobHandler;
    private NpcGoapAgent _npcGoapAgent;
    private NpcAgentGoapData _npcAgentGoapData;
    private VillageInfo _villageInfo;
    private GameAcademy _academy;
    private int _numActions;
    private string _action = "";
    private int _agentId;
    private bool _isFirstReset = true;
    private float _currentTime;
    private bool _shouldResetAgent;
    private int _maxActions;
    private bool _useManualDecisions;
    private MeshRenderer _meshRenderer;
    private bool _wasInitialized;

    public int AgentId { get => _agentId; set => _agentId = value; }
    public Transform HomePosition { get => _homePosition; set => _homePosition = value; }
    public Building HomeBuilding { get => _homeBuilding; set => _homeBuilding = value; }
    public WaitHandler WaitHandler { get => _waitHandler; }
    public MoveHandler MoveHandler { get => _moveHandler; }
    public InteractionHandler InteractionHandler { get => _interactionHandler; }
    public LocationHandler LocationHandler { get => _locationHandler; set => _locationHandler = value; }
    public NpcState NpcState { get => _npcState; }
    public ResourceHandler ResourceHandler { get => _resourceHandler; }
    public bool ShouldMonitorNpc { get => shouldMonitorNpc; set => shouldMonitorNpc = value; }
    public string Action { get => _action; set => _action = value; }
    public int NumActions { get => _numActions; }
    public VillageInfo VillageInfo { get => _villageInfo; }
    public JobHandler JobHandler { get => _jobHandler; }
    public ActionHandler ActionHandler { get => _actionHandler; }
    public ActionMaskingHandler ActionMaskingHandler { get => _actionMaskingHandler; }
    public NpcAgentGoapData NpcAgentGoapData { get => _npcAgentGoapData; }
    public NpcGoapAgent NpcGoapAgent { get => _npcGoapAgent; set => _npcGoapAgent = value; }

    private void Awake()
    {
        _decisionLog = new Dictionary<string, int>();
        _npcAgentGoapData = new NpcAgentGoapData();
    }

    public void init(AgentInfo agentInfo, ResourceHandler resourceHandler,
        VillageInfo villageInfo, JobHandler jobHandler, NpcGoapAgent npcGoapAgent)
    {
        _npcState = new NpcState(agentInfo, _npcGoapAgent);
        _resourceHandler = resourceHandler;
        _villageInfo = villageInfo;
        _jobHandler = jobHandler;
        _npcGoapAgent = npcGoapAgent;

        _useManualDecisions = GameAcademy.USE_MANUAL_DECISIONS;

        gameObject.name += "_ " + agentInfo.JobId + "_" + agentInfo.Name + "_" + agentInfo.Age.ToString();

        int gameMode = GameAcademy.GAME_MODE;
        _maxActions = GameAcademy.MAX_NPC_ACTIONS;
        _shouldResetAgent = gameMode != GameAcademy.GAME_MODE_REGULAR_INFERENCE && gameMode != GameAcademy.GAME_MODE_FAKE_INFERENCE;

        _meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
        _academy = FindObjectOfType<GameAcademy>();
        _moveHandler = gameObject.AddComponent<MoveHandler>();
        _waitHandler = gameObject.AddComponent<WaitHandler>();

        if (GameAcademy.USE_MANUAL_DECISIONS) _manualDecisionMaker = new ManualDecisionMaker(this);

        if (!_shouldResetAgent) // Not needed for training.
        {
            _interactionHandler = gameObject.AddComponent<InteractionHandler>();
            _monitorHandler = gameObject.AddComponent<MonitorHandler>();
            _monitorHandler.init(this, _waitHandler, _moveHandler);
        }

        _actionHandler = new ActionHandler(this, _actionPlanFinishedCallback);
        _actionMaskingHandler = new ActionMaskingHandler(this);

        setJob(_npcState.AgentInfo.JobId);

        if (_wasInitialized)
        {
            AgentReset();
        } else
        {
            _wasInitialized = true;
        }
    }

    public void setJob(string jobId)
    {
        if (_npcState.AgentInfo.JobId != null) _villageInfo.changeJobAmount(-1, jobId);
        _villageInfo.changeJobAmount(1, jobId);
        _npcState.AgentInfo.JobId = jobId;

        // Don't change materials during training.
        if (!_shouldResetAgent) _meshRenderer.material = _jobHandler.getMaterialFromJobId(jobId);
    }

    public void logReward(string action, float reward)
    {
        _actionHandler.logReward(action, reward);
    }

    public void updateAgentTime(float deltaTime, float currentTime, int daysPassed)
    {
        while (daysPassed > 0) daysPassed -= 1;
        _currentTime = currentTime == -1f ? _currentTime + deltaTime : currentTime;
        _npcState.updateState(_currentTime, deltaTime);
    }

    public void resetMoveAndWait()
    {
        if (_moveHandler.IsMoving) VillageInfo.changeAmountMoving(false);
        _moveHandler.reset();

        if (_waitHandler.IsWaiting) VillageInfo.changeAmountWaiting(false);
        _waitHandler.reset();
    }

    public override void AgentReset()
    {
        if (!_wasInitialized)
        {
            _wasInitialized = true;
            return;
        }

        resetMoveAndWait();
        _npcState.resetState(_currentTime); // Resets needs etc.
        _numActions = 0;

        gameObject.transform.position = _homePosition.position; // Reset position of the agent.

        if (_isFirstReset && !GameAcademy.USE_GOAP)
        {
            _isFirstReset = false;
            _actionHandler.runInitialAction(ActionHandler.INITIAL_ACTION_WAIT, AgentReset, requestModelOrManualDecision);
        } else
        {
            requestModelOrManualDecision();
        }
    }

    public void requestModelOrManualDecision()
    {
        if (GameAcademy.USE_MANUAL_DECISIONS) _executeAction(_manualDecisionMaker.makeDecision());
        else if (GameAcademy.USE_GOAP) return;
        else RequestDecision();
    }

    // 30 Observations
    public override void CollectObservations()
    {
        AddVectorObs(_npcState.NeedHunger);
        AddVectorObs(_npcState.NeedThirst);
        AddVectorObs(_npcState.NeedEducation);
        AddVectorObs(_npcState.NeedCommunication);
        AddVectorObs(_npcState.NeedFaith);

        AddVectorObs(_npcState.Recreation);
        AddVectorObs(_npcState.Exhaustion);

        AddVectorObs(_npcState.AttFittness);
        AddVectorObs(_npcState.AttIntelligence);
        AddVectorObs(_npcState.AttCompanionship);
        AddVectorObs(_npcState.AttCommodities);
        AddVectorObs(_npcState.AttFerocity);

        AddVectorObs(_npcState.Sleep);
        AddVectorObs(_npcState.Work);

        AddVectorObs(_npcState.getMoneyNormalized());

        AddVectorObs(_currentTime);

        // Only add resource observations if we ignore the village agent and do not ignore resources in general.
        if (GameAcademy.IGNORE_VILLAGE_AGENT && !GameAcademy.IGNORE_ALL_RESOURCES)
        {
            foreach (string resourceId in ResourceHandler.ALL_RESOURCES)
            {
                AddVectorObs(_resourceHandler.getResourceNormalized(resourceId));
            }
        }

        SetActionMask(_actionMaskingHandler.getIntActionMask(this));
    }

    // TODO maybe we should remember for how long a need value has been 0, to give the agent an incentive to update the value immediately. 
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        _executeAction(NpcActions.actionNameFromId(Mathf.RoundToInt(vectorAction[0])));
    }

    private void _executeAction(string actionId)
    {
        if (actionId == null || actionId == "")
        {
            Debug.Log("Invalid Action ID " + actionId + " when trying to make a decision.");
            return;
        }

        _academy.IncrementNumAgentActions(actionId);

        if (_decisionLog.ContainsKey(actionId)) _decisionLog[actionId] += 1; else _decisionLog[actionId] = 1;
        _action = actionId;
        _numActions++;

        if (GameAcademy.LOG_ATTRIBUTES) _villageInfo.addAgentAttributesForAction(actionId, _npcState.getAttributesClone());

        _actionHandler.runActionPlan(actionId);
    }

    private void _actionPlanFinishedCallback()
    {
        // TODO check what makes sense for the step reward.
        //_addStepRewards(); // Punishes agent for having need values under the threshold.
        _addStepPenalties();

        if (_numActions >= _maxActions && _shouldResetAgent) // Maximum amount of actions reached. Agent is never done during inference.
        {
            Done();
            _numActions = 0;
        }
        else // Request new decision.
        {
            requestModelOrManualDecision();
        }
    }

    // TODO see what makes sense here.
    // Only punish fixed value when below threshold and reward if all values above threshold? something like that?
    private void _addStepRewards()
    {
        bool allNeedsAboveThreshold = true;
        foreach (string needType in _npcState.NeedKeys)
        {
            if (_npcState.getNeedByType(needType) < BaseActionPlan.NEED_PENALTY_THRESHOLD)
            {
                allNeedsAboveThreshold = false;
                break;
            }
        }
        if (allNeedsAboveThreshold && _npcState.Exhaustion >= BaseActionPlan.NEED_PENALTY_THRESHOLD) AddReward(0.15f);
    }

    // ???????????
    // TODO vielleicht sind deswegen die rewards von recreation immer so low?
    private void _addStepPenalties()
    {
        if (_action == NpcActions.SLEEP) return;
        float penalty = 0f;
        foreach (string needType in _npcState.NeedKeys)
        {
            if (_npcState.getNeedByType(needType) <= 0f)
            {
                penalty -= 0.025f;
            }
        }
        if (penalty < 0f) AddReward(penalty);
    }
}

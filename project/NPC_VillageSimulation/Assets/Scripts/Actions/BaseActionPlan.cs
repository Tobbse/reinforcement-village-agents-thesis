using System.Collections.Generic;
using System;
using UnityEngine;

public class BaseActionPlan
{
    public const float NEED_PENALTY_THRESHOLD = 0.3f;
    public const float RECREATION_PENALTY_THRESHOLD = 0.2f;
    public const float RESOURCE_PENALTY_THRESHOLD = 0.35f;
    public const float NEED_REWARD_MULTIPLIER = 0.35f;
    public const float ATT_REWARD_MULTIPLIER = 1.33f;
    public const float RESOURCE_REWARD_MULTIPLIER = 0.15f;
    public const float RECREATION_REWARD_MULTIPLIER = 0.06f;
    public const float WORK_REWARD_MULTIPLIER = 2.0f;
    public const float SLEEP_REWARD_MULTIPLIER = 2.0f;
    public const float DEFAULT_ACTION_SECONDS = 30;
    public const float HIGH_CLASS_WORK_MONEY_REWARD = 0.7f;
    public const float LOW_CLASS_WORK_MONEY_REWARD = 0.4f;

    protected const float DEFAULT_NEED_GAIN = 1.0f;

    protected string _actionType;
    protected float _money;
    protected NpcAgent _npc;
    protected NpcState _npcState;
    protected TimeController _timeControl;
    protected ResourceHandler _resourceHandler;
    protected LocationHandler _locationHandler;
    protected Dictionary<string, float> _initNeedsClone;
    protected float _initRecreation;

    private Action _allActionsExecutedCallback;
    private List<BaseAction> _actions;
    private int _currentAction;
    private float _startTime;

    public BaseActionPlan(string actionType, Action allActionsExecutedCallback, NpcAgent npc)
    {
        _actionType = actionType;
        _allActionsExecutedCallback = allActionsExecutedCallback;
        _npc = npc;
        _npcState = npc.NpcState;
        _resourceHandler = _npc.ResourceHandler;
        _locationHandler = _npc.LocationHandler;
        _timeControl = GameObject.FindObjectOfType<TimeController>();

        _actions = new List<BaseAction>();
        _addPlanActions();
    }

    public virtual bool areRequirementsMet()
    {
        throw new System.NotImplementedException("areRequirementsMet is not implemented on " + this.GetType());
    }

    // Executed once to add several actions.
    protected virtual void _addPlanActions()
    {
        throw new System.NotImplementedException("_addPlanActions is not implemented on " + this.GetType());
    }

    protected virtual float _getReward()
    {
        throw new System.NotImplementedException("_getRewards is not implemented on " + this.GetType());
    }

    // Optional. Executed each time after execution.
    protected virtual void _additionalFinishAction()
    {
        // Override if needed.
    }

    // Optional. Executed each time before running the actions.
    protected virtual void _initRun()
    {
        _startTime = Time.time;
        _npc.VillageInfo.changeAmountForAction(true, _actionType);
        _currentAction = 0;
        _initNeedsClone = _npcState.getNeedsClone();
        _initRecreation = _npcState.Recreation;
        if (_money != 0f) _npcState.payMoney(_money);
    }

    public void runActions()
    {
        _initRun();

        if (_actions == null || _actions.Count == 0)
        {
            _onFinish();
            _allActionsExecutedCallback();
        } else
        {
            _executeNextAction();
        }
    }

    protected bool _hasFreeSpot(string locationType)
    {
        return _locationHandler.hasFreeSpots(locationType);
    }

    protected bool _hasEnoughMoney(float cost)
    {
        return _npcState.Money >= cost;
    }

    protected bool _hasEnoughResource(string resourceId, int cost)
    {
        if (GameAcademy.IGNORE_ALL_RESOURCES) return true;
        return _resourceHandler.getResource(resourceId) >= cost;
    }

    protected bool _hasResourceCapacity(string resourceId, int amount)
    {
        if (GameAcademy.IGNORE_ALL_RESOURCES) return true;

        int capacity = _resourceHandler.getResourceCapacity(resourceId);
        int currentAmount = _resourceHandler.getResource(resourceId);
        return currentAmount + amount <= capacity;
    }

    protected float _getNeedReward(string needType, float multiplier)
    {
        return (NEED_PENALTY_THRESHOLD - _initNeedsClone[needType]) * multiplier;
    }

    protected float _getRecreationReward(float multiplier)
    {
        return (RECREATION_PENALTY_THRESHOLD - _initRecreation) * multiplier;
    }

    protected float _calculateAttributeMultReward(string attributeType, float reward, bool invert = false)
    {
        float mult = _npcState.getAttributeByType(attributeType) - 0.5f;
        if (invert) mult *= -1;
        if (reward < 0f) return reward;
        float multREward = reward * (1f + (ATT_REWARD_MULTIPLIER * mult));
        return multREward;
    }

    protected float _getResourceReward(float amount, float multiplier)
    {
        return (RESOURCE_PENALTY_THRESHOLD - amount) * multiplier;
    }

    protected void _addMoveAction(string locationType, bool force = false,
        int pickBuildingStrategy = MoveAction.PICK_BUILDING_STRATEGY_CLOSEST,
        Building npcHomeBuilding = null)
    {
        _addAction(new MoveAction(_actionType, _npc, locationType, force, pickBuildingStrategy, npcHomeBuilding));
    }

    protected void _addWaitAction(float seconds)
    {
        _addAction(new WaitAction(_actionType, _npc, seconds));
    }

    protected void _addVacateAction(string locationType)
    {
        _addAction(new VacateSlotAction(_actionType, _npc, locationType));
    }

    protected void _addAction(BaseAction newAction)
    {
        _actions.Add(newAction);
    }

    protected void _moveAndWaitRealSeconds(
        float realSeconds, string locationType, bool force = false,
        int locationStrategy = MoveAction.PICK_BUILDING_STRATEGY_CLOSEST,
        Building npcHomeBuilding = null)
    {
        _addMoveAction(locationType, force, locationStrategy, npcHomeBuilding);
        _addWaitAction(realSeconds);
        if (locationStrategy != MoveAction.PICK_BUILDING_STRATEGY_HOME) _addVacateAction(locationType);
    }

    protected void _moveAndWaitGameSeconds(
        float gameSeconds, string locationType, bool force = false,
        int locationStrategy = MoveAction.PICK_BUILDING_STRATEGY_CLOSEST,
        Building npcHomeBuilding = null)
    {
        float realSeconds = TimeController.realSecondsFromGameSeconds(gameSeconds);
        _moveAndWaitRealSeconds(realSeconds, locationType, force,
            locationStrategy, npcHomeBuilding);
    }

    protected void _moveAndWaitGameMinutes(
    float gameMinutes, string locationType, bool force = false,
    int locationStrategy = MoveAction.PICK_BUILDING_STRATEGY_CLOSEST,
    Building npcHomeBuilding = null)
    {
        float realSeconds = TimeController.realSecondsFromGameMinutes(gameMinutes);
        _moveAndWaitRealSeconds(realSeconds, locationType, force,
            locationStrategy, npcHomeBuilding);
    }

    protected void _moveAndWaitGameHours(
        float gameHours, string locationType, bool force = false,
        int locationStrategy = MoveAction.PICK_BUILDING_STRATEGY_CLOSEST,
        Building npcHomeBuilding = null)
    {
        float realSeconds = TimeController.realSecondsFromGameHours(gameHours);
        _moveAndWaitRealSeconds(realSeconds, locationType, force,
            locationStrategy, npcHomeBuilding);
    }

    protected void _onFinish()
    {
        _npc.VillageInfo.changeAmountForAction(false, _actionType);
        float reward = _getReward();

        if (GameAcademy.LOG_REWARDS) _npc.logReward(_actionType, reward);
        _npc.AddReward(reward);

        _additionalFinishAction();

        if (GameAcademy.LOG_ACTION_DURATIONS) _npc.VillageInfo.addActionDuration(_actionType, Time.time - _startTime);
    }

    protected virtual void _actionPlanCancelledCallback()
    {
        _npc.VillageInfo.changeAmountForAction(false, _actionType);
        Debug.LogWarning("ActionPlan has been cancelled for agent " + _npc.AgentId.ToString() + " when executing Action: " + _actions[_currentAction].GetType().ToString() + " in Action Plan: " + this.GetType().ToString() + ".");
        _npc.resetMoveAndWait();
        _npc.requestModelOrManualDecision();
        _currentAction = 0;
    }

    private void _executeNextAction()
    {
        _npc.StartCoroutine(_actions[_currentAction].execute(_nextActionExecutedCallback, _actionPlanCancelledCallback));
    }

    private void _nextActionExecutedCallback()
    {
        _currentAction += 1;

        if (_currentAction >= _actions.Count)
        {
            _onFinish();
            _allActionsExecutedCallback();
        }
        else
        {
            _executeNextAction();
        }
    }
}

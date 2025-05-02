using UnityEngine;
using System;
using System.Collections.Generic;

public class ActionHandler
{
    public const string INITIAL_ACTION_WAIT = "MANUAL_ACTION_INIT_WAIT";

    private NpcAgent _npc;
    private Dictionary<string, BaseActionPlan> _actionPlans;
    private Action _actionPlanFinishedCallback;
    private Dictionary<string, float> _rewardLog;
    private Dictionary<string, int> _actionAmounts;
    private Dictionary<string, Queue<float>> _rewardQueue;
    private Dictionary<string, float> _averageRewards;

    public ActionHandler(NpcAgent npc, Action actionPlanFinishedCallback)
    {
        _npc = npc;
        _actionPlanFinishedCallback = actionPlanFinishedCallback;

        _setupVars();
        _setupActionPlans();
    }

    public void runActionPlan(string actionName)
    {
        if (!_actionPlans.ContainsKey(actionName) || _actionPlans[actionName] == null)
        {
            Debug.LogWarning("No Action Plan found for action type: " + actionName);
            _actionPlanFinishedCallback();
            return;
        }
        _actionPlans[actionName].runActions();
    }

    public void runInitialAction(string actionName, Action initialWaitCallback, Action cancelCallback)
    {
        if (actionName == INITIAL_ACTION_WAIT)
        {
            WaitAction action = new WaitAction(actionName, _npc, UnityEngine.Random.value * 60f);
            _npc.StartCoroutine(action.execute(initialWaitCallback, cancelCallback));
        }
    }

    public void logReward(string action, float reward)
    {
        _rewardLog[action] += reward;
        _actionAmounts[action] += 1;

        if (GameAcademy.DEBUG_MODE)
        {
            if (!_rewardQueue.ContainsKey(action)) _rewardQueue[action] = new Queue<float>();
            _rewardQueue[action].Enqueue(reward);
            if (_rewardQueue[action].Count >= 100) _rewardQueue[action].Dequeue();

            _averageRewards[action] = _rewardLog[action] / (float)_actionAmounts[action];
        }
    }

    public bool areRequirementsMetForAction(string actionId)
    {
        return _actionPlans[actionId].areRequirementsMet();
    }

    private void _setupActionPlans()
    {
        _actionPlans = new Dictionary<string, BaseActionPlan>();

        _actionPlans[NpcActions.HUNGER_BREAD] = new HungerBreadActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.HUNGER_MEAT] = new HungerMeatActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.THIRST] = new ThirstActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.SLEEP] = new SleepActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.EDUCATION_SCHOOL] = new EducationSchoolActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.EDUCATION_LIBRARY] = new EducationLibraryActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.EDUCATION_COLLEGE] = new EducationCollegeActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.COMMUNICATION_CHAT] = new CommunicationChatActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.RELIGION_CHURCH] = new ReligionChurchActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.RECREATION_SOCCER] = new RecreationSoccerActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.RECREATION_TAVERN] = new RecreationTavernActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.RECREATION_CAFE] = new RecreationCafeActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.RECREATION_SHOOTING] = new RecreationShootingActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.RECREATION_SHOP] = new RecreationShopActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.RECREATION_THEATER] = new RecreationTheaterActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.RECREATION_VISIT] = new RecreationVisitActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.RECREATION_BARBECUE] = new RecreationBarbecueActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.RESTING] = new ExhaustionRestActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.WORK_HUNTING] = new WorkMeatHuntingActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.WORK_BUTCHER] = new WorkMeatButcherActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.WORK_GRAIN_FARMING] = new WorkBreadFarmGrainActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.WORK_WINDMILL] = new WorkBreadWindmillActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.WORK_BAKERY] = new WorkBreadBakeryActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.WORK_HOPS_FARMING] = new WorkBeerFarmHopsActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.WORK_BREWERY] = new WorkBeerBreweryActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.WORK_LOGGING] = new WorkWoodLoggingActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.WORK_CARPENTER] = new WorkWoodCarpenterActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.WORK_MINING] = new WorkToolsMiningActionPlan(_actionPlanFinishedCallback, _npc);
        _actionPlans[NpcActions.WORK_BLACKSMITH] = new WorkToolsBlacksmithActionPlan(_actionPlanFinishedCallback, _npc);
    }

    private void _setupVars()
    {
        _averageRewards = new Dictionary<string, float>();
        _rewardQueue = new Dictionary<string, Queue<float>>();
        _rewardLog = new Dictionary<string, float>();
        _actionAmounts = new Dictionary<string, int>();

        foreach (string action in NpcActions.ALL_ACTIONS)
        {
            if (GameAcademy.DEBUG_MODE)
            {
                _averageRewards[action] = 0f;
                _rewardQueue[action] = new Queue<float>();
            }
            _rewardLog[action] = 0f;
            _actionAmounts[action] = 0;
        }
    }
}

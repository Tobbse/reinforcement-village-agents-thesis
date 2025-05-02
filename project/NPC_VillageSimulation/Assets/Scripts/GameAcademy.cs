using MLAgents;
using UnityEngine;
using System.Collections.Generic;

public class GameAcademy : Academy
{
    public const int GAME_MODE_REGULAR_INFERENCE = 0;
    public const int GAME_MODE_FAKE_INFERENCE = 1;
    public const int GAME_MODE_SINGLE_AGENT_TRAINING = 2;
    public const int GAME_MODE_MULTI_AGENT_TRAINING = 3;

    public static int GAME_MODE;
    public static int NUM_AGENTS_PER_VILLAGE;
    public static int NUM_VILLAGES_REGISTERED;
    public static int MAX_NPC_ACTIONS;
    public static float AGENT_STOPPING_DISTANCE;
    public static float RYTHM_VARIATION;
    public static bool USE_FAKE_INFERENCE;
    public static bool IS_TRAINING;
    public static bool VERBOSE;
    public static bool DEBUG_MODE;
    public static bool USE_MANUAL_DECISIONS;
    public static bool IGNORE_VILLAGE_AGENT;
    public static bool USE_GOAP;
    public static bool NPC_LOADING_DONE;
    public static bool LOG_ACTION_DURATIONS;
    public static bool LOG_REWARDS;
    public static bool LOG_NEEDS;
    public static bool LOG_ACTION_MASK_AMOUNTS;
    public static bool LOG_ATTRIBUTES;
    public static bool IGNORE_ALL_RESOURCES;
    public static bool IGNORE_JOB_MASK;

    [Tooltip("Prefab for the villages that will be instantiated.")]
    public GameObject villagePrefab;
    [Tooltip("Define how many agents should be spawned per village." +
        "This should be in order of 25s (25, 50, 75...)," +
        " for the jobs distribution to work properly.")]
    public int numAgentsPerVillage25 = 50;
    [Tooltip("Define how many decisions the agents should perform until they are reset.")]
    public int actionsPerAgentUntilReset = 100;
    [Tooltip("Tracks some additional data useful for debugging when activated.")]
    public bool debugMode;
    [Tooltip("Since high timescale levels do not scale well with NavMeshAgents, a kind of fake" +
        "inference will have to be used, in which agents teleport to their destinations and" +
        "additionally wait for an amount of time that is equal to the time it would have " +
        "taken to move there.")]
    public bool useFakeInference = false;
    [Tooltip("Prints some additional information when activated.")]
    public bool verbose = false;
    [Tooltip("Decides how many steps will be performed per agent between VillageAgent decisions.")]
    public int actionsUntilVillagAction = 5;
    [Tooltip("Decides how many decisions will be made by the VillageAgent until Done is called.")]
    public int villageDecisionsReset = 50;
    [Tooltip("Uses manual decisions instead of the trained model.")]
    public bool useManualNpcDecisions = false;
    [Tooltip("Ignores the village agent and sticks with the initial distribution of jobs.")]
    public bool ignoreVillageAgent = true;
    [Tooltip("Uses GOAP agents instead of a trained model.")]
    public bool useGoap = false;
    [Tooltip("Determines how far from his target an agent will stop when moving to a destination.")]
    public float agentStoppingDistance = 0.5f;
    [Tooltip("Decides the amount of variation between the day/night cycles of the agents.")]
    public float agentRythmVariation = 0.2f;
    [Tooltip("Determines whether the duration of actions should be tracked in VillageInfo.")]
    public bool logActionDurations = false;
    [Tooltip("Determines whether the rewards should be tracked for each action to gain some statistical information.")]
    public bool logRewards = false;
    [Tooltip("Logs the average needs of NPCs at certain times for later analysis.")]
    public bool logNeeds = false;
    [Tooltip("Logs the average attributes of agents for each action.")]
    public bool logAttributes = false;
    [Tooltip("Determines whether to log the amount of actions that are masked respectively.")]
    public bool logActionMaskAmounts = false;
    [Tooltip("Completely ignores all resource handling when set to true.")]
    public bool ignoreAllResources = false;
    [Tooltip("Ignores the job that has been designated to an angent and instead doesn't mask jobs.")]
    public bool ignoreJobMask = false;

    private List<Village> _villages;
    private int _stepsSinceVillageDecision;
    private int _numActionsVillageDecisionTotal;
    private int _numVillageAgentDecisions;
    private int _numVillageDecisionsSoFar;
    private int _numVillageResetsSoFar;
    private Dictionary<string, int> _performedActions;

    public override void InitializeEnvironment()
    {
        base.InitializeEnvironment();

        NUM_AGENTS_PER_VILLAGE = numAgentsPerVillage25;
        USE_FAKE_INFERENCE = useFakeInference;
        MAX_NPC_ACTIONS = actionsPerAgentUntilReset;
        VERBOSE = verbose;
        DEBUG_MODE = debugMode;
        USE_MANUAL_DECISIONS = useManualNpcDecisions;
        IGNORE_VILLAGE_AGENT = ignoreVillageAgent;
        USE_GOAP = useGoap;
        AGENT_STOPPING_DISTANCE = agentStoppingDistance;
        LOG_ACTION_DURATIONS = logActionDurations;
        LOG_REWARDS = logRewards;
        LOG_ACTION_MASK_AMOUNTS = logActionMaskAmounts;
        IGNORE_ALL_RESOURCES = ignoreAllResources;
        IGNORE_JOB_MASK = ignoreJobMask;
        LOG_NEEDS = logNeeds;
        LOG_ATTRIBUTES = logAttributes;
        RYTHM_VARIATION = agentRythmVariation;

        IS_TRAINING = !GetIsInference();
        GAME_MODE = _determineGameMode();
        Monitor.SetActive(!IS_TRAINING);

        _performedActions = new Dictionary<string, int>();
        foreach (string actionId in NpcActions.ALL_ACTIONS) _performedActions[actionId] = 0;

        _villages = new List<Village>();
        Village[] otherVillages = FindObjectsOfType<Village>();
        for (int i = 0; i < otherVillages.Length; i++)
        {
            Village village = otherVillages[i];
            if (!village.enabled) continue;
            _villages.Add(village);
            village.initVillage();
        }
        FindObjectOfType<GameUI>().initAgents();

        _numActionsVillageDecisionTotal = _villages.Count * numAgentsPerVillage25 * actionsUntilVillagAction;

        Debug.Log(_numActionsVillageDecisionTotal.ToString() + " agent decisions will be made until village decisions.");
        Debug.Log("That is " + actionsUntilVillagAction.ToString() + " decisions per agent for each village.");
        Debug.Log(villageDecisionsReset.ToString() + " village decisions will be made until a village reset.");
        Debug.Log((villageDecisionsReset * _numActionsVillageDecisionTotal).ToString() + " agent decisions will be made until a village reset.");
    }

    public void IncrementNumAgentActions(string actionId)
    {
        _stepsSinceVillageDecision++;
        _performedActions[actionId]++;

        if (IS_TRAINING && _numVillageAgentDecisions >= villageDecisionsReset)
        {
            _numVillageResetsSoFar++;
            Debug.Log("VillageAgent resets per village: #" + _numVillageResetsSoFar.ToString() + ".");
            foreach (Village village in _villages)
            {
                village.resetNpcJobs();
                village.VillageAgent.Done();
            }
            _numVillageAgentDecisions = 0;
            _stepsSinceVillageDecision = 0;
        }

        if (_stepsSinceVillageDecision >= _numActionsVillageDecisionTotal)
        {
            _numVillageDecisionsSoFar++;
            Debug.Log("Village decision per village: #" + _numVillageDecisionsSoFar.ToString() + ".");
            _stepsSinceVillageDecision = 0;
            _numVillageAgentDecisions++;
            foreach (Village village in _villages) village.VillageAgent.RequestDecision();
        }
    }

    private int _determineGameMode()
    {
        bool multiAgent = NUM_AGENTS_PER_VILLAGE > 1;
        bool fakeInference = USE_FAKE_INFERENCE;

        if (!IS_TRAINING) return fakeInference ? GAME_MODE_FAKE_INFERENCE : GAME_MODE_REGULAR_INFERENCE;
        else return multiAgent ? GAME_MODE_MULTI_AGENT_TRAINING : GAME_MODE_SINGLE_AGENT_TRAINING;
    }
}

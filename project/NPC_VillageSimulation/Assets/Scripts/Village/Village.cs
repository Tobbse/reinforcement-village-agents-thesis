using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using MLAgents;
using System;
using UnityEngine.UI;
using CielaSpike;
using System.IO;
using System.Globalization;

public class Village : MonoBehaviour
{
    public GameObject agentPrefab;
    public GameObject agentSpawningParent;
    public Material grainFarmerMaterial;
    public Material millerMaterial;
    public Material bakerMaterial;
    public Material hopsFarmerMaterial;
    public Material brewerMaterial;
    public Material hunterMaterial;
    public Material butcherMaterial;
    public Material loggerMaterial;
    public Material carpenterMaterial;
    public Material minerMaterial;
    public Material blacksmithMaterial;

    private bool debugMode;
    private bool useFakeInference;
    private bool verbose;
    private List<string> _jobsDistribution;
    private ResourceHandler _resourceHandler;
    private LocationHandler _locationHandler;
    private TimeController _timeController;
    private LightHandler _lightHandler;
    private JobHandler _jobHandler;
    private VillageInfo _villageInfo;
    private VillageAgent _villageAgent;
    private bool _useManualDecisions;
    private List<NpcAgent> _villageNpcs;
    private Dictionary<int, bool> _npcLoadingTasks;
    private Dictionary<float, List<Dictionary<string, float>>> _needsLog;
    private Dictionary<float, Dictionary<string, float>> _averageNeedsLog;

    public ResourceHandler ResourceHandler { get => _resourceHandler; }
    public VillageAgent VillageAgent { get => _villageAgent; }
    public VillageInfo VillageInfo { get => _villageInfo; }
    public JobHandler JobHandler { get => _jobHandler; }
    public List<NpcAgent> VillageNpcs { get => _villageNpcs; }

    public void initVillage()
    {
        _timeController = gameObject.AddComponent<TimeController>();
        _locationHandler = gameObject.AddComponent<LocationHandler>();
        _resourceHandler = new ResourceHandler();
        _lightHandler = ScriptableObject.CreateInstance<LightHandler>();

        if (GameAcademy.LOG_NEEDS)
        {
            _needsLog = new Dictionary<float, List<Dictionary<string, float>>>();
            _averageNeedsLog = new Dictionary<float, Dictionary<string, float>>();
        }

        _initVillageInfo();
        _spawnVillageAgent();
        _spawnNpcs();
        _lightHandler.init(gameObject.GetComponentsInChildren<Light>());
        _initNpcJobs();
        StartCoroutine(_initNpcs());
    }

    public void updateTime(float deltaTime, float currentTime, int daysPassed, bool shouldLogNeeds)
    {
        foreach (NpcAgent npc in _villageNpcs) npc.updateAgentTime(deltaTime, currentTime, daysPassed);
        _lightHandler.updateLights(currentTime);

        Monitor.Log("Time", currentTime);
        Monitor.Log("ORE, TOOLS", new float[] { _resourceHandler.getResourceNormalized(ResourceHandler.ORE), _resourceHandler.getResourceNormalized(ResourceHandler.TOOLS) });
        Monitor.Log("LOGS, FIREWOOD", new float[] { _resourceHandler.getResourceNormalized(ResourceHandler.WOODEN_LOG), _resourceHandler.getResourceNormalized(ResourceHandler.FIREWOOD) });
        Monitor.Log("VENISON, MEAT", new float[] { _resourceHandler.getResourceNormalized(ResourceHandler.RAW_VENISON), _resourceHandler.getResourceNormalized(ResourceHandler.COOKED_MEAT) });
        Monitor.Log("HOPS, BEER", new float[] { _resourceHandler.getResourceNormalized(ResourceHandler.HOPS), _resourceHandler.getResourceNormalized(ResourceHandler.BEER) });
        Monitor.Log("GRAIN, FLOUR, BREAD", new float[] { _resourceHandler.getResourceNormalized(ResourceHandler.GRAIN), _resourceHandler.getResourceNormalized(ResourceHandler.FLOUR), _resourceHandler.getResourceNormalized(ResourceHandler.BREAD) });

        // This is some hacky way to log the agent's needs to a csv. Super quick and dirty but I had no time.
        if (!shouldLogNeeds || !GameAcademy.LOG_NEEDS) return;

        float logPoint = (float)Math.Floor(currentTime / TimeController.NEED_LOG_TIME_INTERVAL);
        float averageSleep = 0f;
        float averageWork = 0f;
        float averageExhaustion = 0f;
        float averageRecreation = 0f;
        int numNpcs = _villageNpcs.Count;
        Dictionary<string, float> averageNeeds = new Dictionary<string, float>();
        foreach (string needType in NpcState.ALL_NEEDS) averageNeeds[needType] = 0f;
        foreach (NpcAgent agent in _villageNpcs)
        {
            NpcState state = agent.NpcState;
            foreach (string needType in NpcState.ALL_NEEDS)
            {
                averageNeeds[needType] += state.getNeedByType(needType);
            }
            averageSleep += state.Sleep;
            averageWork += state.Work;
            averageExhaustion += state.Exhaustion;
            averageRecreation += state.Recreation;
        }
        averageNeeds[NpcState.SLEEP] = averageSleep;
        averageNeeds[NpcState.WORK] = averageWork;
        averageNeeds[NpcState.EXHAUSTION] = averageExhaustion;
        averageNeeds[NpcState.RECREATION] = averageRecreation;
        foreach (string needType in new List<string>(averageNeeds.Keys)) averageNeeds[needType] /= numNpcs;

        if (!_needsLog.ContainsKey(logPoint)) _needsLog[logPoint] = new List<Dictionary<string, float>>();
        _needsLog[logPoint].Add(averageNeeds);



        Dictionary<string, float> averageLogPointNeeds = new Dictionary<string, float>();
        foreach (string needKey in averageNeeds.Keys) averageLogPointNeeds[needKey] = 0f;
        List<Dictionary<string, float>> listOfDicts = _needsLog[logPoint];

        foreach (Dictionary<string, float> needDict in listOfDicts)
        { // Foreach dict in the list.
            foreach (string needType in new List<string>(needDict.Keys)) // Add needs from all dictionaries, for the specific logpoint list.
            {
                averageLogPointNeeds[needType] += needDict[needType];
            }
        }

        foreach (string needType in new List<string>(averageLogPointNeeds.Keys)) // Calculate averages.
        {
            averageLogPointNeeds[needType] /= listOfDicts.Count;
        }
        _averageNeedsLog[logPoint] = averageLogPointNeeds;

        bool write = false; // Can be set to true by debugger to write CSV.
        if (write)
        {
            string[] allNeeds = new string[] { NpcState.NEED_HUNGER, NpcState.NEED_THIRST, NpcState.NEED_EDUCATION,
            NpcState.NEED_COMMUNICATION, NpcState.NEED_FAITH, NpcState.RECREATION, NpcState.SLEEP, NpcState.WORK, NpcState.EXHAUSTION };
            string headline = "LogPoint,NEED_HUNGER,NEED_THIRST,NEED_EDUCATION,NEED_COMMUNICATION,NEED_FAITH,RECREATION,SLEEP,WORK,EXHAUSTION\n";
            string content = "";
            foreach (float averageLogPoint in _averageNeedsLog.Keys)
            {
                content += averageLogPoint.ToString(new CultureInfo("en-US")) + ",";
                Dictionary<string, float> needs = _averageNeedsLog[averageLogPoint];
                for (int i = 0; i < allNeeds.Length; i++)
                {
                    string needType = allNeeds[i];
                    content += needs[needType].ToString(new CultureInfo("en-US"));
                    if (i < allNeeds.Length - 1) content += ",";
                    else content += "\n";
                }
            }
            using (FileStream fs = File.Create("needs_output_" + System.DateTime.UtcNow.Hour + "_" + System.DateTime.UtcNow.Minute +  "_" +  System.DateTime.UtcNow.Second + ".csv"))
            {
                byte[] bytes = new System.Text.UTF8Encoding(true).GetBytes(headline + content);
                fs.Write(bytes, 0, bytes.Length);
            }
        }
    }

    public void resetNpcJobs()
    {
        int len = _villageNpcs.Count;
        for (int i = 0; i < len; i++) _villageNpcs[i].setJob(_jobsDistribution[i]);
    }

    private void _spawnVillageAgent()
    {
        _villageAgent = gameObject.GetComponent<VillageAgent>();
    }

    private void _spawnNpcs()
    {
        _villageNpcs = new List<NpcAgent>();

        for (int i = 0; i < GameAcademy.NUM_AGENTS_PER_VILLAGE; i++)
        {
            int agentId = i + 1; // Starting at 1.
            GameObject newAgent = Instantiate(agentPrefab, agentSpawningParent.transform, false);
            newAgent.name = agentId >= 10 ? "Agent_" + agentId.ToString() : "Agent_0" + agentId.ToString();
            NpcAgent npc = newAgent.GetComponent<NpcAgent>();
            npc.AgentId = agentId;
            _villageNpcs.Add(npc);
        }
        _locationHandler.init(gameObject.GetComponentsInChildren<Building>(), gameObject.GetComponentsInChildren<BuildingHome>(), _villageNpcs);
    }

    private IEnumerator _initNpcs()
    {
        _npcLoadingTasks = new Dictionary<int, bool>();
        for (int i = 0; i < _villageNpcs.Count; i++)
        {
            _npcLoadingTasks[i] = false;
            _villageNpcs[i].StartCoroutine(_initAgent(i));
        }
        yield return StartCoroutine(CheckTasksCompleted());
        _villageAgent.init(this);
        GameAcademy.NPC_LOADING_DONE = true;
        _timeController.init(this);
    }

    /**
     * Total: 25, different Jobs: 11
     * Adds the following initial jobs:
     * 5 Grain Farmers, 1 Miller, 1 Baker
     * 4 Hops Farmers, 1 Brewer
     * 3 Miners, 1 Blacksmith
     * 3 Hunters, 1 Butcher
     * 4 Loggers, 1 Carpenter
     * */
    private void _initNpcJobs()
    {
		string[] higherJobs = new string[] {
			JobHandler.JOB_MILLER, JobHandler.JOB_BAKER, JobHandler.JOB_BREWER,
			JobHandler.JOB_BLACKSMITH, JobHandler.JOB_BUTCHER, JobHandler.JOB_CARPENTER
		};
		string[] lowerJobs = new string[] {
			JobHandler.JOB_GRAIN_FARMER, JobHandler.JOB_HOPS_FARMER,
			JobHandler.JOB_MINER, JobHandler.JOB_HUNTER, JobHandler.JOB_LOGGER
		};
		string[] extraJobs = new string[] {
			JobHandler.JOB_GRAIN_FARMER, JobHandler.JOB_GRAIN_FARMER,
			JobHandler.JOB_HOPS_FARMER, JobHandler.JOB_LOGGER
		};
		_jobHandler = new JobHandler(_resourceHandler, _villageInfo, _locationHandler, grainFarmerMaterial,
            millerMaterial, bakerMaterial, hopsFarmerMaterial, brewerMaterial, hunterMaterial, butcherMaterial,
            loggerMaterial, carpenterMaterial, minerMaterial, blacksmithMaterial);
		_jobsDistribution = new List<string>();

        while (_jobsDistribution.Count < GameAcademy.NUM_AGENTS_PER_VILLAGE)
		{
			foreach (string higherJob in higherJobs) _jobsDistribution.Add(higherJob);
			for (int i = 0; i < 3; i++) foreach (string lowerJob in lowerJobs) _jobsDistribution.Add(lowerJob);
			foreach (string extraJob in extraJobs) _jobsDistribution.Add(extraJob);
		}
    }

    private IEnumerator CheckTasksCompleted()
    {
        while (true)
        {
            bool completed = true;
            foreach (bool loadingState in _npcLoadingTasks.Values)
            {
                if (!loadingState)
                {
                    completed = false;
                    break;
                }
            }
            if (completed)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    private IEnumerator _initAgent(int i)
    {
        NpcAgent npcAgent = _villageNpcs[i];
        NpcGoapAgent goapAgent = null;
        npcAgent.LocationHandler = _locationHandler;

        int age = Mathf.RoundToInt(UnityEngine.Random.value * AgentInfo.MAX_AGE);
        bool isMale = i % 2 == 0;
        string jobId = _jobsDistribution[i];
        string agentName = isMale ?
                      AgentInfo.NPC_NAMES_MALE[Mathf.RoundToInt((AgentInfo.NPC_NAMES_MALE.Length - 1) * UnityEngine.Random.value)] :
                      AgentInfo.NPC_NAMES_FEMALE[Mathf.RoundToInt((AgentInfo.NPC_NAMES_FEMALE.Length - 1) * UnityEngine.Random.value)];

        AgentInfo npcInfo = new AgentInfo(npcAgent.AgentId, age, isMale, jobId, agentName);

        if (GameAcademy.USE_GOAP)
        {
            yield return Ninja.JumpToUnity;
            goapAgent = npcAgent.gameObject.AddComponent<NpcGoapAgent>();
            VillageGoapActionAdder goapActionAdder = new VillageGoapActionAdder();
            yield return goapActionAdder.createGoapActions(npcAgent, goapAgent, _villageInfo);
            goapAgent.Init(npcAgent, goapActionAdder.GoapActions);
        }
        npcAgent.init(npcInfo, _resourceHandler, _villageInfo, _jobHandler, goapAgent);
        _npcLoadingTasks[i] = true;
        yield return null;
    }

    private void _initVillageInfo()
    {
        int villageId = GameAcademy.NUM_VILLAGES_REGISTERED++;
        bool showWorkAmounts = villageId == 0;
        GameObject[] uiWorkCounters = GameObject.FindGameObjectsWithTag(GameTags.UI_WORK_COUNTER);
        Dictionary<string, int> jobAmounts = new Dictionary<string, int>();
        Dictionary<string, Text> jobAmountTexts = new Dictionary<string, Text>();
        foreach (string job in JobHandler.ALL_JOBS)
        {
            bool found = false;
            foreach (GameObject counter in uiWorkCounters)
            {
                if (counter.name.Contains(job))
                {
                    Text[] texts = counter.GetComponentsInChildren<Text>();
                    foreach (Text text in texts)
                    {
                        if (text.name.Contains("Amount"))
                        {
                            if (showWorkAmounts) jobAmountTexts[job] = text;
                            jobAmounts[job] = 0;
                            found = true;
                        }
                    }
                }
            }
            if (!found) Debug.LogError("Did not find UI work tracking text for job: " + job + ".");
        }

        _villageInfo = new VillageInfo(villageId, jobAmounts, jobAmountTexts, showWorkAmounts);
    }
}

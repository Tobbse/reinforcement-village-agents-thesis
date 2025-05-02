using System.Collections.Generic;
using UnityEngine;

public class JobHandler
{
    public const string JOB_GRAIN_FARMER = "Grainfarmer";
    public const string JOB_MILLER = "Miller";
    public const string JOB_BAKER = "Baker";
    public const string JOB_HOPS_FARMER = "Hopsfarmer";
    public const string JOB_BREWER = "Brewer";
    public const string JOB_HUNTER = "Hunter";
    public const string JOB_BUTCHER = "Butcher";
    public const string JOB_LOGGER = "Logger";
    public const string JOB_CARPENTER = "Carpenter";
    public const string JOB_MINER = "Miner";
    public const string JOB_BLACKSMITH = "Blacksmith";

    public static string[] ALL_JOBS = new string[]
    {
        JOB_GRAIN_FARMER, JOB_MILLER, JOB_BAKER,
        JOB_HOPS_FARMER, JOB_BREWER,
        JOB_HUNTER, JOB_BUTCHER,
        JOB_LOGGER, JOB_CARPENTER,
        JOB_MINER, JOB_BLACKSMITH
    };

    private Dictionary<string, KeyValuePair<string,Material>> _jobs;

    public JobHandler(ResourceHandler resourceHandler, VillageInfo villageInfo, LocationHandler locationHandler,
        Material grainFarmer, Material miller, Material baker, Material hopsFarmer, Material brewer,
        Material hunter, Material butcher, Material logger, Material carpenter, Material miner, Material blacksmith)
    {
        _jobs = new Dictionary<string, KeyValuePair<string, Material>>();
        _jobs[JOB_GRAIN_FARMER] = new KeyValuePair<string, Material>(NpcActions.WORK_GRAIN_FARMING, grainFarmer);
        _jobs[JOB_MILLER] = new KeyValuePair<string, Material>(NpcActions.WORK_WINDMILL, miller);
        _jobs[JOB_BAKER] = new KeyValuePair<string, Material>(NpcActions.WORK_BAKERY, baker);
        _jobs[JOB_HOPS_FARMER] = new KeyValuePair<string, Material>(NpcActions.WORK_HOPS_FARMING, hopsFarmer);
        _jobs[JOB_BREWER] = new KeyValuePair<string, Material>(NpcActions.WORK_BREWERY, brewer);
        _jobs[JOB_HUNTER] = new KeyValuePair<string, Material>(NpcActions.WORK_HUNTING, hunter);
        _jobs[JOB_BUTCHER] = new KeyValuePair<string, Material>(NpcActions.WORK_BUTCHER, butcher);
        _jobs[JOB_LOGGER] = new KeyValuePair<string, Material>(NpcActions.WORK_LOGGING, logger);
        _jobs[JOB_CARPENTER] = new KeyValuePair<string, Material>(NpcActions.WORK_CARPENTER, carpenter);
        _jobs[JOB_MINER] = new KeyValuePair<string, Material>(NpcActions.WORK_MINING, miner);
        _jobs[JOB_BLACKSMITH] = new KeyValuePair<string, Material>(NpcActions.WORK_BLACKSMITH, blacksmith);
    }

    public string getActionIdFromJobId(string jobId)
    {
        return _jobs[jobId].Key;
    }

    public string getJobIdFromActionId(string actionId)
    {
        foreach (string jobId in _jobs.Keys)
        {
            if (_jobs[jobId].Key == actionId) return jobId;
        }
        return "";
    }

    public Material getMaterialFromJobId(string jobId)
    {
        return _jobs[jobId].Value;
    }
}

using MLAgents;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VillageAgent : Agent
{
    private ResourceHandler _resourceHandler;
    private JobHandler _jobHandler;
    private List<NpcAgent> _npcs;
    private VillageInfo _villageInfo;
    private Dictionary<string, int> _jobToIndex;
    private Dictionary<string, string> _jobToResource;
    private Dictionary<int, string> _indexToJob;
    private Dictionary<string, List<NpcAgent>> _npcsByJobs;
    private bool _wasInitialized;

    public override void AgentReset()
    {
        if (!_wasInitialized) return;
        _resourceHandler.reset();
        RequestDecision();
    }

    public void init(Village villageInitializer)
    {
        _resourceHandler = villageInitializer.ResourceHandler;
        _jobHandler = villageInitializer.JobHandler;
        _villageInfo = villageInitializer.VillageInfo;
        _npcs = villageInitializer.VillageNpcs;

        _jobToIndex = new Dictionary<string, int>();
        _jobToIndex[JobHandler.JOB_GRAIN_FARMER] = 0;
        _jobToIndex[JobHandler.JOB_MILLER] = 1;
        _jobToIndex[JobHandler.JOB_BAKER] = 2;
        _jobToIndex[JobHandler.JOB_HOPS_FARMER] = 3;
        _jobToIndex[JobHandler.JOB_BREWER] = 4;
        _jobToIndex[JobHandler.JOB_HUNTER] = 5;
        _jobToIndex[JobHandler.JOB_BUTCHER] = 6;
        _jobToIndex[JobHandler.JOB_LOGGER] = 7;
        _jobToIndex[JobHandler.JOB_CARPENTER] = 8;
        _jobToIndex[JobHandler.JOB_MINER] = 9;
        _jobToIndex[JobHandler.JOB_BLACKSMITH] = 10;

        _indexToJob = new Dictionary<int, string>();
        foreach (string job in _jobToIndex.Keys) _indexToJob[_jobToIndex[job]] = job;

        _jobToResource = new Dictionary<string, string>();
        _jobToResource[JobHandler.JOB_GRAIN_FARMER] = ResourceHandler.GRAIN;
        _jobToResource[JobHandler.JOB_MILLER] = ResourceHandler.FLOUR;
        _jobToResource[JobHandler.JOB_BAKER] = ResourceHandler.BREAD;
        _jobToResource[JobHandler.JOB_HOPS_FARMER] = ResourceHandler.HOPS;
        _jobToResource[JobHandler.JOB_BREWER] = ResourceHandler.BEER;
        _jobToResource[JobHandler.JOB_HUNTER] = ResourceHandler.RAW_VENISON;
        _jobToResource[JobHandler.JOB_BUTCHER] = ResourceHandler.COOKED_MEAT;
        _jobToResource[JobHandler.JOB_LOGGER] = ResourceHandler.WOODEN_LOG;
        _jobToResource[JobHandler.JOB_CARPENTER] = ResourceHandler.FIREWOOD;
        _jobToResource[JobHandler.JOB_MINER] = ResourceHandler.ORE;
        _jobToResource[JobHandler.JOB_BLACKSMITH] = ResourceHandler.TOOLS;

        _npcsByJobs = new Dictionary<string, List<NpcAgent>>();
        foreach (string job in _jobToIndex.Keys) _npcsByJobs[job] = new List<NpcAgent>();
        foreach (NpcAgent npc in _npcs)
        {
            _npcsByJobs[npc.NpcState.AgentInfo.JobId].Add(npc);
            if (GameAcademy.IGNORE_VILLAGE_AGENT) _villageInfo.changeJobAmount(1, npc.NpcState.AgentInfo.JobId);
        }

        _wasInitialized = true;
        RequestDecision();
    }

    public override void CollectObservations()
    {
        foreach (string job in JobHandler.ALL_JOBS)
        {
            float amount = _villageInfo.getNumWorkersForJobNormalized(job);
            AddVectorObs(amount);
        }
        foreach (string resource in ResourceHandler.ALL_RESOURCES)
        {
            float amount = _resourceHandler.getResourceNormalized(resource);
            AddVectorObs(amount);
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (GameAcademy.IGNORE_VILLAGE_AGENT || !_wasInitialized) return;

        float smallestValue = 1;
        int smallestIndex = -1;
        float largestValue = -1;
        int largestIndex = -1;
        for (int i = 0; i < vectorAction.Length; i++)
        {
            float value = vectorAction[i];
            if (value < smallestValue)
            {
                smallestValue = value;
                smallestIndex = i;
            }
            if (value > largestValue)
            {
                largestValue = value;
                largestIndex = i;
            }
        }

        // Don't do anything, no change needed.
        if (largestValue < 0.3f && smallestValue > -0.3f)
        {
            AddReward(-0.01f);
            return;
        }

        string negJob = _indexToJob[smallestIndex];
        string negResource = _jobToResource[negJob];

        string posJob = _indexToJob[largestIndex];
        string posResource = _jobToResource[posJob];

        _assignNewWorkerJob(negJob, posJob);
        _addRewardsNew(negJob, posJob);
    }

    private void _addRewardsNew(string removedJob, string addedJob)
    {
        Dictionary<string, float> resourceAmounts = new Dictionary<string, float>();
        Dictionary<string, float> resourceDeviations = new Dictionary<string, float>();
        float sum = 0f;
        foreach (string resourceType in ResourceHandler.ALL_RESOURCES)
        {
            float amount = _resourceHandler.getResourceNormalized(resourceType);
            resourceAmounts[resourceType] = amount;
            sum += amount;
        }
        float mean = sum / ResourceHandler.ALL_RESOURCES.Length;
        float averageDeviation = 0f;
        foreach (string resourceType in resourceAmounts.Keys)
        {
            float amount = _resourceHandler.getResourceNormalized(resourceType);
            float difference = amount - mean;
            bool isPositive = difference >= 0;
            difference = isPositive ? difference + 1f : difference - 1f;
            float deviation = Mathf.Pow(difference, 2) - 1f;
            averageDeviation += deviation;
            if (!isPositive) deviation *= -1f;
            resourceDeviations[resourceType] = deviation;
        }
        averageDeviation /= resourceAmounts.Count;

        string addedResourceType = _jobToResource[addedJob];
        float addedResource = _resourceHandler.getResourceNormalized(addedResourceType);
        float addedDiviation = resourceDeviations[addedResourceType];
        AddReward(addedDiviation * -1f);

        string removedResourceType = _jobToResource[removedJob];
        float removedResource = _resourceHandler.getResourceNormalized(removedResourceType);
        float removedDiviation = resourceDeviations[removedResourceType];
        AddReward(removedDiviation);
    }

    private void _assignNewWorkerJob(string oldJobId, string newJobId)
    {
        List<NpcAgent> npcsWithOldJob = _npcsByJobs[oldJobId];
        if (npcsWithOldJob.Count == 0)
        {
            Debug.LogWarning("Trying to switch npc jobs, but there is no npc with the job that was requested to be changed.");
            return;
        }
        int randomIndex = Mathf.RoundToInt(UnityEngine.Random.value * (npcsWithOldJob.Count - 1));
        NpcAgent npc = npcsWithOldJob[randomIndex];
        _npcsByJobs[newJobId].Add(npc);
        npc.setJob(newJobId);
        npcsWithOldJob.RemoveAt(randomIndex);
    }
}
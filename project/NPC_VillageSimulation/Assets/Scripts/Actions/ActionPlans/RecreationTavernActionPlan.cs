using System;

public class RecreationTavernActionPlan : BaseActionPlan
{
    public const int ACTION_CONSUME_BEER = 4;

    private static string[] TAVERN_BONUS_JOBS = new string[]
    {
        JobHandler.JOB_BREWER, JobHandler.JOB_MINER, JobHandler.JOB_HUNTER, JobHandler.JOB_BLACKSMITH
    };

    public RecreationTavernActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.RECREATION_TAVERN, allActionsExecutedCallback, npc)
    {
        _money = 0.1f;
    }

    public override bool areRequirementsMet()
    {
        if (!_hasEnoughMoney(_money)) return false;
        if (!_hasEnoughResource(ResourceHandler.BEER, ACTION_CONSUME_BEER)) return false;
        if (!_hasFreeSpot(LocationTypes.TAVERN)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitRealSeconds(DEFAULT_ACTION_SECONDS * 2f, LocationTypes.TAVERN);
    }

    protected override float _getReward()
    {
        float reward = _getRecreationReward(RECREATION_REWARD_MULTIPLIER);
        reward = _calculateAttributeMultReward(NpcState.ATT_INTELLIGENCE, reward, true);
        reward = _calculateAttributeMultReward(NpcState.ATT_FITNESS, reward, true);
        return reward > 0 ? reward * 0.8f : reward;
    }

    protected override void _initRun()
    {
        base._initRun();
        _npc.ResourceHandler.useResource(ACTION_CONSUME_BEER, ResourceHandler.BEER);
    }
}

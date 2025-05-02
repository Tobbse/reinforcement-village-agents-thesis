using SwordGC.AI.Goap;

public class ButcherGoapAction : BaseWorkGoapAction
{
    public ButcherGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
    }

    protected override void _init()
    {
        _jobId = JobHandler.JOB_BUTCHER;
        _locationType = LocationTypes.BUTCHER;

        _usedResourceType = ResourceHandler.RAW_VENISON;
        _resourceNeededEffect = Effects.HAS_VENISON_FOR_WORK;

        _addedResourceType = ResourceHandler.COOKED_MEAT;
        _addedResourceAmount = WorkMeatButcherActionPlan.ACTION_CREATE_COOKED_MEAT;
        _resourceGainedEffect = Effects.HAS_MEAT_FOR_STORAGE;
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        float seconds = TimeController.gameSecondsFromRealHours(4f);
        _waitForIngameSeconds(seconds);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new ButcherGoapAction(this.agent, _npc);
    }
}
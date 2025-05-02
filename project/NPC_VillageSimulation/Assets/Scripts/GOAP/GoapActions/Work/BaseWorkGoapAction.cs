using SwordGC.AI.Goap;

public class BaseWorkGoapAction : BaseNpcGoapAction
{
    protected string _jobId;
    protected string _addedResourceType;
    protected int _addedResourceAmount;
    protected string _usedResourceType;
    protected string _resourceGainedEffect;
    protected string _resourceNeededEffect;

    public BaseWorkGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        goal = GoapGoal.Goals.WorkGoapGoal;

        if (_usedResourceType != null && _usedResourceType != "")
        {
            preconditions.Add(_resourceNeededEffect, true);
            effects.Add(_resourceNeededEffect, false);
        }
        preconditions.Add(_resourceGainedEffect, false);
        if (_resourceGainedEffect != Effects.HAS_TOOL_FOR_WORK) preconditions.Add(Effects.HAS_TOOL_FOR_WORK, true);

        effects.Add(_resourceGainedEffect, true);
        if (_resourceGainedEffect != Effects.HAS_TOOL_FOR_WORK) effects.Add(Effects.HAS_TOOL_FOR_WORK, false);
        cost = 50;
    }

    public override void Perform()
    {
        base.Perform();
        _npc.NpcAgentGoapData.addResource(_addedResourceType, _addedResourceAmount);
        if (_usedResourceType != null && _usedResourceType != "")
        {
            _npc.NpcAgentGoapData.removeResource(_usedResourceType);
        }
        _npc.NpcAgentGoapData.removeResource(ResourceHandler.TOOLS);
    }

    protected override bool CheckProceduralPreconditions(DataSet data)
    {
        if (_npc.NpcState.AgentInfo.JobId != _jobId) return false;
        return base.CheckProceduralPreconditions(data);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BaseWorkGoapAction(this.agent, _npc);
    }
}
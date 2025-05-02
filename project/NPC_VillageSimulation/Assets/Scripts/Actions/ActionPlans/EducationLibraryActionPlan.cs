using System;

public class EducationLibraryActionPlan : BaseActionPlan
{
    public EducationLibraryActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.EDUCATION_LIBRARY, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        if (_npcState.AgentInfo.Age <= 35) return false;
        if (!_hasFreeSpot(LocationTypes.LIBRARY)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitGameHours(2f, LocationTypes.LIBRARY);
    }

    protected override float _getReward()
    {
        
        return _getNeedReward(NpcState.NEED_EDUCATION, NEED_REWARD_MULTIPLIER);
    }

    protected override void _additionalFinishAction()
    {
        _npcState.satisfyNeedByType(NpcState.NEED_EDUCATION, DEFAULT_NEED_GAIN);
    }
}

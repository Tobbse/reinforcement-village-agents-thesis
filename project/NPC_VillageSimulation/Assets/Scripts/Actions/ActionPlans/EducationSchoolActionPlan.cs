using System;

// TODO - restrict age. maybe have college too?
// school until 18, college until 28, then library?
// education could be more critical when people are young?
// 
public class EducationSchoolActionPlan : BaseActionPlan
{
    public EducationSchoolActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.EDUCATION_SCHOOL, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        if (_npcState.AgentInfo.Age > 18) return false;
        if (!_hasFreeSpot(LocationTypes.SCHOOL)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitGameHours(2f, LocationTypes.SCHOOL);
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

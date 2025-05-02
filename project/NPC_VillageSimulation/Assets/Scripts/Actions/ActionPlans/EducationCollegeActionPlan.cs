using System;

// TODO - restrict age. maybe have college too?
// school until 18, college until 28, then library?
// education could be more critical when people are young?
// 
public class EducationCollegeActionPlan : BaseActionPlan
{
    public EducationCollegeActionPlan(Action allActionsExecutedCallback, NpcAgent npc)
        : base(NpcActions.EDUCATION_COLLEGE, allActionsExecutedCallback, npc) { }

    public override bool areRequirementsMet()
    {
        if (_npcState.AgentInfo.Age <= 18 || _npcState.AgentInfo.Age > 35) return false;
        if (!_hasFreeSpot(LocationTypes.COLLEGE)) return false;
        return true;
    }

    protected override void _addPlanActions()
    {
        _moveAndWaitGameHours(2f, LocationTypes.COLLEGE);
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

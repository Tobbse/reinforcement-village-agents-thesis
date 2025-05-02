using System.Collections;
using SwordGC.AI.Goap;

public class BaseGetFromStorageGoapAction : BaseNpcGoapAction
{
    protected string _effectType;
    protected string _resourceType;
    protected int _neededResourceAmount;

    public BaseGetFromStorageGoapAction(GoapAgent goapAgent, NpcAgent npc) : base(goapAgent, npc)
    {
        //preconditions.Add(_effectType, false);
        effects.Add(_effectType, true);

        cost = 50;
    }

    protected override bool CheckProceduralPreconditions(DataSet data)
    {
        if (_npc.ResourceHandler.getResource(_resourceType) < _neededResourceAmount) return false;
        return base.CheckProceduralPreconditions(data);
    }

    public override void Perform()
    {
        base.Perform();
        _isPerforming = true;
        _npc.ResourceHandler.useResource(_neededResourceAmount, _resourceType);
        _waitForIngameSeconds(1f);
    }

    protected override BaseNpcGoapAction _getDirectClone()
    {
        return new BaseGetFromStorageGoapAction(this.agent, _npc);
    }
}
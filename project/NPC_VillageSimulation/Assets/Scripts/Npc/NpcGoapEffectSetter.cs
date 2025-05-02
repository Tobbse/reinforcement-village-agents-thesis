using UnityEngine;
using SwordGC.AI.Goap;

public class NpcGoapEffectSetter : ScriptableObject
{
    private NpcGoapAgent _goapAgent;

    public NpcGoapEffectSetter(NpcGoapAgent goapAgent)
    {
        _goapAgent = goapAgent;
    }

    public void setGoapEffect(string needType, bool state)
    {
        switch (needType)
        {
            case GoapAction.Effects.IS_HUNGRY:
            case NpcState.NEED_HUNGER:
                _addGoapStateEffect(GoapAction.Effects.IS_HUNGRY, state);
                break;

            case GoapAction.Effects.IS_THIRSTY:
            case NpcState.NEED_THIRST:
                _addGoapStateEffect(GoapAction.Effects.IS_THIRSTY, state);
                break;

            case GoapAction.Effects.NEEDS_EDUCATION:
            case NpcState.NEED_EDUCATION:
                _addGoapStateEffect(GoapAction.Effects.NEEDS_EDUCATION, state);
                break;

            case GoapAction.Effects.NEEDS_COMMUNICATION:
            case NpcState.NEED_COMMUNICATION:
                _addGoapStateEffect(GoapAction.Effects.NEEDS_COMMUNICATION, state);
                break;

            case GoapAction.Effects.NEEDS_FAITH:
            case NpcState.NEED_FAITH:
                _addGoapStateEffect(GoapAction.Effects.NEEDS_FAITH, state);
                break;

            case GoapAction.Effects.NEEDS_SLEEP:
            case NpcState.SLEEP:
                _addGoapStateEffect(GoapAction.Effects.NEEDS_SLEEP, state);
                break;

            case GoapAction.Effects.NEEDS_WORK:
            case NpcState.WORK:
                _addGoapStateEffect(GoapAction.Effects.NEEDS_WORK, state);
                break;

            case GoapAction.Effects.NEEDS_EXHAUSTION:
                _addGoapStateEffect(GoapAction.Effects.NEEDS_EXHAUSTION, state);
                break;

            case GoapAction.Effects.NEEDS_RECREATION:
                _addGoapStateEffect(GoapAction.Effects.NEEDS_RECREATION, state);
                break;
        }
    }

    private void _addGoapStateEffect(string effect, bool state)
    {
        _goapAgent.dataSet.SetData(effect, state);
    }
}
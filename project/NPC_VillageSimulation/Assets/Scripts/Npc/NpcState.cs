using UnityEngine;
using SwordGC.AI.Goap;
using System.Collections.Generic;

public class NpcState
{
    public const string NEED_HUNGER = "NEED_HUNGER";
    public const string NEED_THIRST = "NEED_THIRST";
    public const string NEED_EDUCATION = "NEED_EDUCATION";
    public const string NEED_COMMUNICATION = "NEED_COMMUNICATION";
    public const string NEED_FAITH = "NEED_FAITH";

    public const string SLEEP = "SLEEP";
    public const string WORK = "WORK";
    public const string EXHAUSTION = "EXHAUSTION";
    public const string RECREATION = "RECREATION";

    public const string ATT_FITNESS = "ATT_FITNESS";
    public const string ATT_INTELLIGENCE = "ATT_INTELLIGENCE";
    public const string ATT_COMPANIONSHIP = "ATT_COMPANIONSHIP";
    public const string ATT_COMMODITIES = "ATT_COMMODITIES";
    public const string ATT_FEROCITY = "ATT_FEROCITY";

    private const float MAX_MONEY = 2.5f;

    public static string[] ALL_NEEDS = new string[] { NEED_HUNGER, NEED_THIRST, NEED_EDUCATION, NEED_COMMUNICATION, NEED_FAITH };

    private NeedCycleHandler _needCycleHandler;
    private Dictionary<string, float> _needs;
    private Dictionary<string, float> _attributes;
    private Dictionary<string, float> _secondsSinceSatisfaction;
    private float _exhaustion;
    private float _recreation;
    private float _sleep;
    private float _work;
    private float _money;
    private bool _shouldSleep;
    private bool _useGoap;
    private AgentInfo _agentInfo;
    private NpcGoapEffectSetter _goapEffectSetter;

    public Dictionary<string, float>.KeyCollection NeedKeys { get { return _needs.Keys; } }
    public Dictionary<string, float>.KeyCollection AttributeKeys { get { return _attributes.Keys; } }
    public bool ShouldSleep { get => _shouldSleep; set => _shouldSleep = value; }
    public NeedCycleHandler NeedCycleHandler { get => _needCycleHandler; }
    public AgentInfo AgentInfo { get => _agentInfo; }

    public float NeedHunger { get { return _needs[NEED_HUNGER]; } private set { _needs[NEED_HUNGER] = value; } }
    public float NeedThirst { get { return _needs[NEED_THIRST]; } private set { _needs[NEED_THIRST] = value; } }
    public float NeedEducation { get { return _needs[NEED_EDUCATION]; } private set { _needs[NEED_EDUCATION] = value; } }
    public float NeedCommunication { get { return _needs[NEED_COMMUNICATION]; } private set { _needs[NEED_COMMUNICATION] = value; } }
    public float NeedFaith { get { return _needs[NEED_FAITH]; } private set { _needs[NEED_FAITH] = value; } }
    public float AttFittness { get { return _attributes[ATT_FITNESS]; } private set { _attributes[ATT_FITNESS] = value; } }
    public float AttIntelligence { get { return _attributes[ATT_INTELLIGENCE]; } private set { _attributes[ATT_INTELLIGENCE] = value; } }
    public float AttCompanionship { get { return _attributes[ATT_COMPANIONSHIP]; } private set { _attributes[ATT_COMPANIONSHIP] = value; } }
    public float AttCommodities { get { return _attributes[ATT_COMMODITIES]; } private set { _attributes[ATT_COMMODITIES] = value; } }
    public float AttFerocity { get { return _attributes[ATT_FEROCITY]; } private set { _attributes[ATT_FEROCITY] = value; } }
    public float Recreation { get { return _recreation; } private set { _recreation = value; } }
    public float Exhaustion { get { return _exhaustion; } private set { _exhaustion = value; } }
    public float Sleep { get { return _sleep; } private set { _sleep = value; } }
    public float Work { get { return _work; } private set { _work = value; } }
    public float Money { get { return _money; } set { _money = value; } }

    public NpcState(AgentInfo agentInfo, NpcGoapAgent goapAgent)
    {
        _agentInfo = agentInfo;
        _needs = new Dictionary<string, float>();
        _attributes = new Dictionary<string, float>();
        _secondsSinceSatisfaction = new Dictionary<string, float>();
        _needCycleHandler = new NeedCycleHandler(_agentInfo);
        _useGoap = goapAgent != null;
        if (_useGoap) _goapEffectSetter = new NpcGoapEffectSetter(goapAgent);
    }

    public void satisfyNeedByType(string type, float value)
    {
        _needs[type] = Mathf.Min(1.0f, _needs[type] + value);
        _secondsSinceSatisfaction[type] = 0f;
        _recreation = _needCycleHandler.calculateRecreationValue(_needs);
    }

    public void updateExhaustion(float addValue) { Exhaustion = Mathf.Clamp(Exhaustion + addValue, 0f, 1f); }

    public float getAttributeByType(string type) { return _attributes[type]; }

    public float getNeedByType(string type) { return _needs[type]; }

    public void resetState(float currentTime)
    {
        _money = 0.3f;
        _needCycleHandler.RythmVariation = GameAcademy.RYTHM_VARIATION * Random.value;

        Exhaustion = 0.5f + Random.value * 0.5f;
        _needs = new Dictionary<string, float>();

        foreach (string needType in ALL_NEEDS) _needs[needType] = 0.3f + (Random.value * 0.7f);
        foreach (string needType in ALL_NEEDS) _secondsSinceSatisfaction[needType] = Random.value * 750f;
        if (_useGoap) {
            foreach (string effect in GoapAction.Effects.ALL_EFFECTS) _goapEffectSetter.setGoapEffect(effect, false);
        }
        _secondsSinceSatisfaction[SLEEP] = 0f;
        _secondsSinceSatisfaction[WORK] = 0f;

        updateState(currentTime, 0f);
        _attributes = AttributeHandler.getAttributesByJobId(_agentInfo.JobId);
    }

    public Dictionary<string, float> getNeedsClone()
    {
        Dictionary<string, float> clone = new Dictionary<string, float>();
        foreach (string needType in _needs.Keys) clone[needType] = _needs[needType];
        return clone;
    }


    public Dictionary<string, float> getAttributesClone()
    {
        Dictionary<string, float> clone = new Dictionary<string, float>();
        foreach (string attributeType in _attributes.Keys) clone[attributeType] = _attributes[attributeType];
        return clone;
    }

    // TODO figure out value for exhaustion. TODO also needs exhaustion when working?
    public void updateState(float currentTime, float deltaTime)
    {
        _exhaustion += 0.0002f * deltaTime;

        List<string> needsToUpdate = new List<string>();
        foreach (string needType in _needs.Keys) needsToUpdate.Add(needType);

        foreach (string needType in needsToUpdate)
        {
            _secondsSinceSatisfaction[needType] += deltaTime;
            _needs[needType] = _needCycleHandler.getUpdatedNeedValue(needType, _secondsSinceSatisfaction[needType]);
            if (_useGoap && _needs[needType] < 0.1f) _goapEffectSetter.setGoapEffect(needType, true);
        }
        _recreation = _needCycleHandler.calculateRecreationValue(_needs);
        _sleep = _needCycleHandler.calculateSleepValue(currentTime);
        _work = _needCycleHandler.calculateWorkValue(currentTime);

        if (!_useGoap) return;
        if (_sleep < 0.1f) _goapEffectSetter.setGoapEffect(SLEEP, true);
        if (_work < 0.1f) _goapEffectSetter.setGoapEffect(WORK, true);
        if (_recreation < 0.1f) _goapEffectSetter.setGoapEffect(GoapAction.Effects.NEEDS_RECREATION, true);
        if (_exhaustion < 0.1f) _goapEffectSetter.setGoapEffect(GoapAction.Effects.NEEDS_EXHAUSTION, true);
    }

    public void payMoney(float amount)
    {
        float money = _money - amount;
        if (money < 0f) Debug.LogWarning("Trying to pay more money then the agent has.");
        _money = Mathf.Clamp(money, 0f, MAX_MONEY);
    }

    public void gainMoney(float amount) { _money = Mathf.Clamp(_money + amount, 0f, MAX_MONEY); }

    public float getMoneyNormalized()
    {
        return _money / MAX_MONEY;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class NeedCycleHandler
{
    public const float HUNGER_CYCLE_SECONDS = TimeController.DAY_CYCLE_REAL_SECONDS * 0.5f;
    public const float THIRST_CYCLE_SECONDS = TimeController.DAY_CYCLE_REAL_SECONDS * 0.35f;
    public const float EDUCATION_CYCLE_SECONDS = TimeController.DAY_CYCLE_REAL_SECONDS * 1.9f;
    public const float COMMUNICATION_CYCLE_SECONDS = TimeController.DAY_CYCLE_REAL_SECONDS * 0.95f;
    public const float FAITH_CYCLE_SECONDS = TimeController.DAY_CYCLE_REAL_SECONDS * 3f;

    private const float HUNGER_DROP_RATE = 2.5f;
    private const float THIRST_DROP_RATE = 2.5f;
    private const float EDUCATION_DROP_RATE = 2.5f;
    private const float COMMUNICATION_DROP_RATE = 2.5f;
    private const float FAITH_DROP_RATE = 2.5f;

    private const float SLEEP_FUNCTION_HORIZONTAL_POS = -0.125f;
    private const float WOKR_SLEEP_OFFSET = 0.2f;

    private Dictionary<string, float> _cycleLenghts;
    private Dictionary<string, float> _cycleDropRates;
    private float _currentTime;
    private float _rythmVariation;
    private AgentInfo _agentInfo;

    public float RythmVariation { get => _rythmVariation; set => _rythmVariation = value; }

    public NeedCycleHandler(AgentInfo agentInfo)
    {
        _agentInfo = agentInfo;

        _cycleLenghts = new Dictionary<string, float>();
        _cycleLenghts[NpcState.NEED_HUNGER] = HUNGER_CYCLE_SECONDS;
        _cycleLenghts[NpcState.NEED_THIRST] = THIRST_CYCLE_SECONDS;
        _cycleLenghts[NpcState.NEED_EDUCATION] = EDUCATION_CYCLE_SECONDS;
        _cycleLenghts[NpcState.NEED_COMMUNICATION] = COMMUNICATION_CYCLE_SECONDS;
        _cycleLenghts[NpcState.NEED_FAITH] = FAITH_CYCLE_SECONDS;

        _cycleDropRates = new Dictionary<string, float>();
        _cycleDropRates[NpcState.NEED_HUNGER] = HUNGER_DROP_RATE;
        _cycleDropRates[NpcState.NEED_THIRST] = THIRST_DROP_RATE;
        _cycleDropRates[NpcState.NEED_EDUCATION] = EDUCATION_DROP_RATE;
        _cycleDropRates[NpcState.NEED_COMMUNICATION] = COMMUNICATION_DROP_RATE;
        _cycleDropRates[NpcState.NEED_FAITH] = FAITH_DROP_RATE;
    }

    public void setCycleLengthForNeed(string needType, float cycleLength)
    {
        _cycleLenghts[needType] = cycleLength;
    }

    public float getUpdatedNeedValue(string needType, float timeSinceSatisfaction)
    {
        return _getFunctionValue(_cycleLenghts[needType], _cycleDropRates[needType], timeSinceSatisfaction);
    }

    // TODO check if this recreation calculation makes sense.
    public float calculateRecreationValue(Dictionary<string, float> needs)
    {
        float recreationValue = 0f;
        foreach (string needType in needs.Keys)
        {
            float need = needs[needType];
            if (need < 0.2f) recreationValue += (1f - need) * 100;
            else if (need < 0.3f) recreationValue += (1f - need) * 10f;
            else if (need < 0.4f) recreationValue += (1f - need) * 3f;
            else recreationValue += 1f - need;
        }
        recreationValue = recreationValue / needs.Count / 1.5f;
        return Mathf.Clamp(recreationValue, 0f, 1f);
    }

    // Equivalent to function sin(4PI * (x - 0.125)) * 0.5 + 0.5, but clamped and partial.
    public float calculateSleepValue(float currentTime)
    {
        float baseOffsetX = SLEEP_FUNCTION_HORIZONTAL_POS - _rythmVariation;

        bool exceedsBorders = _exceedsBorders(0.125f - baseOffsetX, 0.625f - baseOffsetX, currentTime);
        if (!exceedsBorders) return 1f;

        float frequency = 4f * Mathf.PI;
        float offset = currentTime + baseOffsetX;
        float funcRes = Mathf.Sin(frequency * offset) * 0.5f + 0.5f;
        return Mathf.Clamp(funcRes, 0f, 1f);
    }

    // Equivalent to function sin(4PI * (x - 0.175)) * 0.5 + 0.5, but clamped and partial.
    public float calculateWorkValue(float currentTime)
    {
        // Agents do not work below this age, but go to school and college.
        //if (_agentInfo.Age < 35) return 1;

        float baseOffsetX = SLEEP_FUNCTION_HORIZONTAL_POS - (SleepActionPlan.SLEEP_DURATION_HOURS / 24f) - WOKR_SLEEP_OFFSET - _rythmVariation;

        bool exceedsBorders = _exceedsBorders(0.125f - baseOffsetX, 0.625f - baseOffsetX, currentTime);
        if (!exceedsBorders) return 1f;

        float frequency = 4f * Mathf.PI;
        float offset = currentTime + baseOffsetX;
        float funcRes = Mathf.Sin(frequency * offset) * 0.5f + 0.5f;

        return Mathf.Clamp(funcRes, 0f, 1f);
    }

    private bool _exceedsBorders(float minBorder, float maxBorder, float value)
    {
        if (minBorder < 0f)
        {
            float temp = minBorder;
            minBorder = minBorder + 1f;
        } else if (maxBorder > 1f)
        {
            maxBorder = maxBorder - 1f;
        } else
        {
            return value > maxBorder || value < minBorder;
        }

        return value > maxBorder && value < minBorder;
    }

    // Equivalent to function: (1 - ((x / cycleDuration)^n))
    private float _getFunctionValue(float cycleLengthSeconds, float exponent, float xValue)
    {
        float baseVal = xValue / cycleLengthSeconds;
        float pow = Mathf.Pow(baseVal, exponent);
        return Mathf.Clamp(1f - pow, 0f, 1f);
    }
}

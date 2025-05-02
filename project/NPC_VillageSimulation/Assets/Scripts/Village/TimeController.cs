using UnityEngine;
using System.Collections.Generic;
using MLAgents;

public class TimeController : MonoBehaviour
{
    public const float DAY_CYCLE_REAL_SECONDS = 1100;
    public const float SECONDS_PER_DAY = 24f * 60 * 60;
    public const float REAL_SECOND_INGAME_DURATION = SECONDS_PER_DAY / DAY_CYCLE_REAL_SECONDS;
    public const float ONE_SECOND_NORMALIZED = 1f / SECONDS_PER_DAY;
    public const float NEED_LOG_TIME_INTERVAL = 1f / 24f / 6f;

    private float _currentTime;
    private float _days;
    private int _daysPassed;
    private List<NpcAgent> _npcs;
    private ResourceHandler _resourceHandler;
    private LightHandler _lightHandler;
    private Village  _village;
    private float _timeSinceLastLog;

    private void Awake()
    {
        enabled = false;
    }

    public static float gameSecondsFromRealSeconds(float realSeconds)
    {
        return ONE_SECOND_NORMALIZED * DAY_CYCLE_REAL_SECONDS * realSeconds;
    }

    public static float gameSecondsFromRealMinutes(float realMinutes)
    {
        return gameSecondsFromRealSeconds(realMinutes) * 60;
    }

    public static float gameSecondsFromRealHours(float realHours)
    {
        return gameSecondsFromRealSeconds(realHours) * 3600;
    }

    public static float realSecondsFromGameSeconds(float gameSeconds)
    {
        return gameSeconds / REAL_SECOND_INGAME_DURATION;
    }

    public static float realSecondsFromGameMinutes(float gameMinutes)
    {
        return realSecondsFromGameSeconds(gameMinutes) * 60;
    }

    public static float realSecondsFromGameHours(float gameHours)
    {
        return realSecondsFromGameSeconds(gameHours) * 3600;
    }

    public void init(Village village)
    {
        _village = village;
        _resourceHandler = village.ResourceHandler;

        enabled = !(GameAcademy.GAME_MODE == GameAcademy.GAME_MODE_SINGLE_AGENT_TRAINING); // During Single Agent Training, time will be updated automagically after each agent action.
    }

    public void advanceTime(float deltaTime)
    {
        _setCurrentTime(deltaTime);
        _village.updateTime(deltaTime, _currentTime, _daysPassed, _timeSinceLastLog == 0);
    }

    private void Update()
    {
        advanceTime(Time.deltaTime);
    }

    private void _setCurrentTime(float deltaTime)
    {
        float timePassed = deltaTime * ONE_SECOND_NORMALIZED * REAL_SECOND_INGAME_DURATION;
        _currentTime += timePassed;
        _checkIfDaysPassed();

        _timeSinceLastLog += timePassed;
        if (_timeSinceLastLog >= NEED_LOG_TIME_INTERVAL) _timeSinceLastLog = 0f;
    }

    private void _checkIfDaysPassed()
    {
        _daysPassed = 0;
        while (_currentTime > 1f)
        {
            _days++;
            Monitor.Log("Days", _days.ToString());
            _currentTime -= 1f;
            _daysPassed += 1;
            _handleDayPassed();
        }
    }

    private void _handleDayPassed()
    {
        int useWood = GameAcademy.NUM_AGENTS_PER_VILLAGE;
        int currentWood = _resourceHandler.getResource(ResourceHandler.FIREWOOD);
        if (_resourceHandler.getResourceNormalized(ResourceHandler.FIREWOOD) > 0.5f)
        {
            for (int i = 0; i < GameAcademy.NUM_AGENTS_PER_VILLAGE; i++) if (Random.value > 0.5f) useWood += 1;
        }
        if (useWood > currentWood)
        {
            if (GameAcademy.VERBOSE) Debug.Log("Trying to use " + useWood.ToString() + " firewood with only " + currentWood.ToString() + " in storage.");
            useWood = currentWood;
        }
        _resourceHandler.useResource(useWood, ResourceHandler.FIREWOOD); // -1 For each agent each day.
    }
}

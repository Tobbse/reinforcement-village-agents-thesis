using UnityEngine;

public class MonitorHandler : MonoBehaviour
{
    private NpcAgent _npc;
    private NpcAgent[] _npcs;
    private ResourceHandler _resourceHandler;
    private bool _shouldMonitor;
    private WaitHandler _waitHandler;
    private MoveHandler _moveHandler;

    public bool ShouldMonitor { get => _shouldMonitor; }

    public void init(NpcAgent npc, WaitHandler waitHandler, MoveHandler MoveHandler)
    {
        _npc = npc;
        _waitHandler = waitHandler;
        _moveHandler = MoveHandler;
        _resourceHandler = _npc.ResourceHandler;

        _npcs = FindObjectsOfType<NpcAgent>();
        _setDefaultMonitorValues();
    }

    void Update()
    {
        if (_npc != null && _npc.ShouldMonitorNpc)
        {
            if (_shouldMonitor == false) _setDefaultMonitorValues();
            _shouldMonitor = true;

            GameMonitor.LogToPos("Agent:", _npc.AgentId + ", " + _npc.NpcState.AgentInfo.Name, GameMonitor.LOG_LOCATION_TOP_RIGHT);
            GameMonitor.LogToPos("Hunger/Thirst", new float[] { _npc.NpcState.NeedHunger, _npc.NpcState.NeedThirst }, GameMonitor.LOG_LOCATION_TOP_RIGHT);
            GameMonitor.LogToPos("Edu/Com/Fai", new float[] { _npc.NpcState.NeedEducation, _npc.NpcState.NeedCommunication, _npc.NpcState.NeedFaith }, GameMonitor.LOG_LOCATION_TOP_RIGHT);
            GameMonitor.LogToPos("Rec/Exh", new float[] { _npc.NpcState.Recreation, _npc.NpcState.Exhaustion }, GameMonitor.LOG_LOCATION_TOP_RIGHT);
            GameMonitor.LogToPos("Sle/Wor", new float[] { _npc.NpcState.Sleep, _npc.NpcState.Work }, GameMonitor.LOG_LOCATION_TOP_RIGHT);
            GameMonitor.LogToPos("Actions", Mathf.RoundToInt((float)_npc.NumActions / 1000f).ToString() + " k", GameMonitor.LOG_LOCATION_TOP_RIGHT);
            GameMonitor.LogToPos("Current Action", _npc.Action, GameMonitor.LOG_LOCATION_TOP_RIGHT);
            GameMonitor.LogToPos("Money", _npc.NpcState.Money, GameMonitor.LOG_LOCATION_TOP_RIGHT);
            GameMonitor.LogToPos("Moving", _moveHandler.IsMoving ? 1f : -1f, GameMonitor.LOG_LOCATION_TOP_RIGHT);
            GameMonitor.LogToPos("Waiting", _waitHandler.IsWaiting ? 1f : -1f, GameMonitor.LOG_LOCATION_TOP_RIGHT);
        } else if (_shouldMonitor)
        {
            _setDefaultMonitorValues();
            _shouldMonitor = false;
        }
    }

    private void _setDefaultMonitorValues()
    {
        GameMonitor.LogToPos("Agent:", "", GameMonitor.LOG_LOCATION_TOP_RIGHT);
        GameMonitor.LogToPos("Hunger/Thirst", new float[] { }, GameMonitor.LOG_LOCATION_TOP_RIGHT);
        GameMonitor.LogToPos("Edu/Com/Fai", new float[] { }, GameMonitor.LOG_LOCATION_TOP_RIGHT);
        GameMonitor.LogToPos("Rec/Exh", new float[] { }, GameMonitor.LOG_LOCATION_TOP_RIGHT);
        GameMonitor.LogToPos("Sle/Wor", new float[] { }, GameMonitor.LOG_LOCATION_TOP_RIGHT);
        GameMonitor.LogToPos("Actions", "", GameMonitor.LOG_LOCATION_TOP_RIGHT);
        GameMonitor.LogToPos("Current Action", "", GameMonitor.LOG_LOCATION_TOP_RIGHT);
        GameMonitor.LogToPos("Money", 0f, GameMonitor.LOG_LOCATION_TOP_RIGHT);
        GameMonitor.LogToPos("Last Reward", "", GameMonitor.LOG_LOCATION_TOP_RIGHT);
        GameMonitor.LogToPos("Moving", "", GameMonitor.LOG_LOCATION_TOP_RIGHT);
        GameMonitor.LogToPos("Waiting", "", GameMonitor.LOG_LOCATION_TOP_RIGHT);

        foreach (NpcAgent npc in _npcs)
        {
            if (npc.AgentId != _npc.AgentId) npc.shouldMonitorNpc = false;
        }
    }
}

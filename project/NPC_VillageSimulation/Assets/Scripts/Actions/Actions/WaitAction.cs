using System;
using UnityEngine;
using System.Collections;

public class WaitAction : BaseAction
{
    private float _seconds;
    private Action _callback;

    public WaitAction(string actionType, NpcAgent npc, float seconds)
        : base(actionType, npc)
    {
        _seconds = seconds;
    }

    public override IEnumerator execute(Action callback, Action cancelCallback)
    {
        _callback = callback;
        _npc.VillageInfo.changeAmountWaiting(true);

        switch (GameAcademy.GAME_MODE)
        {
            case GameAcademy.GAME_MODE_REGULAR_INFERENCE:
                _handleInference();
                break;

            case GameAcademy.GAME_MODE_SINGLE_AGENT_TRAINING:
                _handleSingleAgentTraining();
                break;

            case GameAcademy.GAME_MODE_MULTI_AGENT_TRAINING:
            case GameAcademy.GAME_MODE_FAKE_INFERENCE:
                _handleFakeInference();
                break;
            default:
                Debug.LogError("Unknown Game Mode " + GameAcademy.GAME_MODE.ToString() + ".");
                cancelCallback();
                break;
        }
        yield return null;
    }

    private void _waitingFinishedCallback()
    {
        _npc.VillageInfo.changeAmountWaiting(false);
        _callback();
    }

    // Waits for a specific amount of time and updates the needs by using the Mono update function.
    private void _handleInference()
    {
        _npc.WaitHandler.wait(_seconds, _waitingFinishedCallback);
    }

    // Instead of waiting, the need decrease is updated right away.
    private void _handleSingleAgentTraining()
    {
        _timeController.advanceTime(_seconds);
        _waitingFinishedCallback();
    }

    // The waiting process is not skipped, but accelerated.
    private void _handleFakeInference()
    {
        _npc.WaitHandler.wait(_seconds, _waitingFinishedCallback);
    }
}

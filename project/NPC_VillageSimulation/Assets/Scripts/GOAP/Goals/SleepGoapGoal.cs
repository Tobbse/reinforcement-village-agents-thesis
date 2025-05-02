using SwordGC.AI.Goap;

public class SleepGoapGoal : GoapGoal
{
    public SleepGoapGoal(string key, float multiplier = 1) : base(key, multiplier)
    {
    }

    public override void UpdateMultiplier(DataSet data)
    {
        // Fancy function that lowers the multiplier if this happens often.
    }
}
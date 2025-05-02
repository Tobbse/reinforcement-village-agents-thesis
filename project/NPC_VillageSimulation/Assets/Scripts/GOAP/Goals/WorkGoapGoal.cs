using SwordGC.AI.Goap;

public class WorkGoapGoal : GoapGoal
{
    public WorkGoapGoal(string key, float multiplier = 1.5f) : base(key, multiplier)
    {
    }

    public override void UpdateMultiplier(DataSet data)
    {
        // Fancy function that lowers the multiplier if this happens often.
    }
}
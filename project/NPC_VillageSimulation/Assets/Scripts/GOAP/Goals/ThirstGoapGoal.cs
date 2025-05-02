using SwordGC.AI.Goap;

public class ThirstGoapGoal : GoapGoal
{
    public ThirstGoapGoal(string key, float multiplier = 1.9f) : base(key, multiplier)
    {
    }

    public override void UpdateMultiplier(DataSet data)
    {
        // Fancy function that lowers the multiplier if this happens often.
    }
}
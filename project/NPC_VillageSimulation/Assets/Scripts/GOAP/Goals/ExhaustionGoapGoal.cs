using SwordGC.AI.Goap;

public class ExhaustionGoapGoal : GoapGoal
{
    public ExhaustionGoapGoal(string key, float multiplier = 5f) : base(key, multiplier)
    {
    }

    public override void UpdateMultiplier(DataSet data)
    {
        // Fancy function that lowers the multiplier if this happens often.
    }
}
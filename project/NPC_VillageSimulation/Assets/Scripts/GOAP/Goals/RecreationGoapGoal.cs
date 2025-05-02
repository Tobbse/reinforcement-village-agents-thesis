using SwordGC.AI.Goap;

public class RecreationGoapGoal : GoapGoal
{
    public RecreationGoapGoal(string key, float multiplier = 6f) : base(key, multiplier)
    {
    }

    public override void UpdateMultiplier(DataSet data)
    {
        // Fancy function that lowers the multiplier if this happens often.
    }
}
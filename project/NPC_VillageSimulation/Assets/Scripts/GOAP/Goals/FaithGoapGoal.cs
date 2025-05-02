using SwordGC.AI.Goap;

public class FaithGoapGoal : GoapGoal
{
    public FaithGoapGoal(string key, float multiplier = 4f) : base(key, multiplier)
    {
    }

    public override void UpdateMultiplier(DataSet data)
    {
        // Fancy function that lowers the multiplier if this happens often.
    }
}
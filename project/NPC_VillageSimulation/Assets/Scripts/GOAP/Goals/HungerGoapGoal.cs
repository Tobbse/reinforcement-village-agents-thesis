using SwordGC.AI.Goap;

public class HungerGoapGoal : GoapGoal
{
    public HungerGoapGoal(string key, float multiplier = 2f) : base(key, multiplier)
    {
    }

    public override void UpdateMultiplier(DataSet data)
    {
        // Fancy function that lowers the multiplier if this happens often.
    }
}
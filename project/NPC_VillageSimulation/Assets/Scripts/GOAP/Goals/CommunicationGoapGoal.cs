using SwordGC.AI.Goap;

public class CommunicationGoapGoal : GoapGoal
{
    public CommunicationGoapGoal(string key, float multiplier = 3f) : base(key, multiplier)
    {
    }

    public override void UpdateMultiplier(DataSet data)
    {
        // Fancy function that lowers the multiplier if this happens often.
    }
}
using System.Collections.Generic;

public class NpcAgentGoapData
{
    private Dictionary<string, int> _carrying;

    public NpcAgentGoapData()
    {
        _carrying = new Dictionary<string, int>();
    }

    public Dictionary<string, int> Carrying { get => _carrying; }

    public void addResource(string resourceType, int amount)
    {
        if (_carrying.ContainsKey(resourceType))
        {
            _carrying[resourceType] += amount;
        } else
        {
            _carrying[resourceType] = amount;
        }
    }
    public int removeResource(string resourceType)
    {
        if (!_carrying.ContainsKey(resourceType))
        {
            return 0;
        } else
        {
            int amount = _carrying[resourceType];
            _carrying.Remove(resourceType);
            return amount;
        }
    }

    public int useResource(string resourceType, int amount)
    {
        if (_carrying.ContainsKey(resourceType))
        {
            int carriedAmount = _carrying[resourceType];
            int available = carriedAmount - amount;
            if (available <= 0)
            {
                _carrying.Remove(resourceType);
                return amount + available;
            } else
            {
                return amount;
            }
        } else
        {
            return 0;
        }
    }
}
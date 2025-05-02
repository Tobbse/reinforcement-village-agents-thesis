using UnityEngine;

public class BuildingHome : Building
{
    private int _homeId = -1;

    public int HomeId { get => _homeId; }

    protected override void Awake()
    {
        char[] nameChars = name.ToCharArray();
        string number = "";
        for (int i = nameChars.Length - 1; i >= 0; i--)
        {
            char ch = nameChars[i];
            if (char.IsDigit(ch)) number = ch + number;
            else break;
        }
        if (number == "") Debug.LogError("Could not get home id for home with name " + name);
        else _homeId = int.Parse(number);

        base.Awake();
    }
}

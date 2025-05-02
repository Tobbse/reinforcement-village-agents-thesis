using System.Collections.Generic;
using SwordGC.AI.Goap;

public class VillageGoapActionAdder
{
    public List<GoapAction> _goapActions;

    public List<GoapAction> GoapActions { get { return _goapActions; } }

    public System.Collections.IEnumerator createGoapActions(NpcAgent npc, NpcGoapAgent goapAgent, VillageInfo villageInfo)
    {
        _goapActions = new List<GoapAction>();

        _goapActions.Add(new CommunicationCafeGoapAction(goapAgent, npc));
        _goapActions.Add(new CommunicationChatGoapAction(goapAgent, npc));
        _goapActions.Add(new CommunicationCafeGoapAction(goapAgent, npc));
        _goapActions.Add(new EducationLibraryGoapAction(goapAgent, npc));
        _goapActions.Add(new EducationSchoolGoapAction(goapAgent, npc));
        _goapActions.Add(new ExhaustionRestGoapAction(goapAgent, npc));
        _goapActions.Add(new HungerBreadGoapAction(goapAgent, npc));
        _goapActions.Add(new HungerMeatGoapAction(goapAgent, npc));
        _goapActions.Add(new RecreationSoccerGoapAction(goapAgent, npc));
        _goapActions.Add(new RecreationTavernGoapAction(goapAgent, npc));
        _goapActions.Add(new RecreationVisitGoapAction(goapAgent, npc));
        _goapActions.Add(new ReligionChurchGoapAction(goapAgent, npc));
        _goapActions.Add(new SleepGoapAction(goapAgent, npc));
        _goapActions.Add(new ThirstGoapAction(goapAgent, npc));
        _goapActions.Add(new BringBeerToStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new BringBreadToStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new BringFirewoodToStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new BringFlourToStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new BringGrainToStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new BringHopsToStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new BringLogsToStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new BringMeatToStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new BringOreToStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new BringToolsToStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new BringVenisonToStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new GetBeerFromStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new GetBreadFromStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new GetFirewoodFromStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new GetFlourFromStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new GetGrainFromStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new GetHopsFromStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new GetLogsFromStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new GetMeatFromStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new GetOreFromStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new GetToolsFromStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new GetVenisonFromStorageGoapAction(goapAgent, npc));
        _goapActions.Add(new BakeryGoapAction(goapAgent, npc));
        _goapActions.Add(new BlacksmithGoapAction(goapAgent, npc));
        _goapActions.Add(new BreweryGoapAction(goapAgent, npc));
        _goapActions.Add(new ButcherGoapAction(goapAgent, npc));
        _goapActions.Add(new GrainFarmingGoapAction(goapAgent, npc));
        _goapActions.Add(new HopsFarmingGoapAction(goapAgent, npc));
        _goapActions.Add(new HuntingGoapAction(goapAgent, npc));
        _goapActions.Add(new MiningGoapAction(goapAgent, npc));
        _goapActions.Add(new WindmillGoapAction(goapAgent, npc));
        _goapActions.Add(new CarpenterGoapAction(goapAgent, npc));
        _goapActions.Add(new LoggingGoapAction(goapAgent, npc));

        foreach (var goapAction in _goapActions)
        {
            goapAction.SetTarget(npc.gameObject);
            npc.StartCoroutine((goapAction as BaseNpcGoapAction).addTarget());
        }

        List<string> goapActionNames = new List<string>();
        foreach (GoapAction act in _goapActions)
        {
            string typeStr = act.GetType().ToString();
            villageInfo.changeAmountForActionToZero(typeStr);
            goapActionNames.Add(typeStr);
        }
        BaseNpcGoapAction.ALL_GOAP_ACTIONS = goapActionNames;

        yield return null;
    }
}

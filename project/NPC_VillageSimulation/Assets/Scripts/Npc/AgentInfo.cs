public class AgentInfo
{
    public const float MAX_AGE = 80f;

    public static string[] NPC_NAMES_MALE = new string[]
    {
        "Andreas", "Armin", "Bernd", "Boris", "Charlie", "Emil", "Eduard", "Friedrich", "Frank", "Finn", "Gerrit", "Gillermo", "Giovanni", "Herbert", "Hans", "Heinrich", "Jan", "Josef", "Julian",
        "Karl", "Lars", "Martin", "Maxi", "Nils", "Niklas", "Obelix", "Peter", "Quiesel", "Ralf", "Simon", "Tobi", "Tim", "Timo", "Ulrich", "Uwe", "Walter", "Winfried", "Xaver"
    };

    public static string[] NPC_NAMES_FEMALE = new string[]
    {
        "Alice", "Annika", "Anna", "Birgit", "Beate", "Beatrice", "Carolin", "Clara", "Christine", "Doris", "Dorothea", "Ellie", "Elsa", "Franziska", "Gertrude", "Gisela", "Hannah", "Hildegard",
        "Johanna", "Julia", "Jessica", "Jana", "Jaqueline", "Kerstin", "Lea", "Mandy", "Nala", "Natalie", "Nadine", "Petra", "Rabea", "Silvia", "Sandra", "Sofia", "Sara", "Tina", "Ulrike", "Waltraud"
    };

    private int _age;
    private bool _isMale;
    private string _jobId;
    private string _name;
    private int _agentId;

    public int Age { get => _age; set => _age = value; }
    public bool IsMale { get => _isMale; set => _isMale = value; }
    public string JobId { get => _jobId; set => _jobId = value; }
    public string Name { get => _name; set => _name = value; }
    public int AgentId { get => _agentId; set => _agentId = value; }

    public AgentInfo(int agentId, int age, bool isMale, string jobId, string name)
    {
        _agentId = agentId;
        _age = age;
        _isMale = isMale;
        _jobId = jobId;
        _name = name;
    }

    public float getAgeNormalized()
    {
        return _age / MAX_AGE;
    }
}

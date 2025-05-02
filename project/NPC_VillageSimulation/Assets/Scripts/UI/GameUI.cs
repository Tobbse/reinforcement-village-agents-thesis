using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TMP_Dropdown cameraSelect;

    private GameObject[] _roofs;
    private GameObject[] _trees;
    private GameObject[] _stones;
    private GameObject[] _fences;
    private List<Light> _agentLights;
    private List<Light> _cityLights;
    private bool _roofsEnabled = true;
    private bool _treesEnabled = true;
    private bool _stonesEnabled = true;
    private bool _fencesEnabled = true;
    private bool _cityLightsEnabled = true;
    private bool _agentLightsEnabled = true;
    private bool _agentsLoaded;
    private NpcAgent[] _npcs;
    private Camera[] _cameras;
    private OrbitCameraBehaviour _orbitCameraBehavior;

    // Start is called before the first frame update
    void Start()
    {
        enabled = false;

        _roofs = GameObject.FindGameObjectsWithTag(GameTags.TAG_ROOF);
        _trees = GameObject.FindGameObjectsWithTag(GameTags.TAG_TREE);
        _stones = GameObject.FindGameObjectsWithTag(GameTags.TAG_STONE);
        _fences = GameObject.FindGameObjectsWithTag(GameTags.TAG_FENCE);
        _orbitCameraBehavior = GameObject.FindObjectOfType<OrbitCameraBehaviour>();

        Light[] lights = GameObject.FindObjectsOfType<Light>();
        _cityLights = new List<Light>();
        foreach (Light light in lights) if (!light.name.Contains("Agent") && !light.name.Contains("Sun")) _cityLights.Add(light);

        // Disable mesh and lights for performance improvements when training.
        GameAcademy academy = GameObject.FindObjectOfType<GameAcademy>();
        if (!academy.GetIsInference())
        {
            toggleCityLights();
            toggleAgentLights();
            toggleTrees();
            toggleFences();
            toggleRoofs();
            toggleStones();
        }

        _cameras = FindObjectsOfType<Camera>();
        _setActiveCamera(0);

        for (int i = 0; i < _cameras.Length; i++) {
            cameraSelect.AddOptions(new List<Sprite>(new Sprite[] { null } ));
            cameraSelect.options[i].text = _cameras[i].gameObject.name;
        }
    }

    private void _setActiveCamera(int index)
    {
        for (int i = 0; i < _cameras.Length; i++)
        {
            bool isCorrectCamera = i == index;
            _cameras[i].gameObject.SetActive(isCorrectCamera);
            if (isCorrectCamera) _orbitCameraBehavior.setActiveCamera(_cameras[i]);
        }
    }

    public void initAgents()
    {
        _npcs = GameObject.FindObjectsOfType<NpcAgent>();
    }

    public void toggleCamera()
    {
        _setActiveCamera(cameraSelect.value);
    }

    public void toggleRoofs()
    {
        _roofsEnabled = !_roofsEnabled;
        foreach (GameObject roof in _roofs) roof.SetActive(_roofsEnabled);
    }

    public void toggleTrees()
    {
        _treesEnabled = !_treesEnabled;
        foreach (GameObject tree in _trees) tree.SetActive(_treesEnabled);
    }

    public void toggleStones()
    {
        _stonesEnabled = !_stonesEnabled;
        foreach (GameObject stone in _stones) stone.SetActive(_stonesEnabled);
    }

    public void toggleFences()
    {
        _fencesEnabled = !_fencesEnabled;
        foreach (GameObject fence in _fences) fence.SetActive(_fencesEnabled);
    }

    public void toggleCityLights()
    {
        _cityLightsEnabled = !_cityLightsEnabled;
        foreach (Light cityLight in _cityLights) cityLight.gameObject.SetActive(_cityLightsEnabled);
    }

    public void toggleAgentLights()
    {
        if (_npcs == null) return;
        if (!_agentsLoaded || _agentLights == null)
        {
            _agentsLoaded = true;
            _agentLights = new List<Light>();
            foreach (NpcAgent agent in _npcs) _agentLights.Add(agent.GetComponentInChildren<Light>()); 
        }
        _agentLightsEnabled = !_agentLightsEnabled;
        foreach (Light agentLight in _agentLights) agentLight.gameObject.SetActive(_agentLightsEnabled);
    }
}

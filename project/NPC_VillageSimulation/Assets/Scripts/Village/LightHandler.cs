using UnityEngine;
using System.Collections.Generic;

public class LightHandler : ScriptableObject
{
    private Light _sun;
    private List<Light> _bonfires;
    private List<Transform> _bonfireParticleTransforms;
    private List<Light> _villageLamps;
    private List<Light> _agentLights;

    public void init (Light[] villageLights)
    {
        _initLights(villageLights);
        if (GameAcademy.IS_TRAINING) _disableLights();
    }

    public void updateLights(float currentTime)
    {
        _handleSun(currentTime);

        if (GameAcademy.IS_TRAINING) return;

        float lampIntensity = _sun.intensity < 1f ? 1.7f : 0f;
        foreach (Light light in _villageLamps)
        {
            if (light == null || !light.isActiveAndEnabled) break;
            if (!Mathf.Approximately(light.intensity,lampIntensity)) light.intensity = lampIntensity;
        }
        float baseBonfireIntensity = (1.5f - _sun.intensity) * 10f;
        foreach (Light bonfire in _bonfires)
        {
            float bonfireScaledIntensity = baseBonfireIntensity * bonfire.transform.localScale.x;
            if (bonfire == null || !bonfire.isActiveAndEnabled) break;
            if (!Mathf.Approximately(bonfire.intensity, bonfireScaledIntensity)) bonfire.intensity = bonfireScaledIntensity;
        }
        float agentLightIntensity = (1.5f - _sun.intensity) * 10f / 1.5f;
        foreach (Light agentLight in _agentLights)
        {
            if (agentLight == null || !agentLight.isActiveAndEnabled) break;
            if (!Mathf.Approximately(agentLight.intensity, agentLightIntensity)) agentLight.intensity = agentLightIntensity;
        }
        foreach (Transform bonfireTransform in _bonfireParticleTransforms)
        {
            if (bonfireTransform == null) return;
            Vector3 particlesPos = bonfireTransform.localPosition;
            bonfireTransform.localPosition = new Vector3(particlesPos.x, -0.1f - (_sun.intensity / 1.5f), particlesPos.z);
        }
    }

    private void _handleSun(float currentTime)
    {
        _sun.transform.localRotation = Quaternion.Euler((currentTime * 360f) - 90f, 170f, 0f);

        float multiplier = 1f;
        if (currentTime <= 0.28f)
        {
            multiplier = Mathf.Clamp01((currentTime - 0.25f) * (1f / 0.03f));
        }
        else if (currentTime >= 0.75f)
        {
            multiplier = Mathf.Clamp01(1f - ((currentTime - 0.78f) * (1f / 0.03f)));
        }
        _sun.intensity = multiplier * 1.5f;
    }

    private void _initLights(Light[] villageLights)
    {
        _sun = GameObject.Find("Sun").GetComponent<Light>();

        _villageLamps = new List<Light>();
        _bonfires = new List<Light>();
        _agentLights = new List<Light>();
        _bonfireParticleTransforms = new List<Transform>();

        foreach (Light light in villageLights)
        {
            if (light.name.Contains("Lamp")) _villageLamps.Add(light);
            if (light.name.Contains("Bonfire"))
            {
                _bonfires.Add(light);
                _bonfireParticleTransforms.Add(light.gameObject.transform.parent);
            }
            if (light.name.Contains("Agent")) _agentLights.Add(light);
        }
    }

    // Lights are disabled during training to increase performance.
    private void _disableLights()
    {
        foreach (Light agentLight in _agentLights) agentLight.enabled = false;
        foreach (Light lamp in _villageLamps) lamp.enabled = false;
        foreach (Light bonfire in _bonfires) bonfire.enabled = false;
        foreach (Transform bonfireTransform in _bonfireParticleTransforms)
        {
            ParticleSystem particles = bonfireTransform.gameObject.GetComponent<ParticleSystem>();
            if (particles != null) particles.Stop();
        }
    }
}

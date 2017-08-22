using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InteriorLightsController : NetworkBehaviour
{
    public bool isRedAlert = false;

    private LightmapData[] normalLightMaps;
    private LightmapData[] redAlertLightMaps;
    public Texture2D[] redAlertMapTex;

    public List<GameObject> lights;
    public Color redLightColour;
    private Color normalLightColour;
    public Material redLightMat;
    public Material normalLightMat;

    public List<GameObject> ribbonLights;
    public Material redRibbonMat;
    public Material blueRibbonMat;

    public List<ReflectionProbe> reflectionProbes;

    public List<GameObject> affectedUiItems;
    public Color normalUiColour;
    public Color redUiColour;

    private void Start()
    {
        normalLightMaps = LightmapSettings.lightmaps;

        redAlertLightMaps = new LightmapData[redAlertMapTex.Length];

        for (int i = 0; i < redAlertMapTex.Length; i++)
        {
            redAlertLightMaps[i] = new LightmapData();
            redAlertLightMaps[i].lightmapColor = redAlertMapTex[i];
        }

        normalLightColour = lights[0].GetComponent<Light>().color;
    }

    [Command]
    public void CmdRedAlert()
    {
        RpcRedAlert();
    }
    [ClientRpc]
    private void RpcRedAlert()
    {
        LightmapSettings.lightmaps = redAlertLightMaps;

        foreach(GameObject light in lights)
        {
            light.GetComponent<Light>().color = redLightColour;
            light.GetComponent<Renderer>().material = redLightMat;
        }
        foreach (GameObject light in ribbonLights)
        {
            light.GetComponent<Renderer>().material = redRibbonMat;
        }
        foreach(ReflectionProbe probe in reflectionProbes)
        {
            probe.RenderProbe();
        }
        foreach(GameObject ui in affectedUiItems)
        {
            if (ui.GetComponent<Image>())
            {
                ui.GetComponent<Image>().color = redUiColour;
            }
        }

        isRedAlert = true;
    }

    [Command]
    public void CmdNoAlert()
    {
        RpcNoAlert();
    }
    [ClientRpc]
    private void RpcNoAlert()
    {
        LightmapSettings.lightmaps = normalLightMaps;

        foreach (GameObject light in lights)
        {
            light.GetComponent<Light>().color = normalLightColour;
            light.GetComponent<Renderer>().material = normalLightMat;
        }
        foreach (GameObject light in ribbonLights)
        {
            light.GetComponent<Renderer>().material = blueRibbonMat;
        }
        foreach (ReflectionProbe probe in reflectionProbes)
        {
            probe.RenderProbe();
        }
        foreach (GameObject ui in affectedUiItems)
        {
            if (ui.GetComponent<Image>())
            {
                ui.GetComponent<Image>().color = normalUiColour;
            }
        }

        isRedAlert = false;
    }

    [Command]
    public void CmdFlipAlert()
    {
        if (!isRedAlert)
        {
            RpcRedAlert();
        }
        else
        {
            RpcNoAlert();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdFlipAlert();
        }
    }
}
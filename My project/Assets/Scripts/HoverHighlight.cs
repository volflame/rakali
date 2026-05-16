using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class HoverHighlight : MonoBehaviour
{
    private Renderer _renderer;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    private GameObject descTooltip;

    void Awake()  
    {
        // _renderer = GetComponent<Renderer>();
        // _renderer.material.EnableKeyword("_EMISSION");
        // // _renderer.material.SetColor("_EmissionColor", Color.yellow * 2f);
    }

    public void OnHoverEnter(GameObject tooltip)
    {
        //  _renderer.material.SetColor(EmissionColor, Color.yellow * 0.5f)
        if (descTooltip == null)
        {
            descTooltip = Instantiate(tooltip, transform.position, Quaternion.identity);    //  + Vector3.right * 0.1f
        }
        else
        {
            descTooltip.SetActive(true);
        }
    }
    public void OnHoverExit()
    {
        // _renderer.material.SetColor(EmissionColor, Color.black);  
        descTooltip?.SetActive(false);
    }
}

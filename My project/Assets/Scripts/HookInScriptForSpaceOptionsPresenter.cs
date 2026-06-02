using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class HookInScriptForOptionsPresenter : MonoBehaviour
{
    // Drag the same CanvasGroup here that OptionsPresenter uses,
    // so we only act when options are actually visible
    [SerializeField] CanvasGroup? optionsCanvasGroup;

    private void Update()
    {
        // Only act when the options UI is visible and interactive
        if (optionsCanvasGroup == null) return;
        if (!optionsCanvasGroup.interactable) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            // Find all active OptionItems in the scene and select the highlighted one
            var optionItems = GetComponentsInChildren<OptionItem>(includeInactive: false);
            foreach (var item in optionItems)
            {
                if (item.IsHighlighted)
                {
                    item.InvokeOptionSelected();
                    break;
                }
            }
        }
    }
}
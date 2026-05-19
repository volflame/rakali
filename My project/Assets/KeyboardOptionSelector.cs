using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Yarn.Unity;

public class KeyboardOptionSelector : MonoBehaviour {
    [SerializeField] GameObject optionsListView;

    void Update() {
        // Find all active option buttons each frame
        List<Button> buttons = GetActiveOptionButtons();
        if (buttons.Count == 0) return;

        // If nothing is selected yet, auto-select the first button
        if (EventSystem.current.currentSelectedGameObject == null) {
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        }
    }

    List<Button> GetActiveOptionButtons() {
        List<Button> active = new List<Button>();
        foreach (Button b in optionsListView.GetComponentsInChildren<Button>()) {
            if (b.gameObject.activeInHierarchy) active.Add(b);
        }
        return active;
    }
}
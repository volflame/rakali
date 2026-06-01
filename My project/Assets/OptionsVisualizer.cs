using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsVisualizer : MonoBehaviour
{
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private float fontSize = 24f;
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color selectedColor = Color.yellow;
    [SerializeField] private float textWidth = 300f;
    [SerializeField] private float textHeight = 60f;

    void Update()
    {
        TextMeshProUGUI[] options = GetComponentsInChildren<TextMeshProUGUI>();

        if (options.Length == 0) return;

        // Force select first option if nothing is selected
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            Selectable firstSelectable = options[0].GetComponentInParent<Selectable>();
            if (firstSelectable != null)
                EventSystem.current.SetSelectedGameObject(firstSelectable.gameObject);
        }

        foreach (TextMeshProUGUI option in options)
        {
            if (font != null)
                option.font = font;

            option.fontSize = fontSize;
            option.alignment = TextAlignmentOptions.Center;

            option.enableWordWrapping = true;
            option.overflowMode = TextOverflowModes.Truncate;

            // Highlight selected option
            bool isSelected = option.GetComponentInParent<Selectable>()?.gameObject 
                == EventSystem.current.currentSelectedGameObject;
            option.color = isSelected ? selectedColor : normalColor;

            RectTransform rect = option.GetComponent<RectTransform>();
            if (rect != null)
                rect.sizeDelta = new Vector2(textWidth, textHeight);

            Selectable selectable = option.GetComponentInParent<Selectable>();
            if (selectable != null)
            {
                Navigation nav = selectable.navigation;
                nav.mode = Navigation.Mode.Automatic;
                selectable.navigation = nav;
            }
        }
    }
}
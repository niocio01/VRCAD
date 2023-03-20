using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RadioButtonController : MonoBehaviour
{
    [System.Serializable]
    public class SelectionChanged : UnityEvent<string>{ }

    [SerializeField]
    public SelectionChanged onSelectionChanged;

    [SerializeField] Color DeselectedColor = Color.white;
    [SerializeField] Color HoverColor = Color.white;
    [SerializeField] Color SelectedColor = Color.white;
    [SerializeField] RadioButton DefaultButton = null;

    [SerializeField]
    private RadioButton[] RadioButtons;

    private Button[] Buttons;
    private RadioButton CurrentlySelected = null;

    private void Awake()
    {
        List<Button> buttons = new List<Button>();

        foreach (RadioButton RadioButton in RadioButtons)
        {
            buttons.Add(RadioButton.GetComponent<Button>());
        }
        Buttons = buttons.ToArray();

        CurrentlySelected = DefaultButton;
        CurrentlySelected.BackgroundImage.color = SelectedColor;
    }

    private void OnEnable()
    {
        foreach(Button button in Buttons)
        {
            button.onClick.AddListener(delegate { HandleButtonPressed(button); });
        }
    }

    private void OnDisable()
    {
        foreach (Button button in Buttons)
        {
            button.onClick.RemoveListener(delegate { HandleButtonPressed(button); });
        }
    }

    void HandleButtonPressed(Button button)
    {
        CurrentlySelected.BackgroundImage.color = DeselectedColor;

        CurrentlySelected = button.GetComponent<RadioButton>();
        CurrentlySelected.BackgroundImage.color = SelectedColor;

        string Name = button.name;

        onSelectionChanged?.Invoke(button.name);        
    }
}

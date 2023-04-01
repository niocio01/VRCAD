using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI_Components
{
    public class RadioButtonController : MonoBehaviour
    {
        [System.Serializable] public class SelectionChanged : UnityEvent<string>{ }

        [SerializeField] public SelectionChanged onSelectionChanged;

        [SerializeField] Color deselectedColor = Color.white;
        [SerializeField] Color selectedColor = Color.white;
        [SerializeField] RadioButton defaultButton;

        [SerializeField]
        private RadioButton[] radioButtons;

        private Button[] _buttons;
        private RadioButton _currentlySelected;

        private void Awake()
        {
            List<Button> buttons = new List<Button>();

            foreach (RadioButton radioButton in radioButtons)
            {
                buttons.Add(radioButton.GetComponent<Button>());
            }
            _buttons = buttons.ToArray();

            _currentlySelected = defaultButton;
            _currentlySelected.backgroundImage.color = selectedColor;
        }
        private void OnEnable()
        {
            foreach(Button button in _buttons)
            {
                button.onClick.AddListener(delegate { HandleButtonPressed(button); });
            }
        }
        private void OnDisable()
        {
            foreach (Button button in _buttons)
            {
                button.onClick.RemoveListener(delegate { HandleButtonPressed(button); });
            }
        }
        void HandleButtonPressed(Button button)
        {
            _currentlySelected.backgroundImage.color = deselectedColor;

            _currentlySelected = button.GetComponent<RadioButton>();
            _currentlySelected.backgroundImage.color = selectedColor;
        
            onSelectionChanged?.Invoke(button.name);        
        }
    }
}

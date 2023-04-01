using UnityEngine;
using UnityEngine.UI;

namespace UI_Components
{
    [RequireComponent(typeof(Image))]
    public class RadioButton : MonoBehaviour
    {
        [SerializeField] public Image backgroundImage;

        private void OnValidate()
        {
            backgroundImage = GetComponent<Image>();
        }
    }
}

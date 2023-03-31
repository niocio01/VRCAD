using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class RadioButton : MonoBehaviour
{
    [SerializeField] public Image backgroundImage;

    private void OnValidate()
    {
        backgroundImage = GetComponent<Image>();
    }
}

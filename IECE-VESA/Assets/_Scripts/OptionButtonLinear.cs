using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Submits an option to the GameManager
/// </summary>
public class OptionButtonLinear : MonoBehaviour
{
    public Option option;           // this value is assigned by the GameManager the instant the button its attached to is created


    /// <summary>
    /// Called when this button is clicked
    /// </summary>
    public void OnClick()
    {
        GameManager.Instance.SubmitOptionLinear(option);
    }
}

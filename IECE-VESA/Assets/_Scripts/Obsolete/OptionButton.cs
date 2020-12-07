using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MonoBehaviour
{
    public Option option;
    

    /// <summary>
    /// Called when this button is clicked
    /// </summary>
    public void OnClick()
    {
        GameManager gameManager = GameManager.Instance;

        if (option.comment == "")
        {
            gameManager.SubmitOption(option);
        }
        else
        {
            ContinueButton continueButton = ContinueButton.Instance;
            continueButton.option = option;
            continueButton.gameObject.SetActive(true);
            gameManager.DisplayComment(option.comment);
            gameManager.ClearExcessButtons();
        }
    }
}

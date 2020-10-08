using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionButton : MonoBehaviour
{
    public Option option;
    


    public void OnClick()
    {
        GameManager gameManager = GameManager.Instance;

        if (option.pointsWorth == -1)
        {
            gameManager.FailThem();
        }
        else
        {
            gameManager.AddPoints(option.pointsWorth);
            gameManager.LoadNextQuestion(option.letter.ToString());
        }

        gameManager.DisplayComment(option.comment);
    }
}

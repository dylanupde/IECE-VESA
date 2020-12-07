using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calls on the GameManager to load the next question when clicked
/// </summary>
public class ContinueButtonLinear : MonoBehaviour
{
    public static ContinueButtonLinear Instance;

    GameManager gameManager;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Uhhh why are there two ContinueButtons in the scene?", gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }
    

    public void OnClick()
    {
        gameManager.LoadNextQuestionLinear();
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    public static ContinueButton Instance;

    [HideInInspector] public Option option;


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
        
    }


    public void OnClick()
    {
        GameManager.Instance.SubmitOption(option);
    }
}

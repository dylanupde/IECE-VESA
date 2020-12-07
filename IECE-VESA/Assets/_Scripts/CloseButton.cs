using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Disables any amount of gameobjects when clicked and enables any one object as well if desired
/// </summary>
public class CloseButton : MonoBehaviour
{
    [SerializeField] GameObject[] gameObjectsToDisable;
    [SerializeField] GameObject[] gameObjectsToEnable;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void OnClick()
    {
        foreach (GameObject thisGameObject in gameObjectsToDisable)
        {
            thisGameObject.SetActive(false);
        }
        foreach (GameObject thisGameObject in gameObjectsToEnable)
        {
            thisGameObject.SetActive(true);
        }
    }
}

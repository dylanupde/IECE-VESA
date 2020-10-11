using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class takes a programmer-chosen label for a scenario and assigns it to a scenario so it knows where the camera should look when it comes up
/// </summary>
public class CameraPosition : MonoBehaviour
{
    [SerializeField] string scenarioAssociated;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

        Scenario scenario;

        if (gameManager.scenariosDict.TryGetValue(scenarioAssociated, out scenario))
        {
            scenario.cameraPosition = transform;
        }
        else
        {
            Debug.Log("Can't find scenario associated with this camera position!", gameObject);
        }

        gameObject.SetActive(false);
    }
}

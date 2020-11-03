using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class takes a programmer-chosen label for a scenario and assigns it to a scenario so it knows where the camera should look when it comes up
/// </summary>
public class CameraPosition : MonoBehaviour
{
    [SerializeField] string scenarioAssociated;     // the scenario label that this camera angle is associated with
    [SerializeField] float transitionTime = 0.7f;
    [SerializeField] bool moveCamera = false;       // set this to true if we're moving the camera as a transition, not fading

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

        Scenario scenario;

        // Set the scenario with the scenarioAssociated label to have this camera position associated with it
        if (gameManager.scenariosDict.TryGetValue(scenarioAssociated, out scenario))
        {
            scenario.cameraPositionTransform = transform;
            scenario.moveCamera = moveCamera;
            scenario.transitionTime = transitionTime;
        }
        // If a scenario doesn't exist via the given label, call the designer a dummy
        else
        {
            Debug.LogError("Can't find scenario associated with this camera position!", gameObject);
        }

        // Deactivate this object so it doesn't actually show up during gameplay
        gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class takes a scenario/question label and assigns it to that question so it knows where the camera should be when the question comes up
/// </summary>
public class CameraPosition : MonoBehaviour
{
    [SerializeField] string[] questionsAssociated;     // the scenario label that this camera angle is associated with
    [SerializeField] float transitionTime = 0.7f;
    [SerializeField] bool moveCamera = false;       // set this to true if we're moving the camera as a transition, not fading

    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

        // For every question associated with this camera position...
        foreach (var thisQuestionAssociated in questionsAssociated)
        {
            Question scenario;

            // Set the scenario with the scenarioAssociated label to have this camera position associated with it
            if (gameManager.scenariosLinearDict.TryGetValue(thisQuestionAssociated, out scenario))
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
        }

        // Deactivate this object so it doesn't actually show up during gameplay
        gameObject.SetActive(false);
    }
}

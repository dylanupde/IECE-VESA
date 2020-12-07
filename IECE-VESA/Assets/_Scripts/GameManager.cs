using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] GameObject scenarioParent;     // the parent object over all the UI stuff related to the scenarios
    [SerializeField] GameObject optionObj;          // the UI option button's gameobject
    [SerializeField] GameObject continueButtonObj;  // the UI continue button's gameobject
    [SerializeField] GameObject bioObj;             // the UI bio parent (not the pictures)
    [SerializeField] Text scenarioText;             // the UI text component where scenario strings are displayed
    [SerializeField] Text bioText;             // the UI bio text's gameobject
    [SerializeField] Text scoreText;                // the UI text component of the current score
    [SerializeField] Text commentText;              // the UI text component of the comment that pops up when an option is chosen
    [SerializeField] RawImage blackImage;           // the black image that fades the scene in and out
    [SerializeField] AnimationCurve cameraMoveAnimCurve;

    [HideInInspector] public Dictionary<string, Bio> biosDict;               // the dictionary of children's bios, where the key is the child's name
    [HideInInspector] public Dictionary<string, Question> scenariosDict;        // -OBSOLETE- the dictionary of aaaall the scenarios, where the key is their label
    [HideInInspector] public Dictionary<string, Question> scenariosLinearDict;  // the dictionary of aaaall the scenarios, where the key is their label
    [HideInInspector] public List<Child> childList;                             // the list of the children's Child scripts

    Transform cameraTransform;                  // the transform component on the camera
    RectTransform buttonGroupRectTransform;     // the RectTransform component on the group of buttons. We need this to reset the button layout when we load new options on the screen
    RectTransform bioRect;                      // the RectTransform on the bio text object
    RectTransform scenarioPanelRect;            // the RectTransform on the Panel for the scenario text
    SpecialEventInvoker specialEventInvoker;    
    ContinueButtonLinear continueButton;        // a reference to the continue button's ContinueButton script
    string currentScenarioLabel = "";           // keeps track of the current scenario we're on via the scenario labels (OBSOLETE: ONLY NEEDED FOR BRANCHING PATHS)
    int currentGroupNum = 1;                    // the scenario group we're currently dealing with
    int currentQuestionNum = 1;                 // the question number we're on
    int currentScore = 0;                       // keeps track of our current score
    int currentMaxScore = 0;                    // keeps track of the MOST points the player could have if they were the smartest boi


    void Awake()
    {
        // Make sure there's only one of these
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize the dictionaries
        biosDict = new Dictionary<string, Bio>();
        scenariosDict = new Dictionary<string, Question>();
        scenariosLinearDict = new Dictionary<string, Question>();
        childList = new List<Child>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Assign variables
        specialEventInvoker = GetComponent<SpecialEventInvoker>();
        buttonGroupRectTransform = optionObj.transform.parent.GetComponent<RectTransform>();
        continueButton = ContinueButtonLinear.Instance;
        cameraTransform = Camera.main.transform;
        bioRect = bioText.gameObject.GetComponent<RectTransform>();
        scenarioPanelRect = scenarioText.transform.parent.GetComponent<RectTransform>();

        bioObj.SetActive(false);

        // FOR TESTING ONLY
        //foreach (KeyValuePair<string, Bio> thisPair in biosDict)
        //{
        //    Debug.Log(thisPair.Key);
        //    Debug.Log(thisPair.Value.stats);
        //    Debug.Log(thisPair.Value.description);
        //}

        //foreach (KeyValuePair<string, Question> thisPair in scenariosLinearDict)
        //{
        //    Debug.Log(thisPair.Key + ": " + thisPair.Value.text);

        //    foreach (Option thisOption in thisPair.Value.optionsList)
        //    {
        //        Debug.Log(thisOption.letter + ": " + thisOption.text);
        //        Debug.Log(thisOption.comment + " Pts: " + thisOption.pointsWorth);
        //    }
        //}


        // Load up the first scenario
        StartCoroutine(TransitionToNewScenarioLinear("1-1", true));
    }





    // Public method that calls the coroutine
    public void LoadNextQuestionLinear()
    {
        currentQuestionNum++;
        StartCoroutine(TransitionToNewScenarioLinear(currentGroupNum + "-" + currentQuestionNum));
    }

    /// <summary>
    /// Fades out, then loads the new question and fades back in
    /// </summary>
    /// <param name="inputQuestionKey"></param>
    /// <param name="inputIsFirstQuestion"></param>
    /// <returns></returns>
    IEnumerator TransitionToNewScenarioLinear(string inputQuestionKey, bool inputIsFirstQuestion = false)
    {
        // If it's the first question of the scenario, reset the score and scenario label
        if (inputIsFirstQuestion) { currentScore = 0; }

        // Clear the previous buttons
        ClearExcessButtons();

        // Add the option letter to our current scenario label
        Question question;

        // Load up the scenario with the current label (if there is one)
        if (scenariosLinearDict.TryGetValue(inputQuestionKey, out question))
        {
            float transitionTimeCurrent = question.transitionTime;

            // Fade out if there's a specific camera position associated with this scenario
            if (question.cameraPositionTransform != null)
            {
                // If we should move to this camera position
                if (question.moveCamera)
                {
                    StartCoroutine(MoveCamera(question.cameraPositionTransform, transitionTimeCurrent));
                    yield return new WaitForSeconds(transitionTimeCurrent);
                }
                // Otherwise, we should fade to this camera position
                else
                {
                    // Start fading out, and wait for this to be done
                    blackImage.gameObject.SetActive(true);
                    StartCoroutine(FadeAlpha(1f, transitionTimeCurrent));

                    yield return new WaitForSeconds(transitionTimeCurrent + 0.5f);
                }
            }

            // Set the scenario text box to have this scenario's text
            // NOTE: the Replace method is a workaround to making \n actually mean "new line". The text should originally have /n indicate a new line 
            scenarioText.text = question.text.Replace("/n", "\n").Replace("/t", "\t");

            // Adjust the height of the scenario panel to match the height of the text (plus a little more) 
            scenarioPanelRect.sizeDelta = new Vector2(scenarioPanelRect.sizeDelta.x, LayoutUtility.GetPreferredHeight(scenarioText.rectTransform) + 15f);

            optionObj.SetActive(true);

            // See if this question involves any special event too happen
            specialEventInvoker.AttemptSpecialEvent(inputQuestionKey);

            // For each option that this scenario has, make a button for it
            foreach (Option thisOption in question.optionsList)
            {
                GameObject newOptionObj = Instantiate(optionObj, optionObj.transform.parent);

                newOptionObj.transform.GetChild(0).GetComponent<Text>().text = thisOption.text;
                newOptionObj.GetComponent<OptionButtonLinear>().option = thisOption;
            }

            // Clear the comment, disable our reference option box, and reload the button layout
            commentText.text = "";
            optionObj.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonGroupRectTransform);

            // Each scenario can get a max score of 10 points, so add that to the potential MAX points that the player could get
            currentMaxScore += 10;

            // Fade the camera back in if necessary
            if (question.cameraPositionTransform != null)
            {
                continueButton.gameObject.SetActive(false);
                cameraTransform.position = question.cameraPositionTransform.position;
                cameraTransform.rotation = question.cameraPositionTransform.rotation;

                // Fade back in
                StartCoroutine(FadeAlpha(0f, transitionTimeCurrent));
                yield return new WaitForSeconds(transitionTimeCurrent);
                blackImage.gameObject.SetActive(false);
            }
        }
    }



    /// <summary>
    /// Fades in or out the alpha of the black image over a specified amount of time
    /// </summary>
    /// <param name="inputEndAlpha"></param>
    /// <param name="inputFadeTime"></param>
    /// <returns></returns>
    IEnumerator FadeAlpha(float inputEndAlpha, float inputFadeTime)
    {
        // Assign where the lerp should start and end, and its speed
        Color startColor = blackImage.color;
        Color endColor = blackImage.color;
        endColor.a = inputEndAlpha;
        float lerpSpeed = 1f / inputFadeTime;
        float t = 0f;

        while (t <= 1f)
        {
            t += lerpSpeed * Time.deltaTime;
            blackImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        blackImage.color = endColor;
    }


    /// <summary>
    /// Lerps the camera to the end transform's position/rotation over an input period of time
    /// </summary>
    /// <param name="inputEndTransform"></param>
    /// <param name="inputTransitionTime"></param>
    /// <returns></returns>
    IEnumerator MoveCamera(Transform inputEndTransform, float inputTransitionTime)
    {
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;
        float lerpSpeed = 1f / inputTransitionTime;
        float t = 0f;

        while (t <= 1f)
        {
            t += lerpSpeed * Time.deltaTime;
            cameraTransform.position = Vector3.Lerp(startPos, inputEndTransform.position, cameraMoveAnimCurve.Evaluate(t));
            cameraTransform.rotation = Quaternion.Slerp(startRot, inputEndTransform.rotation, cameraMoveAnimCurve.Evaluate(t));

            yield return null;
        }

        cameraTransform.position = inputEndTransform.position;
        print(cameraTransform.position);
        cameraTransform.rotation = inputEndTransform.rotation;
    }
    

    /// <summary>
    /// Destroys all the visible buttons from the scene (not the prefab)
    /// </summary>
    public void ClearExcessButtons()
    {
        if (buttonGroupRectTransform.childCount > 1)
        {
            for (int i = 1; i < buttonGroupRectTransform.childCount; i++)
            {
                Destroy(buttonGroupRectTransform.GetChild(i).gameObject);
            }
        }

    }


    /// <summary>
    /// Adds points and updates the point UI display
    /// </summary>
    /// <param name="inputPoints"></param>
    public void AddPoints(int inputPoints)
    {
        currentScore += inputPoints;

        scoreText.text = currentScore.ToString() + " / " + currentMaxScore;
    }


    /// <summary>
    /// Called when the user clicks an option
    /// </summary>
    /// <param name="inputOption"></param>
    public void SubmitOptionLinear(Option inputOption)
    {
        // Otherwise, add the points that this option is worth to the score and load the next question
        AddPoints(inputOption.pointsWorth);
        scenarioText.text = inputOption.comment;
        scenarioPanelRect.sizeDelta = new Vector2(scenarioPanelRect.sizeDelta.x, LayoutUtility.GetPreferredHeight(scenarioText.rectTransform) + 15f);
        ClearExcessButtons();

        continueButton.gameObject.SetActive(true);
    }



    /// <summary>
    /// Makes the UI display whatever string is put in as a comment
    /// </summary>
    /// <param name="inputComment"></param>
    public void DisplayComment(string inputComment)
    {
        commentText.text = inputComment;
    }


    /// <summary>
    /// Displays the bio and bio pic of any kid by name
    /// </summary>
    /// <param name="inputName"></param>
    public void DisplayBio(string inputName)
    {
        bioObj.SetActive(true);

        Bio bio;

        if (biosDict.TryGetValue(inputName, out bio))
        {
            bioText.text = "\n" + inputName + "\n" + bio.stats + "\n" + bio.description;
            bioRect.sizeDelta = new Vector2(bioRect.sizeDelta.x, LayoutUtility.GetPreferredHeight(bioText.rectTransform) + 15f);
            scenarioParent.SetActive(false);
            bio.bioImageObj.SetActive(true);
        }
        else
        {
            Debug.LogError("OH NO! There is no child by this name! Make sure each gameobject with the Child script has the child's name as the gameobject name.");
        }
    }


    /// <summary>
    /// Because objects can be clicked THROUGH UI objects, 
    /// </summary>
    /// <param name="inputBool"></param>
    public void MakeChildrenHighlightable(bool inputBool)
    {
        foreach (Child thisChild in childList)
        {
            thisChild.ShowOutline(inputBool);
            thisChild.highlightable = inputBool;
        }
    }





    // OBSOLETE STUFF
    #region

    // Public method that calls the coroutine
    public void LoadNextQuestion(string inputScenarioChar, bool inputIsFirstQuestion = false)
    {
        StartCoroutine(TransitionToNewScenarioLinear(inputScenarioChar, inputIsFirstQuestion));
    }


    /// <summary>
    /// -OBSOLETE- Fades out, then loads the new scenario and fades back in
    /// </summary>
    /// <param name="inputScenarioChar"></param>
    /// <param name="inputIsFirstQuestion"></param>
    /// <returns></returns>
    IEnumerator TransitionToNewScenario(string inputScenarioChar, bool inputIsFirstQuestion = false)
    {
        // If it's the first question, reset the score and scenario label
        if (inputIsFirstQuestion)
        {
            currentScenarioLabel = "";
            currentScore = 0;
        }

        // Clear the previous buttons
        ClearExcessButtons();

        // Add the option letter to our current scenario label
        currentScenarioLabel += inputScenarioChar;
        Question scenario;

        // Load up the scenario with the current label (if there is one)
        if (scenariosDict.TryGetValue(currentScenarioLabel, out scenario))
        {
            float transitionTimeCurrent = scenario.transitionTime;

            // Fade out if there's a specific camera position associated with this scenario
            if (scenario.cameraPositionTransform != null)
            {
                if (scenario.moveCamera)
                {
                    StartCoroutine(MoveCamera(scenario.cameraPositionTransform, transitionTimeCurrent));
                    yield return new WaitForSeconds(transitionTimeCurrent);
                }
                else
                {
                    // Start fading out, and wait for this to be done
                    blackImage.gameObject.SetActive(true);
                    StartCoroutine(FadeAlpha(1f, transitionTimeCurrent));

                    yield return new WaitForSeconds(transitionTimeCurrent + 0.5f);
                }
            }

            // Set the scenario text box to have this scenario's text
            scenarioText.text = scenario.text;
            optionObj.SetActive(true);

            // For each option that this scenario has, make a button for it
            foreach (Option thisOption in scenario.optionsList)
            {
                GameObject newOptionObj = Instantiate(optionObj, optionObj.transform.parent);

                newOptionObj.transform.GetChild(0).GetComponent<Text>().text = thisOption.text;
                newOptionObj.GetComponent<OptionButton>().option = thisOption;
            }

            // Clear the comment, disable our reference option box, and reload the button layout
            commentText.text = "";
            optionObj.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonGroupRectTransform);

            // Each scenario can get a max score of 10 points, so add that to the potential MAX points that the player could get
            currentMaxScore += 10;

            // Move the camera if there's a specific camera position associated with this scenario
            if (scenario.cameraPositionTransform != null)
            {
                continueButton.gameObject.SetActive(false);
                cameraTransform.position = scenario.cameraPositionTransform.position;
                cameraTransform.rotation = scenario.cameraPositionTransform.rotation;

                // Fade back in
                StartCoroutine(FadeAlpha(0f, transitionTimeCurrent));
                yield return new WaitForSeconds(transitionTimeCurrent);
                blackImage.gameObject.SetActive(false);
            }
        }
    }


    /// <summary>
    /// Called when the user clicks an option
    /// </summary>
    /// <param name="inputOption"></param>
    public void SubmitOption(Option inputOption)
    {
        // If it's an option that makes you fail, fail the user
        if (inputOption.pointsWorth == -1)
        {
            FailThem();
        }
        // Otherwise, add the points that this option is worth to the score and load the next question
        else
        {
            AddPoints(inputOption.pointsWorth);
            LoadNextQuestion(inputOption.letter.ToString());
        }

        DisplayComment(inputOption.comment);
    }



    /// <summary>
    /// Fails the user
    /// </summary>
    public void FailThem()
    {
        ClearExcessButtons();
        AddPoints(-currentScore);

        scenarioText.text = "AUTOMATIC FAIL";
    }

    #endregion

}













/// <summary>
/// A scenario is a class that stores aaall the relevant information to a scenario, including what options are available to it
/// </summary>
public class Question
{
    public List<string> namesList;          // the names of all the characters associated with this scenario
    public List<Option> optionsList;        // all the Options that this scenario has
    public string text;              // the actual text for the scenario
    public Transform cameraPositionTransform;        // gets assigned by a Camera Position object
    public float transitionTime;            // how long it should take for the camera to transition to this scenario (whether that be fade or move)
    public bool moveCamera;                 // if this is false, we will fade out the camera and fade it in during transition to this scenario. If true, we'll just move the camera there over time

    public Question()
    {
        namesList = new List<string>();
        optionsList = new List<Option>();
        cameraPositionTransform = null;
    }

    public void AddName(string inputName)
    {
        namesList.Add(inputName);
    }


    public void AddOption(string inputText, int pointsWorth)
    {
        optionsList.Add(new Option(inputText, pointsWorth));
    }

    public void AddOption(Option inputOption)
    {
        optionsList.Add(inputOption);
    }
}



/// <summary>
/// Stores info for an option, including the letter associated with it (A, B, C, D, etc), the text, the comment (for when you select this option), and how many points it's worth
/// </summary>
public class Option
{
    public char letter = 'Z';       // The letter associated
    public string text;             // The actual line of text for this option
    public int pointsWorth;         // If this is a -1, it means that picking this option is an instant FAIL
    public string comment = "";     // The comment that gets displayed if the player picks this answer

    public Option(string inputText, int inputPointsWorth = 0)
    {
        text = inputText;
        pointsWorth = inputPointsWorth;
    }
}



public class Bio
{
    public string stats;
    public string description;
    public GameObject bioImageObj;
}
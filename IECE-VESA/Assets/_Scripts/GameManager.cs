using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] Text scenarioText;             // the UI text component where scenario strings are displayed
    [SerializeField] GameObject optionObj;          // the UI option button's gameobject
    [SerializeField] GameObject continueButtonObj;  // the UI continue button's gameobject
    [SerializeField] Text scoreText;                // the UI text component of the current score
    [SerializeField] Text commentText;              // the UI text component of the comment that pops up when an option is chosen
    [SerializeField] RawImage blackImage;           // the black image that fades the scene in and out
    [SerializeField] AnimationCurve cameraMoveAnimCurve;

    [HideInInspector] public Dictionary<string, string> biosDict;               // the dictionary of children's bios, where the key is the child's name
    [HideInInspector] public Dictionary<string, Scenario> scenariosDict;        // the dictionary of aaaall the scenarios, where the key is their label

    Transform cameraTransform;                  // the transform component on the camera
    RectTransform buttonGroupRectTransform;     // the RectTransform component on the group of buttons. We need this to reset the button layout when we load new options on the screen
    ContinueButton continueButton;              // a reference to the continue button's ContinueButton script
    string currentScenarioLabel = "";           // keeps track of the current scenario we're on via the scenario labels
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
        biosDict = new Dictionary<string, string>();
        scenariosDict = new Dictionary<string, Scenario>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Assign variables
        buttonGroupRectTransform = optionObj.transform.parent.GetComponent<RectTransform>();
        continueButton = ContinueButton.Instance;
        cameraTransform = Camera.main.transform;

        // Load up the first scenario
        LoadNextQuestion("1", true);
    }


    // Public method that calls the coroutine
    public void LoadNextQuestion(string inputScenarioChar, bool inputIsFirstQuestion = false)
    {
        StartCoroutine(TransitionToNewScenario(inputScenarioChar, inputIsFirstQuestion));
    }


    /// <summary>
    /// Fades out, then loads the new scenario and fades back in
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
        Scenario scenario;

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
            scenarioText.text = scenario.description;
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
        cameraTransform.rotation = inputEndTransform.rotation;
    }
    

    /// <summary>
    /// Deletes all the visible buttons from the scene
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


    // Adds points and updates the point UI display
    public void AddPoints(int inputPoints)
    {
        currentScore += inputPoints;

        scoreText.text = currentScore.ToString() + " / " + currentMaxScore;
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


    /// <summary>
    /// Makes the UI display whatever string is put in as a comment
    /// </summary>
    /// <param name="inputComment"></param>
    public void DisplayComment(string inputComment)
    {
        commentText.text = inputComment;
    }
}




/// <summary>
/// A scenario is a class that stores aaall the relevant information to a scenario, including what options are available to it
/// </summary>
public class Scenario
{
    public List<string> namesList;          // the names of all the characters associated with this scenario
    public List<Option> optionsList;        // all the Options that this scenario has
    public string description;              // the actual text for the scenario
    public Transform cameraPositionTransform;        // gets assigned by a Camera Position object
    public float transitionTime;            // how long it should take for the camera to transition to this scenario (whether that be fade or move)
    public bool moveCamera;                 // if this is false, we will fade out the camera and fade it in during transition to this scenario. If true, we'll just move the camera there

    public Scenario()
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
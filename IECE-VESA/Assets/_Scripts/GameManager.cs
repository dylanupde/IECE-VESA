using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] Text scenarioText;
    [SerializeField] GameObject optionObj;
    [SerializeField] GameObject continueButtonObj;
    [SerializeField] Text scoreText;
    [SerializeField] Text commentText;
    [SerializeField] RawImage blackImage;

    [HideInInspector] public Dictionary<string, string> biosDict;
    [HideInInspector] public Dictionary<string, Scenario> scenariosDict;

    Transform cameraTransform;
    RectTransform buttonGroupRectTransform;
    ContinueButton continueButton;
    Scenario scenario;
    string currentScenarioLabel = "";
    int currentScore = 0;
    int currentMaxScore = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        biosDict = new Dictionary<string, string>();
        scenariosDict = new Dictionary<string, Scenario>();
    }

    // Start is called before the first frame update
    void Start()
    {
        buttonGroupRectTransform = optionObj.transform.parent.GetComponent<RectTransform>();
        continueButton = ContinueButton.Instance;
        cameraTransform = Camera.main.transform;

        StartCoroutine(DEMOStart());
    }


    IEnumerator DEMOStart()
    {
        yield return null;
        LoadNextQuestion("1", true);
    }


    public void LoadNextQuestion(string inputScenarioChar, bool inputIsFirstQuestion = false)
    {
        StartCoroutine(FadeOutFadeInWithNewScenario(inputScenarioChar, inputIsFirstQuestion));
    }



    IEnumerator FadeOutFadeInWithNewScenario(string inputScenarioChar, bool inputIsFirstQuestion = false)
    {
        // TEXT LOADING STUFF
        if (inputIsFirstQuestion)
        {
            currentScenarioLabel = "";
            currentScore = 0;
        }

        // Clear the previous buttons
        ClearExcessButtons();

        currentScenarioLabel += inputScenarioChar;

        if (scenariosDict.TryGetValue(currentScenarioLabel, out scenario))
        {
            float fadeTime = 0.7f;

            //Relocate the camera if we should
            if (scenario.cameraPosition != null)
            {
                blackImage.gameObject.SetActive(true);

                StartCoroutine(FadeAlpha(1f, fadeTime));

                yield return new WaitForSeconds(fadeTime + 0.5f);
            }


            scenarioText.text = scenario.description;
            optionObj.SetActive(true);

            foreach (Option thisOption in scenario.optionsList)
            {
                GameObject newOptionObj = Instantiate(optionObj, optionObj.transform.parent);

                newOptionObj.transform.GetChild(0).GetComponent<Text>().text = thisOption.text;
                newOptionObj.GetComponent<OptionButton>().option = thisOption;
            }

            commentText.text = "";
            optionObj.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonGroupRectTransform);

            currentMaxScore += 10;

            if (scenario.cameraPosition != null)
            {
                continueButton.gameObject.SetActive(false);
                cameraTransform.position = scenario.cameraPosition.position;
                cameraTransform.rotation = scenario.cameraPosition.rotation;
                
                StartCoroutine(FadeAlpha(0f, fadeTime));
                yield return new WaitForSeconds(fadeTime);
                blackImage.gameObject.SetActive(false);
            }
        }
    }


    IEnumerator FadeAlpha(float inputEndAlpha, float inputFadeTime)
    {
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


    public void AddPoints(int inputPoints)
    {
        currentScore += inputPoints;

        scoreText.text = currentScore.ToString() + " / " + currentMaxScore;
    }


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


    public void FailThem()
    {
        ClearExcessButtons();
        AddPoints(-currentScore);

        scenarioText.text = "AUTOMATIC FAIL";
    }


    public void DisplayComment(string inputComment)
    {
        commentText.text = inputComment;
    }
}





public class Scenario
{
    public List<string> namesList;
    public List<Option> optionsList;
    public string description;
    public Transform cameraPosition;              // This is set to a ludicrously high value by default so we know if it's not explicitly assigned to a real camera position

    public Scenario()
    {
        namesList = new List<string>();
        optionsList = new List<Option>();
        cameraPosition = null;
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


public class Option
{
    public char letter = 'Z';
    public string text;
    public int pointsWorth;
    public string comment = "";

    public Option(string inputText, int inputPointsWorth = 0)
    {
        text = inputText;
        pointsWorth = inputPointsWorth;
    }
}
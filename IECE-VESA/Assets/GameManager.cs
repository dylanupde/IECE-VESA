using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] Text scenarioText;
    [SerializeField] GameObject optionObj;
    [SerializeField] Text scoreText;
    [SerializeField] Text commentText;

    [HideInInspector] public Dictionary<string, string> biosDict;
    [HideInInspector] public Dictionary<string, Scenario> scenariosDict;

    RectTransform buttonGroupRectTransform;
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

        StartCoroutine(DEMOStart());
    }


    IEnumerator DEMOStart()
    {
        yield return null;
        LoadNextQuestion("1", true);
    }


    public void LoadNextQuestion(string inputScenarioChar, bool inputIsFirstQuestion = false)
    {
        if (inputIsFirstQuestion)
        {
            currentScenarioLabel = "";
            currentScore = 0;
        }

        // Clear the previous buttons
        ClearExcessButtons();

        currentScenarioLabel += inputScenarioChar;
        Scenario scenario;

        if (scenariosDict.TryGetValue(currentScenarioLabel, out scenario))
        {
            scenarioText.text = scenario.description;
            optionObj.SetActive(true);

            foreach (Option thisOption in scenario.optionsList)
            {
                GameObject newOptionObj = Instantiate(optionObj, optionObj.transform.parent);

                newOptionObj.transform.GetChild(0).GetComponent<Text>().text = thisOption.text;
                newOptionObj.GetComponent<OptionButton>().option = thisOption;
            }

            optionObj.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(buttonGroupRectTransform);

            currentMaxScore += 10;
        }
    }


    private void ClearExcessButtons()
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

    public Scenario()
    {
        namesList = new List<string>();
        optionsList = new List<Option>();
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
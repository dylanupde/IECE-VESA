using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class TextDocConverter : MonoBehaviour
{
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

        ReadInTextDocs();
        //StartCoroutine(DEMOCoroutine());

        //foreach (KeyValuePair<string, string> thisPair in biosDict)
        //{
        //    Debug.Log(thisPair.Key + ": " + thisPair.Value);
        //}

        //foreach (KeyValuePair<string, Scenario> thisScenarioPair in gameManager.scenariosDict)
        //{
        //    Debug.Log(thisScenarioPair.Key + ": " + thisScenarioPair.Value.description);

        //    foreach (Option thisOption in thisScenarioPair.Value.optionsList)
        //    {
        //        Debug.Log(thisOption.letter + ") " + thisOption.text + " Pts: " + thisOption.pointsWorth);
        //    }
        //}

        
    }


    private void ReadInTextDocs()
    {
        // ========================================
        // ================= BIOS =================
        // ========================================

        bool lastLineSaidName = false;
        string line;
        string name = "";

        StreamReader biosStreamReader = File.OpenText(Application.streamingAssetsPath + "/Bios.txt");

        do
        {
            // Assign the next line to the string variable "line"
            line = biosStreamReader.ReadLine();
            
            // If this line isn't blank...
            if (line != null && line.Length > 0 && line[0] != '@')
            {
                // If the last line didn't start with "Name"...
                if (!lastLineSaidName)
                {
                    // If this line DOES start with name, then record the name and flag that the next line will be the bio
                    if (line.Substring(0, 4).ToLower() == "name")
                    {
                        lastLineSaidName = true;

                        name = ExtractStringAfterChar(line, ':');
                    }
                    // Otherwise, not that this line isn't a name line
                    else
                    {
                        lastLineSaidName = false;
                    }
                }
                // Otherwise, this is the description for the kid
                else
                {
                    // Flag that the last line wasn't a name and add the name and line to our bios dict
                    lastLineSaidName = false;
                    gameManager.biosDict.Add(name, line);

                    // This line isn't technically necessary, but may help us with debugging if something messes up
                    name = "-EMPTY-";
                }
            }
        // Keep doing this as long as there's still lines to be read
        } while (line != null);
        biosStreamReader.Close();



        // =======================================
        // ============== SCENARIOS ==============
        // =======================================
        
        StreamReader scenariosStreamReader = File.OpenText(Application.streamingAssetsPath + "/Scenarios.txt");
        Scenario currentScenario = null;
        Option currentOption = null;
        bool lastLineWasOption = false;
        bool lastLineWasPoints = false;

        do
        {
            line = scenariosStreamReader.ReadLine();

            // If this line isn't blank...
            if (line != null && line.Length > 0 && line[0] != '@')
            {
                // If it's a scenario description line...
                if (line.Contains(":") && Char.IsNumber(line[0]))
                {
                    lastLineWasPoints = false;
                    currentOption = null;
                    currentScenario = new Scenario();

                    gameManager.scenariosDict.Add(ExtractStringBeforeChar(line, ':'), currentScenario);
                    currentScenario.description = ExtractStringAfterChar(line, ':');
                }
                // If it's the points for an option...
                else if (lastLineWasOption)
                {
                    lastLineWasOption = false;
                    lastLineWasPoints = true;
                    string pointsString;

                    if (line.ToLower().Contains("f"))
                    {
                        currentOption.pointsWorth = -1;
                    }
                    else
                    {
                        if (line.Contains(":"))
                        {
                            pointsString = ExtractStringAfterChar(line, ':');
                        }
                        else
                        {
                            pointsString = line;
                        }
                        
                        int points = int.Parse(pointsString);
                        currentOption.pointsWorth = points;
                    }

                    currentScenario.AddOption(currentOption);
                }
                else if (lastLineWasPoints && line[0] == '"')
                {
                    currentOption.comment = ExtractStringAfterChar(line, '"');
                }
                // If it's an option line...
                else if (line[1] == ')')
                {
                    lastLineWasOption = true;
                    lastLineWasPoints = false;

                    currentOption = new Option(ExtractStringAfterChar(line, ')'));
                    currentOption.letter = line[0];
                }
            }
        } while (line != null);

        scenariosStreamReader.Close();
    }





    private string ExtractStringAfterChar(string inputString, char inputChar)
    {
        int placeToRemoveAt = inputString.IndexOf(inputChar);
        if (inputString[placeToRemoveAt + 1] == ' ') placeToRemoveAt++;

        return inputString.Remove(0, placeToRemoveAt + 1);
    }

    private string ExtractStringBeforeChar(string inputString, char inputChar)
    {
        int placeToRemoveAt = inputString.IndexOf(inputChar);
        if (inputString[placeToRemoveAt + 1] == ' ') placeToRemoveAt++;

        return inputString.Remove(placeToRemoveAt - 1);
    }


    private string RemoveFirstSpaceIfThere(string inputString)
    {
        string outputString = inputString;
        if (outputString[0] == ' ') outputString = outputString.Remove(0, 1);
        return outputString;
    }





    IEnumerator DEMOCoroutine()
    {
        yield return null;

        //while (true)
        //{
        //    foreach (Scenario thisScenario in scenariosDict)
        //    {
        //        string scenarioString = thisScenario.description + "\n";

        //        foreach (Option thisOption in thisScenario.optionsList)
        //        {
        //            scenarioString += thisOption.text;
        //            scenarioString += "\n";
        //        }

        //        scenarioText.text = scenarioString;


        //        // BIOS
        //        string biosString = "";

        //        foreach (string thisName in thisScenario.namesList)
        //        {
        //            biosString += thisName + "\n";
        //            biosString += biosDict[thisName] + "\n\n";
        //        }

        //        biosText.text = biosString;

        //        yield return new WaitForSeconds(15f);
        //    }
        //}
    }
}
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
                    // Take not of some stuff and set a new currentScenario that we can add things to
                    lastLineWasPoints = false;
                    currentOption = null;
                    currentScenario = new Scenario();

                    // Add this to the scenarios dictionary and add the description to that scenario
                    gameManager.scenariosDict.Add(ExtractStringBeforeChar(line, ':'), currentScenario);
                    currentScenario.description = ExtractStringAfterChar(line, ':');
                }
                // If it's the points for an option...
                else if (lastLineWasOption)
                {
                    lastLineWasOption = false;
                    lastLineWasPoints = true;
                    string pointsString;

                    // If there's an F on this line, then it's a failure
                    if (line.ToLower().Contains("f"))
                    {
                        currentOption.pointsWorth = -1;
                    }
                    else
                    {
                        // Get the points for this line
                        if (line.Contains(":"))
                        {
                            pointsString = ExtractStringAfterChar(line, ':');
                        }
                        else
                        {
                            pointsString = line;
                        }
                        
                        // Assign those points to the option we're currently dealing with
                        int points = int.Parse(pointsString);
                        currentOption.pointsWorth = points;
                    }

                    // Now that the option we're working with has some points assigned, add it to the options list for the current scenario
                    currentScenario.AddOption(currentOption);
                }
                // If this line is a comment, add this comment to the option
                else if (lastLineWasPoints && line[0] == '"')
                {
                    currentOption.comment = ExtractStringAfterChar(line, '"');
                }
                // If it's an option line...
                else if (line[1] == ')')
                {
                    lastLineWasOption = true;
                    lastLineWasPoints = false;

                    // Add this option with its text and letter
                    currentOption = new Option(ExtractStringAfterChar(line, ')'));
                    currentOption.letter = line[0];
                }
            }
        } while (line != null);

        scenariosStreamReader.Close();
    }




    /// <summary>
    /// A handy method that returns the text in a string after the input character
    /// </summary>
    /// <param name="inputString"></param>
    /// <param name="inputChar"></param>
    /// <returns></returns>
    private string ExtractStringAfterChar(string inputString, char inputChar)
    {
        int placeToRemoveAt = inputString.IndexOf(inputChar);
        if (inputString[placeToRemoveAt + 1] == ' ') placeToRemoveAt++;

        return inputString.Remove(0, placeToRemoveAt + 1);
    }

    /// <summary>
    /// A handy method that returns the text in a string before the input character
    /// </summary>
    /// <param name="inputString"></param>
    /// <param name="inputChar"></param>
    /// <returns></returns>
    private string ExtractStringBeforeChar(string inputString, char inputChar)
    {
        int placeToRemoveAt = inputString.IndexOf(inputChar);
        if (inputString[placeToRemoveAt + 1] == ' ') placeToRemoveAt++;

        return inputString.Remove(placeToRemoveAt - 1);
    }

    /// <summary>
    /// A handy method that removes the space at the beginning of a string if it's there
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns></returns>
    private string RemoveFirstSpaceIfThere(string inputString)
    {
        string outputString = inputString;
        if (outputString[0] == ' ') outputString = outputString.Remove(0, 1);
        return outputString;
    }
}
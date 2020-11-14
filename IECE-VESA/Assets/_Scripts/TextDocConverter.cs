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
        bool lastLineSaidStats = false;
        string line;
        string kidName = "";
        Bio currentBio = null;


        StreamReader biosStreamReader = File.OpenText(Application.streamingAssetsPath + "/Bios.txt");

        do
        {
            // Assign the next line to the string variable "line"
            line = biosStreamReader.ReadLine();
            
            // If this line isn't blank...
            if (line != null && line.Length > 0 && line[0] != '@')
            {
                // If the last line started with "Name" (meaning this is the stats)...
                if (lastLineSaidName)
                {
                    // Flag that the last line wasn't a name and add the name and line to our bios dict
                    lastLineSaidName = false;
                    lastLineSaidStats = true;
                    currentBio.stats = line;
                }
                // Otherwise, if the last line was our stats (meaning this is the description)...
                else if (lastLineSaidStats)
                {
                    lastLineSaidStats = false;
                    lastLineSaidName = false;
                    currentBio.description = line;
                    gameManager.biosDict.Add(kidName, currentBio);
                    currentBio = null;

                    // This line isn't technically necessary, but may help us with debugging if something messes up
                    kidName = "-EMPTY-";
                }
                // Otherwise, this is the name for the kid
                else
                {
                    // If this line DOES start with name, then record the name and flag that the next line will be the bio
                    if (line.Substring(0, 4).ToLower() == "name")
                    {
                        lastLineSaidName = true;

                        currentBio = new Bio();
                        kidName = ExtractStringAfterChar(line, ':');
                    }
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
                    currentScenario.question = ExtractStringAfterChar(line, ':');
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



        // =======================================
        // ============== SCENARIOS ==============
        // =======================================

        StreamReader scenariosLinearStreamReader = File.OpenText(Application.streamingAssetsPath + "/ScenariosLinear.txt");
        currentScenario = null;
        currentOption = null;
        int currentGroup = 0;
        string currentQuestion = "";
        bool lastLineWasQuestion = false;
        
        do
        {
            line = scenariosLinearStreamReader.ReadLine();

            // If this line isn't blank...
            if (line != null && line.Length > 0 && line[0] != '@')
            {
                // If it's a group line...
                if (line.Contains(":") && ExtractStringBeforeChar(line, ':').ToLower() == "group")
                {
                    lastLineWasQuestion = false;

                    // Set the currentGroup we're in
                    string groupString = ExtractStringAfterChar(line, ':');
                    currentGroup = int.Parse(groupString);
                }
                // If it's a question...
                else if (line.Contains(":") && ExtractStringBeforeChar(line, ':').ToLower() == "question")
                {
                    lastLineWasQuestion = true;
                    currentQuestion = line;

                    currentScenario = new Scenario();
                    gameManager.scenariosLinearList.Add(currentScenario);
                    currentScenario.question = currentQuestion;
                    currentScenario.group = currentGroup;
                }
                // If it's an option line...
                else if (line.Length > 1 && line[1] == ':' && Char.IsLetter(line[0]))
                {
                    // Add this option with its text and letter
                    currentOption = new Option(ExtractStringAfterChar(line, ':'));
                    currentOption.letter = line[0];
                    currentScenario.AddOption(currentOption);
                }
                // If this line is a comment, add this comment to the option
                else if (line.Contains(":") && ExtractStringBeforeChar(line, ':').ToLower() == "comment")
                {
                    currentOption.comment = ExtractStringAfterChar(line, '"');
                }
                else if (Char.IsNumber(line[0]))
                {
                    currentOption.pointsWorth = int.Parse(line);
                }
                else if (lastLineWasQuestion)
                {
                    currentQuestion += line;
                    currentScenario.question = currentQuestion;
                }
            }
        } while (line != null);

        scenariosLinearStreamReader.Close();
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
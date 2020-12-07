using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// Converts the text documents into instances of Bio, Question and Option
/// </summary>
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
                        kidName = StringManipulator.ExtractStringAfterChar(line, ':');
                    }
                }
            }
        // Keep doing this as long as there's still lines to be read
        } while (line != null);
        biosStreamReader.Close();



        // =======================================
        // ============== SCENARIOS (OBSOLETE) ==============
        // =======================================
        #region
        StreamReader scenariosStreamReader = File.OpenText(Application.streamingAssetsPath + "/Scenarios.txt");
        Question currentQuestion = null;
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
                    currentQuestion = new Question();

                    // Add this to the scenarios dictionary and add the description to that scenario
                    gameManager.scenariosDict.Add(StringManipulator.ExtractStringBeforeChar(line, ':'), currentQuestion);
                    currentQuestion.text = StringManipulator.ExtractStringAfterChar(line, ':');
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
                            pointsString = StringManipulator.ExtractStringAfterChar(line, ':');
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
                    currentQuestion.AddOption(currentOption);
                }
                // If this line is a comment, add this comment to the option
                else if (lastLineWasPoints && line[0] == '"')
                {
                    currentOption.comment = StringManipulator.ExtractStringAfterChar(line, '"');
                }
                // If it's an option line...
                else if (line[1] == ')')
                {
                    lastLineWasOption = true;
                    lastLineWasPoints = false;

                    // Add this option with its text and letter
                    currentOption = new Option(StringManipulator.ExtractStringAfterChar(line, ')'));
                    currentOption.letter = line[0];
                }
            }
        } while (line != null);

        scenariosStreamReader.Close();
        #endregion


        // ==============================================
        // ============== SCENARIOS LINEAR ==============
        // ==============================================

        StreamReader scenariosLinearStreamReader = File.OpenText(Application.streamingAssetsPath + "/ScenariosLinear.txt");
        currentQuestion = null;
        currentOption = null;
        int currentGroup = 0;
        string currentQuestionText = "";
        bool lastLineWasQuestion = false;
        
        do
        {
            line = scenariosLinearStreamReader.ReadLine();

            // If this line isn't blank...
            if (line != null && line.Length > 0 && line[0] != '@')
            {
                // If it's a group line...
                if (line.Contains(":") && StringManipulator.ExtractStringBeforeChar(line, ':').ToLower() == "scenario")
                {
                    lastLineWasQuestion = false;

                    // Set the currentGroup we're in
                    string groupString = StringManipulator.ExtractStringAfterChar(line, ':');
                    currentGroup = int.Parse(groupString);
                }
                // If it's a question...
                else if (line.Contains(":") && Char.IsNumber(line[0]))
                {
                    lastLineWasQuestion = true;
                    currentQuestionText = StringManipulator.ExtractStringAfterChar(line, ':');
                    string keyString = currentGroup.ToString() + "-" + StringManipulator.ExtractStringBeforeChar(line, ':');

                    currentQuestion = new Question();
                    gameManager.scenariosLinearDict.Add(keyString, currentQuestion);
                    currentQuestion.text = currentQuestionText;
                }
                // If it's an option line...
                else if (line.Length > 1 && line[1] == ':' && Char.IsLetter(line[0]))
                {
                    // Add this option with its text and letter
                    currentOption = new Option(StringManipulator.ExtractStringAfterChar(line, ':'));
                    currentOption.letter = line[0];
                    currentQuestion.AddOption(currentOption);
                }
                // If this line is a comment, add this comment to the option
                else if (line.Contains(":") && StringManipulator.ExtractStringBeforeChar(line, ':').ToLower() == "comment")
                {
                    currentOption.comment = StringManipulator.ExtractStringAfterChar(line, ':');
                }
                else if (Char.IsNumber(line[0]))
                {
                    currentOption.pointsWorth = int.Parse(line);
                }
                else if (lastLineWasQuestion)
                {
                    currentQuestionText += "/n" + line;
                    currentQuestion.text = currentQuestionText;
                }
            }
        } while (line != null);

        scenariosLinearStreamReader.Close();
    }
}
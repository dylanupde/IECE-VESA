using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Child : MonoBehaviour
{
    [HideInInspector] public bool highlightable = true;
    [HideInInspector] public bool outlinerIsOn;

    List<Outline> outlineList;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        outlineList = new List<Outline>();

        // For every child of our first child, if it has an Outline script, add it to our list of outline scripts
        for (int i = 0; i < transform.childCount; i++)
        {
            Outline thisOutline = transform.GetChild(i).GetComponent<Outline>();

            if (thisOutline != null)
            {
                outlineList.Add(thisOutline);
            }
        }

        gameManager.childList.Add(this);
    }


    /// <summary>
    /// Shows the outline of the kid
    /// </summary>
    /// <param name="inputShouldDisplayRenderer"></param>
    public void ShowOutline(bool inputShouldDisplayRenderer)
    {
        // If we're trying to turn the outline on but we're not highlightable, get outta here
        if (inputShouldDisplayRenderer == true && !highlightable) return;
        // If we're already at the setting being requested, get outta here
        if (outlinerIsOn == inputShouldDisplayRenderer) return;

        // Record if the outlines are on or not
        outlinerIsOn = inputShouldDisplayRenderer;
        // Turn all the outliners on or off
        foreach (Outline thisOutline in outlineList)
        {
            thisOutline.eraseRenderer = !inputShouldDisplayRenderer;
        }
    }
}

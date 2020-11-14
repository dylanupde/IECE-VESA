using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Child : MonoBehaviour
{
    List<Outline> outlineList;

    // Start is called before the first frame update
    void Start()
    {
        outlineList = new List<Outline>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Outline thisOutline = transform.GetChild(i).GetComponent<Outline>();

            if (thisOutline != null)
            {
                outlineList.Add(thisOutline);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void ShowOutline(bool inputShouldDisplayRenderer)
    {
        foreach (Outline thisOutline in outlineList)
        {
            thisOutline.eraseRenderer = !inputShouldDisplayRenderer;
        }
    }
}

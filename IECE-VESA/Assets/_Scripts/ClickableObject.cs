using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sends the Child a message to turn on or off the outliner when this object is mouse-overed/not mouse-overed/clicked 
/// </summary>
public class ClickableObject : MonoBehaviour
{
    GameManager gameManager;
    Child myChild;

    bool mouseIsOver;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        Transform currentTransform = transform;

        while (currentTransform.parent != null)
        {
            myChild = currentTransform.GetComponent<Child>();

            if (myChild) break;

            currentTransform = currentTransform.parent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        myChild.ShowOutline(mouseIsOver);
    }
    


    void OnMouseEnter()
    {
        mouseIsOver = true;
    }


    void OnMouseExit()
    {
        mouseIsOver = false;
    }


    private void OnMouseDown()
    {
        if (myChild.outlinerIsOn) gameManager.DisplayBio(myChild.gameObject.name);
    }
}

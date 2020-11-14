using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    GameManager gameManager;
    Child child;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        Transform currentTransform = transform;

        while (currentTransform.parent != null)
        {
            child = currentTransform.GetComponent<Child>();

            if (child) break;

            currentTransform = currentTransform.parent;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    


    void OnMouseEnter()
    {
        child.ShowOutline(true);
    }


    void OnMouseExit()
    {
        child.ShowOutline(false);
    }


    private void OnMouseDown()
    {
        gameManager.DisplayBio(child.gameObject.name);
    }
}

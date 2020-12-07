using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stays on an image object and assigns itself to a child based on the name input into the inspector. After "Start" runs, it does nothing
/// </summary>
public class ImageAssigner : MonoBehaviour
{
    [SerializeField] string childName;

    // Start is called before the first frame update
    void Start()
    {
        Bio bio;

        if (GameManager.Instance.biosDict.TryGetValue(childName, out bio))
        {
            bio.bioImageObj = gameObject;
        }
        else
        {
            Debug.LogError("OH NO!!! No child of this name doesn't exist. Type in the child's name associated with this image.", gameObject);
        }

        gameObject.SetActive(false);
    }
}

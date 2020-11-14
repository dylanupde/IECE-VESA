using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void OnClick()
    {
        transform.parent.gameObject.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Called by the GameManager to invoke special events based on the question
/// </summary>
public class SpecialEventInvoker : MonoBehaviour
{
    [SerializeField] Animator giaAnim;
    [SerializeField] Animator marquisAnim;
    [SerializeField] Animator brooksAnim;
    [SerializeField] Animator shreyaAnim;
    [SerializeField] Animator fatimaAnim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Does something based on the question input. To make a question do something, simply add cases to the switch statement
    /// </summary>
    /// <param name="inputKeyString"></param>
    public void AttemptSpecialEvent(string inputKeyString)
    {
        ResetAllIdleTriggers();

        bool giaSet = false;
        bool marquisSet = false;
        bool brooksSet = false;
        bool shreyaSet = false;
        bool fatimaSet = false;

        Debug.Log("<color=red>CALLED</color>" + inputKeyString);

        switch (inputKeyString)
        {
            case "1-10":
                brooksAnim.SetTrigger("getSad");
                brooksSet = true;
                break;
            case "1-11":
                shreyaAnim.SetTrigger("getHappy");
                shreyaSet = true;
                giaAnim.SetTrigger("getScared");
                giaSet = true;
                break;
            case "1-13":
                brooksAnim.SetTrigger("getTalking");
                brooksSet = true;
                break;
            case "1-14":
                marquisAnim.SetTrigger("getLaughing");
                marquisSet = true;
                break;
            case "1-15":
                fatimaAnim.SetTrigger("getAngry");
                fatimaSet = true;
                break;
            default:
                break;
        }


        if (giaSet == false)
        {
            giaAnim.SetTrigger("getIdle");
        }
        if (marquisSet == false)
        {
            marquisAnim.SetTrigger("getIdle");
        }
        if (brooksSet == false)
        {
            brooksAnim.SetTrigger("getIdle");
        }
        if (shreyaSet == false)
        {
            shreyaAnim.SetTrigger("getIdle");
        }
        if (fatimaSet == false)
        {
            fatimaAnim.SetTrigger("getIdle");
        }
    }


    private void ResetAllIdleTriggers()
    {
        giaAnim.ResetTrigger("getIdle");
        marquisAnim.ResetTrigger("getIdle");
        brooksAnim.ResetTrigger("getIdle");
        shreyaAnim.ResetTrigger("getIdle");
        fatimaAnim.ResetTrigger("getIdle");
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes the students start their sitidle animation at different times and different speeds so it's harder to see that they're doing the same thing. This script will likely be modified/scrapped in the future.
/// </summary>
public class AnimationModifier : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] float startFrame = 0f;
    [SerializeField] [Range(0.5f, 1.5f)] float animationSpeed = 1f;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.speed = 0f;
        anim.Play("TK_idle1", 0, startFrame);
        anim.speed = animationSpeed;
    }
}

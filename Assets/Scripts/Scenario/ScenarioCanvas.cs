using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioCanvas : MonoBehaviour
{
    public static ScenarioCanvas instance;

    NameLabel leftNameLabel;
    NameLabel rightNameLabel;

    void Awake()
    {
        instance = this;

        leftNameLabel = transform.Find("Left Character Name").GetComponent<NameLabel>();
        rightNameLabel = transform.Find("Right Character Name").GetComponent<NameLabel>();
    }

    public NameLabel GetRightNameLabel()
    {
        return rightNameLabel;
    }

    public NameLabel GetLeftNameLabel()
    {
        return leftNameLabel;
    }
}

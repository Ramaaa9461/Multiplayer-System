using System.Collections;
using UnityEngine;
using Net;
using System;

public class ReflectionSystem : MonoBehaviour
{
    Reflection reflection;

    private void Start()
    {
        reflection = new();
        reflection.consoleDebugger += WriteConsoleDebugger;
    }

    private void LateUpdate()
    {
        if (reflection != null)
        {
            reflection.UpdateReflection();
        }
    }

    void WriteConsoleDebugger(string message)
    {
        Debug.Log(message);
    }
}

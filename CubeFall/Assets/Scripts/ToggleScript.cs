using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ToggleScript
{
    public static void EnableScript(MonoBehaviour script) // Call this to enable a script
    {
        script.enabled = true;
    }

    public static void DisableScript(MonoBehaviour script) // Call this to disable a script
    {
        script.enabled = false;
    }
}

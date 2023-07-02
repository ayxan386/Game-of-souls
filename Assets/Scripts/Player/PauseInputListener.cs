using System;
using UnityEngine;

public class PauseInputListener : MonoBehaviour
{
    public static event Action<bool> OnPausePressed;

    private void OnPause()
    {
        OnPausePressed?.Invoke(true);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GlobalGameStatus
{
    private static GlobalGameStatus instance = null;
    public static GlobalGameStatus shared
    {
        get
        {
            if (instance == null)
            {
                instance = new GlobalGameStatus();
            }
            return instance;
        }
    }

    public bool userJustDied = false;

    public void StartMusic()
    {
        
    }
}
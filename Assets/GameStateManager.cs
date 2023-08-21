using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this script takes input from one script, and sends it out to all scripts to update
public static class GameStateManager 
{
    public static int gameState = 0;

    public static void SetGameState(int state)
    {
        gameState = state;
    }

    public static int GetGameState()
    {
        return gameState;
    }
}

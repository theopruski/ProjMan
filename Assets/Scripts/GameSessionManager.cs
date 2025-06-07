using System.Diagnostics;
using UnityEngine;

// supprime le high score une fois que le jeu reset
public class GameSessionManager : MonoBehaviour
{
    // verifie si c'est ça ete cleared
    private static bool alreadyCleared = false;

    void Awake()
    {
        if (!alreadyCleared)
        {
            // supprime le score
            PlayerPrefs.DeleteKey("HighScores");
            PlayerPrefs.Save();
            alreadyCleared = true;
        }
    }
}


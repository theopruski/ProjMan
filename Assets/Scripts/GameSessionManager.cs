using System.Diagnostics;
using UnityEngine;

// suprimme le high score une fois que cela reset
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
            alreadyCleared = true; // Ne le refait plus tant que le jeu reste ouvert
        }
    }
}


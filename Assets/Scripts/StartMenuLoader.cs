using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StartMenuLoader : MonoBehaviour
{
    public string mainSceneName = "Projet Manhattan";
    public InputActionProperty launchAction; // commande à utiliser

    // initialisation de démarage du véhicule
    public static bool autoStart = false;

    private void OnEnable()
    {
        // activation du script
        if (launchAction.action != null)
            launchAction.action.Enable();
    }

    private void OnDisable()
    {
        // désactivation du script
        if (launchAction.action != null)
            launchAction.action.Disable();
    }

    private void Update()
    {
        // vérifie si le bouton est pressé
        if (launchAction.action != null && launchAction.action.WasPressedThisFrame())
        {
            // démarrage du véhicule
            autoStart = true;
            Debug.Log("Changement de scène !");
            SceneManager.LoadScene(mainSceneName);
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StartMenuLoader : MonoBehaviour
{
    public string mainSceneName = "Projet Manhattan";
    public InputActionProperty launchAction; // commande � utiliser

    // initialisation de d�marage du v�hicule
    public static bool autoStart = false;

    private void OnEnable()
    {
        // activation du script
        if (launchAction.action != null)
            launchAction.action.Enable();
    }

    private void OnDisable()
    {
        // d�sactivation du script
        if (launchAction.action != null)
            launchAction.action.Disable();
    }

    private void Update()
    {
        // v�rifie si le bouton est press�
        if (launchAction.action != null && launchAction.action.WasPressedThisFrame())
        {
            // d�marrage du v�hicule
            autoStart = true;
            Debug.Log("Changement de sc�ne !");
            SceneManager.LoadScene(mainSceneName);
        }
    }
}

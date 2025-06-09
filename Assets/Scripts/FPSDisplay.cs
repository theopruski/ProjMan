using UnityEngine;
using TMPro;

public class FPSDisplay : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private float deltaTime = 0.0f;

    void Update()
    {
         // Stabiliser l'affichage des FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        // Calcul de FPS
        float fps = 1.0f / deltaTime;
        // Affichage du FPS
        fpsText.text = string.Format("FPS : {0:0.}", fps);
    }
}
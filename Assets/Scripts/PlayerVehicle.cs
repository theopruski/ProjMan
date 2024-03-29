using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerVehicle : MonoBehaviour
{
    private float speed = 10.0f; // vitesse de déplacement du véhicule
    private float turnSpeed = 25.0f; // vitesse de rotation du véhicule
    private float horizontalInput; // input horizontal du joueur
    private float forwardInput; // input vertical du joueur
    private int coinCount; // nombre de pièces collectée
    public float timeLimit = 60f; // limite de temps en secondes
    private float timer; // compteur de temps
    private bool gameOver; // indicateur si le jeu est terminé
    public TextMeshProUGUI counterText; // texte affichant le nombre de pièces collectées
    public TextMeshProUGUI timerText; // texte affichant le compteur de temps
    public TextMeshProUGUI gameOverText; // texte affichant le message de défaite
    public TextMeshProUGUI winText; // texte affichant le message de victoire
    public AudioSource music; // source audio de la musique

    void Start()
    {
        GameObject counterObject = GameObject.Find("Counter"); // trouver l'objet de compteur dans la scène
        coinCount = 0; // initialiser le nombre de pièces collectées
        SetCountText(); // mettre à jour le texte du compteur
        timer = timeLimit; // initialiser le compteur de temps
        gameOverText.gameObject.SetActive(false); // désactiver le texte de défaite
        winText.gameObject.SetActive(false); // désactiver le texte de victoire
    }

    void Update()
    {
        if (!gameOver)
        {
            // récupérer les inputs du joueur
            horizontalInput = Input.GetAxis("Horizontal");
            forwardInput = Input.GetAxis("Vertical");

            // déplacer le véhicule en avant
            transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
            transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);

            // mettre à jour le compteur de temps
            timer -= Time.deltaTime;
            timerText.text = "Timer: " + Mathf.CeilToInt(timer);

            // vérifier si le temps est écoulé
            if (timer <= 0)
            {
                GameOver();
                gameOver = true;
                Time.timeScale = 0;
                gameOverText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!gameOver && other.gameObject.CompareTag("coins"))
        {
            // si la voiture rentre en collision avec l'objet, l'objet est désactiver
            other.gameObject.SetActive(false);
            coinCount = coinCount + 100;
            SetCountText();
        } // vérifier si le jeu est toujours en cours et si l'objet a le tag "FinishLine"
        else if (!gameOver && other.gameObject.CompareTag("FinishLine"))
        {
            Win();
            gameOver = true; // arrêter le jeu
            Time.timeScale = 0;
            winText.gameObject.SetActive(true);
        }
    }

    void SetCountText()
    {
        // mettre à jour le texte du compteur avec le nouveau nombre de pièces collectées
        counterText.text = "Count: " + coinCount.ToString();
    }

    void GameOver()
    {
        // active le texte de défaite, coupe le son du jeu et arrete le jeu
        gameOver = true;
        Time.timeScale = 0;
        gameOverText.gameObject.SetActive(true);
        music.Stop();
    }

    void Win()
    {
        // active le texte de victoire, coupe le son du jeu et arrete le jeu
        gameOver = true;
        Time.timeScale = 0;
        winText.gameObject.SetActive(true);
        music.Stop();
    }
}
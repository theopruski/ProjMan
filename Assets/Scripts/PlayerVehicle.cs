using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlayerVehicle : MonoBehaviour
{
    public GameObject startMenu; // assigner le menu de démarrage dans l'inspecteur
    public GameObject endMenu; // assigner le menu de fin dans l'inspecteur
    public Button startButton; // assigner le bouton de démarrage dans l'inspecteur
    public Button restartButton; // assigner le bouton de redémarrage dans l'inspecteur
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
    private bool hasGameStarted = false; // Vérifie si le jeu a commencé ou non

    void Start()
    {
        GameObject counterObject = GameObject.Find("Counter"); // trouver l'objet de compteur dans la scène
        coinCount = 0; // initialiser le nombre de pièces collectées
        SetCountText(); // mettre à jour le texte du compteur
        timer = timeLimit; // initialiser le compteur de temps
        gameOverText.gameObject.SetActive(false); // désactiver le texte de défaite
        winText.gameObject.SetActive(false); // désactiver le texte de victoire
        startMenu.SetActive(true); // activer le menu de démarrage

        // ajout des écouteurs d'événements aux boutons
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    void Update()
    {
        if (hasGameStarted && !gameOver)
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
        if (hasGameStarted && !gameOver && other.gameObject.CompareTag("coins"))
        {
            // si la voiture rentre en collision avec l'objet, l'objet est désactiver
            other.gameObject.SetActive(false);
            coinCount = coinCount + 100;
            SetCountText();
        } // vérifier si le jeu est toujours en cours et si l'objet a le tag "FinishLine"
        else if (hasGameStarted && !gameOver && other.gameObject.CompareTag("FinishLine"))
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
        // active le menu de fin
        endMenu.SetActive(true);
    }

    void Win()
    {
        // active le texte de victoire, coupe le son du jeu et arrete le jeu
        gameOver = true;
        Time.timeScale = 0;
        winText.gameObject.SetActive(true);
        music.Stop();
        // active le menu de fin
        endMenu.SetActive(true);
    }

    public void StartGame()
    {
        hasGameStarted = true; // le jeu a commencé
        gameOver = false; // réinitialise l'indicateur de fin de jeu
        timer = timeLimit; // réinitialise le compteur de temps
        gameOverText.gameObject.SetActive(false); // désactive le texte de défaite
        winText.gameObject.SetActive(false); // désactive le texte de victoire
        counterText.gameObject.SetActive(true); // active le compteur de pièces
        timerText.gameObject.SetActive(true); // active le compteur de temps
        gameObject.SetActive(true); // active le joueur
        startMenu.SetActive(false); // désactiver le menu de démarrage
        endMenu.SetActive(false); // désactiver le menu de fin
    }

    public void RestartGame()
    {
        hasGameStarted = true; // le jeu a commencé
        gameOver = false; // réinitialiser l'indicateur de fin de jeu
        coinCount = 0; // réinitialiser le nombre de pièces collectées
        SetCountText(); // mettre à jour le texte du compteur
        timer = timeLimit; // réinitialise le compteur de temps
        gameOverText.gameObject.SetActive(false); // désactive le texte de défaite
        winText.gameObject.SetActive(false); // désactive le texte de victoire
        counterText.gameObject.SetActive(true); // activer le compteur de pièces
        timerText.gameObject.SetActive(true); // activer le compteur de temps
        gameObject.SetActive(true); // active le joueur
        endMenu.SetActive(false); // désactive le menu de fin
        // réinitialise la position du joueur
        transform.position = new Vector3(-694.6f, 254.75f, 2756f);
        transform.rotation = Quaternion.Euler(0f, -242.472f, 0f);
        // réinitialise la vitesse et la rotation du véhicule
        speed = 10.0f;
        turnSpeed = 25.0f;
        music.Play(); // réactiver le son
        Time.timeScale = 1; // réinitialise le temps
        gameOver = false; // réinitialise gameOver

    }
}
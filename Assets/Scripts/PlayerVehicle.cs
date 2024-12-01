using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

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
    private bool hasGameStarted = false; // vérifie si le jeu a commencé ou non
    public TextMeshProUGUI LeaderboardText; // texte affichant le message du leaderboard
    public TextMeshProUGUI endGameCountText; // texte affichant le compteur pour le leaderboard
    public TextMeshProUGUI endGameTimerText; // texte affichant le timer pour le leaderboard
    public int coinsLostOnCollisionCar = 50; // nombre de pièce perdu a la collision avec les AIVehicle
    public int coinsLostOnCollisionBus = 75; // nombre de pièce perdu a la collision avec les BusAI
    public List<(int count, float timer)> highScores = new List<(int, float)>();
    public TextMeshProUGUI LeaderboardTab; // texte affichant le timer pour le leaderboard
    public GameObject[] coins; // tableau pour stocker toutes les pièces
    private WeatherController weatherController; // Référence au script weatherController
    public float rainSpeed = 5.0f; // Vitesse réduite sous la pluie
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

        LeaderboardText.gameObject.SetActive(false); // désactiver le texte du leaderboard
        endGameCountText.gameObject.SetActive(false); // désactiver le texte du compteur pour le leaderboard
        endGameTimerText.gameObject.SetActive(false); // désactiver le texte du timer pour le leaderboard
        LeaderboardTab.enabled = false; // désactiver le tableau des scores
        ShowHighScores(); // affichage du tableau des scores
        weatherController = FindObjectOfType<WeatherController>();
    }

    void Update()
    {
        if (hasGameStarted && !gameOver)
        {
            // Ajuster la vitesse en fonction de la pluie
            if (weatherController != null && weatherController.IsRaining)
            {
                speed = rainSpeed; // Réduire la vitesse si la pluie est active
                turnSpeed = 15.0f; // Réduire également la vitesse de rotation
            }
            else
            {
                speed = 10.0f; // Remettre la vitesse normale sinon
                turnSpeed = 25.0f; // Remettre la rotation normale
            }
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

    private void OnCollisionEnter(Collision collision)
    {
        // vérifie si le joueur a percuté une voiture AI
        if (collision.gameObject.CompareTag("AIVehicle"))
        {
            // faire perdre des points
            coinCount -= coinsLostOnCollisionCar;
            // mise à jour du texte du compteur
            SetCountText();
        }
        else if (collision.gameObject.CompareTag("BusAI"))
        {
            // faire perdre des points
            coinCount -= coinsLostOnCollisionBus;
            // mise à jour du texte du compteur
            SetCountText();
        }
    }


    void SetCountText()
    {
        // mettre à jour le texte du compteur avec le nouveau nombre de pièces collectées
        counterText.text = "Count: " + Mathf.Max(coinCount, -1000).ToString();
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
        LeaderboardTab.enabled = true; // activer le tableau des scores
        highScores.Add((coinCount, float.NegativeInfinity)); // ajoute le score actuel du joueur
        ShowHighScores(); // affichage du tableau des scores
        LeaderboardText.gameObject.SetActive(true); // activer le texte du leaderboard
        endGameCountText.gameObject.SetActive(true); // activer le texte du compteur pour le leaderboard
        endGameTimerText.gameObject.SetActive(true); // activer le texte du timer pour le leaderboard
        endGameCountText.text = "Count: " + coinCount.ToString(); // affichage du nombre de point
        endGameTimerText.text = "Timer: DNF"; // affichage du temps écoulé : "Did Not Finish" pour la defaite 
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
        LeaderboardTab.enabled = true; // activer le tableau des score
        highScores.Add((coinCount, timeLimit - timer)); // ajoute le score actuel du joueur
        ShowHighScores(); // affichage du tableau des scores
        endGameCountText.gameObject.SetActive(true); // activer le texte du leaderboard
        endGameTimerText.gameObject.SetActive(true); // activer le texte du compteur pour le leaderboard
        LeaderboardText.gameObject.SetActive(true); // activer le texte du timer pour le leaderboard
        endGameCountText.text = "Count: " + coinCount.ToString(); // affichage du nombre de point
        endGameTimerText.text = "Timer: " + Mathf.CeilToInt(timeLimit - timer).ToString(); // affichage du temps écoulé
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
        transform.position = new Vector3(-791.7f, 254.04f, 2810.54f);
        transform.rotation = Quaternion.Euler(0f, -242.472f, 0f);
        // réinitialise la vitesse et la rotation du véhicule
        speed = 10.0f;
        turnSpeed = 25.0f;
        music.Play(); // réactiver le son
        Time.timeScale = 1; // réinitialise le temps
        gameOver = false; // réinitialise gameOver
        // réactiver toutes les pièces
        foreach (GameObject coin in coins)
        {
            coin.SetActive(true);
        }

    }

    // affiche les 5 meilleurs scores triés par score et temps
    void ShowHighScores()
    {
        var sortedScores = highScores.OrderByDescending(score => score.count) // tri en fonction du nombre de pièces
                                     .ThenBy(score => score.timer == float.NegativeInfinity ? float.PositiveInfinity : score.timer) // tri en fonction du temps (les scores DNF sont mis en dernier)
                                     .ThenByDescending(score => score.count < 0 ? score.count : 0) // tri en fonction des scores négatifs
                                     .ThenBy(score => score.timer == float.NegativeInfinity ? float.PositiveInfinity : score.timer) // tri en fonction du temps pour les scores négatifs
                                     .Take(5); // prend les 5 premiers scores

        // crée une chaîne de caractères pour afficher les scores
        string highScoreText = "High Scores:\n";
        // parcourir les scores triés
        for (int i = 0; i < sortedScores.Count(); i++)
        {
            // formater le temps en chaîne de caractères (afficher "DNF" pour les scores DNF)
            string timerText = (sortedScores.ElementAt(i).timer == float.NegativeInfinity) ? "DNF" : sortedScores.ElementAt(i).timer.ToString("F2");
            // ajoute le score à la chaîne de caractères
            highScoreText += string.Format("{0}. Count: {1}, Timer: {2}\n", i + 1, sortedScores.ElementAt(i).count, timerText);
        }
        // affiche les scores dans l'onglet Leaderboard
        LeaderboardTab.text = highScoreText;
    }
}

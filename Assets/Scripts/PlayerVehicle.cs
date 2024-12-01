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
    public GameObject startMenu; // assigner le menu de d�marrage dans l'inspecteur
    public GameObject endMenu; // assigner le menu de fin dans l'inspecteur
    public Button startButton; // assigner le bouton de d�marrage dans l'inspecteur
    public Button restartButton; // assigner le bouton de red�marrage dans l'inspecteur
    private float speed = 10.0f; // vitesse de d�placement du v�hicule
    private float turnSpeed = 25.0f; // vitesse de rotation du v�hicule
    private float horizontalInput; // input horizontal du joueur
    private float forwardInput; // input vertical du joueur
    private int coinCount; // nombre de pi�ces collect�e
    public float timeLimit = 60f; // limite de temps en secondes
    private float timer; // compteur de temps
    private bool gameOver; // indicateur si le jeu est termin�
    public TextMeshProUGUI counterText; // texte affichant le nombre de pi�ces collect�es
    public TextMeshProUGUI timerText; // texte affichant le compteur de temps
    public TextMeshProUGUI gameOverText; // texte affichant le message de d�faite
    public TextMeshProUGUI winText; // texte affichant le message de victoire
    public AudioSource music; // source audio de la musique
    private bool hasGameStarted = false; // v�rifie si le jeu a commenc� ou non
    public TextMeshProUGUI LeaderboardText; // texte affichant le message du leaderboard
    public TextMeshProUGUI endGameCountText; // texte affichant le compteur pour le leaderboard
    public TextMeshProUGUI endGameTimerText; // texte affichant le timer pour le leaderboard
    public int coinsLostOnCollisionCar = 50; // nombre de pi�ce perdu a la collision avec les AIVehicle
    public int coinsLostOnCollisionBus = 75; // nombre de pi�ce perdu a la collision avec les BusAI
    public List<(int count, float timer)> highScores = new List<(int, float)>();
    public TextMeshProUGUI LeaderboardTab; // texte affichant le timer pour le leaderboard
    public GameObject[] coins; // tableau pour stocker toutes les pi�ces
    private WeatherController weatherController; // R�f�rence au script weatherController
    public float rainSpeed = 5.0f; // Vitesse r�duite sous la pluie
    void Start()
    {
        GameObject counterObject = GameObject.Find("Counter"); // trouver l'objet de compteur dans la sc�ne
        coinCount = 0; // initialiser le nombre de pi�ces collect�es
        SetCountText(); // mettre � jour le texte du compteur
        timer = timeLimit; // initialiser le compteur de temps
        gameOverText.gameObject.SetActive(false); // d�sactiver le texte de d�faite
        winText.gameObject.SetActive(false); // d�sactiver le texte de victoire
        startMenu.SetActive(true); // activer le menu de d�marrage

        // ajout des �couteurs d'�v�nements aux boutons
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);

        LeaderboardText.gameObject.SetActive(false); // d�sactiver le texte du leaderboard
        endGameCountText.gameObject.SetActive(false); // d�sactiver le texte du compteur pour le leaderboard
        endGameTimerText.gameObject.SetActive(false); // d�sactiver le texte du timer pour le leaderboard
        LeaderboardTab.enabled = false; // d�sactiver le tableau des scores
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
                speed = rainSpeed; // R�duire la vitesse si la pluie est active
                turnSpeed = 15.0f; // R�duire �galement la vitesse de rotation
            }
            else
            {
                speed = 10.0f; // Remettre la vitesse normale sinon
                turnSpeed = 25.0f; // Remettre la rotation normale
            }
            // r�cup�rer les inputs du joueur
            horizontalInput = Input.GetAxis("Horizontal");
            forwardInput = Input.GetAxis("Vertical");

            // d�placer le v�hicule en avant
            transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
            transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);

            // mettre � jour le compteur de temps
            timer -= Time.deltaTime;
            timerText.text = "Timer: " + Mathf.CeilToInt(timer);

            // v�rifier si le temps est �coul�
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
            // si la voiture rentre en collision avec l'objet, l'objet est d�sactiver
            other.gameObject.SetActive(false);
            coinCount = coinCount + 100;
            SetCountText();
        } // v�rifier si le jeu est toujours en cours et si l'objet a le tag "FinishLine"
        else if (hasGameStarted && !gameOver && other.gameObject.CompareTag("FinishLine"))
        {
            Win();
            gameOver = true; // arr�ter le jeu
            Time.timeScale = 0;
            winText.gameObject.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // v�rifie si le joueur a percut� une voiture AI
        if (collision.gameObject.CompareTag("AIVehicle"))
        {
            // faire perdre des points
            coinCount -= coinsLostOnCollisionCar;
            // mise � jour du texte du compteur
            SetCountText();
        }
        else if (collision.gameObject.CompareTag("BusAI"))
        {
            // faire perdre des points
            coinCount -= coinsLostOnCollisionBus;
            // mise � jour du texte du compteur
            SetCountText();
        }
    }


    void SetCountText()
    {
        // mettre � jour le texte du compteur avec le nouveau nombre de pi�ces collect�es
        counterText.text = "Count: " + Mathf.Max(coinCount, -1000).ToString();
    }

    void GameOver()
    {
        // active le texte de d�faite, coupe le son du jeu et arrete le jeu
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
        endGameTimerText.text = "Timer: DNF"; // affichage du temps �coul� : "Did Not Finish" pour la defaite 
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
        endGameTimerText.text = "Timer: " + Mathf.CeilToInt(timeLimit - timer).ToString(); // affichage du temps �coul�
    }

    public void StartGame()
    {
        hasGameStarted = true; // le jeu a commenc�
        gameOver = false; // r�initialise l'indicateur de fin de jeu
        timer = timeLimit; // r�initialise le compteur de temps
        gameOverText.gameObject.SetActive(false); // d�sactive le texte de d�faite
        winText.gameObject.SetActive(false); // d�sactive le texte de victoire
        counterText.gameObject.SetActive(true); // active le compteur de pi�ces
        timerText.gameObject.SetActive(true); // active le compteur de temps
        gameObject.SetActive(true); // active le joueur
        startMenu.SetActive(false); // d�sactiver le menu de d�marrage
        endMenu.SetActive(false); // d�sactiver le menu de fin
    }

    public void RestartGame()
    {
        hasGameStarted = true; // le jeu a commenc�
        gameOver = false; // r�initialiser l'indicateur de fin de jeu
        coinCount = 0; // r�initialiser le nombre de pi�ces collect�es
        SetCountText(); // mettre � jour le texte du compteur
        timer = timeLimit; // r�initialise le compteur de temps
        gameOverText.gameObject.SetActive(false); // d�sactive le texte de d�faite
        winText.gameObject.SetActive(false); // d�sactive le texte de victoire
        counterText.gameObject.SetActive(true); // activer le compteur de pi�ces
        timerText.gameObject.SetActive(true); // activer le compteur de temps
        gameObject.SetActive(true); // active le joueur
        endMenu.SetActive(false); // d�sactive le menu de fin
        // r�initialise la position du joueur
        transform.position = new Vector3(-791.7f, 254.04f, 2810.54f);
        transform.rotation = Quaternion.Euler(0f, -242.472f, 0f);
        // r�initialise la vitesse et la rotation du v�hicule
        speed = 10.0f;
        turnSpeed = 25.0f;
        music.Play(); // r�activer le son
        Time.timeScale = 1; // r�initialise le temps
        gameOver = false; // r�initialise gameOver
        // r�activer toutes les pi�ces
        foreach (GameObject coin in coins)
        {
            coin.SetActive(true);
        }

    }

    // affiche les 5 meilleurs scores tri�s par score et temps
    void ShowHighScores()
    {
        var sortedScores = highScores.OrderByDescending(score => score.count) // tri en fonction du nombre de pi�ces
                                     .ThenBy(score => score.timer == float.NegativeInfinity ? float.PositiveInfinity : score.timer) // tri en fonction du temps (les scores DNF sont mis en dernier)
                                     .ThenByDescending(score => score.count < 0 ? score.count : 0) // tri en fonction des scores n�gatifs
                                     .ThenBy(score => score.timer == float.NegativeInfinity ? float.PositiveInfinity : score.timer) // tri en fonction du temps pour les scores n�gatifs
                                     .Take(5); // prend les 5 premiers scores

        // cr�e une cha�ne de caract�res pour afficher les scores
        string highScoreText = "High Scores:\n";
        // parcourir les scores tri�s
        for (int i = 0; i < sortedScores.Count(); i++)
        {
            // formater le temps en cha�ne de caract�res (afficher "DNF" pour les scores DNF)
            string timerText = (sortedScores.ElementAt(i).timer == float.NegativeInfinity) ? "DNF" : sortedScores.ElementAt(i).timer.ToString("F2");
            // ajoute le score � la cha�ne de caract�res
            highScoreText += string.Format("{0}. Count: {1}, Timer: {2}\n", i + 1, sortedScores.ElementAt(i).count, timerText);
        }
        // affiche les scores dans l'onglet Leaderboard
        LeaderboardTab.text = highScoreText;
    }
}

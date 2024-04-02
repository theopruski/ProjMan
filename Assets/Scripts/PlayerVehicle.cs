using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

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
    private bool hasGameStarted = false; // V�rifie si le jeu a commenc� ou non

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
    }

    void Update()
    {
        if (hasGameStarted && !gameOver)
        {
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

    void SetCountText()
    {
        // mettre � jour le texte du compteur avec le nouveau nombre de pi�ces collect�es
        counterText.text = "Count: " + coinCount.ToString();
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
        transform.position = new Vector3(-694.6f, 254.75f, 2756f);
        transform.rotation = Quaternion.Euler(0f, -242.472f, 0f);
        // r�initialise la vitesse et la rotation du v�hicule
        speed = 10.0f;
        turnSpeed = 25.0f;
        music.Play(); // r�activer le son
        Time.timeScale = 1; // r�initialise le temps
        gameOver = false; // r�initialise gameOver

    }
}
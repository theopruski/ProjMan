using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.Rendering.PostProcessing;

public class PlayerVehicle : MonoBehaviour
{
    public GameObject startMenu; // assigner le menu de d�marrage dans l'inspecteur
    public GameObject endMenu; // assigner le menu de fin dans l'inspecteur
    public Button startButton; // assigner le bouton de d�marrage dans l'inspecteur
    public Button restartButton; // assigner le bouton de red�marrage dans l'inspecteur
    public Button professionalRisksButton; // assigner le bouton Professional risks
    public Button centreDuBurnoutButton; // assigner le bouton centreduburnout.org
    public Button infoBurnoutButton; // assigner le bouton Info Burn-out
    private float speed = 10.0f; // vitesse de d�placement du v�hicule
    private float turnSpeed = 25.0f; // vitesse de rotation du v�hicule
    private float horizontalInput; // input horizontal du joueur
    private float forwardInput; // input vertical du joueur
    public int salary; // argent collect�
    public float timeLimit = 60f; // limite de temps en secondes
    public float timer; // compteur de temps
    private bool gameOver; // indicateur si le jeu est termin�
    public TextMeshProUGUI salaryText; // texte affichant l'argent collect�
    public TextMeshProUGUI timerText; // texte affichant le compteur de temps
    public TextMeshProUGUI firedGameOverText; // texte affichant le message de d�faite pour retards
    public TextMeshProUGUI dieGameOverText; // texte affichant le message de d�faite pour vie � z�ro
    public AudioSource music; // source audio de la musique
    private bool hasGameStarted = false; // v�rifie si le jeu a commenc� ou non
    public TextMeshProUGUI LeaderboardText; // texte affichant le message du leaderboard
    public TextMeshProUGUI endGameCountText; // texte affichant le compteur pour le leaderboard
    public TextMeshProUGUI endGameTimerText; // texte affichant le timer pour le leaderboard
    public int salaryLostOnCollisionCar = 50; // argent perdu a la collision avec les AIVehicle
    public int salaryLostOnCollisionBus = 75; // argent de pi�ce perdu a la collision avec les BusAI
    public List<(int salary, float timer)> highScores = new List<(int, float)>();
    public TextMeshProUGUI LeaderboardTab; // texte affichant le timer pour le leaderboard
    //public GameObject[] coins; // tableau pour stocker toutes les pi�ces
    private WeatherController weatherController; // R�f�rence au script weatherController
    public float rainSpeed = 5.0f; // Vitesse r�duite sous la pluie
    public int lateCount = 0; // Compteur de retards
    private const int maxLateCount = 3; // Nombre maximum de retards avant GameOver
    public bool lateCounted = false; // Indicateur si un retard a �t� compt�
    private float totalTimePlayed = 0f; // Temps total pass� en jeu
    public float health = 1f; // Vie du joueur
    public Slider healthBar; // Barre de vie
    private bool wasRaining = false; // �tat pr�c�dent de la pluie
    private bool wasFoggy = false; // �tat pr�c�dent du brouillard
    // Liens de pr�vention
    private string firedLink = "https://www.officiel-prevention.com/dossier/formation/fiches-metier/les-risques-professionnels-des-coursiers";
    private string suicideLink1 = "https://centreduburnout.org/";
    private string suicideLink2 = "https://www.inrs.fr/risques/epuisement-burnout/ce-qu-il-faut-retenir.html";
    public PostProcessVolume postProcessVolume; // R�f�rence au Post-processing
    private Bloom bloom; // R�f�rence au Bloom du Post-processing
    void Start()
    {
        GameObject counterObject = GameObject.Find("Salary"); // trouver l'objet de compteur dans la sc�ne
        salary = 0; // initialiser l'argent collect�
        SetSalaryText(); // mettre � jour le texte du compteur
        timer = timeLimit; // initialiser le compteur de temps
        firedGameOverText.gameObject.SetActive(false); // d�sactiver le texte de d�faite pour retards
        dieGameOverText.gameObject.SetActive(false); // d�sactiver le texte de d�faite pour vie � z�ro
        startMenu.SetActive(true); // activer le menu de d�marrage
        // ajout des �couteurs d'�v�nements aux boutons
        startButton.onClick.AddListener(StartGame);
        restartButton.onClick.AddListener(RestartGame);
        professionalRisksButton.onClick.AddListener(ProfessionalRisks);
        centreDuBurnoutButton.onClick.AddListener(CentreDuBurnout);
        infoBurnoutButton.onClick.AddListener(InfoBurnout);
        LeaderboardText.gameObject.SetActive(false); // d�sactiver le texte du leaderboard
        endGameCountText.gameObject.SetActive(false); // d�sactiver le texte du compteur pour le leaderboard
        endGameTimerText.gameObject.SetActive(false); // d�sactiver le texte du timer pour le leaderboard
        LeaderboardTab.enabled = false; // d�sactiver le tableau des scores
        ShowHighScores(); // affichage du tableau des scores
        weatherController = FindObjectOfType<WeatherController>();
        healthBar.value = health; // Initialisation la barre de vie
        // Initialisation du post-processing
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out bloom);
        }
        // D�sactive les boutons de pr�vention
        professionalRisksButton.gameObject.SetActive(false);
        centreDuBurnoutButton.gameObject.SetActive(false);
        infoBurnoutButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (hasGameStarted && !gameOver)
        {
            // Ajuster la vitesse en fonction de la pluie
            if (weatherController != null)
            {
                if (weatherController.IsRaining && !wasRaining)
                {
                    speed = rainSpeed; // R�duire la vitesse si la pluie est active
                    turnSpeed = 15.0f; // R�duire �galement la vitesse de rotation
                    health -= 0.05f; // Perdre de la vie lorsque la pluie commence
                    wasRaining = true;
                }
                else if (!weatherController.IsRaining && wasRaining)
                {
                    speed = 10.0f; // Remettre la vitesse normale sinon
                    turnSpeed = 25.0f; // Remettre la rotation normale
                    wasRaining = false;
                }

                if (weatherController.IsFoggy && !wasFoggy)
                {
                    health -= 0.03f; // Perdre de la vie lorsque le brouillard commence
                    wasFoggy = true;
                }
                else if (!weatherController.IsFoggy && wasFoggy)
                {
                    speed = 10.0f; // Remettre la vitesse normale sinon
                    turnSpeed = 25.0f; // Remettre la rotation normale
                    wasFoggy = false;
                }
            }
            // r�cup�rer les inputs du joueur
            horizontalInput = Input.GetAxis("Horizontal");
            forwardInput = Input.GetAxis("Vertical");

            // d�placer le v�hicule en avant
            transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
            transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);

            // mettre � jour le compteur de temps
            timer -= Time.deltaTime;
            totalTimePlayed += Time.deltaTime; // mettre � jour le temps total pass� en jeu
            // mettre � jour le texte du compteur et de sa couleur
            if (timer >= 0)
            {
                if (timer <= 10)
                {
                    timerText.color = new Color(1.0f, 0.8f, 0.2f); // couleur orange
                }
                else
                {
                    timerText.color = Color.green; // couleur verte
                }
                timerText.text = "Timer: " + Mathf.CeilToInt(timer);
            }
            else
            {
                timerText.color = Color.red; // couleur rouge
                timerText.text = "Timer: -" + Mathf.Abs(Mathf.CeilToInt(timer));

                // V�rifie si le timer a atteint -10 secondes
                if (timer <= -10 && !lateCounted)
                {
                    // Incr�mente le compteur de retards
                    lateCount++;
                    lateCounted = true; // Retard a �t� compt� pour ce cycle
                    health -= 0.1f; // Perdre de la vie pour le retard

                    // V�rifie si le nombre maximum de retards est atteint
                    if (lateCount >= maxLateCount)
                    {
                        GameOver("You're fired !");
                    }
                }
            }

            // Mettre � jour la barre de vie
            healthBar.value = health;

            // Ajuste l'intensit� du Bloom en fonction de la vie restante
            if (bloom != null)
            {
                bloom.intensity.value = Mathf.Lerp(0f, 50f, 1f - health);
            }

            // V�rifier si la vie est � z�ro
            if (health <= 0)
            {
                GameOver("You commit suicide !");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasGameStarted && !gameOver && other.gameObject.CompareTag("Salary"))
        {
            // si la voiture rentre en collision avec l'objet, l'objet est d�sactiver
            other.gameObject.SetActive(false);
            salary = salary + 100;
            SetSalaryText();
        } 
    }

    private void OnCollisionEnter(Collision collision)
    {
        // v�rifie si le joueur a percut� une voiture AI
        if (collision.gameObject.CompareTag("AIVehicle"))
        {
            // faire perdre des points
            salary -= salaryLostOnCollisionCar;
            // mise � jour du texte du compteur
            SetSalaryText();
            health -= 0.25f; // Perdre 25% de vie
        }
        else if (collision.gameObject.CompareTag("BusAI"))
        {
            // faire perdre des points
            salary -= salaryLostOnCollisionBus;
            // mise � jour du texte du compteur
            SetSalaryText();
            health -= 0.25f; // Perdre 25% de vie
        }

        // Mettre � jour la barre de vie
        healthBar.value = health;

        // Ajuste l'intensit� du Bloom en fonction de la vie restante
        if (bloom != null)
        {
            bloom.intensity.value = Mathf.Lerp(0f, 50f, 1f - health); // Ajuste l'intensit� du Bloom
        }

        // V�rifier si la vie est � z�ro
        if (health <= 0)
        {
            GameOver("You commit suicide !");
        }
    }


    public void SetSalaryText()
    {
        // mettre � jour le texte du compteur avec le nouveau nombre de pi�ces collect�es
        salaryText.text = "Salary: " + Mathf.Max(salary, -100000).ToString();
    }

    public void GameOver(string message)
    {
        // active le texte de d�faite, coupe le son du jeu et arrete le jeu
        gameOver = true;
        Time.timeScale = 0;
        music.Stop();
        // active le menu de fin
        endMenu.SetActive(true);
        LeaderboardTab.enabled = true; // activer le tableau des scores
        //highScores.Add((salary, float.NegativeInfinity)); // ajoute le score actuel du joueur
        highScores.Add((salary, totalTimePlayed)); // Ajouter le score actuel du joueur
        ShowHighScores(); // affichage du tableau des scores
        LeaderboardText.gameObject.SetActive(true); // activer le texte du leaderboard
        endGameCountText.gameObject.SetActive(true); // activer le texte du compteur pour le leaderboard
        endGameTimerText.gameObject.SetActive(true); // activer le texte du timer pour le leaderboard
        endGameCountText.text = "Salary: " + salary.ToString(); // affichage du nombre de point
        endGameTimerText.text = "Timer: " + Mathf.CeilToInt(totalTimePlayed).ToString(); // Affichage du temps pass� en jeu
        // Mettre � jour le texte de fin de jeu en fonction de la raison
        if (message == "You're fired !")
        {
            firedGameOverText.gameObject.SetActive(true);
            dieGameOverText.gameObject.SetActive(false);
            professionalRisksButton.gameObject.SetActive(true);
            centreDuBurnoutButton.gameObject.SetActive(false);
            infoBurnoutButton.gameObject.SetActive(false);
        }
        else if (message == "You commit suicide !")
        {
            firedGameOverText.gameObject.SetActive(false);
            dieGameOverText.gameObject.SetActive(true);
            professionalRisksButton.gameObject.SetActive(false);
            centreDuBurnoutButton.gameObject.SetActive(true);
            infoBurnoutButton.gameObject.SetActive(true);
        }
    }

    public void StartGame()
    {
        hasGameStarted = true; // le jeu a commenc�
        gameOver = false; // r�initialise l'indicateur de fin de jeu
        timer = timeLimit; // r�initialise le compteur de temps
        firedGameOverText.gameObject.SetActive(false); // d�sactive le texte de d�faite pour retards
        dieGameOverText.gameObject.SetActive(false); // d�sactive le texte de d�faite pour vie � z�ro
        salaryText.gameObject.SetActive(true); // active le compteur d'argent
        timerText.gameObject.SetActive(true); // active le compteur de temps
        gameObject.SetActive(true); // active le joueur
        startMenu.SetActive(false); // d�sactiver le menu de d�marrage
        endMenu.SetActive(false); // d�sactiver le menu de fin
        health = 1f; // R�initialiser la vie
        healthBar.value = health; // R�initialiser la barre de vie
        // D�sactive les boutons de pr�vention
        professionalRisksButton.gameObject.SetActive(false);
        centreDuBurnoutButton.gameObject.SetActive(false);
        infoBurnoutButton.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        hasGameStarted = true; // le jeu a commenc�
        gameOver = false; // r�initialiser l'indicateur de fin de jeu
        salary = 0; // r�initialiser le nombre d'argent collect�
        SetSalaryText(); // mettre � jour le texte du compteur
        timer = timeLimit; // r�initialise le compteur de temps
        lateCount = 0; // R�initialise le compteur de retards
        lateCounted = false; // R�initialise l'indicateur de retard compt�
        totalTimePlayed = 0f; // R�initialise le temps total pass� en jeu
        firedGameOverText.gameObject.SetActive(false); // d�sactive le texte de d�faite pour retards
        dieGameOverText.gameObject.SetActive(false); // d�sactive le texte de d�faite pour vie � z�ro
        salaryText.gameObject.SetActive(true); // activer le compteur d'argent
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
        health = 1f; // R�initialiser la vie
        healthBar.value = health; // R�initialiser la barre de vie
        // D�sactive les boutons de pr�vention
        professionalRisksButton.gameObject.SetActive(false);
        centreDuBurnoutButton.gameObject.SetActive(false);
        infoBurnoutButton.gameObject.SetActive(false);
    }

    // affiche les 5 meilleurs scores tri�s par score et temps
    void ShowHighScores()
    {
        var sortedScores = highScores.OrderByDescending(score => score.salary) // tri en fonction du nombre de pi�ces
                                     .ThenBy(score => score.timer == float.NegativeInfinity ? float.PositiveInfinity : score.timer) // tri en fonction du temps (les scores DNF sont mis en dernier)
                                     .ThenByDescending(score => score.salary < 0 ? score.salary : 0) // tri en fonction des scores n�gatifs
                                     .ThenBy(score => score.timer == float.NegativeInfinity ? float.PositiveInfinity : score.timer) // tri en fonction du temps pour les scores n�gatifs
                                     .Take(5); // prend les 5 premiers scores

        // cr�e une cha�ne de caract�res pour afficher les scores
        string highScoreText = "High Scores :\n";
        // parcourir les scores tri�s
        for (int i = 0; i < sortedScores.Count(); i++)
        {
            string timerText = Mathf.CeilToInt(sortedScores.ElementAt(i).timer).ToString();
            // ajoute le score � la cha�ne de caract�res
            highScoreText += string.Format("{0}. Salary : {1}, Timer : {2}\n", i + 1, sortedScores.ElementAt(i).salary, timerText);
        }
        // affiche les scores dans l'onglet Leaderboard
        LeaderboardTab.text = highScoreText;
    }

    // M�thodes pour ouvrir les URL
    public void ProfessionalRisks()
    {
        Application.OpenURL(firedLink);
    }

    public void CentreDuBurnout()
    {
        Application.OpenURL(suicideLink1);
    }

    public void InfoBurnout()
    {
        Application.OpenURL(suicideLink2);
    }
}

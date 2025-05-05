using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;

public class EndMenuLoader : MonoBehaviour
{
    // boutons de l'UI
    public Button restartButton;
    public Button professionalRisksButton;
    public Button centreDuBurnoutButton;
    public Button infoBurnoutButton;

    // textes de l'UI
    public TextMeshProUGUI LeaderboardTab;
    public TextMeshProUGUI endGameCountText;
    public TextMeshProUGUI endGameTimerText;
    public TextMeshProUGUI firedGameOverText;
    public TextMeshProUGUI dieGameOverText;

    // sc�ne principale � recharger
    public string mainSceneName = "Projet Manhattan";

    // commande � utiliser
    public InputActionProperty restartAction;
    public InputActionProperty professionalRisksAction;
    public InputActionProperty centreDuBurnoutAction;
    public InputActionProperty infoBurnoutAction;

    // liste des scores (salaire, temps)
    private List<(int salary, float timer)> highScores = new List<(int, float)>();

    // liens de pr�vention boutons de pr�vention
    private string firedLink = "https://www.officiel-prevention.com/dossier/formation/fiches-metier/les-risques-professionnels-des-coursiers";
    private string suicideLink1 = "https://centreduburnout.org/";
    private string suicideLink2 = "https://www.inrs.fr/risques/epuisement-burnout/ce-qu-il-faut-retenir.html";

    void Start()
    {
        // affecte les �v�nements de clic sur les boutons
        restartButton.onClick.AddListener(RestartGame);
        professionalRisksButton.onClick.AddListener(ProfessionalRisks);
        centreDuBurnoutButton.onClick.AddListener(CentreDuBurnout);
        infoBurnoutButton.onClick.AddListener(InfoBurnout);

        // configure les actions d'entr�e (Input System)
        restartAction.action.performed += _ => RestartGame();
        professionalRisksAction.action.performed += _ => ProfessionalRisks();
        centreDuBurnoutAction.action.performed += _ => CentreDuBurnout();
        infoBurnoutAction.action.performed += _ => InfoBurnout();

        // active les actions d'entr�e
        restartAction.action.Enable();
        professionalRisksAction.action.Enable();
        centreDuBurnoutAction.action.Enable();
        infoBurnoutAction.action.Enable();

        // masque les messages de fin de jeu au d�marrage
        firedGameOverText.gameObject.SetActive(false);
        dieGameOverText.gameObject.SetActive(false);

        // affiche les meilleurs scores
        ShowHighScores();

        // affiche les donn�es de fin de partie si disponibles
        if (!string.IsNullOrEmpty(GameData.gameOverMessage))
        {
            GameOver(GameData.gameOverMessage, GameData.finalSalary, GameData.finalTime);
        }
    }

    // affiche les 5 meilleurs scores tri�s par score et temps
    void ShowHighScores()
    {
        var sortedScores = highScores
            .OrderByDescending(score => score.salary) // tri en fonction du nombre de pi�ces
            .ThenBy(score => score.timer == float.NegativeInfinity ? float.PositiveInfinity : score.timer) // tri en fonction du temps (les scores DNF sont mis en dernier)
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

    // ouvre un lien vers les risques professionnels
    public void ProfessionalRisks()
    {
        Application.OpenURL(firedLink);
    }

    // ouvre un lien vers le centre du burnout
    public void CentreDuBurnout()
    {
        Application.OpenURL(suicideLink1);
    }

    // ouvre un lien vers des infos sur le burnout
    public void InfoBurnout()
    {
        Application.OpenURL(suicideLink2);
    }

    // recharge la sc�ne principale pour recommencer le jeu
    public void RestartGame()
    {
        Time.timeScale = 1f; // Au cas o� le temps serait gel�
        SceneManager.LoadScene(mainSceneName);
    }

    // affiche les messages et donn�es � la fin de la partie
    public void GameOver(string message, int salary, float totalTimePlayed)
    {
        endGameCountText.text = "Salary: " + salary.ToString();
        endGameTimerText.text = "Timer: " + Mathf.CeilToInt(totalTimePlayed).ToString();

        // affiche le message en fonction de la cause du GameOver
        if (message == "You're fired !")
        {
            firedGameOverText.gameObject.SetActive(true);
            dieGameOverText.gameObject.SetActive(false);
        }
        else if (message == "You commit suicide !")
        {
            firedGameOverText.gameObject.SetActive(false);
            dieGameOverText.gameObject.SetActive(true);
        }
    }
}

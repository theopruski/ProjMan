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

    // scène principale à recharger
    public string mainSceneName = "Projet Manhattan";

    // commande à utiliser
    public InputActionProperty restartAction;
    public InputActionProperty professionalRisksAction;
    public InputActionProperty centreDuBurnoutAction;
    public InputActionProperty infoBurnoutAction;

    // liste des scores (salaire, temps)
    private List<(int salary, float timer)> highScores = new List<(int, float)>();

    // liens de prévention boutons de prévention
    private string firedLink = "https://www.officiel-prevention.com/dossier/formation/fiches-metier/les-risques-professionnels-des-coursiers";
    private string suicideLink1 = "https://centreduburnout.org/";
    private string suicideLink2 = "https://www.inrs.fr/risques/epuisement-burnout/ce-qu-il-faut-retenir.html";

    void Start()
    {
        // affecte les événements de clic sur les boutons
        restartButton.onClick.AddListener(RestartGame);
        professionalRisksButton.onClick.AddListener(ProfessionalRisks);
        centreDuBurnoutButton.onClick.AddListener(CentreDuBurnout);
        infoBurnoutButton.onClick.AddListener(InfoBurnout);

        // affiche le score
        LoadHighScores();
        ShowHighScores();

        // configure les actions d'entrée (Input System)
        restartAction.action.performed += _ => RestartGame();
        professionalRisksAction.action.performed += _ => ProfessionalRisks();
        centreDuBurnoutAction.action.performed += _ => CentreDuBurnout();
        infoBurnoutAction.action.performed += _ => InfoBurnout();

        // active les actions d'entrée
        restartAction.action.Enable();
        professionalRisksAction.action.Enable();
        centreDuBurnoutAction.action.Enable();
        infoBurnoutAction.action.Enable();

        // masque les messages de fin de jeu au démarrage
        firedGameOverText.gameObject.SetActive(false);
        dieGameOverText.gameObject.SetActive(false);

        // affiche les meilleurs scores
        ShowHighScores();

        // ➕ Affiche toujours le score final
        GameOver(GameData.gameOverMessage, GameData.finalSalary, GameData.finalTime);
    }


    [System.Serializable]
    public class ScoreEntry
    {
        public int salary;
        public float timer;
    }

    [System.Serializable]
    public class ScoreList
    {
        public List<ScoreEntry> scores = new List<ScoreEntry>();
    }

    // sauvegarde les scores
    void SaveHighScores()
    {
        ScoreList list = new ScoreList();
        foreach (var entry in highScores)
        {
            list.scores.Add(new ScoreEntry { salary = entry.salary, timer = entry.timer });
        }

        string json = JsonUtility.ToJson(list);
        PlayerPrefs.SetString("HighScores", json);
        PlayerPrefs.Save();
    }


    // recupère les scores
    void LoadHighScores()
    {
        string json = PlayerPrefs.GetString("HighScores", "");
        if (!string.IsNullOrEmpty(json))
        {
            ScoreList list = JsonUtility.FromJson<ScoreList>(json);
            highScores = list.scores.Select(e => (e.salary, e.timer)).ToList();
        }
    }


    // montre les 5 plus haut score
    void ShowHighScores()
    {
        string highScoreText = "High Scores :\n";
        for (int i = 0; i < highScores.Count; i++)
        {
            string timerText = Mathf.CeilToInt(highScores[i].timer).ToString();
            highScoreText += $"{i + 1}. Salary : {highScores[i].salary}, Timer : {timerText}\n";
        }
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

    // recharge la scène principale pour recommencer le jeu
    public void RestartGame()
    {
        Time.timeScale = 1f; // Au cas où le temps serait gelé
        SceneManager.LoadScene(mainSceneName);
    }

    // affiche les messages et données à la fin de la partie
    public void GameOver(string message, int salary, float totalTimePlayed)
    {
        // Utilise un message par défaut si aucun n'est fourni
        if (string.IsNullOrEmpty(message))
            message = "Game Over";

        // Met à jour les textes de score final
        endGameCountText.text = "Salary: " + salary.ToString();
        endGameTimerText.text = "Timer: " + Mathf.CeilToInt(totalTimePlayed).ToString();

        // Affiche ou masque les bons éléments selon le type de Game Over
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
        else
        {
            // Cas général pour d'autres messages
            firedGameOverText.gameObject.SetActive(false);
            dieGameOverText.gameObject.SetActive(false);
            professionalRisksButton.gameObject.SetActive(false);
            centreDuBurnoutButton.gameObject.SetActive(false);
            infoBurnoutButton.gameObject.SetActive(false);
        }

        // Enregistre ce score dans la liste des meilleurs scores
        highScores.Add((salary, totalTimePlayed));

        // Trie et garde uniquement les 5 meilleurs
        highScores = highScores
            .OrderByDescending(s => s.salary)
            .ThenBy(s => s.timer)
            .Take(5)
            .ToList();

        // Sauvegarde dans PlayerPrefs
        SaveHighScores();

        // Réaffiche le leaderboard
        ShowHighScores();

    }

}

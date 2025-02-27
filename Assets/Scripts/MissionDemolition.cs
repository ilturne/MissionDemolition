using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public enum GameMode
{
    idle,
    playing,
    levelEnd
}   
public class MissionDemolition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    static public MissionDemolition S; // A private Singleton
    [Header("Set in Inspector")]
    public TMP_Text uitLevel;
    public TMP_Text uitShots;
    public Button uitButton;
    public Vector3 castlePos;
    public GameObject[] castles;
    public GameObject gameOverPanel;

    [Header("Set Dynamically")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle;
    public GameMode mode = GameMode.idle;
    public string showing = "Slingshot";

    public bool ProjectilePowerUp = false;
    public int ProjectilePowerUpCount = 3;

    void Start()
    {
        S = this;
        if (castles == null || castles.Length == 0)
        {
            Debug.LogError("Castles Array is Empty");
        }
        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel()
    {
        if (castle != null)
        {
            Destroy(castle);
        }

        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject pTemp in gos)
        {
            Destroy(pTemp);
        }

        castle = Instantiate(castles[level]); 
        castle.transform.position = castlePos;

        shotsTaken = 0;
        SwitchView("Show Both");
        ProjectileLine.S.Clear();
        Goal.goalMet = false;
        UpdateGUI();
        mode = GameMode.playing;
    }

    void UpdateGUI()
    {
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots Taken: " + shotsTaken;
    }

    void Update()
    {
        UpdateGUI();
        if ((mode == GameMode.playing) && Goal.goalMet && !BadGoal.BadGoalMet)
        {
            mode = GameMode.levelEnd;
            SwitchView("Show Both");
            Invoke("NextLevel", 2f);
        }

        if ((mode == GameMode.playing) && BadGoal.BadGoalMet)
        {
            mode = GameMode.levelEnd;
            SwitchView("Show Both");
            Invoke("GameOver", 2f);
        }

        if (shotsTaken >= 3 && !Goal.goalMet && AllProjectilesStopped()) {
            GameOver();
        }
    }

    public bool AllProjectilesStopped()
    {
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");

        foreach (GameObject proj in projectiles)
        {
            Rigidbody rb = proj.GetComponent<Rigidbody>();
            if (rb != null && rb.linearVelocity.magnitude > 0.1f) // If any projectile is still moving
            {
                return false;
            }
        }

        return true; // Only return true if ALL projectiles have stopped
    }

    void NextLevel()
    {
        level++;
        if (level == levelMax)
        {
            GameOver();
            return;
        }
        StartLevel();
    }

    public void GameOver() {
        gameOverPanel.SetActive(true);
        mode = GameMode.levelEnd;
        SwitchView("Show Both");
    }

    public void RestartGame() {
        gameOverPanel.SetActive(false);
        level = 0;
        shotsTaken = 0;
        mode = GameMode.idle;
        Goal.goalMet = false;
        BadGoal.BadGoalMet = false;
        ProjectilePowerUp = false;
        ProjectilePowerUpCount = 3;
        StartLevel();
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void SwitchView(string eView = "")
    {
        if (eView == "")
        {
            eView = uitButton.GetComponentInChildren<TextMeshProUGUI>().text;
        }
        showing = eView;
        switch (showing)
        {
            case "Show Slingshot":
                FollowCam.POI = null;
                uitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show Castle";
                break;
            case "Show Castle":
                FollowCam.POI = S.castle;
                uitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show Both";
                break;
            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Show Slingshot";
                break;
        }
    }

    public static void ShotFired()
    {
        S.shotsTaken++;

        if (S.ProjectilePowerUp)
        {
            S.ProjectilePowerUpCount--;
            if (S.ProjectilePowerUpCount <= 0)
            {
                S.ProjectilePowerUp = false;
                S.ProjectilePowerUpCount = 3;
            }
        }
    }
}

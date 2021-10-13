using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    static GameController _gc;
    public static GameController gc
    {
        get
        {
            if (_gc == null) _gc = FindObjectOfType<GameController>();
            if (_gc == null) _gc = Instantiate(new GameObject()).AddComponent<GameController>();
            return _gc;
        }
    }

    [SerializeField] float timeToAjust_ = 20;
    public float timeToAjust { get { return timeToAjust_; } }

    public int level;
    public int points { get; private set; }
    public int xp;
    public int bestPoints { get; private set; }
    [SerializeField] TMPro.TextMeshProUGUI pointText;
    [SerializeField] TMPro.TextMeshProUGUI bestPointText;
    [SerializeField] UnityEvent deathEvents;
    [SerializeField] int equippedSkin;


    [SerializeField] int startVelocity;
    [SerializeField] int maxVelocity;
    public static int globalVelocity;
    public Queue<GameObject> entitiesToMoveQueue = new Queue<GameObject>();
    [SerializeField] private List<GameObject> entitiesToMove = new List<GameObject>();
    [SerializeField] private Queue<GameObject> entitiesToDestroy = new Queue<GameObject>();
    public ScoreBeaviour scoreBeaviour { get; private set; }

    private void Start()
    {
        scoreBeaviour = FindObjectOfType<ScoreBeaviour>();
        Application.targetFrameRate = 60;
        LoadGame();
        level = 0;
        globalVelocity = startVelocity;
    }

    #region Move Entities
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R)) ResetSaveGame();
        AddEntities();
        MoveEntity();
        DestroyEntities();
    }

    void AddEntities()
    {
        if (entitiesToMoveQueue == null) return;
        for (int i = 0; i < entitiesToMoveQueue.Count; i++) entitiesToMove.Add(entitiesToMoveQueue.Dequeue());
    }

    void MoveEntity()
    {
        if (entitiesToMove == null) return;
        float t = Time.deltaTime;
        foreach (GameObject trans in entitiesToMove)
        {
            trans.transform.Translate(Vector2.left * globalVelocity * t, Space.World);
            if (trans.transform.position.x < -15) 
                entitiesToDestroy.Enqueue(trans);
        }
    }
    void DestroyEntities()
    {
        if (entitiesToDestroy == null) return;
        for(int i = 0; i < entitiesToDestroy.Count; i++)
        {
            GameObject gam = entitiesToDestroy.Dequeue();
            entitiesToMove.Remove(gam);
            gam.SetActive(false);
        }
    }
    void DestroyAllEntities()
    {
        entitiesToDestroy = new Queue<GameObject>();
        entitiesToMove = new List<GameObject>();
        entitiesToMoveQueue = new Queue<GameObject>();

        AutoMovement[] toDestroy = FindObjectsOfType<AutoMovement>();
        foreach (AutoMovement am in toDestroy) am.gameObject.SetActive(false);
    }
    #endregion

    public void AdPoints()
    {
        if(globalVelocity > 0)
        points++;
        pointText.text = points.ToString("D4");
    }
    public void LevelUp()
    {
        level++;

        switch (level)
        {
            case 1:
                break;
            default:
                if (level > FindObjectOfType<Spawner>().obstacles.Count)
                {
                    globalVelocity = globalVelocity >= maxVelocity ? maxVelocity : globalVelocity++;
                    Debug.Log("New Velocity: " + globalVelocity);
                }
                break;
        }
    }
    public void RestartGame()
    {
        level = 0;
        globalVelocity = startVelocity;

        bestPointText.text = "Best: " + bestPoints.ToString("D4");
        scoreBeaviour.points = points;
        scoreBeaviour.StartCoroutine(scoreBeaviour.UpdateScore());
        points = 0;
        pointText.text = points.ToString("D4");

        deathEvents.Invoke();
        DestroyAllEntities();
    }

    #region SaveGame
    public class SaveProfile
    {
        public int bestPoints;
        public int xp;
    }
    public void SaveGame()
    {
        if (points > bestPoints) bestPoints = points;
        bestPointText.text = "Best: " + bestPoints.ToString("D4");

        SaveProfile saveProfile = new SaveProfile();
        saveProfile.bestPoints = bestPoints;
        saveProfile.xp = xp;

        string saveString = JsonUtility.ToJson(saveProfile);

        File.WriteAllText(Application.persistentDataPath + "/saveFile.json", saveString);

        Debug.Log(saveString);
        Debug.Log(Application.persistentDataPath + "/saveFile.json");
    }
    public void ResetSaveGame()
    {
        SaveProfile saveProfile = new SaveProfile();
        saveProfile.bestPoints = 0;
        saveProfile.xp = 0;

        string saveString = JsonUtility.ToJson(saveProfile);

        File.WriteAllText(Application.persistentDataPath + "/saveFile.json", saveString);

        Debug.Log(saveString);
        Debug.Log(Application.persistentDataPath + "/saveFile.json");

        LoadGame();
    }
    public void LoadGame()
    {
        if (!File.Exists(Application.persistentDataPath + "/saveFile.json")) return;

        SaveProfile loadProfile;
        string loadString = File.ReadAllText(Application.persistentDataPath + "/saveFile.json");

        loadProfile = JsonUtility.FromJson<SaveProfile>(loadString);
        bestPoints = loadProfile.bestPoints;
        xp = loadProfile.xp;

        bestPointText.text = "Best: " + bestPoints.ToString("D4");
        Debug.Log(loadProfile);
    }
    #endregion
}

public static class Extentions
{
    public static GameObject InstantiateFromQueue(GameObject entity, List<AutoMovement> q)
    {
        AutoMovement am = entity.GetComponent<AutoMovement>();

        foreach(AutoMovement a in q)
        {
            if(am.objectTypeId == a.objectTypeId && !a.gameObject.activeInHierarchy)
            {
                a.gameObject.SetActive(true);
                return a.gameObject;
            }
        }
        GameObject g = Object.Instantiate(entity);
        q.Add(g.GetComponent<AutoMovement>());
        return g;
    }
}


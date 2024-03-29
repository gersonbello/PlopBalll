using System.Collections;
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
    [SerializeField] UnityEvent onStart;
    [SerializeField] UnityEvent onGameOver;
    [SerializeField] UnityEvent deathEvents;
    public Skin equippedSkin;

    bool gameStarted;
    [SerializeField] int startVelocity { get { return equippedSkin != null ? equippedSkin.startVelocity : 0; } }
    [SerializeField] int maxVelocity { get { return equippedSkin != null ? equippedSkin.maxVelocity : 0; } }
    public static int globalVelocity;
    public Queue<GameObject> entitiesToMoveQueue = new Queue<GameObject>();
    private List<GameObject> entitiesToMove = new List<GameObject>();
    private Queue<GameObject> entitiesToDestroy = new Queue<GameObject>();
    public ScoreBeaviour scoreBeaviour { get; private set; }
    public PointsIndicator_Behaviour pIBehaviour { get; private set; }
    public SkinMenuBehaviour skinMenuBeaviour { get; private set; }
    public SkinsMananger skinMananger;
    public CameraBehaviour cameraBehaviour { get; private set; }
    public List<Skin> gameSkins = new List<Skin>();

    private void Start()
    {
        scoreBeaviour = FindObjectOfType<ScoreBeaviour>();
        pIBehaviour = FindObjectOfType<PointsIndicator_Behaviour>();

        skinMenuBeaviour = FindObjectOfType<SkinMenuBehaviour>();
        skinMananger = FindObjectOfType<SkinsMananger>();
        cameraBehaviour = FindObjectOfType<CameraBehaviour>();
        Object[] skinsToLoad = Resources.LoadAll("Prefabs/Skins" , typeof(Skin));
        foreach (Object s in skinsToLoad) gameSkins.Add((Skin)s);
        skinMenuBeaviour.skins = gameSkins;

        Application.targetFrameRate = 60;
        LoadGame();
        level = 0;
        globalVelocity = startVelocity;
    }

    #region Move Entities
    private void FixedUpdate()
    {
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
            if (Camera.main.WorldToScreenPoint(trans.transform.position).x < -0.15f) 
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
        pIBehaviour.AddIndicator(1, null);
    }

    public void AdPoints(int pointsToAdd, Color pointsTextColor)
    {
        if (globalVelocity > 0)
            points += pointsToAdd;
        pointText.text = points.ToString("D4");
        pIBehaviour.AddIndicator(pointsToAdd, pointsTextColor);
    }
    public void LevelUp()
    {
        level++;
        switch (level)
        {
            case 1:
                break;
            default:
                    globalVelocity = globalVelocity >= maxVelocity ? maxVelocity : globalVelocity + 1;
                    Debug.Log("New Velocity: " + globalVelocity);
                break;
        }
    }
    public void StartGame()
    {
        if (gameStarted) return;
        gameStarted = true;
        onStart.Invoke();
    }

    public void GameOver()
    {
        gameStarted = false;
        onGameOver.Invoke();
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
        PlayerPrefs.SetInt("BestScore", bestPoints);
        PlayerPrefs.SetInt("Xp", xp);
    }
    public void LoadGame()
    {
        bestPoints = PlayerPrefs.GetInt("BestScore", 0);
        xp = PlayerPrefs.GetInt("Xp", 0);
        bestPointText.text = "Best: " + bestPoints.ToString("D4");
    }
    #endregion
}

public static class Extentions
{
    public static GameObject InstantiateFromQueue(this GameObject G, GameObject entity, List<AutoMovement> q)
    {
        AutoMovement am = entity.GetComponent<AutoMovement>();

        foreach(AutoMovement a in q)
        {
            if(!a.gameObject.activeInHierarchy && am.objectTypeId == a.objectTypeId)
            {
                a.gameObject.SetActive(true);
                return a.gameObject;
            }
        }
        GameObject g = Object.Instantiate(entity);
        q.Add(g.GetComponent<AutoMovement>());
        return g;
    }
    public static GameObject InstantiateFromQueue(this GameObject G, GameObject entity, List<GameObject> q)
    {
        foreach (GameObject a in q)
        {
            if (!a.activeInHierarchy && entity.tag == a.tag)
            {
                a.gameObject.SetActive(true);
                return a.gameObject;
            }
        }
        GameObject g = Object.Instantiate(entity);
        q.Add(g);
        return g;
    }
}


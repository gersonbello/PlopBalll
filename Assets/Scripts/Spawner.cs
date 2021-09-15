using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameController gc;
    public GameObject[] obstacles;
    [SerializeField] int startObstaclesID = 2;
    [SerializeField] List<AutoMovement> amQueue = new List<AutoMovement>();
    

    public void SpawnObject()
    {
        int a = startObstaclesID + gc.level;
        GameObject gO = Extentions.InstantiateFromQueue(obstacles[Random.Range(0, a > obstacles.Length ? obstacles.Length: a)], amQueue);
        gO.transform.position = new Vector3(transform.position.x, gO.transform.position.y, 0);

        float timeToNextSpawn = Random.Range(.25f, Mathf.Max(1f, 1.5f - gc.level / 10));

        Invoke("SpawnObject", timeToNextSpawn); // Garante spawn randomizado
    }

    public void SetEnemyQueue(List<GameObject> enemys)
    {
        amQueue.Clear();
        foreach(GameObject g in enemys)
        {
            amQueue.Add(g.GetComponent<AutoMovement>());
        }
    }

    private void OnEnable() { Invoke("SpawnObject", 3); }

    private void OnDisable() { CancelInvoke("SpawnObject"); }


}

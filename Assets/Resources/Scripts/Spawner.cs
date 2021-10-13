using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public GameController gc;
    public List<GameObject> obstacles = new List<GameObject>();
    [SerializeField] int startObstaclesID = 2;
    [SerializeField] List<AutoMovement> amQueue = new List<AutoMovement>();
    

    public IEnumerator SpawnObject(float time)
    {
        yield return new WaitForSeconds(time);
        int a = startObstaclesID + gc.level;
        GameObject gO = Extentions.InstantiateFromQueue(obstacles[Random.Range(0, a > obstacles.Count ? obstacles.Count: a)], amQueue);
        gO.transform.position = new Vector3(transform.position.x, gO.transform.position.y, 0);

        float timeToNextSpawn = Random.Range(.25f, Mathf.Max(.5f, 1f - gc.level / 10));

        StartCoroutine(SpawnObject(timeToNextSpawn)); // Garante spawn randomizado
    }

    public void SetEnemyQueue(List<GameObject> enemys)
    {
        amQueue.Clear();
        obstacles.Clear();
        foreach(GameObject g in enemys)
        {
            obstacles.Add(g);
        }
    }

    private void OnEnable() { StartCoroutine(SpawnObject(3)); }

    private void OnDisable() { StopAllCoroutines(); }


}

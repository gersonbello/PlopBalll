using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class SkinsMananger : MonoBehaviour
{
    [SerializeField] List<Skin> gameLevelsSkins = new List<Skin>();
    public int levelSkin;
    [SerializeField] Transform GameParent;
    [SerializeField] Transform GroundParent;
    [SerializeField] Spawner spawner;
    GameObject playerRef;
    public bool readyToLevelUp;
    public Image readyToPlopImage;

    [SerializeField] private UnityEvent onSkinChange;

    private void OnEnable()
    {
        GameController.gc.skinMananger = this;
        levelSkin = 0;
        if (GameController.gc.equippedSkin != null) SetSkin(gameLevelsSkins[levelSkin]);
        StopAllCoroutines();
        StartCoroutine(ReadyToPlop());
    }

    public void SetSkin(Skin skinToSet)
    {
        Destroy(GroundParent.GetChild(0).gameObject);
        Instantiate(skinToSet.groundSkin, GroundParent);
        GameObject player = FindObjectOfType<Player>().gameObject;
        Vector3 playerPos = player.transform.position;

        Destroy(player);
        playerRef = Instantiate(skinToSet.playerSkin);
        if (playerPos != null)
        {
            playerRef.transform.position = playerPos;
        }
        playerRef.transform.parent = GameParent;

        FindObjectOfType<Spawner>().SetEnemyQueue(skinToSet.enemysSkins);
    }

    public void LevelUp()
    {
        if (!readyToLevelUp) return;
        readyToLevelUp = false;
        int newLevel = 0;
        do
        {
            newLevel = Random.Range(0, gameLevelsSkins.Count);
        }
        while (newLevel == levelSkin);
        levelSkin = newLevel;
        if(levelSkin < gameLevelsSkins.Count) SetSkin(gameLevelsSkins[levelSkin]);
        GameController.gc.cameraBehaviour.StopCoroutine(GameController.gc.cameraBehaviour.SetBlackCamera());
        GameController.gc.cameraBehaviour.StartCoroutine(GameController.gc.cameraBehaviour.SetBlackCamera());
        onSkinChange.Invoke();
        StartCoroutine(ReadyToPlop());
    }

    public IEnumerator ReadyToPlop()
    {
        readyToPlopImage.color = Color.white;
        float timeToAjust = GameController.gc.timeToAjust;
        float cloack = 0;
        while(cloack < timeToAjust)
        {
            cloack += Time.deltaTime;
            readyToPlopImage.fillAmount = cloack / timeToAjust;
            yield return null;
        }
        readyToPlopImage.color = Color.yellow;
        readyToLevelUp = true;
    }
}

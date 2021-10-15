using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkinsMananger : MonoBehaviour
{
    [SerializeField] Skin startSkin;
    [SerializeField] Transform GameParent;
    [SerializeField] Transform GroundParent;
    [SerializeField] Spawner spawner;

    private void OnEnable()
    {
        if (GameController.gc.equippedSkin != null) SetSkin(GameController.gc.equippedSkin);
    }

    public void SetSkin(Skin skinToSet)
    {
        Destroy(GroundParent.GetChild(0).gameObject);
        Instantiate(skinToSet.groundSkin, GroundParent);

        Destroy(FindObjectOfType<Player>().gameObject);
        Instantiate(skinToSet.playerSkin).transform.parent = GameParent;

        FindObjectOfType<Spawner>().SetEnemyQueue(skinToSet.enemysSkins);
    }
}

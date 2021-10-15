using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Skin", menuName = "Game Skin")]
public class Skin : ScriptableObject
{
    public string skinName = "Normal Mode";

    public int startVelocity = 10;
    public int maxVelocity = 15;
    public GameObject playerSkin;
    public GameObject groundSkin;
    public List<GameObject> enemysSkins;
}


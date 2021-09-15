using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Skin", menuName = "Game Skin")]
public class Skin : ScriptableObject
{
    public GameObject playerSkin;
    public GameObject groundSkin;
    public List<GameObject> enemysSkins;
}


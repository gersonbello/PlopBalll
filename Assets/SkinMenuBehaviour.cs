using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkinMenuBehaviour : MonoBehaviour
{
    public List<Skin> skins = new List<Skin>();

    [SerializeField] int skinIndex;
    [SerializeField] TextMeshProUGUI skinName;

    public void ChangeSkinIndex(int changeBy)
    {
        skinIndex += changeBy;
        skinIndex = (int)Mathf.Repeat(skinIndex += changeBy, skins.Count);
        skinName.text = skins[skinIndex].skinName;
    }

    public void EquipSkin()
    {
        GameController.gc.equippedSkin = skins[skinIndex];
    }
}

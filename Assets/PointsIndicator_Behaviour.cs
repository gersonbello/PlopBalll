using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PointsIndicator_Behaviour : MonoBehaviour
{
    public List<GameObject> pointsIndicators = new List<GameObject>();
    public GameObject pointsIndicatorText;
    public Color baseTextColor;
    public Vector3 pointIndicatorOffset;

    public void AddIndicator(int pointsToAdd, Color? textColor)
    {
        GameObject pointsObject = gameObject.InstantiateFromQueue(pointsIndicatorText, pointsIndicators);
        pointsObject.transform.parent = this.transform;
        pointsObject.transform.position = FindObjectOfType<Player>().transform.position + pointIndicatorOffset;
        TextMeshProUGUI indicatorText = pointsObject.GetComponent<TextMeshProUGUI>();
        indicatorText.text = "+" + pointsToAdd;
        indicatorText.color = textColor.HasValue ? textColor.Value : baseTextColor;
    }
}

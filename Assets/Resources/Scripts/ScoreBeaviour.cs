using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBeaviour : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bestScoreText;
    [SerializeField] Image xpBar;
    [SerializeField] TextMeshProUGUI xpText;
    [SerializeField] TextMeshProUGUI pointsText;
    [SerializeField] GameObject bestIndicator;
    public int points;

    public IEnumerator UpdateScore()
    {
        bestIndicator.SetActive(points >= GameController.gc.bestPoints);
        bestScoreText.text = $"Best: {GameController.gc.bestPoints.ToString().PadLeft(3, '0')}";

        pointsText.text = "00";
        xpBar.fillAmount = GameController.gc.xp / 1000f;

        int oldXp = GameController.gc.xp;
        xpText.text = $"XP: {oldXp.ToString().PadLeft(3, '0')} / 1000";

        GameController.gc.xp += points;
        GameController.gc.SaveGame();
        if (GameController.gc.xp >= 1000)
        {
            GameController.gc.xp -= 1000;
        }

        yield return new WaitForSeconds(1);

        float newPoints = 0;
        while (newPoints < points)
        {
            newPoints++;
            pointsText.text = newPoints.ToString().PadLeft(2, '0');
            yield return null;
        }

        newPoints = 0;
        while (newPoints < points)
        {
            newPoints++;
            oldXp++;
            if(oldXp >= 1000)
            {
                oldXp = 0;
            }
            xpBar.fillAmount = oldXp / 1000f;
            xpText.text =$"XP: {oldXp.ToString().PadLeft(3, '0')} / 1000";
            yield return null;
        }
    }
}

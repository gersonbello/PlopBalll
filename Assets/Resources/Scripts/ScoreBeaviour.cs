using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameAnalyticsSDK;

public class ScoreBeaviour : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bestScoreText;
    [SerializeField] Image xpBar;
    [SerializeField] TextMeshProUGUI xpText;
    [SerializeField] TextMeshProUGUI pointsText;
    [SerializeField] GameObject bestIndicator;
    public int points;
    public float maxXp = 10000;

    public IEnumerator UpdateScore()
    {
        bestIndicator.SetActive(points >= GameController.gc.bestPoints);
        bestScoreText.text = $"Best: {GameController.gc.bestPoints.ToString().PadLeft(3, '0')}";

        if(points >= GameController.gc.bestPoints) GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, points.ToString());

        pointsText.text = "00";
        xpBar.fillAmount = GameController.gc.xp / maxXp;

        int oldXp = GameController.gc.xp;
        xpText.text = $"XP: {oldXp.ToString().PadLeft(3, '0')} / {maxXp}";

        GameController.gc.xp += points;
        GameController.gc.SaveGame();
        if (GameController.gc.xp >= maxXp)
        {
            GameController.gc.xp -= (int)maxXp;
        }

        yield return new WaitForSeconds(1);

        float newPoints = 0;
        while (newPoints < points - 1)
        {
            newPoints = Mathf.Lerp(newPoints, points, 2 * Time.deltaTime);
            pointsText.text = newPoints.ToString("F0").PadLeft(2, '0');
            yield return null;
        }
        newPoints = points;
        pointsText.text = newPoints.ToString("F0").PadLeft(2, '0');
        newPoints = oldXp;
        float newXp = GameController.gc.xp;
        while (newPoints < newXp -1)
        {
            newPoints = Mathf.Lerp(newPoints, newXp, 2 * Time.deltaTime);
            if(newPoints >= maxXp)
            {
                newPoints = 0;
            }
            xpBar.fillAmount = newPoints / maxXp;
            xpText.text =$"XP: {newPoints.ToString("F0").PadLeft(3, '0')} / {maxXp}";
            yield return null;
        }
        oldXp = GameController.gc.xp;
        if (oldXp >= maxXp)
        {
            oldXp -= (int)maxXp;
        }
        xpBar.fillAmount = oldXp / maxXp;
        xpText.text = $"XP: {oldXp.ToString("F0").PadLeft(3, '0')} / {maxXp}";
    }
}

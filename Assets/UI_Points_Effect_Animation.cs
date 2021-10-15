using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Points_Effect_Animation : MonoBehaviour
{
    Vector2 startPos;
    public float animationSpeed;
    private void OnEnable()
    {
        startPos = new Vector3();
        transform.position = startPos;
        StopAllCoroutines();
        StartCoroutine(MoveUpAndFade());
    }

    IEnumerator MoveUpAndFade()
    {
        CanvasGroup cG = GetComponent<CanvasGroup>();
        cG.alpha = 1;
        while(cG.alpha > 0)
        {
            float animSpeed = animationSpeed * Time.deltaTime;
            cG.alpha -= animSpeed;
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y + animSpeed);
            transform.position = newPos;
            transform.Translate(Vector2.left * GameController.globalVelocity * Time.deltaTime, Space.World);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}

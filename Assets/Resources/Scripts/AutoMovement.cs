using UnityEngine;
using System.Collections;

public class AutoMovement : MonoBehaviour
{
    [Tooltip("Set the type of the enemy to be compared when the object pool is veryfied, 0 - efect1, 1 - efect2, other number are enemys")]
    public int objectTypeId;
    [Tooltip("Turn on/off animation for y axis")]
    public bool animateY = true;

    /// <summary>
    /// Adds the object to the movement queue in GameController
    /// </summary>
    void OnEnable()
    {
        GameController.gc.entitiesToMoveQueue.Enqueue(gameObject);
        if(animateY) StartCoroutine(AnimateY());
    }

    IEnumerator AnimateY()
    {
        float oldY = transform.position.y;
        float newY = oldY * 5;
        transform.position = new Vector2(transform.position.x, newY);
        yield return new WaitForSeconds(.2f);
        while (Mathf.Abs(newY) - Mathf.Abs(oldY) > .01f)
        {
            Vector3 newPos = transform.position;
            newY = Mathf.Lerp(newY, oldY, 10 * Time.deltaTime);
            newPos.y = newY;
            transform.position = newPos;
            yield return null;
        }
    }
}

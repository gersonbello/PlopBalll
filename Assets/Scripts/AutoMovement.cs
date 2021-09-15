using UnityEngine;

public class AutoMovement : MonoBehaviour
{
    public int objectTypeId;

    void OnEnable()
    {
        GameController.entitiesToMoveQueue.Enqueue(gameObject);
    }
}

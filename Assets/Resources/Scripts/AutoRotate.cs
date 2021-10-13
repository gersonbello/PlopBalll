using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] Vector3 rotateDirection;
    void FixedUpdate() { transform.Rotate(rotateDirection); }
}

using System.Collections;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    Vector3 startPos;
    [SerializeField] float cameraSpeed = .25f;
    [SerializeField] float ajustColorSpeed = .25f;

    void Start()
    {
        startPos = transform.position;
    }
    public IEnumerator SetCameraPosition()
    {
        while (transform.position != startPos)
        {
            transform.position = Vector3.Lerp(transform.position, startPos, cameraSpeed);
            yield return null;
        }
    }

    public IEnumerator SetBlackCamera()
    {
        Camera cam = FindObjectOfType<Camera>();
        while (cam.backgroundColor != new Color())
        {
            Color newColor = Color.Lerp(cam.backgroundColor, new Color(), ajustColorSpeed);
            cam.backgroundColor = newColor;
            yield return null;
        }
    }
}

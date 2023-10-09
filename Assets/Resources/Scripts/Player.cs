using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    [SerializeField] protected bool GameOver;
    [SerializeField] protected float jumpForce, airJumpForce, fallForce, fallJumpForce;
    [SerializeField] protected ParticleSystem groundHitEffect;
    [SerializeField] protected ParticleSystem groundSmashEffect;
    [SerializeField] protected Color camGroundHitColor, camDeathColor;
    [SerializeField] protected List<AutoMovement> amQueue = new List<AutoMovement>();

    [Header("Animation Config")]
    [SerializeField] protected GameObject playerSkin;
    [SerializeField] protected bool rotateAtContact;

    [Header("Events")]
    [SerializeField] private UnityEvent onGroundHit;
    [SerializeField] private UnityEvent onGroundHitLight;
    [SerializeField] private UnityEvent onGroundHitHeavy;
    [SerializeField] private UnityEvent onJump;
    [SerializeField] private UnityEvent onDie;

    protected Rigidbody2D rig;

    protected Vector3 startPos, bottomHitPos;

    protected bool onGround, jump, fallJump, forceFalling;

    [SerializeField] private float timeBeforeEnablePhysics = 3;
    private void Start()
    {
        startPos = transform.position;
        if (timeBeforeEnablePhysics <= 0) return;
        rig.isKinematic = true;
        StartCoroutine(TurnOffKinematicAfter());
    }

    IEnumerator TurnOffKinematicAfter()
    {
        float cloack = 0;
        while (cloack < timeBeforeEnablePhysics)
        {
            if (Input.anyKeyDown) break;
            cloack += Time.deltaTime;
            yield return null;
        }
        fallJump = false;
        rig.isKinematic = false;
        rig.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
    }

    void OnEnable()
    {
        rig = GetComponent<Rigidbody2D>();
        rig.constraints = RigidbodyConstraints2D.FreezePositionX;

        GameController gc = FindObjectOfType<GameController>();
        gc.CancelInvoke();
        gc.InvokeRepeating("LevelUp", gc.timeToAjust, gc.timeToAjust);

        bottomHitPos = Vector3.up * GetComponent<CircleCollider2D>().radius;
        GameController.gc.StartGame();
    }

    void Update() { Controls(); }

    public void Controls()
    {
        if (GameOver) return;

        //Touch
        if (Input.GetMouseButtonDown(0))
        {
            Camera cam = FindObjectOfType<Camera>();
            if (cam.ScreenToViewportPoint(Input.mousePosition).x < .5f && !jump && !onGround)
            {
                jump = true;
                onJump.Invoke();
                ImpulseBall(airJumpForce);
            }
            else
            {
                if (cam.ScreenToViewportPoint(Input.mousePosition).x > .5f && !fallJump)
                {
                    forceFalling = true;
                    fallJump = true;
                    ImpulseBall(-fallForce);
                }
            }
            return;
        }

        //Arrow
        if (Input.GetKeyDown(KeyCode.UpArrow) && !jump && !onGround)
        {
            jump = true;
            onJump.Invoke();
            ImpulseBall(airJumpForce);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && !fallJump)
        {
            forceFalling = true;
            fallJump = true;
            ImpulseBall(-fallForce);
        }
        if (onGround && rig.velocity.normalized == new Vector2())
        {
            forceFalling = false;
            jump = false;
            onGround = true;
            rig.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void ImpulseBall(float force)
    {
        rig.velocity = new Vector2();
        rig.AddForce(Vector3.up * force, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        StopCoroutine(nameof(RotateForTime));
        //if (rotateAtContact) StartCoroutine(RotateForTime(playerSkin, new Vector3(0, 0, Random.Range(-1.5f, -1f)), 3));

        if ((coll.transform.CompareTag("ground") || coll.transform.CompareTag("static ground")) && !GameOver)
        {
            onGroundHit.Invoke();
            if (groundHitEffect != null) gameObject.InstantiateFromQueue(groundHitEffect.gameObject, amQueue).transform.position = transform.position - bottomHitPos;
            if (forceFalling)
            {
                onGroundHitHeavy.Invoke();
                if(coll.transform.CompareTag("ground")) GameController.gc.AdPoints(5 + GameController.gc.level, Color.cyan);
                else GameController.gc.AdPoints(1 + GameController.gc.level, Color.grey);
                forceFalling = false;
                ImpulseBall(fallJumpForce);

                if (groundSmashEffect != null) gameObject.InstantiateFromQueue(groundSmashEffect.gameObject, amQueue).transform.position = transform.position;

                Camera cam = FindObjectOfType<Camera>();
                cam.backgroundColor = camGroundHitColor;
                cam.gameObject.transform.position += Vector3.up * .5f;
                GameController.gc.cameraBehaviour.StopCoroutine(cam.GetComponent<CameraBehaviour>().SetBlackCamera());
                GameController.gc.cameraBehaviour.StartCoroutine(cam.GetComponent<CameraBehaviour>().SetBlackCamera());

                CameraBehaviour CB = cam.GetComponent<CameraBehaviour>();
                CB.StopCoroutine(CB.SetCameraPosition());
                CB.StartCoroutine(CB.SetCameraPosition());
                if (GameController.gc.skinMananger != null) GameController.gc.skinMananger.LevelUp();
            }
            else
            {
                GameController.gc.AdPoints(1 + GameController.gc.level, Color.grey);
                onGroundHitLight.Invoke();
                ImpulseBall(jumpForce);
            }
            jump = false;
            onGround = true;
        }
        else if (coll.transform.CompareTag("enemy"))
        {
            if (GameOver) return;
            rig.constraints = RigidbodyConstraints2D.None;
            rig.AddForce(Vector3.left * airJumpForce, ForceMode2D.Impulse);
            GameController.globalVelocity = 0;
            GameController gc = FindObjectOfType<GameController>();
            gc.CancelInvoke();
            StartCoroutine(Death());
        }
    }

    public IEnumerator Death()
    {
        GameOver = true;
        onDie.Invoke();
        GameController.gc.GameOver();
        Camera cam = FindObjectOfType<Camera>();
        cam.backgroundColor = camDeathColor;
        cam.gameObject.transform.position += Vector3.up * .5f;

        CameraBehaviour CB = cam.GetComponent<CameraBehaviour>();
        StopCoroutine(CB.SetBlackCamera());
        StartCoroutine(CB.SetBlackCamera());
        CB.StopCoroutine(CB.SetCameraPosition());
        CB.StartCoroutine(CB.SetCameraPosition());

        yield return new WaitForSeconds(2);

        Time.timeScale = 1;

        GameOver = false;

        transform.position = startPos;

        GameController.gc.RestartGame();
        StopAllCoroutines();

    }

    public IEnumerator RotateForTime(GameObject rotateObject, Vector3 rotateSpeed, float timeToRotate)
    {
        float zSpeed = rotateSpeed.z;
        while (timeToRotate > 0 || zSpeed < 0)
        {
            timeToRotate -= Time.deltaTime;
            rotateObject.transform.Rotate(rotateSpeed);
            zSpeed -= zSpeed / timeToRotate;
            rotateSpeed.z = zSpeed;
            yield return null;
        }
    }

    private void OnCollisionExit2D(Collision2D coll)
    {
        if ((coll.transform.CompareTag("ground") || coll.transform.CompareTag("static ground")))
        {
            onGround = false;
            fallJump = false;
        }
    }
}

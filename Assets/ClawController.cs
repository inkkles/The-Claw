using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClawController : MonoBehaviour
{
    [SerializeField] GameObject Claw;
    [SerializeField] GameObject ClawVisual;
    [SerializeField] GameObject OnlyCollide;

    public GameObject[] gachaPrefabs;
    public AudioSource grabSound;

    public bool playerIsCaught = false;

    public static ClawController Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [SerializeField] LineRenderer lineRenderer1;
    [SerializeField] LineRenderer lineRenderer2;
    public Material lineMaterial;
    public float lineWidth;

    public bool isOnPlayer = false;
    private Collider col;



    private Vector2 stick;

    [SerializeField] public int numberOfTries = 3;

    [SerializeField] private float timeBetweenTries = 2f;
    [SerializeField] private float controllerDeadzone = 0.1f;

    public Vector2 xBounds;
    public Vector2 zBounds;
    private Vector3 BackPos;
    private Vector3 FrontPos;
    private Vector3 LeftPos;
    private Vector3 RightPos;

    //NOTE:  Z IS RIGHT(Z-) AND LEFT(Z+)
    //  AND  X IS FRONT(X+) AND BACK(X-)
    private void UpdatePositionRefs()
    {
        BackPos = new Vector3(xBounds.y, Claw.transform.position.y, Claw.transform.position.z);
        FrontPos = new Vector3(xBounds.x, Claw.transform.position.y, Claw.transform.position.z);
        LeftPos = new Vector3(Claw.transform.position.x, Claw.transform.position.y, zBounds.x);
        RightPos = new Vector3(Claw.transform.position.x, Claw.transform.position.y, zBounds.y);

        lineRenderer1.SetPosition(0, new Vector3(BackPos.x, transform.position.y, BackPos.z));
        lineRenderer1.SetPosition(1, new Vector3(FrontPos.x, transform.position.y, FrontPos.z));
        lineRenderer1.transform.position = new Vector3(lineRenderer1.transform.position.x,
            lineRenderer1.transform.position.y, BackPos.z);
        lineRenderer2.SetPosition(0, new Vector3(LeftPos.x, transform.position.y, LeftPos.z));
        lineRenderer2.SetPosition(1, new Vector3(RightPos.x, transform.position.y, RightPos.z));
        lineRenderer2.transform.position = new Vector3(lineRenderer2.transform.position.x,
            RightPos.y, lineRenderer2.transform.position.z);
    }

    private void OnDrawGizmos()
    {
        UpdatePositionRefs();
        //The ways these are drawn is purposely cross-like
        Gizmos.color = Color.red;
        Gizmos.DrawLine(BackPos, FrontPos);
        Gizmos.DrawLine(LeftPos, RightPos);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(BackPos, 0.1f);
        Gizmos.DrawSphere(RightPos, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(FrontPos, 0.1f);
        Gizmos.DrawSphere(LeftPos, 0.1f);
    }


    enum State
    {
        MOVING,
        NEUTRAL,
        GRABBING
    }

    private State state;
    private float grabTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        grabSound = GetComponent<AudioSource>();
        state = State.NEUTRAL;
        grabTimer = timeBetweenTries;
        col = GetComponent<Collider>(); //our box collider that is at player level



    }


    void Update()
    {

        if (state == State.GRABBING) return;

        if (Input.GetKeyDown(KeyCode.O))
        {
            grabSound.Play();
            state = State.GRABBING;

            StartCoroutine(Grab());
        }
        // grabTimer -= Time.deltaTime;

        // if (grabTimer <= 0)
        // {
        //     grabTimer = timeBetweenTries;
        //     state = State.GRABBING;
        //     StartCoroutine(Grab());
        // }

        stick = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (stick.magnitude > controllerDeadzone)
        {

            state = State.MOVING;
        }

    }

    public float clawSpeed;
    public float clawAcceleration;

    private float horizontal;
    private float vertical;

    //NOTE:  Z IS RIGHT(Z-) AND LEFT(Z+)
    //  AND  X IS FRONT(X+) AND BACK(X-)

    // claw state machine
    void LateUpdate()
    {
        switch (state)
        {
            case State.NEUTRAL:
                break;
            case State.MOVING:

                //logic for converting normal analog sticks into digital joystick (dont ask why i did this, i wanted controller players to feel the same as the people at the arcade machine)
                //also for moving you move sqrt(2) times faster when going diagonally since i add vertical and horizontal distinctly
                //(thats claw tech)
                //vertical
                if (Mathf.Abs(stick.y) > controllerDeadzone)
                {
                    Claw.transform.position = Vector3.MoveTowards(Claw.transform.position, stick.y > 0 ? BackPos : FrontPos, clawSpeed * Time.deltaTime);
                }

                if (Mathf.Abs(stick.x) > controllerDeadzone)
                {
                    Claw.transform.position = Vector3.MoveTowards(Claw.transform.position, stick.x > 0 ? RightPos : LeftPos, clawSpeed * Time.deltaTime);
                }
                break;
            case State.GRABBING:
                break;
        }
    }

    [SerializeField] private float grabSpeed;
    public IEnumerator Grab()
    {
        Vector3 targetPosition = OnlyCollide.transform.position;
        // Claw.transform.localPosition = new Vector3(0, 0, 0); 

        Vector3 originalPosition = Claw.transform.position;
        float elapsed = 0f;
        ClawVisual.GetComponent<SpriteRenderer>().sprite = ClawVisual.GetComponent<GrabbingClaw>().open;
        while (elapsed < grabSpeed)
        {
            Claw.transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsed / grabSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Claw.transform.position = targetPosition;
        if (ClawVisual.GetComponent<GrabbingClaw>().grabbedPlayer)
        {
            isOnPlayer = true;
            ClawVisual.GetComponent<SpriteRenderer>().sprite = ClawVisual.GetComponent<GrabbingClaw>().closed;
            //Debug.Log("HIT");
            StartCoroutine(caughtPlayer());
        }
        else if (ClawVisual.GetComponent<GrabbingClaw>().grabbedGacha)
        {
            isOnPlayer = false;
            ClawVisual.GetComponent<SpriteRenderer>().sprite = ClawVisual.GetComponent<GrabbingClaw>().closed;
            //Debug.Log("HIT GACHA");
            GameObject gacha = ClawVisual.GetComponent<GrabbingClaw>().gachaGrab;
            gacha.transform.parent = Claw.transform;
            gacha.GetComponent<Rigidbody>().isKinematic = true;
            gacha.GetComponent<Collider>().enabled = false;
            ClawVisual.GetComponent<GrabbingClaw>().grabbedGacha = false;
            ClawVisual.GetComponent<GrabbingClaw>().gachaGrab = null;
            //Instantiate(gachaPrefabs[UnityEngine.Random.Range(0, gachaPrefabs.Length)], originalPosition, Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0));
            Destroy(gacha, 3f);
            
        }
        else
        {
            isOnPlayer = false;
            //Debug.Log("MISSED");
            ClawVisual.GetComponent<SpriteRenderer>().sprite = ClawVisual.GetComponent<GrabbingClaw>().closed;

        }
        //yield return new WaitForSeconds(0.2f); //wait a moment at the bottom
        ClawVisual.GetComponent<SpriteRenderer>().sprite = ClawVisual.GetComponent<GrabbingClaw>().closed;
        float elapsed1 = 0f;
        while (elapsed1 < grabSpeed)
        {
            Claw.transform.position = Vector3.Lerp(targetPosition, originalPosition, elapsed1 / grabSpeed);
            elapsed1 += Time.deltaTime;
            yield return null;
        }
        Claw.transform.position = originalPosition;
        if (!isOnPlayer)
        {
           numberOfTries--; 
        }
        
        //play grab animation
        //yield return new WaitForSeconds(grabSpeed); //wait until animation is don
        //check if our collider is touching the player



        //finish doing grab things
        state = State.NEUTRAL;
    }


    IEnumerator caughtPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject angel = GameObject.FindWithTag("Angel");
        angel.GetComponent<Animator>().SetBool("isAfter", true);
        player.GetComponent<PlayerController>().enabled = false;
        player.GetComponent<Rigidbody>().isKinematic = true;
        player.GetComponentInChildren<Animator>().SetBool("Caught", true);
        player.transform.parent = Claw.transform;
        player.transform.localPosition = new Vector3(-6.5f, 0, 0);
        yield return new WaitForSeconds(6f);
        playerIsCaught = true;
        //return null;
        // player.transform.localPosition = new Vector3(-6.5f, 0,
        // player.transform.localRotation = Quaternion.Euler(player.transform.localRotation.x, 111f, player.transform.localRotation.z);
        // Debug.Log("Caught player");
    }




}

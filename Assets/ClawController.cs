using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ClawController : MonoBehaviour
{
    public static ClawController Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private Vector2 stick;

    [SerializeField] int numberOfTries = 3;

    [SerializeField] private float timeBetweenTries = 2f;
    [SerializeField] private float controllerDeadzone = 0.1f;

    public Vector2 xBounds;
    public Vector2 zBounds;

    private void OnDrawGizmos()
    {
        //The ways these are drawn is purposely cross-like
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(xBounds.x, transform.position.y, transform.position.z),
            new Vector3(xBounds.y, transform.position.y, transform.position.z));
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y, zBounds.x), new Vector3(transform.position.x, transform.position.y, zBounds.y));
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
        state = State.NEUTRAL;
        grabTimer = timeBetweenTries;
    }

    
    void Update()
    {
        if (state == State.GRABBING) return;
        grabTimer -= Time.deltaTime;
        
        if (grabTimer <= 0)
        {
            grabTimer = timeBetweenTries;
            state = State.GRABBING;
            StartCoroutine(Grab());
        }
        
        stick = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (stick.magnitude > controllerDeadzone)
        {
            state = State.MOVING;
        }
        
    }

    public float clawSpeed;


    private float horizontal;

    private float vertical;

    public float clawAcceleration;
    // claw state machine
    void LateUpdate()
    {
        switch (state)
        {
            case State.MOVING:
                if (Mathf.Abs(stick.y) <= controllerDeadzone)
                    horizontal = Mathf.MoveTowards(horizontal, 0, Time.deltaTime * clawAcceleration);
                else horizontal = 0;
                if (Mathf.Abs(stick.x) <= controllerDeadzone)
                    vertical = Mathf.MoveTowards(vertical, 0, Time.deltaTime * clawAcceleration);
                else vertical = 0;
                
                Vector3 targetPosition = new Vector3(
                    Mathf.Clamp(transform.position.x + horizontal, xBounds.x, xBounds.y), transform.position.y,
                    Mathf.Clamp(transform.position.z + vertical, zBounds.x, zBounds.y));

                transform.position = targetPosition;
                break;
            case State.NEUTRAL:
                break;
            case State.GRABBING:
                break;
        }
    }

    public IEnumerator Grab()
    {
        //do grab things
        yield return new WaitForSeconds(1f);
        
        
        //finish doing grab things
        state = State.NEUTRAL;
    }

    
}

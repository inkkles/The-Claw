using System.Collections;
using UnityEngine;

public class GrabbingClaw : MonoBehaviour
{
    public Sprite open;
    public Sprite closed;
    private SpriteRenderer spriteRenderer;
    public bool grabbedPlayer = false;
    public bool grabbedGacha = false;
    public GameObject gachaGrab; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = open;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            grabbedPlayer = true;
            // Debug.Log("GRABBED");
            StartCoroutine(resetGrab());
        }
        else if (other.CompareTag("Gacha"))
        {
            //other.transform.parent = gameObject.transform;
          //  Debug.Log("GACHA");
            grabbedGacha = true;
            gachaGrab = other.gameObject;
            // Debug.Log("HIT GROUND");
            //spriteRenderer.sprite = closed;
            
            
        }
    }

    IEnumerator resetGrab()
    {
       // spriteRenderer.sprite = closed;
        yield return new WaitForSeconds(3f);
       // spriteRenderer.sprite = open;
        grabbedPlayer = false;
    }
    

}

using UnityEngine;

public class Populator : MonoBehaviour
{
    public GameObject[] options;

    public int count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(options[Random.Range(0, options.Length)],
                new Vector3(Random.Range(ClawController.Instance.xBounds.x, ClawController.Instance.xBounds.y),
                    transform.position.y,
                    Random.Range(ClawController.Instance.zBounds.x, ClawController.Instance.zBounds.y)),
                Quaternion.Euler(new Vector3(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f))))
                .transform.SetParent(this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

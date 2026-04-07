using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ballon : MonoBehaviour
{
    public Button button1;
    public float airAdded = 50f;
    public float airLoss = 50f;

    private Vector3 originalPosition;

    void Start()
    {

        originalPosition = transform.position;
        button1.onClick.AddListener(() =>
        {
            transform.localScale += Vector3.one * airAdded;
        });
    StartCoroutine(WaitOneFrame(2.5f));

    }

    void Update()
    {
        transform.localScale -= Vector3.one * Time.deltaTime * airLoss;


//le shaking
        if (transform.localScale.x >= 300)
        {
            float intensity = (transform.localScale.x - 300f) / 200f; 
            transform.position = originalPosition + new Vector3(
                Random.Range(-0.2f, 0.2f) * intensity,
                Random.Range(-0.2f, 0.2f) * intensity,
                0
            );
        }
        else
        {
            transform.position = originalPosition;
        }

        if (transform.localScale.x <= 0)
        {
            Destroy(gameObject);
        }
        if (transform.localScale.x >= 500)
        {
            Destroy(gameObject);
        }


    }

    IEnumerator WaitOneFrame(float timeToWait) 
    {
        yield return new WaitForSeconds(timeToWait);
        airLoss = Random.Range(50f, 220f);

        Debug.Log("cc");
        StartCoroutine(WaitOneFrame(timeToWait));
    }
}
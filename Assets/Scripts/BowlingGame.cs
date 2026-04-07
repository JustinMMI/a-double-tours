using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BowlingGame : MonoBehaviour
{
    public Rigidbody bouleRB;
    public Transform positionDepartBoule;
    public TextMeshProUGUI texteInfo;
    public float forceLancer = 50f;

    private List<Vector3> posInitiales = new List<Vector3>();
    private List<Quaternion> rotInitiales = new List<Quaternion>();
    private GameObject[] toutesLesQuilles;
    private bool aLance = false;

    void Start()
    {
        toutesLesQuilles = GameObject.FindGameObjectsWithTag("Quille");
        foreach (GameObject q in toutesLesQuilles)
        {
            posInitiales.Add(q.transform.position);
            rotInitiales.Add(q.transform.rotation);
        }
        texteInfo.text = "CLIQUEZ SUR LANCER !";
    }

    public void Lancer()
    {
        if (!aLance)
        {
            bouleRB.AddForce(Vector3.forward * forceLancer, ForceMode.Impulse);
            aLance = true;
            Invoke("CheckScore", 4f); // Compte les quilles après 4 secondes
        }
    }

    void CheckScore()
    {
        int tombees = 0;
        foreach (GameObject q in toutesLesQuilles)
        {
            // Si la quille est penchée, elle est tombée
            if (Vector3.Angle(q.transform.up, Vector3.up) > 10f) tombees++;
        }
        texteInfo.text = "SCORE : " + tombees + " QUILLES !";
    }

    public void ResetGame()
    {
        aLance = false;
        texteInfo.text = "PRÊT ?";
        // Reset Boule
        bouleRB.linearVelocity = Vector3.zero;
        bouleRB.angularVelocity = Vector3.zero;
        bouleRB.transform.position = positionDepartBoule.position;
        // Reset Quilles
        for (int i = 0; i < toutesLesQuilles.Length; i++)
        {
            Rigidbody r = toutesLesQuilles[i].GetComponent<Rigidbody>();
            r.linearVelocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;
            toutesLesQuilles[i].transform.position = posInitiales[i];
            toutesLesQuilles[i].transform.rotation = rotInitiales[i];
        }
    }
}
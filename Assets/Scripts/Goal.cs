using UnityEngine;
using System.Collections;
public class Goal : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    static public bool goalMet = false;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            Goal.goalMet = true;
            Material mat = GetComponent<Renderer>().material;
            Color c = mat.color;
            c.a = 1;
            mat.color = c;
        }
    }
}

using System.Collections;
using UnityEngine;

public class FootPrintScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(End());
    }

    IEnumerator End()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

}

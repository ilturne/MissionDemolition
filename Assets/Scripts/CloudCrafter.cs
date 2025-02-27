using UnityEngine;
using System.Collections;
public class CloudCrafter : MonoBehaviour
{
    [Header("Set in Inspector")]
    public int numClouds = 40;
    public GameObject cloudPrefab;
    public Vector3 cloudPosMin = new Vector3(-50, 5, 10);
    public Vector3 cloudPosMax = new Vector3(150, 100, 10);
    public float cloudScaleMin = 1;
    public float cloudScaleMax = 3;
    public float cloudSpeedMult = 0.5f;

    private GameObject[] cloudInstances;

    void Awake()
    {
        cloudInstances = new GameObject[numClouds];
        GameObject anchor = GameObject.Find("CloudAnchor");
        GameObject cloud;
        for (int i = 0; i < numClouds; i++)
        {
            cloud = Instantiate<GameObject>(cloudPrefab);
            Vector3 cPos = Vector3.zero;
            cPos.x = Random.Range(cloudPosMin.x, cloudPosMax.x);
            cPos.y = Random.Range(cloudPosMin.y, cloudPosMax.y);
            cPos.z = Random.Range(cloudPosMin.z, cloudPosMax.z);
            cloud.transform.position = cPos;
            cloud.transform.localScale = Vector3.one * Random.Range(cloudScaleMin, cloudScaleMax);
            cloud.transform.SetParent(anchor.transform);
            cloudInstances[i] = cloud;
        }
    }

    void Update()
    {
        foreach (GameObject cloud in cloudInstances)
        {
            Vector3 cPos = cloud.transform.position;
            cPos.x -= Time.deltaTime * cloudSpeedMult;
            if (cPos.x <= cloudPosMin.x)
            {
                cPos.x = cloudPosMax.x;
            }
            cloud.transform.position = cPos;
        }
    }
}

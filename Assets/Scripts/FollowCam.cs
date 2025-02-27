using UnityEngine;
using System.Collections;
public class FollowCam : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float easing = 0.05f;
    public Vector2 minXY = Vector2.zero;
    static public GameObject POI; // The static point of interest
    [Header("Set Dynamically")]
    public float camZ; // The desired Z pos of the camera
    void Awake()
    {
        camZ = this.transform.position.z;
    }

    void FixedUpdate()
    {
        // if (POI == null) return; // Return if there is no POI
        // Vector3 destination = POI.transform.position;
        Vector3 destination;
        if (POI == null)
        {
            destination = Vector3.zero;
        }
        else
        {
            destination = POI.transform.position;
            if (POI.tag == "Projectile")
            {
                if (POI.GetComponent<Rigidbody>().IsSleeping())
                {
                    POI = null;
                    return;
                }
            }
        }
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);
        destination = Vector3.Lerp(transform.position, destination, easing);
        destination.z = camZ;
        transform.position = destination;
        Camera.main.orthographicSize = destination.y + 10;
    }
}

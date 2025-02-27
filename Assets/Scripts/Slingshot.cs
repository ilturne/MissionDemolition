using UnityEngine;
using System.Collections;

public class Slingshot : MonoBehaviour
{
    static public Slingshot S;
    static public Vector3 LAUNCH_POS {
        get {
            if (S == null) return Vector3.zero;
            return S.launchPos;
        }
    }

    public GameObject launchPoint;
    public LineRenderer rubberBandLine;
    private AudioSource audioSource;

    [Header("Set in Inspector")]
    public float velocityMult = 9f;
    public GameObject prefabProjectile;
    public Transform leftArm;
    public Transform rightArm;
    public AudioClip pullBackSound; 
    public AudioClip releaseSound; 

    [Header("Set Dynamically")]
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private Rigidbody projectileRigidbody;

    public LineRenderer trajectoryLine;
    public int predicitonSteps = 30;
    public float timeStep = 0.1f;

    void Awake() {
        S = this;
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        rubberBandLine = GetComponent<LineRenderer>();
        rubberBandLine.positionCount = 3;
        rubberBandLine.enabled = false;

        audioSource = GetComponent<AudioSource>();
    }

    void OnMouseEnter() {
        launchPoint.SetActive(true);
    }

    void OnMouseExit() {
        launchPoint.SetActive(false);
    }

    void OnMouseDown() {

        if (!MissionDemolition.S.AllProjectilesStopped()) {
            return;
        }
        
        aimingMode = true;
        projectile = Instantiate(prefabProjectile) as GameObject;
        projectile.transform.position = launchPos;
        projectileRigidbody = projectile.GetComponent<Rigidbody>();
        projectileRigidbody.isKinematic = true;

        rubberBandLine.enabled = true;

        if (pullBackSound != null && audioSource != null)
        {
            audioSource.clip = pullBackSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    void Update()
    {
        if (!aimingMode) return;

        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
        Vector3 mouseDelta = mousePos3D - launchPos;
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;

        if (mouseDelta.magnitude > maxMagnitude) {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        rubberBandLine.SetPosition(0, leftArm.position);
        rubberBandLine.SetPosition(1, projectile.transform.position);
        rubberBandLine.SetPosition(2, rightArm.position);

        if (MissionDemolition.S.ProjectilePowerUp) {
            ShowTrajectory(projPos, -mouseDelta * velocityMult);
        }
        else {
            trajectoryLine.enabled = false;
        }

        if (Input.GetMouseButtonUp(0)) {
            aimingMode = false;
            projectileRigidbody.isKinematic = false;
            projectileRigidbody.linearVelocity = -mouseDelta * velocityMult;

            FollowCam.POI = projectile;
            projectile = null;
            MissionDemolition.ShotFired();
            ProjectileLine.S.poi = projectile;

            rubberBandLine.enabled = false;

            if (audioSource != null) {
                audioSource.Stop(); 
                if (releaseSound != null) {
                    audioSource.PlayOneShot(releaseSound); 
                }
            }
        }
    }

    void ShowTrajectory(Vector3 start, Vector3 initialVelocity) {
        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = predicitonSteps;

        Vector3[] trajectoryPoints = new Vector3[predicitonSteps];
        trajectoryPoints[0] = start;

        Vector3 currentPosition = start;
        Vector3 currentVelocity = initialVelocity;

        for (int i = 1; i < predicitonSteps; i++) {
            currentVelocity += Physics.gravity * timeStep;
            currentPosition += currentVelocity * timeStep;

            trajectoryPoints[i] = currentPosition;

            if (Physics.Raycast(trajectoryPoints[i - 1], trajectoryPoints[i] - trajectoryPoints[i - 1], out RaycastHit hit, Vector3.Distance(trajectoryPoints[i - 1], trajectoryPoints[i]))) {
                trajectoryLine.positionCount = i + 1;
                trajectoryPoints[i] = hit.point;
                break;
            }
        }
        trajectoryLine.SetPositions(trajectoryPoints);
    }
}
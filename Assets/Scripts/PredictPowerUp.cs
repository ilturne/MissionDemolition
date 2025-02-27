using UnityEngine;

public class PredictPowerUp : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
      if (other.CompareTag("Projectile"))
        {
            MissionDemolition.S.ProjectilePowerUp = true;
            Destroy(gameObject);
        }  
    }
    
}

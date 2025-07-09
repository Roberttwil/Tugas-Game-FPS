using UnityEngine;

public class LaserScript : MonoBehaviour
{
    public GameObject Laser;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Shoot();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopShooting();
        }

    }

    void Shoot()
    { Laser.SetActive(true); }

    void StopShooting()
    { Laser.SetActive(false); }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialBulletController : MonoBehaviour
{

    [Header("Projectile Settings")]
    public int numberOfProjectiles;
    public int projectileSpeed;
    public GameObject ProjectilePrefab;

    private Vector3 startPoint;
    private const float radius = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            startPoint = transform.position;
            SpawnProjectile(numberOfProjectiles);
        }

    }



    private void SpawnProjectile(int _numberOfProjectiles)
    {

        float angleStep = 360f / _numberOfProjectiles;
        float angle = 0f;

        for(int i = 0; i < _numberOfProjectiles ; i++)
        {
            float projectileDirXPosition = startPoint.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
            float projectileDirYPosition = startPoint.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            Vector3 projectileMoveDirection = (projectileVector - startPoint).normalized * projectileSpeed;


            GameObject tmpObj = Instantiate(ProjectilePrefab, startPoint , Quaternion.identity);

            tmpObj.GetComponent<Rigidbody>().velocity = new Vector3(projectileMoveDirection.x, 0, projectileMoveDirection.y );

            angle += angleStep;


        }



    }


}

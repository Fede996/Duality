using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialBulletController : MonoBehaviour
{

    [Header("Projectile Settings")]
    public int numberOfProjectiles;
    public int projectileSpeed;
    public float Frequency = 100;
    public GameObject ProjectilePrefab;
    public float radialSpeed;

    private Vector3 startPoint;
    private const float radius = 3f;
    private int i;
    private Rigidbody BulletController;
    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        BulletController = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        i++;




        if (i == Frequency) {

            startPoint = transform.position;
            SpawnProjectile(numberOfProjectiles);

            i = 0;
        }

        transform.Rotate( 0f, Time.deltaTime * radialSpeed , 0f);

    }



    private void SpawnProjectile(int _numberOfProjectiles)
    {

        float angleStep = 360f   / _numberOfProjectiles;
        float angle = 0f;

        for(int i = 0; i < _numberOfProjectiles ; i++)
        {
            float projectileDirXPosition = startPoint.x + Mathf.Sin(((angle + transform.rotation.y * Mathf.PI *radialSpeed) * Mathf.PI) / 180) * radius;
            float projectileDirYPosition = startPoint.y + Mathf.Cos(((angle + transform.rotation.y * Mathf.PI * radialSpeed) * Mathf.PI) / 180) * radius;

            //Debug.Log(transform.rotation.y * Mathf.PI * radialSpeed);

            Vector3 projectileVector = new Vector3(projectileDirXPosition, projectileDirYPosition, 0);
            Vector3 projectileMoveDirection = (projectileVector - startPoint).normalized * projectileSpeed;


            GameObject tmpObj = Instantiate(ProjectilePrefab, startPoint , Quaternion.identity);

            tmpObj.GetComponent<Rigidbody>().velocity = new Vector3(projectileMoveDirection.x, 0, projectileMoveDirection.y );

            angle += angleStep;


        }



    }


}

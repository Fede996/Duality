using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SolidTarget : Target
{
     [Header( "Settings" )]
     [SerializeField] private GameObject impactEffect;

     [Server]
     public override void OnHit( RaycastHit hit )
     {
          RpcOnHit( hit.point, hit.normal );
     }

     [ClientRpc]
     private void RpcOnHit( Vector3 point, Vector3 normal )
     {
          SpawnParticles( point, normal );
     }

     // =====================================================================

     private void SpawnParticles( Vector3 position, Vector3 normal )
     {
          GameObject particles = Instantiate( impactEffect );
          particles.transform.position = position;
          particles.transform.forward = normal;
          particles.SetActive( true );
     }
}

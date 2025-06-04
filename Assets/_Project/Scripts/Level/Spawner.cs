using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
     [SerializeField] public List<GameObject> itemsToSpawn;
     [SerializeField] private ParticleSystem particleSystemToPlay; 
     
     [SerializeField] private bool spawnOnEnable = true;

     public void OnEnable()
     {
          if (spawnOnEnable)
          {
               Spawn();
          }
     }
     
     public void Spawn()
     {
          GameObject prefab = itemsToSpawn[Random.Range(0, itemsToSpawn.Count)];
          var item = Instantiate(prefab, transform.position, transform.rotation);
          if (item)
          {
               var physics = item.GetComponent<Rigidbody>();
               physics.useGravity = true;
          }
          particleSystemToPlay.Stop(true);
          particleSystemToPlay.Play(true);
     }
}

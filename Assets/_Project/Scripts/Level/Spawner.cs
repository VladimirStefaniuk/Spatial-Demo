using UnityEngine;

public class Spawner : MonoBehaviour
{
     [SerializeField] public GameObject itemToSpawn;
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
          var item = Instantiate(itemToSpawn, transform.position, transform.rotation);
          if (item)
          {
               var physics = item.GetComponent<Rigidbody>();
               physics.useGravity = true;
          }
          particleSystemToPlay.Stop(true);
          particleSystemToPlay.Play(true);
     }
}

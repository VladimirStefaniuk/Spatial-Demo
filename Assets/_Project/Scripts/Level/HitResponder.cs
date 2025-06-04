using System;
using UnityEngine;
using UnityEngine.Events;

public class HitResponder : MonoBehaviour
{
   [SerializeField] private string tagToCheck = "Interactable";
   [SerializeField] private UnityEvent onHit;
   private void OnCollisionEnter(Collision other)
   {
      Debug.Log(other.gameObject.name);

      if(other.gameObject.CompareTag(tagToCheck))
      {
         var block = other.gameObject.GetComponent<Block>();
         if (block)
         {
            block.StartDissolver();
            onHit.Invoke();
         }
      }
   }
}

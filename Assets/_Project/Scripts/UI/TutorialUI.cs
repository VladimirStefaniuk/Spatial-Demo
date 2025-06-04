using System.Collections;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
   [SerializeField] TMPro.TMP_Text label;
   [SerializeField] private float typingSpeed = 0.05f;
   [SerializeField] private float scaleDuration = 0.2f;
   [SerializeField] private float hideAfterSeconds = 5f;
   
   private Coroutine _animationCoroutine;

   public void Show(string text)
   { 
      gameObject.SetActive(true);
      if (_animationCoroutine != null)
         StopCoroutine(_animationCoroutine);

      _animationCoroutine = StartCoroutine(AnimateShow(text));
   }

   public void Hide()
   {
      gameObject.SetActive(true);
      if (_animationCoroutine != null)
         StopCoroutine(_animationCoroutine);
      _animationCoroutine =  StartCoroutine(AnimateHide());
   }

   private IEnumerator AnimateHide()
   { 
      float time = 0f;
      
      // Scale animation
      while (time < scaleDuration)
      {
         if (time < scaleDuration)
         {
            float t = time / scaleDuration;
            transform.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.zero, t);
         }
         time += Time.deltaTime; 
         yield return null;
      } 
      gameObject.SetActive(false);
   }

   private IEnumerator AnimateShow(string fullText)
   {
      // Start from zero scale
      transform.localScale = Vector3.zero;
      label.text = "";

      float time = 0f;
      int charIndex = 0;
 
      // Scale animation
      while (time < scaleDuration)
      {
         if (time < scaleDuration)
         {
            float t = time / scaleDuration;
            transform.localScale = Vector3.LerpUnclamped(Vector3.zero, Vector3.one, t);
         }
         time += Time.deltaTime; 
         yield return null;
      }
  
      // Typewriter effect
      time = 0;
      while (charIndex < fullText.Length) {
         if (charIndex < fullText.Length && time >= charIndex * typingSpeed)
         {
            label.text += fullText[charIndex];
            charIndex++;
         }

         time += Time.deltaTime;
         yield return null;
      }

      transform.localScale = Vector3.one;
      label.text = fullText;
     
      // Auto-hide after delay
      if (hideAfterSeconds > 0f)
      {
         yield return new WaitForSeconds(hideAfterSeconds);
         Hide();
      }
   }
}

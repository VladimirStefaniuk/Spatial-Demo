using System.Collections;
using UnityEngine;

public enum BlockColor
{
    Red, 
    Blue,
    Green
}

public class Block : MonoBehaviour
{
    private static readonly int DissolveStrength = Shader.PropertyToID("_DissolveStrength");
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    [SerializeField] private float dissolveDuration = 1;
 
    [SerializeField] private BlockColor blockColor;
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    private Material dissolveMaterial;
  
    private float _dissolveStrength = 1;
    private Renderer _renderer;
    private IEnumerator _dissolveRoutine;
 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }
 
    public void StartDissolver()
    {
        if(_dissolveRoutine != null)
            return;
        _dissolveRoutine = DoDissolve();
        StartCoroutine(_dissolveRoutine);
    }
    
    private IEnumerator DoDissolve()
    {
        float elapsedTime = 0;
        dissolveMaterial = _renderer.material;
        Color lerpedColor = startColor;

        while (elapsedTime < dissolveDuration)
        {
            elapsedTime += Time.deltaTime;
            _dissolveStrength = Mathf.Lerp(0, 1, elapsedTime / dissolveDuration);
            _renderer.material.SetFloat(DissolveStrength, _dissolveStrength);
            lerpedColor = Color.Lerp(startColor, endColor, _dissolveStrength);
            _renderer.material.SetColor(BaseColor, lerpedColor);
            yield return null;
        }

        Destroy(dissolveMaterial);
        Destroy(gameObject);
 
        _dissolveRoutine = null;
    }
    
    public BlockColor GetColor() => blockColor; 
}

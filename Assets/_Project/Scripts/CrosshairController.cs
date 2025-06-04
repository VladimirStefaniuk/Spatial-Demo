using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct CrosshairData
{
    [Tooltip("The image that will be used for this weapon's crosshair")]
    public Sprite Sprite;

    [Tooltip("The size of the crosshair image")]
    public int Size;

    [Tooltip("The color of the crosshair image")]
    public Color Color;
}


public class CrosshairController : MonoBehaviour
{
    public enum State { Idle, Interactable, Active }
    
    [SerializeField] private Image crosshair;
    private RectTransform _crosshairReactTransform;
    
    [SerializeField] private CrosshairData defaultCrosshair;
    [SerializeField] private CrosshairData interactableCrosshair;
    [SerializeField] private CrosshairData activeCrosshair;

    private State _currentState;
    private CrosshairData _currentCrosshair;

    private void Awake()
    {
        if (!crosshair)
        {
            Debug.LogError("Crosshair image is not set");
        }
        else
        { 
            _crosshairReactTransform = crosshair.GetComponent<RectTransform>();
        }
    }

    public void SetState(State state)
    {
        if (state == _currentState)
            return;
        
        CrosshairData requestedCrosshair = defaultCrosshair;

        switch (state)
        {
            case State.Idle: 
                requestedCrosshair = defaultCrosshair;
                break;
            case State.Interactable:
                requestedCrosshair = interactableCrosshair;
                break;
            case State.Active:
                requestedCrosshair = activeCrosshair;
                break;
            default:
                Debug.LogError($"Invalid state. Not implemented {state}");
                break;
        }
        
        _currentCrosshair = requestedCrosshair;
        crosshair.sprite = _currentCrosshair.Sprite;
        _crosshairReactTransform.sizeDelta = _currentCrosshair.Size * Vector2.one;
        
        _currentState = state;
        
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    [Header("Particle properties")]
    [SerializeField] private ParticleSystem particleSystemPrefab;
    [SerializeField] private Color effectColor;
    
    public void Explode()
    {
        ParticleSystem particleSystemInstance = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
        ParticleSystem.MainModule mainModule = particleSystemInstance.main;
        mainModule.startColor = effectColor;
        
        Destroy(particleSystemInstance.gameObject, 0.75F);
    }
}
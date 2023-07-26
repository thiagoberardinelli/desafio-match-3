using UnityEngine;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private ParticleSystem particleSystemPrefab;

    public void Explode()
    {
        ParticleSystem particleSystemInstance = Instantiate(particleSystemPrefab, transform.position, Quaternion.identity);
        ParticleSystem.MainModule mainModule = particleSystemInstance.main;
        mainModule.startColor = image.color;
        
        Destroy(particleSystemInstance.gameObject, 0.75F);
    }

    public void SetColor(Color color) => image.color = color;
}
using UnityEngine;

public class SpecialTileView : TileView
{ 
    [SerializeField] private ParticleSystem particleSystemSpawnPrefab;
    
    public void Spawn()
    {
        ParticleSystem particleSystemInstance = Instantiate(particleSystemSpawnPrefab, transform.position, Quaternion.identity);
        ParticleSystem.MainModule mainModule = particleSystemInstance.main;
        mainModule.simulationSpeed = 2F;
        mainModule.startColor = image.color;

        ParticleSystem[] particleSystems = particleSystemInstance.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem particle in particleSystems)
        {
            ParticleSystem.MainModule main = particle.main;
            main.startColor = image.color;
        }
        
        Destroy(particleSystemInstance.gameObject, 0.75F);
    }
}
using UnityEngine;

[CreateAssetMenu(fileName = "TilePrefabRepository", menuName = "Gameplay/TilePrefabRepository")]
public class TilePrefabRepository : ScriptableObject
{
    public TileView tileViewPrefab;
    public Color[] colors;
    
    //public TileView[] tileTypePrefabList;
    public SpecialTileView[] specialTilePrefabList;
}
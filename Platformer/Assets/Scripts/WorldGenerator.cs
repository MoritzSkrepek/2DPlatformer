using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private int worldWidth;
    [SerializeField] private int worldHeight;
    [SerializeField] private TileBase dirtPrefab;
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private GameObject mainCamera; 
    
    private Tilemap worldMap;
    private WorldGenerator _worldGeneratorInstance;
    private WorldGenerator worldGeneratorInstance;

    private Vector3 characterPosition;

    private void Awake()
    {
        if (_worldGeneratorInstance != null && _worldGeneratorInstance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _worldGeneratorInstance = this;
            worldMap = gameObject.GetComponentInChildren<Tilemap>();
        }
    }

    void Start()
    {
        float scale = 0.1f; // Skalierung für den Perlin-Noise-Effekt
        float heightMultiplier = worldHeight;
        float offsetX = UnityEngine.Random.Range(0, 10000);
        float offsetY = UnityEngine.Random.Range(0, 10000);

        for (int x = -worldWidth; x < worldWidth; x++)
        {
            for (int y = -worldHeight; y < worldHeight; y++)
            {
                // Berechne die Höhe des Terrains basierend auf Perlin Noise
                float perlinValue = Mathf.PerlinNoise((x + offsetX) * scale, (y + offsetY) * scale);
                int terrainHeight = Mathf.RoundToInt(perlinValue * heightMultiplier);

                // Terrain bis zur berechneten Höhe erstellen
                for (int currentY = 0; currentY <= terrainHeight; currentY++)
                {
                    Vector3Int blockPosition = new Vector3Int(x, currentY, 0); 
                    worldMap.SetTile(blockPosition, dirtPrefab);
                }
            }
        }
        worldMap.RefreshAllTiles();
        spawnCharacter();
        mainCamera.SetActive(true);
        CenterCameraOnCharacter();
    }

    // Spawnt Character in der Mitte der Welt und an der richtigen Hoehe
    private void spawnCharacter()
    {
        BoundsInt worldBounds = worldMap.cellBounds;
        int spawnPointX = worldBounds.x + worldBounds.size.x / 2; 

        int highestY = -1; 

        // Überprüfe die Höhe nur in dem Bereich, wo die Tiles gesetzt wurden
        for (int y = worldBounds.yMax - 1; y >= worldBounds.yMin; y--) 
        {
            Vector3Int blockPosition = new Vector3Int(spawnPointX, y, 0);
            if (worldMap.GetTile(blockPosition) != null) 
            {
                highestY = y; 
                break; 
            }
        }

        if (highestY != -1)
        {
            characterPosition = worldMap.CellToWorld(new Vector3Int(spawnPointX, highestY+1, 0));
            GameObject playerInstance = Instantiate(characterPrefab, characterPosition, Quaternion.identity);
            playerInstance.tag = "Player";
        }
    }

    private void CenterCameraOnCharacter()
    {
        if (Camera.main != null)
        {
            Vector3 cameraPosition = new Vector3(characterPosition.x, characterPosition.y, Camera.main.transform.position.z);
            Camera.main.transform.position = cameraPosition;
        }
        else
        {
            Debug.LogWarning("Main camera is not assigned.");
        }
    }
}

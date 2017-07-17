using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
[System.Serializable]
public class WorldData
{
    public string seed;
    public Vector2 size;
    public List<WorldChunk> chunks;

    public void ChunksFromTexture(Texture tex)
    {
        List<WorldChunk> chunkList = new List<WorldChunk>();
    }
}

public class WorldChunk
{
    public int chunkId;
    public Vector2 chunkPos;
    public List<WorldBlock> blocks;

    public WorldChunk()
    {
        chunkId = 0;
        chunkPos = Vector3.zero;
        blocks = new List<WorldBlock>();
    }
}

public class WorldBlock
{
    public int blockId;
    public int blockType;
    public Vector2 blockPos;

    public WorldBlock()
    {
        blockId = 0;
        blockType = 0;
        blockPos = Vector3.zero;
    }
}

public static class PerlinNoise
{
    public static Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(512, 512);

        for (int x = 0; x < 512; x++)
        {
            for (int y = 0; y < 512; y++)
            {
                Color color = CalculateColor(x, y, 512, 512, 2, 0, 0);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    public static Texture2D GenerateTexture(int width, int height, float scale)
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y, width, height, scale, 0, 0);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    public static Texture2D GenerateTexture(int width, int height, float scale, int seed)
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y, width, height, scale, seed, seed);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    public static Texture2D GenerateTextureRandom()
    {
        Texture2D texture = new Texture2D(512, 512);
        int seed = Random.Range(0, 2147483646);

        for (int x = 0; x < 512; x++)
        {
            for (int y = 0; y < 512; y++)
            {
                Color color = CalculateColor(x, y, 512, 512, 2, seed, seed);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    public static Texture2D GenerateTextureRandom(int width, int height, float scale)
    {
        Texture2D texture = new Texture2D(width, height);
        int seed = Random.Range(0, 2147483646);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y, width, height, scale, seed, seed);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private static Color CalculateColor(int x, int y, int width, int height, float scale, int offsetX, int offsetY)
    {
        float sample = 0;

        if (width != 0 && height != 0)
        {
            float xCo = (float)x / width * scale + offsetX;
            float yCo = (float)y / height * scale + offsetY;

            sample = Mathf.PerlinNoise(xCo, yCo); 
        }
        return new Color(sample, sample, sample);
    }
}

public class WorldGenerator : MonoBehaviour
{
    public int height, width;
    public float scale;
    Renderer renderQuad;

    public WorldData worldData = new WorldData();

    public GameObject BlockPrefab;

    private void Start()
    {
        renderQuad = GetComponent<Renderer>();
    }

    private void Update()
    {
        renderQuad.material.mainTexture = PerlinNoise.GenerateTexture(width, height, scale);
    }

    public void Generate()
    {

    }
}
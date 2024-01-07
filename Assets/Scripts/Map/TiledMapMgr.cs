using System.Collections.Generic;
using UnityEngine;

public class TiledMapMgr : MonoBehaviour
{
    public static TiledMapMgr instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new GameObject("TiledMapMgr").AddComponent<TiledMapMgr>();
            }

            return m_Instance;
        }
    }

    public Vector2Int mapSize
    {
        get 
        {
            return m_MapSize;
        }
    }

    private  void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }

        m_GridRoot = new GameObject("GridList");
        m_GridRoot.layer = LayerMask.NameToLayer("Map");
    }

    public void GenerateMap(int width, int height, float xOffset, float yOffset, string shapeType)
    {
        m_MapSize.x = width;
        m_MapSize.y = height;
        m_Grid = Resources.Load<GameObject>("grid" + shapeType);
        m_Tileds = new Tiled[width][];

        for (int x = 0; x < width; x++)
        {
            m_Tileds[x] = new Tiled[height];

            for (int y = 0; y < height; y++)
            {

                if (shapeType == "Hexagon")
                {
                    Vector2Int pos = MapUtil.SquarePosToHexagonPosX(x, y);
                    GenerateGrid(pos, xOffset, yOffset, shapeType);
                }
                else if (shapeType == "Square")
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    GenerateGrid(pos, xOffset, yOffset, shapeType);
                }
            }
        }

        m_GridRoot.transform.position = new Vector3(-width * xOffset / 2f, -height * yOffset / 2f + yOffset / 2f, 0f);
    }

    private void GenerateGrid(Vector2Int pos, float xOffset, float yOffset, string shapeType)
    {
        Tiled tiled = GameObject.Instantiate(m_Grid).AddComponent<Tiled>();
        tiled.SetGrid(pos, xOffset, yOffset, shapeType, pos.x + pos.y * m_MapSize.x + 1);
        tiled.SetSprite(shapeType.ToLower() + "1");
        tiled.transform.SetParent(m_GridRoot.transform, false);
        tiled.txtLeftTop.text = string.Empty;
        tiled.txtLeftBottom.text = string.Empty;
        tiled.txtRightBottom.text = string.Empty;
        m_Tileds[pos.x][pos.y] = tiled;
    }

    public Tiled GetGridByPos(int x, int y)
    {
        if (x < 0 || x > m_MapSize.x - 1 || y < 0 || y > m_MapSize.y - 1)
        {
            return null;
        }

        return m_Tileds[x][y];
    }

    public Tiled GetGridByIndex(int index)
    {
        if(index <0 || index > m_MapSize.x * m_MapSize.y)
        {
            return null;
        }

        int y = (index - 1) / m_MapSize.x;
        int x = (index - 1) % m_MapSize.x;

        if (x < 0 || x > m_MapSize.x - 1 || y < 0 || y > m_MapSize.y - 1)
        {
            return null;
        }

        return m_Tileds[x][y];
    }

    private Tiled[][] m_Tileds = null;
    private Vector2Int m_MapSize = Vector2Int.zero;
    private GameObject m_GridRoot = null;
    private GameObject m_Grid = null;

    private static TiledMapMgr m_Instance = null;
}

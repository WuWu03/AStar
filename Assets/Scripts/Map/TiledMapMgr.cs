using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public enum ShapeType
{
    None = 0,
    Square = 1,
    Hexagon = 2,
}
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

    public int mapWidth
    {
        get 
        {
            return m_MapWidth;
        }
    }

    public int mapHeight
    {
        get
        {
            return m_MapHeight;
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

    public void GenerateMap(int width, int height, float xOffset, float yOffset, ShapeType shapeType)
    {
        m_MapWidth = width;
        m_MapHeight = height;

        if (shapeType == ShapeType.Hexagon)
        {
            m_Grid = Resources.Load<GameObject>("gridHexagon");
        }
        else if (shapeType == ShapeType.Square)
        {
            m_Grid = Resources.Load<GameObject>("gridSquare");
        }

        m_ShapeType = shapeType;
        m_Tileds = new Tiled[width][];

        for (int x = 0; x < width; x++)
        {
            m_Tileds[x] = new Tiled[height];

            for (int y = 0; y < height; y++)
            {
                GenerateGrid(x, y, xOffset, yOffset, shapeType);
            }
        }

        m_GridRoot.transform.position = new Vector3(-width * xOffset / 2f, -height * yOffset / 2f + yOffset / 2f, 0f);
    }

    private void GenerateGrid(int x,int y, float xOffset, float yOffset, ShapeType shapeType)
    {
        Tiled tiled = GameObject.Instantiate(m_Grid).AddComponent<Tiled>();

        if (shapeType == ShapeType.Hexagon)
        {
            Vector2Int pos = MapUtil.SquarePosToHexagonPosX(x, y);
            tiled.SetGrid(pos, xOffset, yOffset, shapeType, x + y * m_MapWidth + 1);
            tiled.SetSprite("hexagon1");
        }
        else if (shapeType == ShapeType.Square)
        {
            Vector2Int pos = new Vector2Int(x, y);
            tiled.SetGrid(pos, xOffset, yOffset, shapeType, x + y * m_MapWidth + 1);
            tiled.SetSprite("square1");
        }
      
        tiled.transform.SetParent(m_GridRoot.transform, false);
        tiled.txtLeftTop.text = string.Empty;
        tiled.txtLeftBottom.text = string.Empty;
        tiled.txtRightBottom.text = string.Empty;
        m_Tileds[x][y] = tiled;
    }

    public Tiled GetGridByPos(int x, int y)
    {
        if (x < 0 || x > m_MapWidth - 1 || y < 0 || y > m_MapHeight - 1)
        {
            return null;
        }

        return m_Tileds[x][y];
    }

    public Tiled GetGridByIndex(int index)
    {
        if (index < 0 || index > m_MapWidth * m_MapHeight)
        {
            return null;
        }

        int y = (index - 1) / m_MapWidth;
        int x = (index - 1) % m_MapWidth;
        int mapWidth = m_MapWidth;

        if (x < 0 || x > mapWidth - 1 || y < 0 || y > m_MapHeight - 1)
        {
            return null;
        }

        return m_Tileds[x][y];
    }

    private Tiled[][] m_Tileds = null;
    private int m_MapWidth = 0;
    private int m_MapHeight = 0;
    private ShapeType m_ShapeType = ShapeType.None;
    private GameObject m_GridRoot = null;
    private GameObject m_Grid = null;

    private static TiledMapMgr m_Instance = null;
}

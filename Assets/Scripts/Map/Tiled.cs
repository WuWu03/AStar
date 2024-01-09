using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Tiled:MonoBehaviour
{
    public ShapeType ShapeType
    {
        get
        {
            return m_ShapeType;
        }
    }

    public TextMesh txtPos
    {
        get
        {
            return m_TxtPos;
        }
    }

    public TextMesh txtLeftTop
    {
        get
        {
            return m_TxtLeftTop;
        }
    }

    public TextMesh txtLeftBottom
    {
        get
        {
            return m_TxtLeftBottom;
        }
    }

    public TextMesh txtRightBottom
    {
        get
        {
            return m_TxtRightBottom;
        }
    }


    public Vector2Int pos 
    {
        get
        {
            return m_Pos;
        }
    }

    private void Awake()
    {
        m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        m_TxtPos = transform.GetChild(0).GetComponent<TextMesh>();
        m_TxtLeftTop = transform.GetChild(1).GetComponent<TextMesh>();
        m_TxtLeftBottom = transform.GetChild(2).GetComponent<TextMesh>();
        m_TxtRightBottom = transform.GetChild(3).GetComponent<TextMesh>();

        gameObject.layer = LayerMask.NameToLayer("Map");
        gameObject.tag = "Grid";
        m_TxtLeftTop.text = string.Empty;
        m_TxtLeftBottom.text = string.Empty;
        MapUtil.SetLayer(gameObject, LayerMask.NameToLayer("Map"), true);
    }

    public void SetGrid(Vector2Int pos, float xOffset, float yOffset, ShapeType shapeType, int index)
    {
        gameObject.name = "Grid_" + pos.x + "_" + pos.y;
        m_ShapeType = shapeType;
        m_XOffset = xOffset;
        m_YOffset = yOffset;
        m_SpriteRenderer.sortingOrder = -1;
        SetPos(pos, index);
    }

    public void SetSprite(string spriteName)
    {
        m_SpriteRenderer.sprite = Resources.Load<Sprite>(spriteName);
    }

    public void SetPos(Vector2Int pos, int index)
    {
        transform.position = MapUtil.LogPosToWorldPos(pos, m_XOffset, m_YOffset);

        if (m_TxtPos != null)
        {
            m_TxtPos.text = string.Format("({0},{1})\n{2}", pos.x, pos.y, index);
        }

        m_Pos = pos;
    }

    private float m_XOffset = 0f;
    private float m_YOffset = 0f;
    private ShapeType m_ShapeType = ShapeType.None;

    private SpriteRenderer m_SpriteRenderer;
    private TextMesh m_TxtPos;
    private TextMesh m_TxtLeftTop;
    private TextMesh m_TxtLeftBottom;
    private TextMesh m_TxtRightBottom;
    private Vector2Int m_Pos = Vector2Int.zero;
}

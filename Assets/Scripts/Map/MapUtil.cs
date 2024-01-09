using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System;
using System.Reflection;
using UnityEngine.Networking;

public class MapUtil
{
    /// <summary>
    /// 逻辑坐标转世界坐标
    /// </summary>
    public static Vector3 LogPosToWorldPos(Vector2Int logicPos, float scaleX, float scaleY)
    {
        return new Vector3(logicPos.x * scaleX, logicPos.y * scaleY, 0);
    }

    public static int SquarePosToIndex(int x,int y,int mapWidth)
    {
        return x + y * mapWidth + 1;
    }

    /// <summary>
    /// 四边形坐标转六边形坐标X轴排列
    /// </summary>
    public static Vector2Int SquarePosToHexagonPosX(int x, int y)
    {
        return new Vector2Int(2 * x + y % 2, y);
    }

    /// <summary>
    /// 六边形坐标X轴排列转四边形坐标
    /// </summary>>
    public static Vector2Int HexagonXPosToSquarePos(int x, int y)
    {
        return new Vector2Int((x - y % 2) / 2, y);
    }


 
    public static void SetTag(GameObject go,string tag,bool isSetChild = false)
    {
        if (isSetChild)
        {
            Transform[] childs = go.transform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].tag = tag;
            }
        }

        else go.tag = tag;
    }

    public static void SetLayer(GameObject go, int layer, bool isSetChild = false)
    {
        go.layer = layer;

        if (isSetChild)
        {
            Transform[] childs = go.transform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < childs.Length; i++)
            {
                childs[i].gameObject.layer = layer;
            }
        }
    }
}
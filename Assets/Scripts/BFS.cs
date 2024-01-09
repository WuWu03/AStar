using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class BFS : MonoBehaviour
{
    private void Awake()
    {
        TiledMapMgr.instance.GenerateMap(7, 7, 0.62f, 0.62f, ShapeType.Square);
        StartBFS(3, 3);
    }

    private void Update()
    {
        if (m_Timer < 0 || Time.time - m_Timer > 0.05f)
        {
            if (m_QueuePoints.Count > 0)
            {
                Tiled tiled = TiledMapMgr.instance.GetGridByIndex(m_QueuePoints.Dequeue());
                tiled.SetSprite("square2");
                tiled.txtLeftTop.color = Color.black;
            }

            m_Timer = Time.time;
        }
    }

    private void StartBFS(int x, int y)
    {
        bool[] visits = new bool[7 * 7];
        Queue<int> bfs = new Queue<int>();

        int index = x + y * 7 + 1;
        bfs.Enqueue(index);
        visits[index - 1] = true;

        while (bfs.Count > 0)
        {
            int currIndex = bfs.Dequeue();
            int currX = (currIndex - 1) % 7;
            int currY = (currIndex - 1) / 7;

            m_QueuePoints.Enqueue(currIndex);

            AddPoint(currX - 1, currY, bfs, visits);
            AddPoint(currX + 1, currY, bfs, visits);
            AddPoint(currX, currY - 1, bfs, visits);
            AddPoint(currX, currY + 1, bfs, visits);
        }
    }

    private void AddPoint(int x, int y, Queue<int> bfs, bool[] visits)
    {
        if (x >= 0 && x <= 6 && y >= 0 && y <= 6)
        {
            int index = x + y * 7 + 1;

            if (!visits[index - 1])
            {
                bfs.Enqueue(index);
                visits[index - 1] = true;
            }
        }
    }


    private float m_Timer = -1f;
    private Queue<int> m_QueuePoints = new Queue<int>();
}



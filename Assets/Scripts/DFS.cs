using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DFS : MonoBehaviour
{
    private void Awake()
    {
        TiledMapMgr.instance.GenerateMap(6, 6, 0.62f, 0.62f, "Square");
        StartDFS(0, 0);
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

    private void StartDFS(int x, int y, bool[] visits = null)
    {
        if(visits == null)
        {
            visits = new bool[6 * 6];
        }

        if (x >= 0 && x <= 5 && y >= 0 && y <= 5)
        {
            int index = x + y * 6 + 1;

            if (!visits[index - 1])
            {

                visits[index - 1] = true;
                m_QueuePoints.Enqueue(index);
                StartDFS(x, y + 1, visits);//ио
                StartDFS(x + 1, y, visits);//ср
                StartDFS(x, y - 1, visits);//об
                StartDFS(x - 1, y, visits);//вС
            }
        }
    }

    private float m_Timer = -1f;
    private Queue<int> m_QueuePoints = new Queue<int>();
}


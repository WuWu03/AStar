using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;


public class Dijkstra : MonoBehaviour
{
    class Node
    {
        public int index;
        public Node parent;
        public Node(int index, Node parent)
        {
            this.index = index;
            this.parent = parent;
        }
    }

    private int m_MapWidth = 3;
    private int m_MapHeight = 3;

    private void Awake()
    {
        m_Map = new int[m_MapWidth * m_MapHeight];
        m_MapData = new int[m_MapWidth * m_MapHeight];

        //for (int i = 0; i < 12; i++)
        //{
        //    m_MapData[153 - i * m_MapWidth] = -1;
        //    m_MapData[2 + i * m_MapWidth] = -1;
        //    m_MapData[163 - i * m_MapWidth] = -1;
        //}

        TiledMapMgr.instance.GenerateMap(m_MapWidth, m_MapHeight, 0.62f, 0.62f, ShapeType.Square);

        for (int i = 0; i < m_MapData.Length; i++)
        {
            if (m_MapData[i] == -1)
            {
                Tiled tiled = TiledMapMgr.instance.GetGridByIndex(i + 1);
                tiled.SetSprite("square3");
                tiled.txtPos.color = Color.white;
                tiled.txtLeftTop.color = Color.yellow;
            }
        }

        InitMap();
        StartDijkstra(0, 0);
    }


    private void Update()
    {
        if (m_Timer < 0 || Time.time - m_Timer > 0.05f)
        {
            if (m_QueuePoints.Count > 0)
            {
                int index = m_QueuePoints.Dequeue();

                if (m_MapData[index - 1] != -1)
                {
                    Tiled tiled = TiledMapMgr.instance.GetGridByIndex(index);
                    tiled.SetSprite("square4");
                    tiled.txtPos.color = Color.black;
                    tiled.txtLeftTop.color = Color.red;
                }
            }
            else if (m_QueuePath.Count > 0)
            {
                Tiled tiled = TiledMapMgr.instance.GetGridByIndex(m_QueuePath.Dequeue());
                tiled.SetSprite("square2");
                tiled.txtPos.color = Color.black;
                tiled.txtLeftTop.color = Color.red;
            }

            m_Timer = Time.time;
        }
    }

    private void InitMap()
    {
        m_Nodes = new Node[m_MapWidth * m_MapHeight];

        for (int i = 0; i < m_MapWidth * m_MapHeight; i++)
        {
            Tiled tiled = TiledMapMgr.instance.GetGridByIndex(i + 1);
            int cost = UnityEngine.Random.Range(1, 5);
            m_Map[i] = cost;
            tiled.txtLeftTop.text = cost.ToString();
            tiled.txtLeftTop.color = Color.red;
        }
    }

    private void StartDijkstra(int x, int y)
    {
        Queue<int> dijkstra = new Queue<int>();

        bool[] visits = new bool[m_MapWidth * m_MapHeight];
        int[] dis = new int[m_MapWidth * m_MapHeight];

        for (int i = 0; i < dis.Length; i++)
        {
            dis[i] = int.MaxValue;
        }

        int currNode = x + y * m_MapWidth + 1;
        dis[currNode - 1] = 0;
        m_Nodes[currNode - 1] = new Node(currNode, null);
        dijkstra.Enqueue(currNode);

        while (dijkstra.Count > 0)
        {
            currNode = dijkstra.Dequeue();
            m_QueuePoints.Enqueue(currNode);

            visits[currNode - 1] = true;

            Tiled tiled = TiledMapMgr.instance.GetGridByIndex(currNode);
            tiled.txtLeftBottom.text = dis[currNode - 1].ToString();

            int currX = (currNode - 1) % m_MapWidth;
            int currY = (currNode - 1) / m_MapWidth;

            FindDestNode(currX, currY + 1, visits, dis[currNode - 1], dis, m_Nodes[currNode - 1]);
            FindDestNode(currX, currY - 1, visits, dis[currNode - 1], dis, m_Nodes[currNode - 1]);
            FindDestNode(currX - 1, currY, visits, dis[currNode - 1], dis, m_Nodes[currNode - 1]);
            FindDestNode(currX + 1, currY, visits, dis[currNode - 1], dis, m_Nodes[currNode - 1]);

            if (currNode == m_MapWidth * m_MapHeight)
            {
                visits[currNode - 1] = true;
                break;
            }

            int minDisNode = -1;
            int tempDis = int.MaxValue;

            for (int j = 0; j < dis.Length; j++)
            {
                if (!visits[j] && dis[j] < tempDis)
                {
                    minDisNode = j;
                    tempDis = dis[j];
                }
            }

            if(minDisNode >= 0)
            {
                dijkstra.Enqueue(minDisNode + 1);
            }
        }

        for(int i = 0; i < dis.Length; i++)
        {
            if (visits[i])
            {
                Debug.Log(dis[i]);
            }
        }

        Node node = m_Nodes[currNode - 1];

        while (node != null)
        {
            m_QueuePath.Enqueue(node.index);
            node = node.parent;
        }
    }

    private void FindDestNode(int x, int y, bool[] visits, int currDis, int[] dis, Node currNode)
    {
        if (x >= 0 && x < m_MapWidth && y >= 0 && y < m_MapHeight)
        {
            int index = x + y * m_MapWidth + 1;

            if (m_MapData[index - 1] != -1)
            {
                if (!visits[index - 1])
                {
                    dis[index - 1] = currDis + m_Map[index - 1];
                    m_Nodes[index - 1] = new Node(index, currNode);
                }
                else if (dis[index - 1] > currDis + m_Map[index - 1])
                {
                    dis[index - 1] = currDis + m_Map[index - 1];
                }
            }
        }
    }

    private int[] m_MapData = null;
    private Node[] m_Nodes = null;
    private float m_Timer = -1f;
    private Queue<int> m_QueuePath = new Queue<int>();
    private Queue<int> m_QueuePoints = new Queue<int>();
    private int[] m_Map = null;
}
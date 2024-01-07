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
using UnityEngine.UIElements.Experimental;


public class AStar : MonoBehaviour
{
    class Node:IComparable
    {
        public int index; //½Úµã±àºÅ
        public Node parent;
        public float f;
        public float g;
        public float h;
        public bool isOpen;
        public Node(int index, Node parent, float g, float h)
        {
            this.index = index;
            this.parent = parent;
            this.g = g;
            this.h = h;
            this.f = g + h;
            this.isOpen = true;
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return -1;
            }

            Node node = obj as Node;

            if (node.f == this.f)
            {
                return this.g > node.g ? 1 : -1;
            }

            return this.f > node.f ? 1 : -1;
        }
    }

    private int m_MapWidth = 13;
    private int m_MapHeight = 13;

    private void Awake()
    {
        m_MapData = new int[m_MapWidth * m_MapHeight];

        for (int i = 0; i < 12; i++)
        {
            m_MapData[153 - i * m_MapWidth] = -1;
            m_MapData[2 + i * m_MapWidth] = -1;
            m_MapData[163 - i * m_MapWidth] = -1;
        }

        TiledMapMgr.instance.GenerateMap(m_MapWidth, m_MapHeight, 0.62f, 0.62f, "Square");

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

        StartAStar(0, 0, m_MapWidth - 1, m_MapHeight - 1);
    }


    private void Update()
    {
        if (m_Timer < 0 || Time.time - m_Timer > 0.05f)
        {
            if (m_QueuePoints.Count > 0)
            {
                Node node = m_QueuePoints.Dequeue();

                if (m_MapData[node.index - 1] != -1)
                {
                    Tiled tiled = TiledMapMgr.instance.GetGridByIndex(node.index);
                    tiled.SetSprite("square4");
                    tiled.txtPos.color = Color.black;
                    tiled.txtLeftTop.color = Color.blue;
                    tiled.txtLeftBottom.color = Color.red;
                    tiled.txtRightBottom.color = Color.green;
                }
            }
            else if (m_QueuePath.Count > 0)
            {
                Tiled tiled = TiledMapMgr.instance.GetGridByIndex(m_QueuePath.Dequeue());
                tiled.SetSprite("square2");
                tiled.txtPos.color = Color.black;
                tiled.txtLeftTop.color = Color.blue;
                tiled.txtLeftBottom.color = Color.red;
                tiled.txtRightBottom.color = Color.yellow;
            }

            m_Timer = Time.time;
        }
    }

    private void InitMap()
    {
        for (int i = 0; i < m_MapWidth * m_MapHeight; i++)
        {
            Tiled tiled = TiledMapMgr.instance.GetGridByIndex(i + 1);
            int cost = 1;
            tiled.txtLeftBottom.text = cost.ToString();

            tiled.txtLeftTop.color = Color.yellow;
            tiled.txtLeftBottom.color = Color.red;
            tiled.txtRightBottom.color = Color.green;
        }
    }

    private void StartAStar(int xFrom, int yFrom ,int xTo ,int yTo)
    {
        Queue<Node> astar = new Queue<Node>();
        List<Node> openList= new List<Node>();

        int indexFrom = xFrom + yFrom * m_MapWidth + 1;
        int indexTo = xTo + yTo * m_MapWidth + 1;

        astar.Enqueue(new Node(indexFrom, null, 0, GetDistance(indexFrom, indexTo)));

        Node currNode = null;

        while (astar.Count > 0)
        {
            currNode = astar.Dequeue();
            m_QueuePoints.Enqueue(currNode);

            if (currNode.index == indexTo)
            {
                break;
            }

            currNode.isOpen = false;

            if (!openList.Contains(currNode))
            {
                openList.Add(currNode);
            }

            Tiled tiled = TiledMapMgr.instance.GetGridByIndex(currNode.index);
            tiled.txtRightBottom.text = currNode.h.ToString();
            tiled.txtLeftTop.text = currNode.f.ToString();

            int currX = (currNode.index - 1) % m_MapWidth;
            int currY = (currNode.index - 1) / m_MapWidth;

            FindDestNode(currX, currY + 1, xTo, yTo, currNode, openList);
            FindDestNode(currX, currY - 1, xTo, yTo, currNode, openList);
            FindDestNode(currX - 1, currY, xTo, yTo, currNode, openList);
            FindDestNode(currX + 1, currY, xTo, yTo, currNode, openList);

            Node minDistanceNode = openList.FindAll(node => node.isOpen).Min();

            if (minDistanceNode != null)
            {
                astar.Enqueue(minDistanceNode);
            }
        }

        while(currNode != null)
        {
            m_QueuePath.Enqueue(currNode.index);
            currNode = currNode.parent;
        }
    }

    private void FindDestNode(int x, int y, int xTo, int yTo, Node currNode, List<Node> openList)
    {
        if (x >= 0 && x < m_MapWidth && y >= 0 && y < m_MapHeight)
        {
            int index = x + y * m_MapWidth + 1;
            int toIndex = xTo + yTo * m_MapWidth + 1;

            if (m_MapData[index - 1] != -1)
            {
                Node temp = openList.Find(obj => obj.index == index);

                if (temp == null)
                {
                    temp = new Node(index, currNode, currNode.g + 1, GetDistance(index, toIndex));
                    openList.Add(temp);
                }
                else if (temp.isOpen && temp.g > currNode.g + 1)
                {
                    temp.g = currNode.g + 1;
                    temp.f = temp.g + temp.h;
                    temp.parent = currNode;
                }
            }
        }
    }

    private int GetDistance(int from, int to)
    {
        int xFrom = (from - 1) % m_MapWidth;
        int yFrom = (from - 1) / m_MapWidth;

        int xTo = (to - 1) % m_MapWidth;
        int yTo = (to - 1) / m_MapWidth;

        int xDistance = Mathf.Abs(xTo - xFrom);
        int yDistance = Mathf.Abs(yTo - yFrom);
        return xDistance * xDistance + yDistance * yDistance;
    }

    private int[] m_MapData = null;
    private float m_Timer = -1f;
    private Queue<int> m_QueuePath = new Queue<int>();
    private Queue<Node> m_QueuePoints = new Queue<Node>();
}



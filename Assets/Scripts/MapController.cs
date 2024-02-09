using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class MapController : MonoBehaviour
{
    [SerializeField] Transform mapParent;
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] GameObject linePrefab;
    List<GameObject> lines = new List<GameObject>();

    [SerializeField] GameObject edgePrefab;
    [SerializeField] GameObject cornerPrefab;
    [SerializeField] GameObject narrowPrefab;
    [SerializeField] GameObject endPrefab;

    Map map;

    // Start is called before the first frame update
    void Start()
    {
        NodeGraph g = new NodeGraph(8, 8);
        g.BuildRoom(0, 0, 4, 4);

        GNode gn = g.graph[0, 0];

        /*map = new Map(8, 8);
        map.floorArea(0, 0, 7, 7);

        map.PathMap(new Vector2(3, 3), 1, 3);

        DrawLine(new Vector2(), new Vector2(0, 3));

        InstanceMap();*/
    }

    private void InstanceMap()
    {
        GameObject g;
        Cell cell = null;
        for (int i = 0; i < map.cells.GetLength(0); i++)
        {
            for (int j = 0; j < map.cells.GetLength(1); j++)
            {
                cell = map.cells[i, j];

                if (cell != null)
                {
                    g = Instantiate(floorPrefab);
                    g.transform.position = new Vector3(cell.x, 0, cell.y);

                    cell = null;
                }
            }
        }
    }

    private void DrawOutline()
    {
        List<Node> nodes = new List<Node>();
        Dictionary<Node, bool> traversedNodes = new Dictionary<Node, bool>();

        Node up;
        Node down;
        Node right;
        Node left;



        while (nodes.Count > 0)
        {
            // load neighbor nodes


            // determine neighboring nodes

            // add neighboring nodes

            // work queue
            traversedNodes.Add(nodes[0], true);
            nodes.RemoveAt(0);

            up = null;
            down = null;
            right = null;
            left = null;
        }
    }

    public void DrawLine(Vector2 from, Vector2 to)
    {
        GameObject line = Instantiate(linePrefab);
        line.transform.position = new Vector3(from.x, 0, from.y);
        lines.Add(line);
        line.transform.localScale = new Vector3(1, 1, Vector2.Distance(from, to));
        line.transform.LookAt(new Vector3(to.x, 0, to.y));
    }

    private class NodeGraph
    {
        int width;
        int length;
        public GNode[,] graph { get; private set; }

        public NodeGraph(int _width, int _length) 
        {
            width = _width;
            length = _length;
            graph = new GNode[width, length];
        }

        public void BuildRoom(int x1, int y1 , int x2, int y2) 
        {
            // Assure x1 < x2
            if (x1 > x2) 
            {
                int t = x1;
                x1 = x2; 
                x2 = t;
            }

            // Assure y1 < y2
            if (y1 > y2) 
            {
                int t = y1;
                y1 = y2;
                y2 = t;
            }

            // Create nodes in graph
            for (int i = x1; i < x2; i++) 
            {
                for (int j = y1; j < y2; j++) 
                {
                    graph[i, j] = new GNode(i, j);
                }
            }

            // Iterate through all nodes and create connections
            for (int i = x1; i < x2; i++)
            {
                for (int j = y1; j < y2; j++)
                {
                    GNode node = graph[i, j];

                    if (i - 1 >= 0 && i - 1 > x1) node.down = graph[i-1,j];
                    if (i + 1 < graph.GetLength(0) && i + 1 < x2) node.up = graph[i+1,j];
                    if (j - 1 >= 0 && j - 1 > y1) node.down = graph[i, j - 1];
                    if (j + 1 < graph.GetLength(1) && j + 1 < y2) node.up = graph[i, j + 1];
                }
            }
        }
    }

    private class GNode 
    {
        int x;
        int y;
        float moveCost = 1;

        public GNode up;
        public GNode down;
        public GNode left;
        public GNode right;

        public GNode(int _x, int _y) 
        {
            x = _x;
            y = _y;
        }
    }

    private class Map
    {
        public Cell[,] cells;
        public Wall[,] walls;

        public int[,] pathMap;
        public Vector2 pathStart;

        public Map(int width, int length)
        {
            cells = new Cell[width, length];
            walls = new Wall[width, length + 2];

            pathMap = new int[width, length];
        }

        public void floorArea(int x1, int y1, int x2, int y2)
        {
            x1 = Mathf.Clamp(x1, 0, cells.GetLength(0));
            x2 = Mathf.Clamp(x2, 0, cells.GetLength(0));
            y1 = Mathf.Clamp(y1, 0, cells.GetLength(1));
            y2 = Mathf.Clamp(y2, 0, cells.GetLength(1));

            if (x1 > x2)
            {
                int t = x1;
                x1 = x2;
                x2 = t;
            }

            if (y1 > y2)
            {
                int t = y1;
                y1 = y2;
                y2 = t;
            }

            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    cells[i, j] = new Cell(i, j);
                }
            }
        }

        public void PathMap(Vector2 start, float moveCost, float moveDist)
        {
            List<Node> nodes = new List<Node>();
            nodes.Add(new Node(start.x, start.y));
            pathMap[nodes[0].x, nodes[0].y] = 1;

            while (nodes.Count > 0) 
            {
                Debug.Log("Processing node: " + nodes[0]);

                AddNeighborNodes(nodes, moveCost, moveDist);
                nodes.RemoveAt(0);
            }
        }

        private void AddNeighborNodes(List<Node> nodes, float moveCost, float moveDist) 
        {
            Node currNode = nodes[0];

            if (pathMap[currNode.x, currNode.y] + moveCost > moveDist) 
            {
                return;
            }

            // Up... in range, zero or greater than current + 1
            if (currNode.y + 1 < pathMap.GetLength(1) && 
                (pathMap[currNode.x, currNode.y+1] == 0 || pathMap[currNode.x, currNode.y + 1] > pathMap[currNode.x, currNode.y] + 1)) 
            {
                nodes.Add(new Node(currNode.x, currNode.y + 1));
                pathMap[currNode.x, currNode.y + 1] = pathMap[currNode.x, currNode.y] + 1;
            }

            // Down... in range, zero or greater than current + 1
            if (currNode.y - 1 >= 0 &&
                (pathMap[currNode.x, currNode.y - 1] == 0 || pathMap[currNode.x, currNode.y - 1] > pathMap[currNode.x, currNode.y] + 1))
            {
                nodes.Add(new Node(currNode.x, currNode.y - 1));
                pathMap[currNode.x, currNode.y - 1] = pathMap[currNode.x, currNode.y] + 1;
            }

            // Right... in range, zero or greater than current + 1
            if (currNode.x + 1 < pathMap.GetLength(0) &&
                (pathMap[currNode.x + 1, currNode.y] == 0 || pathMap[currNode.x + 1, currNode.y] > pathMap[currNode.x, currNode.y] + 1))
            {
                nodes.Add(new Node(currNode.x + 1, currNode.y));
                pathMap[currNode.x + 1, currNode.y] = pathMap[currNode.x, currNode.y] + 1;
            }

            // Left... in range, zero or greater than current + 1
            if (currNode.x - 1 >= 0 &&
                (pathMap[currNode.x - 1, currNode.y] == 0 || pathMap[currNode.x - 1, currNode.y] > pathMap[currNode.x, currNode.y] + 1))
            {
                nodes.Add(new Node(currNode.x, currNode.y + 1));
                pathMap[currNode.x - 1, currNode.y] = pathMap[currNode.x, currNode.y] + 1;
            }
        }

        public List<Node> GetOutlineNodes() 
        {
            List<Node> outline = new List<Node>();
            List<Node> queue = new List<Node>();

            while (queue.Count > 0) 
            {
            
            }

            return outline;
        }

        public class Node
        {
            public int x { get; private set; }
            public int y { get; private set; }
            public float cost { get; private set; }

            public Node(int _x, int _y) 
            {
                x = _x;
                y = _y;
            }

            public Node(float _x, float _y) 
            {
                x = (int)_x;
                y = (int)_y;
            }

            public override string ToString()
            {
                return "(" + x + ", " + y + ")";
            }
        }
    }

    private class Cell 
    {
        GameObject cellObject;
        public int x { get; private set; }
        public int y { get; private set; }

        public Cell(int _x, int _y) 
        {
            x = _x; 
            y = _y;
        }
    }

    private class Wall 
    {
        GameObject wallObject;
        public int x { get; private set; }
        public int y { get; private set; }
        public Vector2 position { get; protected set; }

        public Wall() 
        {
            
        }

        public Wall(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public void PlaceWallUp(int _x, int _y)
        {
            x = _x;
            y = _y * 2 + 2;

            position = new Vector2(_x, _y + 0.5f);
        }

        public void PlaceWallDown(int _x, int _y)
        {
            x = _x;
            y = _y * 2;

            position = new Vector2(_x, _y - 0.5f);
        }
    }
}

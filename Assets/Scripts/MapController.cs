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

    NodeGraph nodeMap;

    // Start is called before the first frame update
    void Start()
    {
        nodeMap = new NodeGraph(8, 8);
        nodeMap.BuildRoom(0, 0, 4, 4);
        nodeMap.BuildRoom(4, 0, 6, 7);
        InstanceMap();
    }

    private void FixedUpdate()
    {
        // draw node edges
        DrawEdges();
    }

    private void DrawEdges() 
    {
        Color debugColor = Color.green;

        foreach (Node node in nodeMap.graph) 
        {
            if (node != null) 
            {
                if (node.up != null) Debug.DrawLine(node.position, node.up.position, debugColor);
                if (node.down != null) Debug.DrawLine(node.position, node.down.position, debugColor);
                if (node.right != null) Debug.DrawLine(node.position, node.right.position, debugColor);
                if (node.left != null) Debug.DrawLine(node.position, node.left.position, debugColor);
            }
        }
    }

    private void InstanceMap()
    {
        GameObject g;

        // Iterate through nodes and create floor tiles
        foreach (Node node in nodeMap.graph) 
        {
            if (node != null) 
            {
                g = Instantiate(floorPrefab);
                g.transform.position = node.position;

                //node.SetTile(g);
                //if (node.tile)
                //    node.tile.FadeOut();
            }
        }

        // Add bottom most walls
        Vector3 wallOffset = new Vector3(0, 0, -0.5f);
        for (int i = 0; i < nodeMap.graph.GetLength(0); i++) 
        {
            Node node = nodeMap.graph[i, 0];
            if (node != null) 
            {
                g = Instantiate(wallPrefab);
                g.transform.position = node.position + wallOffset;
                g.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        // Add left most walls
        wallOffset = new Vector3(-0.5f, 0, 0);
        for (int j = 0; j < nodeMap.graph.GetLength(1); j++) 
        {
            Node node = nodeMap.graph[0, j];
            if (node != null)
            {
                g = Instantiate(wallPrefab);
                g.transform.position = node.position + wallOffset;
                g.transform.rotation = Quaternion.Euler(0, 90f, 0);
            }
        }

        // Iterate through all nodes, creating wall where 
        Vector3 wallOffsetUp = new Vector3(0, 0, 0.5f);
        Vector3 wallOffsetRight = new Vector3(0.5f, 0, 0);
        for (int i = 0; i < nodeMap.graph.GetLength(0); i++) 
        {
            for (int j = 0; j < nodeMap.graph.GetLength(1); j++)
            {
                Node node = nodeMap.graph[i,j];
                if (node != null)
                {
                    if (node.right == null)
                    {
                        g = Instantiate(wallPrefab);
                        node.SetTile(g);
                        g.transform.position = node.position + wallOffsetRight;
                        g.transform.rotation = Quaternion.Euler(0, 90f, 0);
                    }

                    if (node.up == null)
                    {
                        g = Instantiate(wallPrefab);
                        node.SetTile(g);
                        g.transform.position = node.position + wallOffsetUp;
                        g.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                }
                else
                {
                    if (i + 1 < nodeMap.graph.GetLength(0) && nodeMap.graph[i + 1, j] != null)
                    {
                        g = Instantiate(wallPrefab);
                        // TODO: refactor so all nodes are populated but might not be pathables
                        // if (node != null)
                            // node.SetTile(g);
                        g.transform.position = new Vector3(i, 0, j) + wallOffsetRight;
                        g.transform.rotation = Quaternion.Euler(0, 90f, 0);
                    }

                    if (j + 1 < nodeMap.graph.GetLength(1) && nodeMap.graph[i, j + 1] != null)
                    {
                        g = Instantiate(wallPrefab);
                        // TODO: refactor so all nodes are populated but might not be pathables
                        // if (node != null)
                            // node.SetTile(g);
                        g.transform.position = new Vector3(i, 0, j) + wallOffsetUp;
                        g.transform.rotation = Quaternion.Euler(0, 0, 0);
                    }
                }

                /// TODO: Remove, only for debugging
                if (node != null && node.tile != null) 
                {
                    //node.tile.FadeOut();
                }
            }
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
        public int width { get; private set; }
        public int length { get; private set; }
        public Node[,] graph { get; private set; }
        private List<Edge> edges = new List<Edge>();

        public NodeGraph(int _width, int _length) 
        {
            width = _width;
            length = _length;
            graph = new Node[width, length];
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
                    graph[i, j] = new Node(i, j);
                }
            }

            // Iterate through all nodes and create connections
            for (int i = x1; i < x2; i++)
            {
                for (int j = y1; j < y2; j++)
                {
                    Node node = graph[i, j];

                    if (i - 1 >= 0 && i - 1 > x1) node.left = graph[i-1,j];
                    if (i + 1 < graph.GetLength(0) && i + 1 < x2) node.right = graph[i+1,j];
                    if (j - 1 >= 0 && j - 1 > y1) node.down = graph[i, j - 1];
                    if (j + 1 < graph.GetLength(1) && j + 1 < y2) node.up = graph[i, j + 1];
                }
            }
        }
    }

    public class Edge
    {
        public Node a { get; private set; }
        public Node b { get; private set; }

        public bool isPassable { get; private set; } = false;
        public bool isViewable { get; private set; } = false;
        public float moveCost { get; private set; }

        public Tile tile;

        public Edge(Node _a, Node _b, bool _passable, float _moveCost)
        {
            a = _a;
            b = _b;
            isPassable = _passable;
            moveCost = _moveCost;
        }

        public void SetNodes(Node _a, Node _b)
        {
            a = _a;
            b = _b;
        }
    }

    public class Node 
    {
        public int x { get; private set; }
        public int y { get; private set; }

        public Tile tile;
        public Node up;
        public Node down;
        public Node left;
        public Node right;

        public Vector3 position { get; private set; }
        public float moveCost = 1;
        public bool isPassable = true;
        public Pawn pawn = null;
        // TODO: interactable -> floor button, chests, objective

        public Node(int _x, int _y) 
        {
            x = _x;
            y = _y;

            position = new Vector3(x, 0, y);
        }

        public void SetTile(GameObject gameObject) 
        {
            tile = gameObject.GetComponent<Tile>();
        }

        public void SetEdges() 
        {
            
        }
    }
}

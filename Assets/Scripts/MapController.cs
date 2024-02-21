using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static MapController;

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
        nodeMap.BuildRoom(0, 0, 3, 3);
        // nodeMap.BuildRoom(4, 0, 6, 7);
        InstanceMap();

        int count = 0;
        foreach (Edge edge in nodeMap.edges) 
        {
            count++;
        }
        Debug.Log(count);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 size = new Vector3(0.25f, 0.25f, 0.25f);
        if (nodeMap != null) 
        {
            foreach (Node node in nodeMap.graph)
            {
                if (node != null)
                    Gizmos.DrawCube(new Vector3(node.x, node.y, node.z), size);
            }
        }
    }


    private void FixedUpdate()
    {
        DebugLines();
    }

    private void DebugLines() 
    {
        if (nodeMap.graph != null) 
        {
            foreach (Edge e in nodeMap.edges) 
            {
                if (e.isPassable)
                {
                    Debug.DrawLine(e.aPos, e.bPos, Color.blue);
                }
                else 
                {
                    Debug.DrawLine(e.aPos, e.bPos, Color.red);
                }
                
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
        public List<Edge> edges = new List<Edge>();

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

            void addEdges(Node curr) 
            {
                int x = curr.x;
                int z = curr.z;

                if (curr.up == null) 
                {
                    curr.up = generateEdge(curr, new int[] { x, 0, z + 1 });
                    if (curr.up.b != null) curr.up.b.down = curr.up;
                }

                if (curr.upRight == null) 
                {
                    curr.upRight = generateEdge(curr, new int[] { x + 1, 0, z + 1 });
                    if (curr.upRight.b != null) curr.upRight.b.downLeft = curr.upRight;
                }
                
                if (curr.right == null)
                {
                    curr.right = generateEdge(curr, new int[] { x + 1, 0, z });
                    if (curr.right.b != null) curr.right.b.left = curr.right;
                }
                
                if (curr.downRight == null)
                {
                    curr.downRight = generateEdge(curr, new int[] { x + 1, 0, z - 1 });
                    if (curr.downRight.b != null) curr.downRight.b.upLeft = curr.downRight;
                }

                if (curr.down == null)
                {
                    curr.down = generateEdge(curr, new int[] { x, 0, z - 1 });
                    if (curr.down.b != null) curr.down.b.up = curr.down;
                }

                if (curr.downLeft == null)
                {
                    curr.downLeft = generateEdge(curr, new int[] { x - 1, 0, z - 1 });
                    if (curr.downLeft.b != null) curr.downLeft.b.upRight = curr.downLeft;
                }

                if (curr.left == null)
                {
                    curr.left = generateEdge(curr, new int[] { x - 1, 0, z });
                    if (curr.left.b != null) curr.left.b.right = curr.left;
                }
                
                if (curr.upLeft == null)
                {
                    curr.upLeft = generateEdge(curr, new int[] { x - 1, 0, z + 1 });
                    if (curr.upLeft.b != null) curr.upLeft.b.downRight = curr.upLeft;
                }
            }
            Edge generateEdge(Node currnode, int[] pos) 
            {
                // In bounds
                if (pos[0] >= 0 && pos[0] < graph.GetLength(0) &&
                    pos[2] >= 0 && pos[2] < graph.GetLength(1) &&
                    graph[pos[0], pos[2]] != null)
                {
                    Node nextnode = graph[pos[0], pos[2]];
                    // Within room: make pathable
                    if (nextnode.x >= x1 && nextnode.x < x2 &&
                        nextnode.z >= y1 && nextnode.y < y2) 
                    {
                        Edge edge = new Edge(currnode, nextnode, true, 0f);
                        edges.Add(edge);
                        return edge;
                    }
                    // Not within room: add wall
                    else 
                    {
                        Edge edge = new Edge(currnode, nextnode, false, 0f);
                        edges.Add(edge);
                        return edge;
                    }
                }
                // Out of bounds: create empty edge with wall
                else 
                {
                    Edge edge = new Edge(currnode, new Vector3(pos[0], pos[1], pos[2]));
                    edges.Add(edge);
                    return edge;
                }
            }

            // Iterate through all nodes and create connections
            for (int i = x1; i < x2; i++)
            {
                for (int j = y1; j < y2; j++)
                {
                    Node node = graph[i, j];

                    // Check if neighboring nodes
                    addEdges(node);
                }
            }
        }
    }

    public class Edge
    {
        public Node a { get; private set; }
        public Node b { get; private set; }
        public Vector3 aPos { get; private set; }
        public Vector3 bPos { get; private set; }

        public bool isPassable { get; private set; } = false;
        public bool isInView { get; private set; } = false;
        public float moveCost { get; private set; } = 1f;

        public Tile tile;

        public Edge(Node _a, Node _b, bool _passable, float _moveCost)
        {
            a = _a;
            b = _b;
            aPos = a.position;
            bPos = b.position;
            isPassable = _passable;
            moveCost = _moveCost;
        }

        public Edge(Node _a, Vector3 _bPos) 
        {
            a = _a;
            aPos = _a.position;
            bPos = _bPos;
            isPassable = false;
            moveCost = 0f;
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
        public int z { get; private set; }

        public Tile tile;

        public Edge up;
        public Edge upRight;
        public Edge right;
        public Edge downRight;
        public Edge down;
        public Edge downLeft;
        public Edge left;
        public Edge upLeft;

        public Vector3 position { get; private set; }
        public float moveCost = 1;
        public bool isPassable = true;
        public Pawn pawn = null;
        // TODO: interactable -> floor button, chests, objective

        public Node(int _x, int _z) 
        {
            x = _x;
            z = _z;

            position = new Vector3(x, 0, z);
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

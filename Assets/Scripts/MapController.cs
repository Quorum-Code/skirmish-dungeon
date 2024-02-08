using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] Transform mapParent;

    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject wallPrefab;

    Map map;

    [SerializeField] GameObject linePrefab;
    List<GameObject> lines = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        map = new Map(8, 8);
        map.floorArea(0, 0, 5, 5);

        DrawLine(new Vector2(), new Vector2(0, 3));

        InstanceMap();
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

    public void Path(Vector2 from, Vector2 to) 
    {

    }

    public void DrawLine(Vector2 from, Vector2 to) 
    {
        GameObject line = Instantiate(linePrefab);
        line.transform.position = new Vector3(from.x, 0, from.y);
        lines.Add(line);
        line.transform.localScale = new Vector3(1, 1, Vector2.Distance(from, to));
        line.transform.LookAt(new Vector3(to.x, 0, to.y));
    }

    private class Map 
    {
        public Cell[,] cells;
        public Wall[,] walls;

        public Map(int width, int length) 
        {
            cells = new Cell[width, length];
            walls = new Wall[width, length + 2];
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

        public Wall() 
        {
            
        }
    }
}

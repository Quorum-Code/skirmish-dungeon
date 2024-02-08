using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] GameObject mapHost;

    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject wallPrefab;


    // Start is called before the first frame update
    void Start()
    {
        Map map = new Map(16, 16, floorPrefab, wallPrefab); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateMap() 
    {

    }

    public class Map
    {
        Cell[,] cells;
        Wall[,] walls;

        GameObject floorPrefab;
        GameObject wallPrefab;

        public Map(int width, int length, GameObject _floorPrefab, GameObject _wallPrefab) 
        {
            Debug.Log("Making a map..");
            cells = new Cell[width, length];
            walls = new Wall[width+1, length+1];
            floorPrefab = _floorPrefab;
            wallPrefab = _wallPrefab;

            floorArea(0, 0, 6, 6);
        }

        private void floorArea(int x1, int y1, int x2, int y2) 
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

            // Create cells
            GameObject g;
            for (int i = x1; i <= x2; i++) 
            {
                for (int j = y1; j <= y2; j++) 
                {
                    cells[i, j] = new Cell();
                    g = Instantiate(floorPrefab);
                    g.transform.position = new Vector3(i, 0, j);
                }
            }

            // Add walls
            for (int i = x1; i <= x2; i++) 
            {
                Wall w = new Wall();
                w.PlaceWallUp(i, y2);

                walls[w.x, w.y] = w;
                g = Instantiate(wallPrefab);
                Vector2 pos = w.position;
                g.transform.position = new Vector3(pos.x, 0, pos.y);
            }

            for (int i = x1; i <= x2; i++) 
            {
                Wall w = new Wall();
                w.PlaceWallDown(i, y1);

                walls[w.x, w.y] = w;
                g = Instantiate(wallPrefab);
                Vector2 pos = w.position;
                g.transform.position = new Vector3(pos.x, 0, pos.y);
            }
        }

        class Cell 
        {
            
        }

        class Wall 
        {
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
}

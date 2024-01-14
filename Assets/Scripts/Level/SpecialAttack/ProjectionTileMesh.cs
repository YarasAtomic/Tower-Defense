using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectionTileMesh : MonoBehaviour {
    public float cellSize = 1;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float yOffset = 0.5f;
    public Material material;

    private GameObject[] _cells;
    private float[] _heights;

    void Start() {
        _cells = new GameObject[gridHeight * gridWidth];
        _heights = new float[(gridHeight + 1) * (gridWidth + 1)];

        for (int z = 0; z < gridHeight; z++) {
            for (int x = 0; x < gridWidth; x++) {
                _cells[z * gridWidth + x] = CreateChild();
            }
        }
    }

    void Update () {
        UpdateSize();
        UpdatePosition();
        UpdateHeights();
        UpdateCells();
    }

    GameObject CreateChild() {
        GameObject go = new GameObject();

        go.name = "Grid Cell";
        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;

        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshFilter>();

        go.GetComponent<MeshFilter>().mesh = CreateMesh();
        go.GetComponent<MeshRenderer>().material = material;

        return go;
    }

    void UpdateSize() {
        int newSize = gridHeight * gridWidth;
        int oldSize = _cells.Length;

        if (newSize == oldSize)
            return;

        GameObject[] oldCells = _cells;
        _cells = new GameObject[newSize];

        if (newSize < oldSize) {
            for (int i = 0; i < newSize; i++) {
                _cells[i] = oldCells[i];
            }

            for (int i = newSize; i < oldSize; i++) {
                Destroy(oldCells[i]);
            }
        }
        else if (newSize > oldSize) {
            for (int i = 0; i < oldSize; i++) {
                _cells[i] = oldCells[i];
            }

            for (int i = oldSize; i < newSize; i++) {
                _cells[i] = CreateChild();
            }
        }

        _heights = new float[(gridHeight + 1) * (gridWidth + 1)];
    }

    void UpdatePosition() {
        transform.position = new Vector3(transform.position.x,0,transform.position.z);
    }

    void UpdateHeights() {
        RaycastHit hitInfo;
        Vector3 origin;

        for (int z = 0; z < gridHeight + 1; z++) {
            for (int x = 0; x < gridWidth + 1; x++) {
                origin = new Vector3(x * cellSize, 200, z * cellSize);
                Physics.Raycast(transform.TransformPoint(origin), Vector3.down, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Terrain"));

                _heights[z * (gridWidth + 1) + x] = hitInfo.point.y;
                Debug.DrawRay(hitInfo.point,Vector3.up,Color.yellow);
            }
        }
    }

    void UpdateCells() {
        for (int z = 0; z < gridHeight; z++) {
            for (int x = 0; x < gridWidth; x++) {
                GameObject cell = _cells[z * gridWidth + x];
                // MeshRenderer meshRenderer = cell.GetComponent<MeshRenderer>();
                MeshFilter meshFilter = cell.GetComponent<MeshFilter>();

                // meshRenderer.material = IsCellValid(x, z) ? cellMaterialValid : cellMaterialInvalid;
                UpdateMesh(meshFilter.mesh, x, z);
            }
        }
    }

    Mesh CreateMesh() {
        Mesh mesh = new Mesh();

        mesh.name = "Grid Cell";
        mesh.vertices = new Vector3[] { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
        mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
        mesh.normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
        mesh.uv = new Vector2[] { new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, 0) };

        return mesh;
    }

    void UpdateMesh(Mesh mesh, int x, int z) {
        mesh.vertices = new Vector3[] {
            MeshVertex(x, z),
            MeshVertex(x, z + 1),
            MeshVertex(x + 1, z),
            MeshVertex(x + 1, z + 1),
        };

        mesh.uv = new Vector2[] {
            TexVertex(x, z),
            TexVertex(x, z + 1),
            TexVertex(x + 1, z),
            TexVertex(x + 1, z + 1),
        };
    }

    Vector3 MeshVertex(int x, int z) {
        return new Vector3(x * cellSize, _heights[z * (gridWidth + 1) + x] + yOffset, z * cellSize);
    }

    Vector2 TexVertex(int x, int z){
        return new Vector2(x / (float)gridWidth, z / (float)gridHeight);
    }
}
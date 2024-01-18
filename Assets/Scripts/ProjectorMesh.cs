using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectorMesh : MonoBehaviour {
    public float cellSize = 1;
    public int gridWidth = 1;
    public int gridHeight = 1;
    public float yOffset = 0.5f;
    public Material material;

    MeshFilter meshFilter;

    MeshRenderer meshRenderer;

    private int nCells = 0;
    private float[] _heights;

    void Start() {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
    }

    void Update () {
        UpdateSize();
        // UpdatePosition();
        UpdateHeights();
        UpdateCells();
    }

    void UpdateSize() {
        int newSize = gridHeight * gridWidth;
        int oldSize = nCells;

        if (newSize != oldSize){
            nCells = newSize;

            _heights = new float[(gridHeight + 1) * (gridWidth + 1)];

            Mesh mesh = new Mesh();
            mesh.name = "Projection";
            mesh.vertices = new Vector3[(gridHeight) * (gridWidth) * 4];
            mesh.triangles = new int[mesh.vertices.Length/2*3];
            mesh.normals = new Vector3[(gridHeight) * (gridWidth) * 4];
            mesh.uv = new Vector2[(gridHeight) * (gridWidth) * 4];

            for(int i = 0; i < mesh.triangles.Length/3;i+=6){
                mesh.triangles[i+0] = 0 + i*4;
                mesh.triangles[i+1] = 1 + i*4;
                mesh.triangles[i+2] = 2 + i*4;

                mesh.triangles[i+3] = 2 + i*4;
                mesh.triangles[i+4] = 1 + i*4;
                mesh.triangles[i+5] = 3 + i*4;
            }

            // for(int i = 0; i < mesh.uv.Length; i++){
            //     mesh.uv[i] = new Vector2(0,0);
            // }

            // for(int i = 0; i < mesh.normals.Length; i++){
            //     mesh.normals[i] = Vector3.up;
            // }

            Debug.Log("gridHeight " + gridHeight + " gridWidth " + gridWidth);
            Debug.Log("triangle length " + mesh.triangles.Length);
            Debug.Log("vertex length " + mesh.vertices.Length);

            meshFilter.mesh = mesh;
        }
    }

    void UpdatePosition() {
        RaycastHit hitInfo;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // ray.direction = Vector3.down * 100;
        // ray.origin = transform.position;
        
        Physics.Raycast(ray, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Terrain"));
        
        Vector3 position = hitInfo.point;

        position.x -= hitInfo.point.x % cellSize + gridWidth * cellSize / 2;
        position.z -= hitInfo.point.z % cellSize + gridHeight * cellSize / 2;
        position.y = 0;

        transform.position = position;
    }

    void UpdateHeights() {
        RaycastHit hitInfo;
        Vector3 origin;

        for (int z = 0; z < gridHeight + 1; z++) {
            for (int x = 0; x < gridWidth + 1; x++) {
                origin = new Vector3(x * cellSize, 200, z * cellSize);
                Physics.Raycast(transform.TransformPoint(origin), Vector3.down, out hitInfo, Mathf.Infinity, LayerMask.GetMask("Terrain"));

                _heights[z * (gridWidth + 1) + x] = hitInfo.point.y;
            }
        }
    }

    void UpdateCells() {
        int i = 0;
        for (int z = 0; z < gridHeight; z++) {
            for (int x = 0; x < gridWidth; x++) {
                UpdateMesh(meshFilter.mesh,i , x, z);
                i+=4;
            }
        }
    }

    void UpdateMesh(Mesh mesh,int i, int x, int z) {
        mesh.vertices[i+0] = MeshVertex(x, z);
        mesh.vertices[i+1] = MeshVertex(x, z + 1);
        mesh.vertices[i+2] = MeshVertex(x + 1, z);
        mesh.vertices[i+3] = MeshVertex(x + 1, z + 1);
    }

    Vector3 MeshVertex(int x, int z) {
        Vector3 pos = new Vector3(x * cellSize, _heights[z * (gridWidth + 1) + x] + yOffset, z * cellSize);
        Debug.DrawRay(transform.position + pos,Vector3.up);
        return pos;
    }
}
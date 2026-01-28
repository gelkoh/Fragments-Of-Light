using UnityEngine;
using UnityEngine.InputSystem;

public class PageClickDetector : MonoBehaviour
{
    private Camera m_mainCamera;
    
    // Separate Canvas Namen für Vorder- und Rückseite
    private string m_frontCanvasName;  // Material Index 2 (Front)
    private string m_backCanvasName;   // Material Index 1 (Back)
    
    [Header("Debug")]
    [SerializeField] private bool showDebugRay = true;
    [SerializeField] private Color debugRayColor = Color.yellow;
    
    private Vector3 lastHitPoint;
    private bool hasLastHit = false;

    void Awake()
    {
        m_mainCamera = Camera.main; 
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        RaycastHit hit;
        Ray ray = m_mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out hit))
        {
            hasLastHit = true;
            lastHitPoint = hit.point;
            
            if (showDebugRay)
            {
                Debug.DrawLine(ray.origin, hit.point, debugRayColor, 2f);
            }

            if (hit.collider.gameObject == gameObject)
            {
                Vector2 uvHit = hit.textureCoord;
                
                // WICHTIG: Bestimme welche Seite getroffen wurde
                int materialIndex = GetHitMaterialIndex(hit);
                
                string targetCanvas = null;
                bool flipHorizontal = false;
                bool flipVertical = false;
                
                if (materialIndex == 2) // Front (rechte Seite)
                {
                    targetCanvas = m_frontCanvasName;
                    flipHorizontal = false;
                    flipVertical = false;
                }
                else if (materialIndex == 1) // Back (linke Seite)
                {
                    targetCanvas = m_backCanvasName;
                    flipHorizontal = true; // Rückseite ist meist gespiegelt
                    flipVertical = false;
                }
                
                if (string.IsNullOrEmpty(targetCanvas))
                {
                    Debug.LogWarning($"No canvas defined for material index {materialIndex}");
                    return;
                }
                
                Debug.Log($"Hit page: {gameObject.name}, Material: {materialIndex}, UV: {uvHit}, Target: {targetCanvas}");
                
                GameObject handlerObject = GameObject.Find(targetCanvas);

                if (handlerObject == null)
                {
                    Debug.LogWarning($"Canvas '{targetCanvas}' not found!");
                    return;
                }

                CanvasClickHandler handler = handlerObject.GetComponent<CanvasClickHandler>();

                if (handler != null)
                {
                    handler.SetFlipping(flipHorizontal, flipVertical);
                    handler.HandlePageClick(uvHit);
                }
                else
                {
                    Debug.LogWarning($"No CanvasClickHandler on '{targetCanvas}'");
                }
            }
        }
        else
        {
            hasLastHit = false;
        }
    }
    
    private int GetHitMaterialIndex(RaycastHit hit)
    {
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return -1;

        Mesh mesh = meshCollider.sharedMesh;
        int[] triangles = mesh.triangles;
        
        // Finde heraus welches Dreieck getroffen wurde
        int triangleIndex = hit.triangleIndex * 3;
        
        if (triangleIndex >= triangles.Length)
            return -1;
        
        // Bestimme Submesh (= Material Index)
        int submeshCount = mesh.subMeshCount;
        int trianglesSoFar = 0;
        
        for (int i = 0; i < submeshCount; i++)
        {
            int submeshTriangleCount = mesh.GetTriangles(i).Length;
            
            if (triangleIndex < trianglesSoFar + submeshTriangleCount)
            {
                return i; // Dieser Submesh wurde getroffen
            }
            
            trianglesSoFar += submeshTriangleCount;
        }
        
        return -1;
    }

    private void OnDrawGizmos()
    {
        if (!hasLastHit) return;
        
        // Zeichne Hit Point
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lastHitPoint, 0.03f);
        
        // Zeichne UV Grid zur Orientierung
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null && meshCollider.sharedMesh != null)
        {
            DrawUVDebugGrid();
        }
    }
    
    private void DrawUVDebugGrid()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        Mesh mesh = meshCollider.sharedMesh;
        
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;
        
        Gizmos.color = Color.green;
        
        // Zeichne UV Grid Linien
        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 v0 = transform.TransformPoint(vertices[triangles[i]]);
            Vector3 v1 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 v2 = transform.TransformPoint(vertices[triangles[i + 2]]);
            
            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v0);
        }
    }

    // NEUE API: Setze Canvas für beide Seiten
    public void SetCanvasNames(string frontCanvas, string backCanvas)
    {
        m_frontCanvasName = frontCanvas;
        m_backCanvasName = backCanvas;
        Debug.Log($"PageClickDetector on {gameObject.name}: Front={frontCanvas}, Back={backCanvas}");
    }
    
    // Rückwärtskompatibilität: Alte Methode setzt beide auf gleich
    public void SetTargetCanvas(string canvasName)
    {
        m_frontCanvasName = canvasName;
        m_backCanvasName = canvasName;
        Debug.Log($"PageClickDetector on {gameObject.name} targets: {canvasName}");
    }

    public string GetFrontCanvas()
    {
        return m_frontCanvasName;
    }
    
    public string GetBackCanvas()
    {
        return m_backCanvasName;
    }
}
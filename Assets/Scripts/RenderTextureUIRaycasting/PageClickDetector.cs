using UnityEngine;
using UnityEngine.InputSystem;

public class PageClickDetector : MonoBehaviour
{
    private Camera m_mainCamera;
    private string m_targetCanvasName;
    
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
                
                Debug.Log($"Hit page: {gameObject.name}, UV: {uvHit}, Target: {m_targetCanvasName}");
                
                GameObject handlerObject = GameObject.Find(m_targetCanvasName);

                if (handlerObject == null)
                {
                    Debug.LogWarning($"Canvas '{m_targetCanvasName}' not found!");
                    return;
                }

                CanvasClickHandler handler = handlerObject.GetComponent<CanvasClickHandler>();

                if (handler != null)
                {
                    handler.HandlePageClick(uvHit);
                }
                else
                {
                    Debug.LogWarning($"No CanvasClickHandler on '{m_targetCanvasName}'");
                }
            }
        }
        else
        {
            hasLastHit = false;
        }
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

    public void SetTargetCanvas(string canvasName)
    {
        m_targetCanvasName = canvasName;
        Debug.Log($"PageClickDetector on {gameObject.name} now targets: {canvasName}");
    }

    public string GetTargetCanvas()
    {
        return m_targetCanvasName;
    }
}
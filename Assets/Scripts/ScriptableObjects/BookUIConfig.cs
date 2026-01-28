using UnityEngine;

[CreateAssetMenu(fileName = "BookUIConfig", menuName = "ScriptableObjects/BookUIConfig")]
public class BookUIConfig : ScriptableObject
{
    public GameObject PagesUICanvasPrefab;

    public GameObject PageBorderPrefab;
    
    public GameObject PreviousPageButtonPrefab;
    public GameObject NextPageButtonPrefab;
    
    public GameObject PageNumberLeftPrefab;
    public GameObject PageNumberRightPrefab;
    
    public Vector3 PreviousPageButtonOffset;
    public Vector3 NextPageButtonOffset;
    public Vector3 PageNumberLeftOffset;
    public Vector3 PageNumberRightOffset;
}
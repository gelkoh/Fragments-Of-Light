using UnityEngine;

public enum PageID
{
    CoverFront,
    Endpaper1,
    FrontispieceAndTitlepage,
    Chapter1Introduction,
    Chapter1Level1Gameplay,
    Chapter1Level2Gameplay,
	Chapter2Introduction,
	Chapter2Level1Gameplay,
	Chapter2Level2Gameplay,
	Chapter2Level3Gameplay,
	Chapter2Level4Gameplay,
	ThanksForPlayingThePrototype,
	Endpaper2,
	CoverBack
}

[CreateAssetMenu(fileName = "BookPageConfig", menuName = "ScriptableObjects/BookPageConfig")]
public class BookPageConfig : ScriptableObject
{
    public PageID ID;
    public bool ShowPageBorder = true;
    
    public bool ShowNextPageButton = true;
    public bool ShowPreviousPageButton = true;
    public bool ShowPageNumberLeft = true;
    public bool ShowPageNumberRight = true;
}
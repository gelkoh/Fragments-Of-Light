using UnityEngine;

[CreateAssetMenu(fileName = "BookSettings", menuName = "ScriptableObjects/BookSettings", order = 2)]
public class BookSettings : ScriptableObject
{
    public AudioClip PageFlipAudioClip;
    
    public Material CoverFrontMaterial;

    public Material Endpaper1LeftMaterial;
    public Material Endpaper1RightMaterial;

    public Material FrontispieceMaterial;
    public Material TitlepageMaterial;

    public Material Chapter1IntroductionLeftMaterial;
    public Material Chapter1IntroductionRightMaterial;

    public Material Chapter1Level1LeftMaterial;
    public Material Chapter1Level1RightMaterial;

    public Material Chapter1Level2LeftMaterial;
    public Material Chapter1Level2RightMaterial;

    public Material Chapter2IntroductionLeftMaterial;
    public Material Chapter2IntroductionRightMaterial;

    public Material Chapter2Level1LeftMaterial;
    public Material Chapter2Level1RightMaterial;

    public Material Chapter2Level2LeftMaterial;
    public Material Chapter2Level2RightMaterial;

 	public Material Chapter2Level3LeftMaterial;
    public Material Chapter2Level3RightMaterial;

 	public Material Chapter2Level4LeftMaterial;
    public Material Chapter2Level4RightMaterial;

	public Material ThanksForPlayingThePrototypeLeftMaterial;
    public Material ThanksForPlayingThePrototypeRightMaterial;

   	public Material Endpaper2LeftMaterial;
    public Material Endpaper2RightMaterial;

    public Material CoverBackMaterial;

	public Material IngameMenuLeftMaterial;
	public Material IngameMenuRightMaterial;
	
	public Material UnusedPageMaterial;
	public Material SpineMaterial;
	public Material CoverSideMaterial;
}
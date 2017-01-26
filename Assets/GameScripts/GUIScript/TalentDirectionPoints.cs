using UnityEngine;
using System.Collections;

public class TalentDirectionPoints : MonoBehaviour 
{
	public UISprite	spHorizontal;
	public UISprite spVertical;
	public UISprite	spUp;
	public UISprite spDown;
	public UISprite spRight;
	public UISprite spLeft;
	public UISprite spPointUp;
	public UISprite spPointDown;
	public UISprite	spPointRight;
	public UISprite spPointLeft;
	//Light
	public UISprite	spLightHorizontal;
	public UISprite spLightVertical;
	public UISprite	spLightUp;
	public UISprite spLightDown;
	public UISprite spLightRight;
	public UISprite spLightLeft;
	public UISprite spLightPointUp;
	public UISprite spLightPointDown;
	public UISprite	spLightPointRight;
	public UISprite spLightPointLeft;
	//-----------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------
	public void InitHide()
	{
		spHorizontal.gameObject.SetActive(false);
		spVertical.gameObject.SetActive(false);
		spUp.gameObject.SetActive(false);
		spDown.gameObject.SetActive(false);
		spRight.gameObject.SetActive(false);
		spLeft.gameObject.SetActive(false);
		spPointUp.gameObject.SetActive(false);
		spPointDown.gameObject.SetActive(false);
		spPointRight.gameObject.SetActive(false);
		spPointLeft.gameObject.SetActive(false);
		//Light
		spLightHorizontal.gameObject.SetActive(false);
		spLightVertical.gameObject.SetActive(false);
		spLightUp.gameObject.SetActive(false);
		spLightDown.gameObject.SetActive(false);
		spLightRight.gameObject.SetActive(false);
		spLightLeft.gameObject.SetActive(false);
		spLightPointUp.gameObject.SetActive(false);
		spLightPointDown.gameObject.SetActive(false);
		spLightPointRight.gameObject.SetActive(false);
		spLightPointLeft.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------
	public void SetAllDepth(int iDepth)
	{
		spHorizontal.depth = iDepth;
		spVertical.depth = iDepth;
		spUp.depth = iDepth;
		spDown.depth = iDepth;
		spRight.depth = iDepth;
		spLeft.depth = iDepth;
		spPointUp.depth = iDepth;
		spPointDown.depth = iDepth;
		spPointRight.depth = iDepth;
		spPointLeft.depth = iDepth;
		//Light
		spLightHorizontal.depth = iDepth;
		spLightVertical.depth = iDepth;
		spLightUp.depth = iDepth;
		spLightDown.depth = iDepth;
		spLightRight.depth = iDepth;
		spLightLeft.depth = iDepth;
		spLightPointUp.depth = iDepth;
		spLightPointDown.depth = iDepth;
		spLightPointRight.depth = iDepth;
		spLightPointLeft.depth = iDepth;
	}
	//-----------------------------------------------------------------------------------------------
	public void ShowFromUp(bool isLight)
	{
		spUp.gameObject.SetActive(!isLight);
		spLightUp.gameObject.SetActive(isLight);
	}
	//-----------------------------------------------------------------------------------------------
	public void ShowFromDown(bool isLight)
	{
		spDown.gameObject.SetActive(!isLight);
		spLightDown.gameObject.SetActive(isLight);
	}
	//-----------------------------------------------------------------------------------------------
	public void ShowFromRight(bool isLight)
	{
		spRight.gameObject.SetActive(!isLight);
		spLightRight.gameObject.SetActive(isLight);
	}
	//-----------------------------------------------------------------------------------------------
	public void ShowFromLeft(bool isLight)
	{
		spLeft.gameObject.SetActive(!isLight);
		spLightLeft.gameObject.SetActive(isLight);
	}
	//-----------------------------------------------------------------------------------------------
	public void ShowToUp(bool isLight)
	{
		spPointUp.gameObject.SetActive(!isLight);
		spLightPointUp.gameObject.SetActive(isLight);
	}
	//-----------------------------------------------------------------------------------------------
	public void ShowToDown(bool isLight)
	{
		spPointDown.gameObject.SetActive(!isLight);
		spLightPointDown.gameObject.SetActive(isLight);
	}
	//-----------------------------------------------------------------------------------------------
	public void ShowToRight(bool isLight)
	{
		spPointRight.gameObject.SetActive(isLight);
		spLightPointRight.gameObject.SetActive(!isLight);
	}
	//-----------------------------------------------------------------------------------------------
	public void ShowToLeft(bool isLight)
	{
		spPointLeft.gameObject.SetActive(!isLight);
		spLightPointLeft.gameObject.SetActive(isLight);
	}
	//-----------------------------------------------------------------------------------------------
	public void ShowHorizontal(bool isLight)
	{
		spHorizontal.gameObject.SetActive(!isLight);
		spLightHorizontal.gameObject.SetActive(isLight);
	}
	//-----------------------------------------------------------------------------------------------
	public void ShowVertical(bool isLight)
	{
		spVertical.gameObject.SetActive(!isLight);
		spLightVertical.gameObject.SetActive(isLight);
	}
	//-----------------------------------------------------------------------------------------------
}

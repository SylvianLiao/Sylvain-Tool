using UnityEngine;
using System.Collections;

public class SellBuffInfo : MonoBehaviour 
{
	public UISprite		spriteBG			= null;
	public UILabel		lbBuffEffect 		= null;
	public UISprite		spriteBuffIcon 		= null;
	public UILabel		lbBuffSell			= null;
	public UISprite		spriteBuffSell		= null;
	public BoxCollider	boxCollider			= null;

	//-----------------------------------------------------------------------------------------------
	void Start()
	{
		Init ();
	}
	//-----------------------------------------------------------------------------------------------
	void Update()
	{

	}
	//-----------------------------------------------------------------------------------------------
	void Init()
	{
		lbBuffEffect.gameObject.SetActive(false);
		lbBuffSell.gameObject.SetActive(false);
		spriteBuffSell.gameObject.SetActive(false);
	}
	//-----------------------------------------------------------------------------------------------
	//-----------------------------------------------------------------------------------------------
}

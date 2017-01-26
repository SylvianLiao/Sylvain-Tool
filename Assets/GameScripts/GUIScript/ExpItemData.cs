using UnityEngine;
using System.Collections;

public class ExpItemData : MonoBehaviour
{
	public UIButton btnSelected 	= null;
	public UILabel	lbItemCount		= null;
	public UISprite	spriteItemIcon	= null;
	public UISprite	spriteBorder	= null;
	public UISprite	spriteBG		= null;
	public UILabel	lbExpValue		= null;
	[HideInInspector]public int	itemIcon =0;
	[HideInInspector]public int	itemGUID =0;
	[HideInInspector]public int itemCount =0;
	[HideInInspector]public int itemindex =-1;
}

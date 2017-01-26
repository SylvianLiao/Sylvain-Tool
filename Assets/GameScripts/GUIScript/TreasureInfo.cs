using UnityEngine;
using System.Collections;

public class TreasureInfo : MonoBehaviour 
{
	public UIWidget			Treasure			= null;
	//寶箱特效
	public UIButton			btnTreasureBox		= null;
	public GameObject		BoxEffect			= null;
	public UISprite			spriteLight			= null;
	//寶物內容
	public UISprite			spriteRewardSet		= null;
	public UISprite			spriteReward		= null;
	public UISprite			spriteRewardMask	= null;
	public UISprite			spriteRewardBG		= null;
	public UILabel			lbReward			= null;
	public UILabel			lbRewardCount		= null;
	private const int 		m_PetPieceID		= 1140;
	//-----------------------------------------------------------------------------------------------------
	//設定寶箱資料內容
	public void SetTreasureData(int ItemCount,int ItemDBID)
	{
		if(ItemCount == 0 || ItemDBID == 0)
		{
			Treasure.gameObject.SetActive(false);
			UnityDebugger.Debugger.LogError("Occur Error with Count Number or No this Item");
			return;
		}
		S_Item_Tmp itemdbf = GameDataDB.ItemDB.GetData(ItemDBID);
		if(itemdbf == null)
		{
			UnityDebugger.Debugger.LogError("No this ItemDBF data with ItemDBID"+ItemDBID.ToString());
			return;
		}
//		if (itemdbf.ItemType == ENUM_ItemType.ENUM_ItemType_PetPiece)
//			Utility.ChangeAtlasSprite(spriteRewardMask , m_PetPieceID);
//		itemdbf.SetRareColor(spriteRewardMask , spriteRewardBG);

		if (itemdbf.ItemType == ENUM_ItemType.ENUM_ItemType_PetPiece)
		{
			//寵物碎片
			itemdbf.SetPetPieceRarity(spriteRewardMask , spriteRewardBG);	
		}
		else
		{
			//一般道具
			itemdbf.SetItemRarity(spriteRewardMask , spriteRewardBG);
		}


		Utility.ChangeAtlasSprite(spriteReward,itemdbf.ItemIcon);		//設定圖
		lbRewardCount.text 	= ItemCount.ToString();						//設定物品個數
		lbReward.text 		= GameDataDB.GetString(itemdbf.iName); 		//設定物品名稱
		itemdbf.SetRareColorString(lbReward);
	}
}

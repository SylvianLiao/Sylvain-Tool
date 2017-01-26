using System;
using System.Collections.Generic;

/// <summary>字串表</summary>
[Serializable]
public class S_StringTable_Tmp : I_BaseDBF
{
    public int GUID;
    public string strZH_TW;
    public string strZH_CN;
    public string strENG_US;

    public S_StringTable_Tmp() { }
    //---------------------------------------------------------------------------------
    public int GetGUID()
    {
        return GUID;
    }
    //---------------------------------------------------------------------------------
    public void ParseJson(string JsonString, IConverter Converter, I_BaseDBF Record)
    {
    }
}
/// <summary>歌曲表</summary>
[Serializable]
public class S_Songs_Tmp : I_BaseDBF
{
    public int GUID;
    public int iGroupID;
    public int iShowName;
    public string strNameTexture;
    public int iComposer;
    public int iFullComposer;
    public bool bIsOriginal;
    public int iIntroduce;
    public int iFullIntroduce;
    public string strAssetName;
    public string strShortAssetName;
    public string strTexture;
    public string strBgMaterial;
    public string strBattleBG;
    public string strMenuMaterial;
    public string strSheetMusicName;
    public int iScore_G;
    public int iScore_S;
    public int iScore_A;
    public int iScore_B;
    public int iScore_C;
    public bool bUseLowVolume;

    //Only Server Load
    public int iDifficulty;
    public int iStar;
    //
    public float fNodeSpeed;

    public S_Songs_Tmp() { }
    //---------------------------------------------------------------------------------
    public int GetGUID()
    {
        return GUID;
    }
    //---------------------------------------------------------------------------------
    public void ParseJson(string JsonString, IConverter Converter, I_BaseDBF Record)
    {
        Dictionary<string, string> values = Converter.deserializeObject(JsonString);
        string strValue;
        strValue = values["iUseLowVolume"].ToString();
        bUseLowVolume = (strValue == "1") ? true : false;
        strValue = values["iIsOriginal"].ToString();
        bIsOriginal = (strValue == "1") ? true : false;
    }
}

public struct ThemeIntroduce
{
    public Enum_ThemeIntroduceStyle m_style;
    public int m_GUID;
    public ThemeIntroduce(Enum_ThemeIntroduceStyle style, int GUID)
    {
        m_style = style;
        m_GUID = GUID;
    }
}
/// <summary>主題館表</summary>
[Serializable]
public class S_Themes_Tmp : I_BaseDBF
{
    public int GUID;
    public int iShowName;
    public string strSongGroupList;
    public string strBgPrefab;
    public string strMusicName;
    public string strInforIcon;
    public string strInforTexture;
    public int iIntroduceLabel_1;
    public int iIntroduceTexture_1;
    public int iIntroduceLabel_2;

    public List<int> m_iSongsGroupList;
    public List<ThemeIntroduce> m_themeIntroStyle;

    public S_Themes_Tmp()
    {
        m_iSongsGroupList = new List<int>();
        m_themeIntroStyle = new List<ThemeIntroduce>();
    }
    //---------------------------------------------------------------------------------
    public int GetGUID()
    {
        return GUID;
    }
    //---------------------------------------------------------------------------------
    public void ParseJson(string JsonString, IConverter Converter, I_BaseDBF Record)
    {
        Dictionary<string, string> values = Converter.deserializeObject(JsonString);
        string strValue;

        //Deal with Song Menu
        strValue = values["strSongGroupList"].ToString();
        string[] strSplit = strValue.Split(new string[1] { ";" }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string str in strSplit)
        {
            int songIndex;
            int.TryParse(str, out songIndex);
            m_iSongsGroupList.Add(songIndex);
        }

        //Deal with Theme Indroduce Content
        int labelIndex = 1;
        int textureIndex = 1;
        foreach (KeyValuePair<string, string> jsonData in values)
        {
            if (jsonData.Key.Equals("iIntroduceLabel_" + labelIndex))
            {
                int stringID;
                int.TryParse(jsonData.Value, out stringID);
                ThemeIntroduce intro = new ThemeIntroduce(Enum_ThemeIntroduceStyle.LabelStyle, stringID);
                m_themeIntroStyle.Add(intro);
                labelIndex++;
            }
            else if (jsonData.Key.Equals("iIntroduceTexture_" + textureIndex))
            {
                int textureID;
                int.TryParse(jsonData.Value, out textureID);
                ThemeIntroduce intro = new ThemeIntroduce(Enum_ThemeIntroduceStyle.TextureStyle, textureID);
                m_themeIntroStyle.Add(intro);
                textureIndex++;
            }        
        }
    }
    //---------------------------------------------------------------------------------
}
/// <summary>貼圖表</summary>
[Serializable]
public class S_TextureTable_Tmp : I_BaseDBF
{
    public int GUID;
    public string strTextureName;
    public string strAtlasName;
    public string strSpriteName;
    public string strMaterialName;
    public string str3D_GUI;

    public S_TextureTable_Tmp() { }
    //---------------------------------------------------------------------------------
    public int GetGUID()
    {
        return GUID;
    }
    //---------------------------------------------------------------------------------
    public void ParseJson(string JsonString, IConverter Converter, I_BaseDBF Record)
    {
    }
}
/// <summary>音效表</summary>
[Serializable]
public class S_Sound_Tmp : I_BaseDBF
{
    public int GUID;
    public string strSoundName;

    public S_Sound_Tmp() { }
    //---------------------------------------------------------------------------------
    public int GetGUID()
    {
        return GUID;
    }
    //---------------------------------------------------------------------------------
    public void ParseJson(string JsonString, IConverter Converter, I_BaseDBF Record)
    {
    }
}
/// <summary>歌曲解鎖表</summary>
[Serializable]
public class S_SongUnlock_Tmp : I_BaseDBF
{
    public int GUID;
    public int iSongGUID;
    public bool IsUnlock;
    public int iConditionSong;
    public Enum_SongRank iConditionRank;
    public string strUnlockStartTime;
    public string strUnlockEndTime;

    public S_SongUnlock_Tmp() { }
    //---------------------------------------------------------------------------------
    public int GetGUID()
    {
        return GUID;
    }
    //---------------------------------------------------------------------------------
    public void ParseJson(string JsonString, IConverter Converter, I_BaseDBF Record)
    {
        Dictionary<string, string> values = Converter.deserializeObject(JsonString);
        string strValue;
        strValue = values["iUnlock"].ToString();
        IsUnlock = (strValue == "1") ? true : false;
    }
}
/// <summary>商店表</summary>
[Serializable]
public class S_Shop_Tmp : I_BaseDBF
{
    public int GUID;
    public int iName;
    public Enum_GoodsType iGoodsType;
    public string strTag;

    public S_Shop_Tmp() { }
    //---------------------------------------------------------------------------------
    public int GetGUID()
    {
        return GUID;
    }
    //---------------------------------------------------------------------------------
    public void ParseJson(string JsonString, IConverter Converter, I_BaseDBF Record)
    {
    }
}
/// <summary>商品表</summary>
[Serializable]
public class S_Goods_Tmp : I_BaseDBF
{
    public int GUID;
    public int iName;
    public Enum_GoodsType iGoodsType;
    public Enum_CurrencyType iCurrencyType;
    public int iOriginalGUID;
    public int iPrice;
    public float fDiscount;
    public int iTag;

    public S_Goods_Tmp() { }
    //---------------------------------------------------------------------------------
    public int GetGUID()
    {
        return GUID;
    }
    //---------------------------------------------------------------------------------
    public void ParseJson(string JsonString, IConverter Converter, I_BaseDBF Record)
    {
    }
}

/// <summary>新手教學表</summary>
[Serializable]
public class S_NewGuide_Tmp : I_BaseDBF
{
    public int GUID;                            //編號
    public Enum_NewGuideType iType;             //教學類型
    public int iNextGUID;                       //下一個教學步驟編號
    public string strStateName;                 //教學欲在哪個State執行
    public string strUIName;                    //教學欲顯示在哪個UI
    public Enum_GuideNextStepCondition iNextCondition;      //下一個教學步驟的條件
    public int iStartBattleGuide;
    public bool StartBattleGuide;               //是否觸發戰鬥教學
    public float fWaitSeconds;                    //教學等待時間
    public int iUseBlackBG;
    public bool UseBlackBG;                     //是否使用黑幕
    public int iUseOriginalFunction;
    public bool UseOriginalFunction;            //是否使用原狀態機的功能
    public S_NewGuide_Tmp(){}
    public int GetGUID() { return GUID; }
    public void ParseJson(string JsonString, IConverter Converter, I_BaseDBF Record)
    {
        Dictionary<string, string> values = Converter.deserializeObject(JsonString);
        string strValue;
        strValue = values["iStartBattleGuide"].ToString();
        StartBattleGuide = (strValue == "1") ? true : false;
        strValue = values["iUseOriginalFunction"].ToString();
        UseOriginalFunction = (strValue == "1") ? true : false;
        strValue = values["iUseBlackBG"].ToString();
        UseBlackBG = (strValue == "1") ? true : false;
    }
}


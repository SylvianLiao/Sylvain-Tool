using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Softstar.GamePacket;
using Softstar;

public enum Enum_GoodsType
{
    Diamond = 0,
    Music,
    Package,
    SkinAndEffects,
    Max,
}

public class ShopState : CustomBehaviorState
{
    private UI_Shop m_uiShop;
    private UI_CommonBackground m_uiCommonBackground;

    private ResourceManager     m_resourceManager;
    private GameDataDB          m_gameDataDB;
    private PlayerDataSystem    m_playerDataSystem;
    private RecordSystem        m_recordSystem;
    //private T_GameDB<ItemmallData> m_goodsDB;
    private PacketHandler_ItemMall m_packet_ItemMall;    

    private Dictionary<Enum_GoodsType, List<ItemmallData>> m_allGoodsDataMap;//<商店類型, <商品guid, 商品資料>>               
    private List<ItemmallData> m_currentGoodsDataList                        //目前選取頁面的商品資料
    {
        get { return m_allGoodsDataMap[m_currentShopPage]; }
    }

    //RunTime Data
    private ItemmallData m_currentChosenGoods;                               //玩家目前選擇的商品資料
    private Enum_GoodsType m_currentShopPage;                               //目前顯示哪個商店頁面
    public Enum_GoodsType CurrentShopPage
    {
        get { return m_currentShopPage; }
        set { m_currentShopPage = value; }
    }
    private List<AsyncLoadOperation> m_goodsTextureLoader;
    private bool m_bIsTextureLoadFinish;
    private List<Coroutine> m_LoadGoodsResourceCoroutine;
    private bool m_bInitFinish;

    public ShopState(GameScripts.GameFramework.GameApplication app) : base(StateName.SHOP_STATE, StateName.SHOP_STATE, app)
    {
        m_gameDataDB = m_mainApp.GetGameDataDB();
        m_resourceManager = m_mainApp.GetResourceManager();
        m_playerDataSystem = m_mainApp.GetSystem<PlayerDataSystem>();
        m_recordSystem = m_mainApp.GetSystem<RecordSystem>();
        //m_goodsDB = m_gameDataDB.GetGameDB<ItemmallData>();
        m_packet_ItemMall = m_mainApp.GetSystem<NetworkSystem>().GetPacketHandler<PacketHandler_ItemMall>();

        m_allGoodsDataMap = new Dictionary<Enum_GoodsType, List<ItemmallData>>();
        m_goodsTextureLoader = new List<AsyncLoadOperation>();
        m_LoadGoodsResourceCoroutine = new List<Coroutine>();
        m_bIsTextureLoadFinish = false;
        m_currentChosenGoods = null;
        m_currentShopPage = Enum_GoodsType.Diamond;
        m_bInitFinish = false;
    }

    //---------------------------------------------------------------------------------------------------
    public override void begin()
    {
        Debug.Log("ShopState begin");

        //Set the GUI witch is this state want to use.
        base.SetGUIType(typeof(UI_Shop));
        base.SetGUIType(typeof(UI_CommonBackground));
        AsyncLoadOperation loader = m_resourceManager.GetResourceASync(Enum_ResourcesType.GUI, typeof(Slot_Goods).Name, typeof(Slot_Goods), null);
        m_guiLoadOperator.Add(loader);
        userData.Add(Enum_StateParam.UseTopBarUI, true);
        userData.Add(Enum_StateParam.UsePlayerInfoUI, true);

        base.begin();

        if (m_bIsAync == false)
        {
            m_uiShop = m_guiManager.AddGUI<UI_Shop>(typeof(UI_Shop).Name);
            m_uiCommonBackground = m_guiManager.AddGUI<UI_CommonBackground>(typeof(UI_CommonBackground).Name);
            StateInit();
        }
    }
    //---------------------------------------------------------------------------------------------------
    protected override void GetGUIAsync(AsyncLoadOperation operater)
    {
        base.GetGUIAsync(operater);

        if (operater.m_Type.Equals(typeof(UI_Shop)))
            m_uiShop = m_guiManager.GetGUI(typeof(UI_Shop).Name) as UI_Shop;
        else if (operater.m_Type.Equals(typeof(UI_CommonBackground)))
            m_uiCommonBackground = m_guiManager.GetGUI(typeof(UI_CommonBackground).Name) as UI_CommonBackground;
    }
    //---------------------------------------------------------------------------------------------------
    protected override void StateInit()
    {
        base.StateInit();

        //跟Server要求商品資料
        m_packet_ItemMall.SendPacket_GetItemmallData();
    }
    /// <summary>取得資料後才開始初始化狀態機</summary>------------------------------------------------------------------------------------
    public void StateInitAfterDataGot(GetItemmallDataResPacket pk)
    {
        //UI初始化
        m_guiManager.Initialize();
        m_uiShop.InitializeUI(m_mainApp.GetStringTable());

        m_currentShopPage = Enum_GoodsType.Music;
        SetShopData(pk.shop_datas);
        SetGoodsData(pk.itemmall_datas);

        InitWrapContent();
        m_uiShop.m_musicPage.m_wcGoodsMenu.onInitializeItem += AssignSongDataToWrapContent;
        m_uiShop.m_musicPage.m_wcGoodsMenu.onInitializeItem += SetDefaultGoods;

        m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));

        m_uiPlayerInfo.ShowButton(false);
        m_uiPlayerInfo.ShowAvatar(false);
        m_uiPlayerInfo.ShowMoney(true);
    }

    //---------------------------------------------------------------------------------------------------
    public override void end()
    {
        ReleaseRuntimeData();
        m_currentShopPage = Enum_GoodsType.Music;
        m_uiShop = null;
        m_uiCommonBackground = null;
        m_guiManager.DeleteGUI(typeof(UI_Shop).Name);
        m_guiManager.DeleteGUI(typeof(UI_CommonBackground).Name);
        base.end();
        Debug.Log("ShopState End");
    }

    //---------------------------------------------------------------------------------------------------
    public override void suspend()
    {
        if (m_bIsSuspendByFullScreenState && m_uiShop != null)
        {
            CheckActiveCommonGUI();
            m_uiShop.Hide();
        }
        RemoveCallBack();
        base.suspend();
        Debug.Log("ShopState suspend");
    }

    //---------------------------------------------------------------------------------------------------
    public override void resume()
    {
        if (m_uiShop != null)
        {
            if (m_bIsSuspendByFullScreenState)
                m_mainApp.MusicApp.StartCoroutine(base.CheckAndActiveGUI(AddCallBack));
            else
            {
                CheckActiveCommonGUI();
                ShowBeSetGUI();
                AddCallBack();
            }

            m_uiPlayerInfo.ShowButton(false);
            m_uiPlayerInfo.ShowAvatar(false);
            m_uiPlayerInfo.ShowMoney(true);
            m_bIsSuspendByFullScreenState = false;
        }
        base.resume();
        Debug.Log("ShopState resume");
    }

    //---------------------------------------------------------------------------------------------------
    public override void update()
    {
        base.update();
    }

    //---------------------------------------------------------------------------------------------------
    public override void AddCallBack()
    {
        m_uiTopBar.m_onButtonBackClick = OnButtonBackClick;
        m_uiTopBar.AddCallBack();
        m_uiCommonBackground.AddCallBack();
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_GoodsType.SkinAndEffects].gameObject).onClick += OnButtonSkinAndEffectsPageClick;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_GoodsType.Package].gameObject).onClick += OnButtonPackagePageClick;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_GoodsType.Music].gameObject).onClick += OnButtonMusicPageClick;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_GoodsType.Diamond].gameObject).onClick += OnButtonDiamondPageClick;
        AddPageCallBack(m_currentShopPage);
    }
    //---------------------------------------------------------------------------------------------------
    private void AddPageCallBack(Enum_GoodsType type)
    {
        switch (type)
        {
            case Enum_GoodsType.Diamond:
                break;
            case Enum_GoodsType.Music:
                for (int i = 0, iCount = m_uiShop.m_musicPage.m_slotGoodsObjList.Count; i < iCount; ++i)
                {
                    UIEventListener.Get(m_uiShop.m_musicPage.m_slotGoodsObjList[i].gameObject).onClick = OnButtonGoodsClick;
                }
                UIEventListener.Get(m_uiShop.m_musicPage.m_buttonBuy.gameObject).onClick = OnButtonBuyClick;
                break;
            case Enum_GoodsType.Package:
                break;
            case Enum_GoodsType.SkinAndEffects:
                break;
        }
    }
    //---------------------------------------------------------------------------------------------------
    public override void RemoveCallBack()
    {
        m_uiTopBar.m_onButtonBackClick = null;
        m_uiTopBar.RemoveCallBack();
        m_uiCommonBackground.RemoveCallBack();
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_GoodsType.SkinAndEffects].gameObject).onClick = null;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_GoodsType.Package].gameObject).onClick = null;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_GoodsType.Music].gameObject).onClick = null;
        UIEventListener.Get(m_uiCommonBackground.m_buttonPages[(int)Enum_GoodsType.Diamond].gameObject).onClick = null;
        RemovePageCallBack(m_currentShopPage);
    }
    //---------------------------------------------------------------------------------------------------
    private void RemovePageCallBack(Enum_GoodsType type)
    {
        switch (type)
        {
            case Enum_GoodsType.Diamond:
                break;
            case Enum_GoodsType.Music:
                for (int i = 0, iCount = m_uiShop.m_musicPage.m_slotGoodsObjList.Count; i < iCount; ++i)
                {
                    UIEventListener.Get(m_uiShop.m_musicPage.m_slotGoodsObjList[i].gameObject).onClick = null;
                }
                UIEventListener.Get(m_uiShop.m_musicPage.m_buttonBuy.gameObject).onClick = null;
                break;
            case Enum_GoodsType.Package:
                break;
            case Enum_GoodsType.SkinAndEffects:
                break;
        }
    }
    #region ButtonEvents
    //---------------------------------------------------------------------------------------------------
    private void OnButtonBackClick()
    {
        if (!isPlaying)
            return;

        m_mainApp.PopStateByScreenShot();
    }
    //-------------------------------------------------------------------------------------------------
    private void OnButtonDiamondPageClick(GameObject go)
    {
        return;
        SwitchShopPage(Enum_GoodsType.Diamond);
    }
    //-------------------------------------------------------------------------------------------------
    private void OnButtonMusicPageClick(GameObject go)
    {
        SwitchShopPage(Enum_GoodsType.Music);
    }
    //-------------------------------------------------------------------------------------------------
    private void OnButtonPackagePageClick(GameObject go)
    {
        return;
        SwitchShopPage(Enum_GoodsType.Package);
    }
    //-------------------------------------------------------------------------------------------------
    private void OnButtonSkinAndEffectsPageClick(GameObject go)
    {
        return;
        SwitchShopPage(Enum_GoodsType.SkinAndEffects);
    }
    #endregion
    #region Common Fuctions
    /// <summary>設定商店資料</summary>-----------------------------------------------------------------------------------------
    private void SetShopData(ShopData[] shopData)
    {
        List<string> shopNameList = new List<string>();
        for (int i = 0, iCount = shopData.Length; i < iCount; ++i)
        {
            Enum_GoodsType type = (Enum_GoodsType)shopData[i].iGoodsType;
            shopNameList.Add(m_mainApp.GetString(shopData[i].iName));
            if (m_allGoodsDataMap.ContainsKey(type))
                continue;
            m_allGoodsDataMap.Add(type, new List<ItemmallData>());
        }
        //"商城", "鑽石", "音樂","禮包", "造型及特效"
        shopNameList.Reverse();
        m_uiCommonBackground.InitializeUI(m_mainApp.GetString(341), shopNameList, false);
        //Test
        m_uiCommonBackground.SwitchPageButton(1);
    }
    /// <summary>設定商品資料</summary>-----------------------------------------------------------------------------------------
    private void SetGoodsData(ItemmallData[] goodsData)
    {
        for (int i = 0, iCount = goodsData.Length; i < iCount; ++i)
        {
            Enum_GoodsType shopType = (Enum_GoodsType)goodsData[i].iGoodsType;
            if (!m_allGoodsDataMap.ContainsKey(shopType))
                continue;
            //過濾玩家已擁有歌曲難度
            SongDifficultyData diffData = m_playerDataSystem.GetSongDifficultyDataBySongGUID(goodsData[i].iOriginalGUID);
            if (diffData.LockStatus == Enum_DifficultyLockStatus.TrueUnlock)
                continue;
            m_allGoodsDataMap[shopType].Add(goodsData[i]);
        }

        m_uiShop.m_musicPage.SwitchNoGoodsLabel(m_currentGoodsDataList.Count == 0);
    }
    /// <summary>從目前的商品清單透過商品GUID取得商品資料</summary>-----------------------------------------------------------------------------------------
    private ItemmallData GetItemMallDataInCurrentShop(int guid)
    {
        for (int i = 0, iCount = m_currentGoodsDataList.Count; i < iCount; ++i)
        {
            if (m_currentGoodsDataList[i].GUID == guid)
                return m_currentGoodsDataList[i];
        }
        return null;
    }
    /// <summary>切換至指定商店頁面</summary>-----------------------------------------------------------------------------------------
    private void SwitchShopPage(Enum_GoodsType type)
    {
        if (m_currentShopPage == type)
            return;

        RemovePageCallBack(m_currentShopPage);
        m_currentShopPage = type;
        AddPageCallBack(m_currentShopPage);
        m_uiShop.SwitchShopPage(type);
    }
    /// <summary>購買商品事件</summary>-----------------------------------------------------------------------------------------
    private void OnButtonBuyClick(GameObject go)
    {
        UIButton btn = go.GetComponent<UIButton>();
        if (btn == null)
            return;

        m_mainApp.PushSystemCheckBox_Param(318, 359, 41, btn.userData, ConfirmBuyGoods, null, true);    //"提醒", "是否確定購買商品?"
    }
    /// <summary>確認購買商品</summary>-----------------------------------------------------------------------------------------
    private void ConfirmBuyGoods(System.Object obj)
    {
        ItemmallData goodsData = (ItemmallData)obj;
        //條件檢查
        if (m_playerDataSystem.GetPlayerDiamond() < goodsData.GetRealPrice())
        {
            m_mainApp.PushSystemCheckBox(318, 360, 0, null, null, true);    //"提醒", "鑽石不足"
            return;
        }

        m_guiManager.ShowLoadingUI(true);
        m_packet_ItemMall.SendPacket_BuyGoods(goodsData.GUID);
    }
    /// <summary>購買商品結果</summary>-----------------------------------------------------------------------------------------
    public void BuyGoodsResult(BuyGoodsResPacket pk)
    {
        if ((Enum_CurrencyType)pk.sync_money_type == Enum_CurrencyType.Diamond)
        {
            m_playerDataSystem.SetPlayerDiamond(pk.sync_now_money);
            m_uiPlayerInfo.SetDiamondUI(pk.sync_now_money);
        }

        switch (m_currentShopPage)
        {
            case Enum_GoodsType.Diamond:
                break;
            case Enum_GoodsType.Music:
                SongUnlockSystem unlockSys = m_mainApp.GetSystem<SongUnlockSystem>();
                unlockSys.UnlockSongDifficulty(m_currentChosenGoods.iOriginalGUID);

                int index = m_currentGoodsDataList.IndexOf(m_currentChosenGoods);
                m_currentGoodsDataList.Remove(m_currentChosenGoods);
                m_uiShop.m_musicPage.SortWrapContent();
                //自動選取下一個
                if (m_currentGoodsDataList.Count >= index)
                    index = m_currentGoodsDataList.Count - 1;
                if (!MoveToGoods(index))
                    MoveToGoods(0);

                m_uiShop.m_musicPage.SwitchNoGoodsLabel(m_currentGoodsDataList.Count == 0);

                if (m_mainApp.gameStateService.checkActiveStates(StateName.CHOOSE_SONG_STATE))
                    NotifyChooseSongUpdate();
                break;
            case Enum_GoodsType.Package:
                break;
            case Enum_GoodsType.SkinAndEffects:
                break;
        }

        /*
        //等待上一個確認視窗完成淡出
        UI_CheckBox uiCheckBox = m_guiManager.GetGUI(typeof(UI_CheckBox).Name) as UI_CheckBox;
        while(uiCheckBox.IsVisible())
            yield return null;
        */
        m_guiManager.ShowLoadingUI(false);

        m_mainApp.PushSystemCheckBox(318, (pk.result) ? 357 : 358, 0, null, null, true);   //"提醒", "購買成功", "購買失敗"
    }
    /// <summary>釋放動態資源</summary>-----------------------------------------------------------------------------------------
    private void ReleaseRuntimeData()
    {
        m_allGoodsDataMap.Clear();
        m_goodsTextureLoader.Clear();
        m_LoadGoodsResourceCoroutine.Clear();
        m_bIsTextureLoadFinish = false;
        m_currentChosenGoods = null;
        m_currentShopPage = Enum_GoodsType.Diamond;
        m_bInitFinish = false;
    }
    #endregion
    #region MusicPage
    //-------------------------------------------------------------------------------------------------
    private void OnButtonGoodsClick(GameObject go)
    {
        Slot_Goods slot = go.GetComponent<Slot_Goods>();
        if (slot == null)
            return;

        m_currentChosenGoods = GetItemMallDataInCurrentShop(slot.m_goodsGUID);
        m_uiShop.m_musicPage.m_buttonBuy.userData = m_currentChosenGoods;
        m_LoadGoodsResourceCoroutine.Add(m_mainApp.MusicApp.StartCoroutine(LoadGoodsResource(m_currentChosenGoods)));
        SetChosenGoodsUI(m_currentChosenGoods);
        MoveHighLight(go);
    }
    /// <summary>生成WrapContent物件，並做基本初始化</summary>-----------------------------------------------------------------------------------------
    private void InitWrapContent()
    {
        GameObject slotObj = null;
        foreach (var loader in m_guiLoadOperator)
        {
            if (loader.m_Type.Equals(typeof(Slot_Goods)))
            {
                slotObj = loader.m_assetObject as GameObject;
                break;
            }
        }
        m_uiShop.m_musicPage.InitWrapContentValue(m_currentGoodsDataList.Count);
        m_uiShop.m_musicPage.CreateSlotGoods(slotObj);
    }

    //-------------------------------------------------------------------------------------------------
    /// <summary>WrapContent塞資料的delegate Function </summary>
    /// <param name="go"></param>
    /// <param name="wrapIndex">WrapContent物件Index</param>
    /// <param name="realIndex">資料Index</param>
    private void AssignSongDataToWrapContent(GameObject song, int wrapIndex, int realIndex)
    {
        Slot_Goods slotGoods = song.GetComponent<Slot_Goods>();
        realIndex = Mathf.Abs(realIndex);
        //沒有資料可塞給物件時則清除物件上的資料並關閉
        if (realIndex > m_currentGoodsDataList.Count - 1)
        {
            slotGoods.Hide();
            slotGoods.ClearData();
            return;
        }

        ItemmallData goodsTmpData = m_currentGoodsDataList[realIndex];
        if (goodsTmpData == null)
            return;

        //將商品資料Assign給Slot實體
        slotGoods.SetData(m_mainApp.GetStringTable(), goodsTmpData, null);

        //若為玩家選擇之商品則高亮
        if (m_currentChosenGoods != null && m_currentChosenGoods.GUID == goodsTmpData.GUID)
        {
            MoveHighLight(slotGoods.gameObject);
        }
        //若不是，且自己擁有高亮框則關閉高亮
        else if (m_uiShop.m_musicPage.IsOwnHighLightSongObj(slotGoods.gameObject))
        {
            m_uiShop.m_musicPage.m_spriteHighLight.gameObject.SetActive(false);
        }

        slotGoods.Show();
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>設定預設選擇的商品</summary>
    private void SetDefaultGoods(GameObject song, int wrapIndex, int realIndex)
    {
        //等待WrapContent塞完最後一筆資料後繼續
        if (wrapIndex != m_uiShop.m_musicPage.m_slotGoodsObjList.Count - 1 || m_currentGoodsDataList.Count <= 0)
            return;

        //選取預設Slot
        int goodsGUID = m_currentGoodsDataList[0].GUID;
        Slot_Goods slot = m_uiShop.m_musicPage.GetSlotGoodsByGUID(goodsGUID);
        MoveHighLight(slot.gameObject);
        m_currentChosenGoods = GetItemMallDataInCurrentShop(slot.m_goodsGUID);
        m_uiShop.m_musicPage.m_buttonBuy.userData = m_currentChosenGoods;

        //設定選取的商品資訊UI
        m_LoadGoodsResourceCoroutine.Add(m_mainApp.MusicApp.StartCoroutine(LoadGoodsResource(m_currentChosenGoods)));
        SetChosenGoodsUI(m_currentChosenGoods);

        m_uiShop.m_musicPage.m_wcGoodsMenu.onInitializeItem -= SetDefaultGoods;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>設定玩家選擇商品之UI</summary>
    private void SetChosenGoodsUI(ItemmallData goodsData)
    {
        if (goodsData == null)
            return;

        m_uiShop.m_musicPage.SetGoodsName(m_mainApp.GetString(goodsData.iName));
        m_uiShop.m_musicPage.SetGoodsPrice(goodsData.GetRealPrice());
        S_Songs_Tmp songTmp = m_gameDataDB.GetGameDB<S_Songs_Tmp>().GetData(goodsData.iOriginalGUID);
        m_uiShop.m_musicPage.SetSongComposer(m_mainApp.GetString(songTmp.iComposer));
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>移動高亮框</summary>
    private void MoveHighLight(GameObject go)
    {
        m_uiShop.m_musicPage.m_spriteHighLight.transform.parent = go.transform;
        m_uiShop.m_musicPage.m_spriteHighLight.transform.localPosition = Vector3.zero;
        m_uiShop.m_musicPage.m_spriteHighLight.gameObject.SetActive(true);
    }
    /// <summary>移動至指定商品，回傳成功與否</summary>------------------------------------------------------------------------------------------------
    private bool MoveToGoods(int dataIndex)
    {
        if (dataIndex >= m_currentGoodsDataList.Count || dataIndex < 0)
            return false;

        //歌曲數量超過一個頁面可顯示的範圍才需要滑動WrapContent
        if (m_currentGoodsDataList.Count > Slot_Shop_MusicPage.m_iEachPageGoodsCount)
        {
            //移動WrapContent
            m_uiShop.m_musicPage.MoveWrapContentTo(dataIndex);
        }

        //點選指定歌曲
        int goodsGUID = m_currentGoodsDataList[dataIndex].GUID;
        Slot_Goods slot = m_uiShop.m_musicPage.GetSlotGoodsByGUID(goodsGUID);
        OnButtonGoodsClick(slot.gameObject);

        return true;
    }
    /// <summary>通知歌曲選單更新</summary>------------------------------------------------------------------------------------------------
    private void NotifyChooseSongUpdate()
    {
        ChooseSongState csState = m_mainApp.GetGameStateByName(StateName.CHOOSE_SONG_STATE) as ChooseSongState;
        csState.IsUpdateSongList = true;
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>讀取讀取玩家選擇商品之資源</summary>
    private IEnumerator LoadGoodsResource(ItemmallData goodsTmp)
    {
        bool loadTextureSync = LoadGoodsTexture(goodsTmp);
        //若有其中一個資源用非同步讀取便顯示loading UI且移除音樂控制的監聽事件
        if (!loadTextureSync)
        {
            m_uiShop.SwitchLoadingUI(true);
        }

        if (loadTextureSync)
            yield break;

        while (!m_bIsTextureLoadFinish)
            yield return null;

        if (!isPlaying)
            yield break;

        if (m_LoadGoodsResourceCoroutine.Count > 0)
        {
            foreach (Coroutine co in m_LoadGoodsResourceCoroutine)
            {
                if (co != null)
                    m_mainApp.MusicApp.StopCoroutine(co);
            }
            m_LoadGoodsResourceCoroutine.Clear();
        }

        m_uiShop.SwitchLoadingUI(false);
    }

    //-------------------------------------------------------------------------------------------------
    /// <summary>讀取讀取玩家選擇商品之貼圖(回傳讀取方式: True=同步, False=非同步)</summary>
    private bool LoadGoodsTexture(ItemmallData goodsData)
    {
        SongData songData = m_playerDataSystem.GetSongDataBySongGUID(goodsData.iOriginalGUID);
        string texturePath = m_playerDataSystem.GetSongTexturePath(songData);
        if (m_resourceManager.CheckLoaderResource(Enum_ResourcesType.Texture, texturePath))
        {
            Texture texture = m_resourceManager.GetResourceSync<Texture>(Enum_ResourcesType.Texture, texturePath);
            m_uiShop.m_musicPage.SetGoodsTexture(texture);
            m_bIsTextureLoadFinish = true;
            return true;
        }
        else
        {
            m_bIsTextureLoadFinish = false;

            if (m_goodsTextureLoader.Count > GameDefine.SHOP_MUISC_PAGE_LOADER_MAX)
            {
                m_goodsTextureLoader[0].CancelLoad();
                m_goodsTextureLoader.RemoveAt(0);
            }

            AsyncLoadOperation loader = m_resourceManager.GetResourceASync(Enum_ResourcesType.Texture, texturePath, typeof(Texture), SetTextureWhenLoadFinish);
            m_goodsTextureLoader.Add(loader);
            return false;
        }
    }
    //-------------------------------------------------------------------------------------------------
    /// <summary>當讀取完成後自動設定商品之貼圖</summary>
    private void SetTextureWhenLoadFinish(AsyncLoadOperation loader)
    {
        if (loader.m_assetObject == null)
        {
            UnityDebugger.Debugger.Log("SetTextureWhenLoadFinish Failed------------------ Texture Path = " + loader.m_strAssetName);
            return;
        }

        //Set Texture
        SongData songData = m_playerDataSystem.GetSongDataBySongGUID(m_currentChosenGoods.iOriginalGUID);
        if (m_playerDataSystem.GetSongTexturePath(songData).Equals(loader.m_strAssetName))
        {
            Texture texture = loader.m_assetObject as Texture;
            m_uiShop.m_musicPage.SetGoodsTexture(texture);
            m_bIsTextureLoadFinish = true;
        }
        m_goodsTextureLoader.Remove(loader);
    }
    #endregion

}
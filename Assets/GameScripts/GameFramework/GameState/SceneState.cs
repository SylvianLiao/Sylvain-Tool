using System.Collections;
using System.Collections.Generic;

public abstract class SceneState : GameState
{
    /// <summary>
    /// 場景名稱
    /// </summary>
    protected string sceneName;

    /// <summary>
    /// 主要的GameApplication
    /// </summary>
    protected GameScripts.GameFramework.GameApplication gameApplication;

    /// <summary>
    /// 將該場景名稱傳入
    /// </summary>
    /// <param name="name"></param>
    /// <param name="sceneName"></param>
    public SceneState(string name, string sceneName, GameScripts.GameFramework.GameApplication gameApplication)
        : base(name)
    {
        this.sceneName = sceneName;
        this.gameApplication = gameApplication;
    }

    public override void begin()
    {

    }

    public override void update()
    {

    }

    public override void onGUI()
    {

    }

    public override void end()
    {
    }

    public override void suspend() { }

    public override void resume() { }

    /// <summary>
    /// 取得場景名稱
    /// </summary>
    /// <returns></returns>
    public virtual string getSceneName()
    {
        return sceneName;
    }
}



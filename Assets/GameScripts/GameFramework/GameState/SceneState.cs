using System.Collections;
using System.Collections.Generic;

public abstract class SceneState : GameState
{
    /// <summary>
    /// �����W��
    /// </summary>
    protected string sceneName;

    /// <summary>
    /// �D�n��GameApplication
    /// </summary>
    protected GameScripts.GameFramework.GameApplication gameApplication;

    /// <summary>
    /// �N�ӳ����W�ٶǤJ
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
    /// ���o�����W��
    /// </summary>
    /// <returns></returns>
    public virtual string getSceneName()
    {
        return sceneName;
    }
}



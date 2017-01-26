using UnityEngine;
using System.Collections;

public class BaseSystem {

    /// <summary>
    /// 主要的GameApplication
    /// </summary>
    protected GameScripts.GameFramework.GameApplication gameApplication;

	public BaseSystem(GameScripts.GameFramework.GameApplication gameApplication)
    {
        this.gameApplication = gameApplication;
    }

    public virtual void Initialize()
    { 
    }

    public virtual void Update()
    {
    }
}

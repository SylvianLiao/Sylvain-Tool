using UnityEngine;
using System.Collections.Generic;


    /// <summary>
    /// 基本GUI框架
    /// 裡面包含個狀態GUI
    /// </summary>
    public abstract class BaseGUI
    {
        public virtual void suspend() { }

        public virtual void resume() { }

        public virtual void begin() { }

        public virtual void update() { }

        /// <summary>
        /// For OnGUI使用
        /// </summary>
        public virtual void onGUI() { }

        public virtual void end() { } 

    }

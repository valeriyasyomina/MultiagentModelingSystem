using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneRendering
{
    public interface ISceneRenderer
    {        
        void Render(IScene scene);
        void InitializeContext();
        int ContextWidth { get; }
        int ContextHeigth { get; }
    }
}

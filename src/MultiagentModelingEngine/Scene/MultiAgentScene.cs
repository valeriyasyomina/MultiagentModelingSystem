using SceneRendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Models;

namespace MultiagentModelingEngine.Scene
{
    public class MultiAgentScene : IScene
    {
        public Tunnel Tunnel { get; protected set; }

        public MultiAgentScene(Tunnel tunnel)
        {
            Tunnel = tunnel ?? throw new ArgumentNullException($"{tunnel}");
        }
        public List<DrawableEntity> Objects => Tunnel.Roadways.SelectMany(r => r.Vehicles.Select(v => v as DrawableEntity)).ToList();     
    }
}

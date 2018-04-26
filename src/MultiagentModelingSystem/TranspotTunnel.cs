using Common;
using DrawWrapperLib;
using MultiagentModelingEngine;
using MultiagentModelingEngine.Configuration;
using MultiagentModelingEngine.Scene;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MultiagentModelingSystem
{
    public partial class TranspotTunnel : Form
    {
        public MultiagentModelingEngine.MultiagentModelingEngine Engine { get; set; }
        public ModelingEngineConfiguration EngineConfiguration { get; set; }
   
        public TranspotTunnel()
        {
            InitializeComponent();         
        }
        
        private void задатьКонфигурациюТоннеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: remove hardcode for PB size
                tunnelPictureBox.Width = 1100;
                tunnelPictureBox.Height = 400;
                EngineConfiguration = ModelingEngineConfiguration.ReadFromFile(@"C:\Users\Lera\Desktop\MultiagentModelingSystem\src\engineConfig.json");
                var sceneRenderer = new MultiAgentSceneRenderer(new PictureBoxDrawWrapper(tunnelPictureBox), EngineConfiguration.DrawingSceneConfiguration);
                Engine = new MultiagentModelingEngine.MultiagentModelingEngine(EngineConfiguration, sceneRenderer);
                Engine.InitializeMultiAgentScene();

                /* var openFileDialog = new OpenFileDialog();
                 openFileDialog.Filter = "json files (*.json)|*.json";
                 if (openFileDialog.ShowDialog() == DialogResult.OK)
                 {
                     EngineConfiguration = ModelingEngineConfiguration.ReadFromFile(openFileDialog.FileName);
                     var sceneRenderer = new MultiAgentSceneRenderer(new PictureBoxDrawWrapper(tunnelPictureBox), EngineConfiguration.DrawingSceneConfiguration);
                     Engine = new MultiagentModelingEngine.MultiagentModelingEngine(EngineConfiguration, sceneRenderer);
                     Engine.InitializeMultiAgentScene();
                 } */
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void начатьМоделированиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Engine.StartModeling();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}

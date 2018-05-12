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
        public bool ModelingSuccessfullyFinished { get; set; }
   
        public TranspotTunnel()
        {
            InitializeComponent();
            сохранитьРезультатВФайлToolStripMenuItem.Enabled = false;
            начатьМоделированиеToolStripMenuItem1.Enabled = false;
            ModelingSuccessfullyFinished = false;
        }
        
        private void задатьКонфигурациюТоннеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: remove hardcode for PB size
                tunnelPictureBox.Width = 1100;
                tunnelPictureBox.Height = 400;
                var fileName = @"C:\Users\Lera\Desktop\MultiagentModelingSystem\src\engineConfig.json";
                EngineConfiguration = ModelingEngineConfiguration.ReadFromFile(fileName);
                var sceneRenderer = new MultiAgentSceneRenderer(new PictureBoxDrawWrapper(tunnelPictureBox), EngineConfiguration.DrawingSceneConfiguration);
                Engine = new MultiagentModelingEngine.MultiagentModelingEngine(EngineConfiguration, sceneRenderer);
                Engine.InitializeMultiAgentScene();
                Engine.OnModelingResultChange += Engine_OnModelingResultChange;

                начатьМоделированиеToolStripMenuItem1.Enabled = true;
                MessageBox.Show($"Конфигурационный файл {fileName} был успешно загружен");

           /*      var openFileDialog = new OpenFileDialog();
                 openFileDialog.Filter = "json files (*.json)|*.json";
                 if (openFileDialog.ShowDialog() == DialogResult.OK)
                 {
                    if (openFileDialog.FileName == null || openFileDialog.FileName.Length == 0)
                        MessageBox.Show("Файл не был выбран");
                    else
                    {
                        EngineConfiguration = ModelingEngineConfiguration.ReadFromFile(openFileDialog.FileName);
                        var sceneRenderer = new MultiAgentSceneRenderer(new PictureBoxDrawWrapper(tunnelPictureBox), EngineConfiguration.DrawingSceneConfiguration);
                        Engine = new MultiagentModelingEngine.MultiagentModelingEngine(EngineConfiguration, sceneRenderer);
                        Engine.InitializeMultiAgentScene();
                    }
                 } */
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);                
            }
        }

        private void Engine_OnModelingResultChange(ModelingResult result)
        {
            lblAllPeopleAmount.Text = result.AllPeopleAmount.ToString();
            lblAllVehiclesAmount.Text = result.AllVehiclesAmount.ToString();
            lblPeopleInSmokeCoversAmount.Text = result.SavedInSmokeCoverPeopleAmount.ToString();
            lblVehiclesLeftTunnelAmount.Text = result.LeftTunnelVehiclesAmount.ToString();
        }      

        private void сохранитьРезультатВФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ModelingSuccessfullyFinished)
                    MessageBox.Show("Для записи результата в файл необходимо сначала провести моделирование");
                else if (Engine != null)
                {
                    var saveFileDialog = new SaveFileDialog();
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if (saveFileDialog.FileName == null || saveFileDialog.FileName.Length == 0)
                            MessageBox.Show("Имя файла не должно быть пустым");
                        else
                        {
                            Engine.WriteModelingResultToFile(saveFileDialog.FileName);
                            MessageBox.Show("Результат моделирования был успешно сохранен в файл");
                        }
                    }
                }
                else
                    MessageBox.Show("Невозможно осуществить запись результата в файл. Обратитесь к разработчику программного продукта");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
             
        private void начатьМоделированиеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                if (EngineConfiguration == null)
                    MessageBox.Show("Для запуска процесса моделирования необходимо сначала загрузить файл конфгурации");
                else if (Engine != null)
                {
                    Engine.StartModeling();
                    MessageBox.Show("Моделирование было успешно завершено\nВы можете сохранить результат в файл");
                    сохранитьРезультатВФайлToolStripMenuItem.Enabled = true;
                    ModelingSuccessfullyFinished = true;
                }
                else
                    MessageBox.Show("Невозможно запустить процесс моделирования. Обратитесь к разработчику программного продукта");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Данный программный продукт предназначен для моделирования и анализа взаимодействия транспортных средств" +
                "в случае возникновение пожара в тоннелях\n\n" +
                "Студент: Сёмина Валерия Алексеевна\n" +
                "Группа: ИУ7-47М\n" +
                "Научный руководитель: Рудаков Игорь Владимимрович");
        }

        private void помощьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1. Для загрузки конфигурационного файла выберите пункт меню 'Файл -> Загрузить файл конфигурации'\n\n" +
                "2. Для проведения моделирования выберите пункт меню 'Сервис -> Начать моделирование'\n\n" +
                "3. Для сохранения результата моделирования в файл выберите пункт меню 'Файл -> Сохранить результат в файл'\n\n" +
                "4. Для выхода из программы выберите пункт меню 'Файл -> Выход' или нажмите на крестик в правом верхнем углу диалогового окна программы");
        }
    }
}

using System;
using System.Windows.Forms;
using WixSharp;
using WixSharp.Forms;

namespace Installer
{
    class Program
    {
        static void Main()
        {
            var project = new ManagedProject("MultiagentModelingSystem",
                             new Dir(@"%ProgramFiles%\MultiagentModelingSystem",
                                 new File("MultiAgentSystem\\MultiagentModelingSystem.exe"),
                                 new File("MultiAgentSystem\\Common.dll"),
                                 new File("MultiAgentSystem\\Common.pdb"),
                                 new File("MultiAgentSystem\\Converters.dll"),
                                 new File("MultiAgentSystem\\Converters.pdb"),
                                 new File("MultiAgentSystem\\DrawWrapperLib.dll"),
                                 new File("MultiAgentSystem\\DrawWrapperLib.pdb"),
                                 new File("MultiAgentSystem\\Models.dll"),
                                 new File("MultiAgentSystem\\Models.pdb"),
                                 new File("MultiAgentSystem\\MultiagentModelingEngine.dll"),
                                 new File("MultiAgentSystem\\MultiagentModelingEngine.pdb"),
                                 new File("MultiAgentSystem\\MultiagentModelingSystem.exe.config"),
                                 new File("MultiAgentSystem\\MultiagentModelingSystem.pdb"),
                                 new File("MultiAgentSystem\\Newtonsoft.Json.dll"),
                                 new File("MultiAgentSystem\\RandomValueGenerator.dll"),
                                 new File("MultiAgentSystem\\RandomValueGenerator.pdb"),
                                 new File("MultiAgentSystem\\SceneRendering.dll"),
                                 new File("MultiAgentSystem\\SceneRendering.pdb")));

            project.GUID = new Guid("6fe30b47-2577-43ad-9095-1861ba25889b");

            project.ManagedUI = ManagedUI.Empty;    //no standard UI dialogs
            project.ManagedUI = ManagedUI.Default;  //all standard UI dialogs

            //custom set of standard UI dialogs
            project.ManagedUI = new ManagedUI();

            project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
                                            .Add(Dialogs.Licence)
                                            .Add(Dialogs.SetupType)
                                            .Add(Dialogs.Features)
                                            .Add(Dialogs.InstallDir)
                                            .Add(Dialogs.Progress)
                                            .Add(Dialogs.Exit);

            project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
                                           .Add(Dialogs.Features)
                                           .Add(Dialogs.Progress)
                                           .Add(Dialogs.Exit);

            project.Load += Msi_Load;
            project.BeforeInstall += Msi_BeforeInstall;
            project.AfterInstall += Msi_AfterInstall;

            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";

            project.BuildMsi();
        }

        static void Msi_Load(SetupEventArgs e)
        {
            if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "Load");
        }

        static void Msi_BeforeInstall(SetupEventArgs e)
        {
            if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "BeforeInstall");
        }

        static void Msi_AfterInstall(SetupEventArgs e)
        {
            if (!e.IsUISupressed && !e.IsUninstalling)
                MessageBox.Show(e.ToString(), "AfterExecute");
        }
    }
}
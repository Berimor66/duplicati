#region Disclaimer / License
// Copyright (C) 2010, Kenneth Skovhede
// http://www.hexad.dk, opensource@hexad.dk
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Duplicati.GUI
{
    /// <summary>
    /// This class handles the Duplicati TrayIcon
    /// </summary>
    public partial class MainForm : Form
    {
        private ServiceStatus StatusDialog;
        private WizardHandler WizardDialog;

        private delegate void EmptyDelegate();

        private TrayIconWrapper m_trayicon;

        public string[] InitialArguments
        {
            get;
            set;
        }

        public MainForm()
        {
            InitializeComponent();

            m_trayicon = new TrayIconWrapper(this);
            m_trayicon.MouseClick += new MouseEventHandler(TrayIcon_MouseClick);
            m_trayicon.MouseDoubleClick += new MouseEventHandler(TrayIcon_MouseDoubleClick);
            m_trayicon.ContextMenuStrip = TrayMenu;

            Program.LiveControl.StateChanged += new EventHandler(LiveControl_StateChanged);
            Program.WorkThread.StartingWork += new EventHandler(WorkThread_StartingWork);
            Program.WorkThread.CompletedWork += new EventHandler(WorkThread_CompletedWork);
            Program.SingleInstance.SecondInstanceDetected += new SingleInstance.SecondInstanceDelegate(SingleInstance_SecondInstanceDetected);
        }

        void LiveControl_StateChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(LiveControl_StateChanged), sender, e);
                return;
            }

            switch (Program.LiveControl.State)
            {
                case LiveControls.LiveControlState.Paused:
                    pauseToolStripMenuItem.Text = Strings.Common.MenuResume;
                    pauseToolStripMenuItem.Checked = true;

                    m_trayicon.Icon = Program.WorkThread.Active ? Properties.Resources.TrayWorkingPause : Properties.Resources.TrayNormalPause;
                    m_trayicon.Text = Strings.MainForm.TrayStatusPause;
                    break;
                case LiveControls.LiveControlState.Running:
                    //Restore the icon and tooltip
                    if (Program.WorkThread.Active)
                        WorkThread_StartingWork(Program.WorkThread, null);
                    else
                        WorkThread_CompletedWork(Program.WorkThread, null);

                    pauseToolStripMenuItem.Checked = false;
                    pauseToolStripMenuItem.Text = Strings.Common.MenuPause;
                    break;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            m_trayicon.Icon = Properties.Resources.TrayNormal;
            m_trayicon.Text = Strings.MainForm.TrayStatusReady;

            LiveControl_StateChanged(Program.LiveControl, null);
            m_trayicon.Visible = true;

            long count = 0;
            lock (Program.MainLock)
                count = Program.DataConnection.GetObjects<Datamodel.Schedule>().Length;

            if (count == 0)
                ShowWizard();
            else if (InitialArguments != null)
                HandleCommandlineArguments(InitialArguments);

            BeginInvoke(new EmptyDelegate(HideWindow));
        }

        private void HideWindow()
        {
            this.Visible = false;
        }

        private void TrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TrayIcon_MouseClick(sender, e);
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (Program.DataConnection.GetObjects<Datamodel.Schedule>().Length == 0)
                    ShowWizard();
                else
                    ShowStatus();
            }
        }

        private void DelayDurationMenu_Click(object sender, EventArgs e)
        {
            Program.LiveControl.Pause((string)((ToolStripItem)sender).Tag);
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.LiveControl.State == LiveControls.LiveControlState.Running)
                Program.LiveControl.Pause();
            else
                Program.LiveControl.Resume();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Program.Runner.Stop();
        }

        private void WorkThread_StartingWork(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(WorkThread_StartingWork), sender, e);
                return;
            }

            m_trayicon.Icon = Properties.Resources.TrayWorking;
            m_trayicon.Text = string.Format(Strings.MainForm.TrayStatusRunning, Program.WorkThread.CurrentTask == null ? "" : Program.WorkThread.CurrentTask.Schedule.Name);
            stopToolStripMenuItem.Enabled = true;
        }

        private void WorkThread_CompletedWork(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new EventHandler(WorkThread_CompletedWork), sender, e);
                return;
            }

            if (Program.LiveControl.State != LiveControls.LiveControlState.Paused)
            {
                m_trayicon.Icon = Properties.Resources.TrayNormal;
                m_trayicon.Text = Strings.MainForm.TrayStatusReady;
            }

            stopToolStripMenuItem.Enabled = false;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Program.WorkThread.Active && MessageBox.Show(Strings.MainForm.ExitWhileBackupIsRunningQuestion, Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            Program.LiveControl.Pause();
            Program.Runner.Stop();
            m_trayicon.Exit();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettings();
        }

        private void statusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowStatus();
        }

        private void wizardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowWizard();
        }

        private void throttleOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ThrottleControl().ShowDialog(this);
        }


        public void ShowStatus()
        {
            if (InvokeRequired)
            {
                Invoke(new EmptyDelegate(ShowStatus));
                return;
            }

            if (StatusDialog == null || !StatusDialog.Visible)
                StatusDialog = new ServiceStatus();

            StatusDialog.Show();
            StatusDialog.Activate();
        }

        public void ShowWizard()
        {
            if (InvokeRequired)
            {
                Invoke(new EmptyDelegate(ShowWizard));
                return;
            }

            if (WizardDialog == null || !WizardDialog.Visible)
                WizardDialog = new WizardHandler();

            WizardDialog.Show();
        }

        public void ShowSettings()
        {
            if (InvokeRequired)
            {
                Invoke(new EmptyDelegate(ShowSettings));
                return;
            }

            lock (Program.MainLock)
            {
                ApplicationSetup dlg = new ApplicationSetup();
                dlg.ShowDialog(this);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_trayicon.Visible = false;
			m_trayicon.Exit();
            Program.LiveControl.StateChanged -= new EventHandler(LiveControl_StateChanged);
            Program.WorkThread.StartingWork -= new EventHandler(WorkThread_StartingWork);
            Program.WorkThread.CompletedWork -= new EventHandler(WorkThread_CompletedWork);
            Program.SingleInstance.SecondInstanceDetected -= new SingleInstance.SecondInstanceDelegate(SingleInstance_SecondInstanceDetected);
        }

        private void SingleInstance_SecondInstanceDetected(string[] commandlineargs)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SingleInstance.SecondInstanceDelegate(SingleInstance_SecondInstanceDetected), new object[] { commandlineargs });
                return;
            }

            if (HandleCommandlineArguments(commandlineargs))
                return;

            //TODO: This actually blocks the app thread, and thus may pile up remote invocations
            ShowWizard();
        }

        private bool HandleCommandlineArguments(string[] _args)
        {
            List<string> args = new List<string>(_args);
            Dictionary<string, string> options = CommandLine.CommandLineParser.ExtractOptions(args);
            if (args.Count == 2 && args[0].ToLower().Trim() == "run-backup")
            {
                Datamodel.Schedule[] schedules = Program.DataConnection.GetObjects<Datamodel.Schedule>("Name LIKE ?", args[1].Trim());
                if (schedules == null || schedules.Length == 0)
                {
                    MessageBox.Show(string.Format(Strings.Program.NamedBackupNotFound, args[1]), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else if (schedules.Length > 1)
                {
                    MessageBox.Show(string.Format(Strings.Program.MultipleNamedBackupsFound, args[1], schedules.Length), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (options.ContainsKey("full"))
                    Program.WorkThread.AddTask(new FullBackupTask(schedules[0]));
                else
                    Program.WorkThread.AddTask(new IncrementalBackupTask(schedules[0]));

                return true;
            }

            if (args.Count == 1 && args[0] == "show-status")
            {
                ShowStatus();
                return true;
            }

            return false;
        }

        public void Run()
        {
            m_trayicon.Run();
        }
    }
}

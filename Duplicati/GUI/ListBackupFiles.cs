#region Disclaimer / License
// Copyright (C) 2009, Kenneth Skovhede
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
    public partial class ListBackupFiles : Form
    {

        private object m_lock = new object();
        private System.Threading.Thread m_thread = null;

        public ListBackupFiles()
        {
            InitializeComponent();

            imageList.Images.Add("folder", Properties.Resources.FolderOpen);
            imageList.Images.Add("newfolder", Properties.Resources.AddedFolder);
            imageList.Images.Add("removedfolder", Properties.Resources.DeletedFolder);
            imageList.Images.Add("file", Properties.Resources.AddedOrModifiedFile);
            imageList.Images.Add("controlfile", Properties.Resources.ControlFile);
            imageList.Images.Add("deletedfile", Properties.Resources.DeletedFile);
            this.Icon = Properties.Resources.TrayNormal;
        }

        public void ShowList(Control owner, Datamodel.Schedule schedule, DateTime when)
        {
            backgroundWorker1.RunWorkerAsync(new object[] { schedule, when });
            this.ShowDialog(owner);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            lock (m_lock)
                m_thread = System.Threading.Thread.CurrentThread;

            try
            {
                object[] args = (object[])e.Argument;
                DuplicatiRunner r = new DuplicatiRunner();
                e.Result = r.ListActualFiles((Datamodel.Schedule)args[0], (DateTime)args[1]);
            }
            catch (System.Threading.ThreadAbortException)
            {
                System.Threading.Thread.ResetAbort();
                e.Cancel = true;
            }
            finally
            {
                lock (m_lock)
                    m_thread = null;
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                this.Close();
            else if (e.Error != null || e.Result == null)
            {
                Exception ex = e.Error;
                if (ex == null)
                    ex = new Exception(Strings.ListBackupFiles.NoDataError);

                MessageBox.Show(this, string.Format(Strings.Common.GenericError, ex.ToString()), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            else
            {
                try
                {
                    ContentTree.BeginUpdate();
                    List<KeyValuePair<Library.Main.RSync.RSyncDir.PatchFileType, string>> entries = e.Result as List<KeyValuePair<Library.Main.RSync.RSyncDir.PatchFileType, string>>;

                    List<string> addedfolders = new List<string>();
                    List<string> removedfolders = new List<string>();
                    List<string> addedfiles = new List<string>();
                    List<string> deletedfiles = new List<string>();
                    List<string> controlfiles = new List<string>();

                    foreach (KeyValuePair<Library.Main.RSync.RSyncDir.PatchFileType, string> x in entries)
                        switch (x.Key)
                        {
                            case Duplicati.Library.Main.RSync.RSyncDir.PatchFileType.AddedFolder:
                                addedfolders.Add(x.Value);
                                break;
                            case Duplicati.Library.Main.RSync.RSyncDir.PatchFileType.DeletedFolder:
                                removedfolders.Add(x.Value);
                                break;
                            case Duplicati.Library.Main.RSync.RSyncDir.PatchFileType.FullOrPartialFile:
                                addedfiles.Add(x.Value);
                                break;
                            case Duplicati.Library.Main.RSync.RSyncDir.PatchFileType.ControlFile:
                                controlfiles.Add(x.Value);
                                break;
                            case Duplicati.Library.Main.RSync.RSyncDir.PatchFileType.DeletedFile:
                                deletedfiles.Add(x.Value);
                                break;
                        }


                    addedfolders.Sort();
                    removedfolders.Sort();
                    deletedfiles.Sort();
                    addedfiles.Sort();
                    controlfiles.Sort();

                    foreach (string s in addedfolders)
                        AddTreeItem(s, 1);
                    foreach (string s in removedfolders)
                        AddTreeItem(s, 2);
                    foreach (string s in addedfiles)
                        AddTreeItem(s, 3);
                    foreach (string s in controlfiles)
                        AddTreeItem(s, 4);
                    foreach (string s in deletedfiles)
                        AddTreeItem(s, 5);
                }
                finally
                {
                    ContentTree.EndUpdate();
                    ContentPanel.Visible = true;
                }
            }

        }

        private void AddTreeItem(string value, int imagekey)
        {
            if (value.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                value = value.Substring(0, value.Length - 1);

            string[] items = value.Split(System.IO.Path.DirectorySeparatorChar);
            TreeNodeCollection parent = ContentTree.Nodes;

            for (int i = 0; i < items.Length; i++)
            {
                TreeNode match = null;
                foreach (TreeNode n in parent)
                    if (n.Text == items[i])
                    {
                        match = n;
                        break;
                    }

                if (match == null)
                {
                    match = new TreeNode(items[i], i == items.Length - 1 ? imagekey : 0, i == items.Length - 1 ? imagekey : 0);
                    switch (match.ImageIndex)
                    {
                        case 0:
                            match.ToolTipText = Strings.ListBackupFiles.TooltipExistingFolder;
                            break;
                        case 1:
                            match.ToolTipText = Strings.ListBackupFiles.TooltipAddedFolder;
                            break;
                        case 2:
                            match.ToolTipText = Strings.ListBackupFiles.TooltipDeletedFolder;
                            break;
                        case 3:
                            match.ToolTipText = Strings.ListBackupFiles.TooltipAddedOrModifiedFile;
                            break;
                        case 4:
                            match.ToolTipText = Strings.ListBackupFiles.TooltipControlFile;
                            break;
                        case 5:
                            match.ToolTipText = Strings.ListBackupFiles.TooltipDeletedFile;
                            break;
                    }
                    parent.Add(match);
                }

                parent = match.Nodes;
            }
        }

        private void ListBackupFiles_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (m_lock)
                if (m_thread != null)
                    m_thread.Abort();
        }
    }
}
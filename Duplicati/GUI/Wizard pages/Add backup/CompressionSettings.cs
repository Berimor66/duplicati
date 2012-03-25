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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Wizard;
using Duplicati.Datamodel;
using Duplicati.Library.Utility;

namespace Duplicati.GUI.Wizard_pages.Add_backup
{
    public partial class CompressionSettings : WizardControl
    {
        private WizardSettingsWrapper m_wrapper;

        public CompressionSettings()
            : base(Strings.CompressionSettings.PageTitle, Strings.CompressionSettings.PageDescription)
        {
            InitializeComponent();

            base.PageEnter += new PageChangeHandler(Compression_PageEnter);
            base.PageLeave += new PageChangeHandler(Compression_PageLeave);
            base.PageDisplay += new PageChangeHandler(Compression_PageDisplay);


            try
            {
                cmbCompression.Items.Clear();

                foreach (Library.Interface.ICompression e in Library.DynamicLoader.CompressionLoader.Modules)
                    cmbCompression.Items.Add(new ComboBoxItemPair<Library.Interface.ICompression>(e.DisplayName, e));

                if (cmbCompression.Items.Count > 0)
                    cmbCompression.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, string.Format("Failed to load the compression modules: {0}", ex.Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Compression_PageDisplay(object sender, PageChangedArgs args)
        {
            try { cmbCompression.Focus(); }
            catch { }
        }

        void Compression_PageLeave(object sender, PageChangedArgs args)
        {
            if (args.Direction == PageChangedDirection.Back)
                return;
            
            var mainItm = (ComboBoxItemPair<Library.Interface.ICompression>)cmbCompression.SelectedItem;

            m_wrapper.CompressionModule = mainItm.Value.FilenameExtension;

            args.NextPage = new PasswordSettings();
        }

        void Compression_PageEnter(object sender, PageChangedArgs args)
        {
            m_wrapper = new WizardSettingsWrapper(m_settings);

            if (!m_valuesAutoLoaded)
            {
                foreach (ComboBoxItemPair<Library.Interface.ICompression> e in cmbCompression.Items)
                {
                    if (e.Value.FilenameExtension.ToLower() == m_wrapper.CompressionModule)
                    {
                        cmbCompression.SelectedItem = e;
                        break;
                    }
                }
            }

            try { cmbCompression.Focus(); }
            catch { }
        }

        private void cmbCompression_SelectedIndexChanged(object sender, EventArgs e)
        {
            var itm = (ComboBox)sender;
            var mainItm = (ComboBoxItemPair<Library.Interface.ICompression>)itm.SelectedItem;

            if (mainItm != null)
                tbCompressionInfo.Text = mainItm.Value.Description;
            else
                itm.SelectedIndex = 0;
        }

    }
}

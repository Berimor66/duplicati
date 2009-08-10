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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Wizard;

namespace Duplicati.GUI.Wizard_pages.Add_backup
{
    public partial class AdvancedOptions : WizardControl
    {
        private WizardSettingsWrapper m_wrapper;

        public AdvancedOptions()
            : base(Strings.AdvancedOptions.PageTitle, Strings.AdvancedOptions.PageDescription)
        {
            InitializeComponent();

            base.PageLeave += new PageChangeHandler(AdvancedOptions_PageLeave);
            base.PageEnter += new PageChangeHandler(AdvancedOptions_PageEnter);
        }

        void AdvancedOptions_PageEnter(object sender, PageChangedArgs args)
        {
            m_wrapper = new WizardSettingsWrapper(m_settings);
            if (!m_valuesAutoLoaded)
            {
                IncludeDuplicatiSetup.Checked = m_wrapper.IncludeSetup;
                EditOverrides.Checked = m_wrapper.Overrides.Count > 0;
            }
        }

        void AdvancedOptions_PageLeave(object sender, PageChangedArgs args)
        {
            m_settings["Advanced:When"] = SelectWhen.Checked;
            m_settings["Advanced:Incremental"] = SelectIncremental.Checked;
            m_settings["Advanced:Throttle"] = ThrottleOptions.Checked;
            m_settings["Advanced:Filters"] = EditFilters.Checked;
            m_settings["Advanced:Filenames"] = EditVolumeFilenames.Checked;
            m_settings["Advanced:Overrides"] = EditOverrides.Checked;

            if (args.Direction == PageChangedDirection.Back)
                return;

            m_wrapper.IncludeSetup = IncludeDuplicatiSetup.Checked;

            if (SelectWhen.Checked)
                args.NextPage = new Wizard_pages.Add_backup.SelectWhen();
            else if (SelectIncremental.Checked)
                args.NextPage = new Wizard_pages.Add_backup.IncrementalSettings();
            else if (ThrottleOptions.Checked)
                args.NextPage = new Wizard_pages.Add_backup.ThrottleOptions();
            else if (EditFilters.Checked)
                args.NextPage = new Wizard_pages.Add_backup.EditFilters();
            else if (EditVolumeFilenames.Checked)
                args.NextPage = new Wizard_pages.Add_backup.GeneratedFilenameOptions();
            else if (EditOverrides.Checked)
                args.NextPage = new Wizard_pages.Add_backup.SettingOverrides();
            else
                args.NextPage = new Wizard_pages.Add_backup.FinishedAdd();
        }
    }
}

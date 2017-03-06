// Copyright 2017 Dishant Langayan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Author:
//  Dishant Langayan <dishant.langayan@solace.com>
//
using NLog;
using Solace.Labs.Excel.SolaceXLCore;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Solace.Labs.Excel.SolaceXLAddIn
{
    public partial class LoggingConfigForm : Form
    {
        private Logger Logger { get; } = ApplicationLogging.CreateLogger<LoggingConfigForm>();

        public LoggingConfigForm()
        {
            InitializeComponent();
            Dictionary<string, LogLevel> levels = new Dictionary<string, LogLevel>
            {
                {"Error", LogLevel.Error},
                {"Warning", LogLevel.Warn},
                {"Information", LogLevel.Info},
                {"Debug", LogLevel.Debug},
                {"Verbose", LogLevel.Trace}
            };
            cbLogLevel.DataSource = new BindingSource(levels, null);
            cbLogLevel.DisplayMember = "Key";
            cbLogLevel.ValueMember = "Value";
            cbLogLevel.SelectedIndex = 2;

            try
            {
                var config = SolaceXLHelper.LoadConfiguration();
                if (config != null)
                {
                    cbLogLevel.SelectedValue = config.AppLogLevel;
                    if (!config.EnableLogging)
                    {
                        cbLogLevel.Enabled = false;
                        cbDisableLogging.Checked = true;
                    }
                    cbLogLevel.Update();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load Solace connection configuration");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            LogLevel level = LogLevel.Info;
            if (cbDisableLogging.Checked)
                level = LogLevel.Off;
            else
                level = (LogLevel)cbLogLevel.SelectedValue;

            // Update Solace Configuration & save
            try
            {
                var config = SolaceXLHelper.LoadConfiguration();
                if (config != null)
                {
                    config.AppLogLevel = level;
                    config.EnableLogging = (cbDisableLogging.Checked) ? false : true;
                    SolaceTransport.Instance.Config = config;
                    SolaceXLHelper.SaveConfiguration(config);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to load and save Solace connection configuration");
            }

            // Finally update all current loggers
            ApplicationLogging.UpdateLogging(level, cbDisableLogging.Checked);

            // Hide the form
            Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void cbDisableLogging_CheckedChanged(object sender, EventArgs e)
        {
            if (cbDisableLogging.Checked)
                cbLogLevel.Enabled = false;
            else
                cbLogLevel.Enabled = true;
        }
    }
}

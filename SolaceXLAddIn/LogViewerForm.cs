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
using Solace.Labs.Excel.SolaceXLCore;
using System;
using System.Windows.Forms;
using System.IO;

namespace Solace.Labs.Excel.SolaceXLAddIn
{
    public partial class LogViewerForm : Form
    {
        private LogWatcher logWatcher;
        private bool frozen = false;

        public LogViewerForm()
        {
            InitializeComponent();

            string logFilePath = SolaceXLHelper.GetLogFilePath();
            tabCtrlLogs.TabPages[0].Text = SolaceXLHelper.LogFileName;

            // Create the LogWatcher
            CreateWatcher(logFilePath);

            logWatcher.OnChanged(this, null);
        }

        private void btnFreeze_Click(object sender, EventArgs e)
        {
            if (frozen)
            {
                btnFreeze.Text = "Freeze";
                frozen = false;
            }
            else
            {
                btnFreeze.Text = "Unfreeze";
                frozen = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            logWatcher.Dispose();
            Hide();
        }

        private void CreateWatcher(string logFilePath)
        {
            logWatcher = new LogWatcher(logFilePath);

            //Set the directory of the file to monitor
            logWatcher.Path = Path.GetDirectoryName(logFilePath);

            //Raise events when the LastWrite or Size attribute is changed
            logWatcher.NotifyFilter = (NotifyFilters.LastWrite | NotifyFilters.Size);

            //Filter out events for only this file
            logWatcher.Filter = Path.GetFileName(logFilePath);

            //Subscribe to the event
            logWatcher.TextChanged += new LogWatcher.LogWatcherEventHandler(Watcher_Changed);

            //Enable the event
            logWatcher.EnableRaisingEvents = true;
        }

        //Occurs when the file is changed
        void Watcher_Changed(object sender, LogWatcherEventArgs e)
        {
            //Invoke the AppendText method if required
            if (txtBoxLogs.InvokeRequired) this.Invoke(new Action(delegate () { AppendText(e.Contents); }));
            else AppendText(e.Contents);
        }

        private void AppendText(string Text)
        {
            //Append the new text to the TextBox
            txtBoxLogs.Text += Text;

            // Update the last updated time label
            lblLastUpdated.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //If the Frozen function isn't enabled then scroll to the bottom of the TextBox
            if (!frozen)
            {
                txtBoxLogs.SelectionStart = txtBoxLogs.Text.Length;
                txtBoxLogs.SelectionLength = 0;
                txtBoxLogs.ScrollToCaret();
            }
        }

        private void LogViewerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            logWatcher.Dispose();
        }
    }
}

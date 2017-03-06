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
using System.Drawing;
using System.Windows.Forms;

namespace Solace.Labs.Excel.SolaceXLAddIn
{
    public partial class SolaceConfigForm : Form
    {
        private Logger Logger { get; } = ApplicationLogging.CreateLogger<SolaceConfigForm>();
        private SolaceConfiguration _config;

        public SolaceConfigForm()
        {
            InitializeComponent();

            if (SolaceTransport.Instance.IsConnected)
                UpdateConnected();
            else
            {
                UpdateNotConnected();
                // Load the config from file
                try
                {
                    _config = SolaceXLHelper.LoadConfiguration();
                    UpdateControls(_config);
                }
                catch (Exception ex)
                {
                    // Log Ignore exception for now
                    Logger.Error(ex, "Failed to load Solace connection configuration");
                }
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (SolaceTransport.Instance.IsConnected)
            {
                // Disconnect from Solace
                SolaceTransport.Instance.Disconnect();
                UpdateNotConnected();
            }
            else
            {
                // Create or update the configuration
                if (_config == null)
                    _config = new SolaceConfiguration();
                _config.Host = txtHost.Text;
                _config.MsgVpn = txtMsgVpn.Text;
                _config.Username = txtUsername.Text;
                _config.Password = txtPassword.Text;
                _config.TopicPrefix = txtTopicPrefix.Text;

                // Validate
                string errorMsg = "";

                if (string.IsNullOrEmpty(_config.Host))
                    errorMsg = "Solace Host is required";
                else if (string.IsNullOrEmpty(_config.MsgVpn))
                    errorMsg = "Message-VPN is required";
                else if (string.IsNullOrEmpty(_config.Username))
                    errorMsg = "Username is required";
                else if (string.IsNullOrEmpty(_config.TopicPrefix))
                    errorMsg = "Topic Prefix is required";

                if (!string.IsNullOrEmpty(errorMsg))
                {
                    txtStatus.Text = errorMsg;
                    txtStatus.BackColor = Color.Orange;
                    MessageBox.Show(errorMsg, "Missing Required Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Connect
                bool connected = SolaceTransport.Instance.Connect(_config);
                if (connected)
                    UpdateConnected();
                else
                    UpdateError("Failed to connect. Enable logging to check error.");


                // Try to save the config
                try
                {
                    SolaceXLHelper.SaveConfiguration(_config);
                }
                catch (Exception ex)
                {
                    // Log Ignore exception for now
                    Logger.Error(ex, "Failed to save Solace connection configuration");
                }
            }

        }

        private void UpdateNotConnected()
        {
            txtStatus.Text = "Disconnected";
            txtStatus.BackColor = Color.Orange;
            btnConnect.Text = "Connect";
        }

        private void UpdateError(string errorMsg)
        {
            txtStatus.Text = errorMsg;
            txtStatus.BackColor = Color.Red;
            btnConnect.Text = "Connect";
        }

        private void UpdateConnected()
        {
            txtStatus.Text = "Connected!";
            txtStatus.BackColor = Color.Green;
            btnConnect.Text = "Disconnect";

            // Update other controls with the configuration
            UpdateControls(SolaceTransport.Instance.Config);
        }

        private void UpdateControls(SolaceConfiguration config)
        {
            if (config != null)
            {
                txtHost.Text = config.Host;
                txtMsgVpn.Text = config.MsgVpn;
                txtUsername.Text = config.Username;
                txtPassword.Text = config.Password;
                txtTopicPrefix.Text = config.TopicPrefix;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}

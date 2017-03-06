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
using ExcelDna.Integration.CustomUI;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Solace.Labs.Excel.SolaceXLAddIn
{
    [ComVisible(true)]
    public class SolaceXLRibbon : ExcelRibbon
    {
        private LogViewerForm logViewerForm;

        public void btnSolaceConfig_Click(IRibbonControl control)
        {
            SolaceConfigForm solConfigForm = new SolaceConfigForm();
            solConfigForm.ShowDialog();
        }

        public void btnAbout_Click(IRibbonControl control)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        public void btnConfigLogging_Click(IRibbonControl control)
        {
            LoggingConfigForm loggingForm = new LoggingConfigForm();
            loggingForm.ShowDialog();
        }

        public void btnViewLogs_Click(IRibbonControl control)
        {
            if (logViewerForm == null || logViewerForm.IsDisposed)
                logViewerForm = new LogViewerForm();
            logViewerForm.Show(); // Should be modaless
            logViewerForm.BringToFront();
        }

        public void btnGetHelp_Click(IRibbonControl control)
        {
            HelpForm helpForm = new HelpForm();
            helpForm.ShowDialog();
        }

        public void btnDocumentation_Click(IRibbonControl control)
        {
            MessageBox.Show("Documentation is currently unavailable. Please contact Solace Services for help.",
                "SolaceXL Documentation", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

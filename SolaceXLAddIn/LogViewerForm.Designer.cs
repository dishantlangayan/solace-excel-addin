﻿namespace Solace.Labs.Excel.SolaceXLAddIn
{
    partial class LogViewerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogViewerForm));
            this.btnFreeze = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblLastUpdated = new System.Windows.Forms.Label();
            this.tabCtrlLogs = new System.Windows.Forms.TabControl();
            this.logTabPage = new System.Windows.Forms.TabPage();
            this.txtBoxLogs = new System.Windows.Forms.RichTextBox();
            this.tabCtrlLogs.SuspendLayout();
            this.logTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFreeze
            // 
            this.btnFreeze.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFreeze.Location = new System.Drawing.Point(13, 13);
            this.btnFreeze.Name = "btnFreeze";
            this.btnFreeze.Size = new System.Drawing.Size(215, 59);
            this.btnFreeze.TabIndex = 0;
            this.btnFreeze.Text = "Freeze";
            this.btnFreeze.UseVisualStyleBackColor = true;
            this.btnFreeze.Click += new System.EventHandler(this.btnFreeze_Click);
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(246, 13);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(215, 59);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(485, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(184, 31);
            this.label1.TabIndex = 2;
            this.label1.Text = "Last Updated:";
            // 
            // lblLastUpdated
            // 
            this.lblLastUpdated.AutoSize = true;
            this.lblLastUpdated.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLastUpdated.Location = new System.Drawing.Point(691, 27);
            this.lblLastUpdated.Name = "lblLastUpdated";
            this.lblLastUpdated.Size = new System.Drawing.Size(60, 31);
            this.lblLastUpdated.TabIndex = 3;
            this.lblLastUpdated.Text = "N/A";
            // 
            // tabCtrlLogs
            // 
            this.tabCtrlLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCtrlLogs.Controls.Add(this.logTabPage);
            this.tabCtrlLogs.Location = new System.Drawing.Point(13, 106);
            this.tabCtrlLogs.Name = "tabCtrlLogs";
            this.tabCtrlLogs.SelectedIndex = 0;
            this.tabCtrlLogs.Size = new System.Drawing.Size(1229, 863);
            this.tabCtrlLogs.TabIndex = 4;
            // 
            // logTabPage
            // 
            this.logTabPage.Controls.Add(this.txtBoxLogs);
            this.logTabPage.Location = new System.Drawing.Point(8, 39);
            this.logTabPage.Name = "logTabPage";
            this.logTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.logTabPage.Size = new System.Drawing.Size(1213, 816);
            this.logTabPage.TabIndex = 0;
            this.logTabPage.Text = "solacexl.log";
            this.logTabPage.UseVisualStyleBackColor = true;
            // 
            // txtBoxLogs
            // 
            this.txtBoxLogs.BackColor = System.Drawing.Color.White;
            this.txtBoxLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBoxLogs.Location = new System.Drawing.Point(3, 3);
            this.txtBoxLogs.Name = "txtBoxLogs";
            this.txtBoxLogs.ReadOnly = true;
            this.txtBoxLogs.Size = new System.Drawing.Size(1207, 810);
            this.txtBoxLogs.TabIndex = 0;
            this.txtBoxLogs.Text = "";
            // 
            // LogViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(1254, 981);
            this.Controls.Add(this.tabCtrlLogs);
            this.Controls.Add(this.lblLastUpdated);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnFreeze);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LogViewerForm";
            this.Text = "SolaceXL Log Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogViewerForm_FormClosing);
            this.tabCtrlLogs.ResumeLayout(false);
            this.logTabPage.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFreeze;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblLastUpdated;
        private System.Windows.Forms.TabControl tabCtrlLogs;
        private System.Windows.Forms.TabPage logTabPage;
        private System.Windows.Forms.RichTextBox txtBoxLogs;
    }
}
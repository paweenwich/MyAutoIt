namespace MyAutoIt
{
    partial class Linage2
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cmbScreenType = new System.Windows.Forms.ComboBox();
            this.btnCaptureToFile = new System.Windows.Forms.Button();
            this.timer10 = new System.Windows.Forms.Timer(this.components);
            this.btnScreenCheck = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.txtScreenStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.utilsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.featuresToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verifyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.averageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAutoPointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtDebug = new System.Windows.Forms.RichTextBox();
            this.chkAutoClick = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.cmbADBDevice = new System.Windows.Forms.ComboBox();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(7, 66);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(88, 66);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cmbScreenType
            // 
            this.cmbScreenType.FormattingEnabled = true;
            this.cmbScreenType.Items.AddRange(new object[] {
            "Main",
            "AutoNoSkill",
            "QuestSingleConfirm",
            "QuestAccept",
            "QuestSkip",
            "QuestComplete",
            "Move"});
            this.cmbScreenType.Location = new System.Drawing.Point(7, 36);
            this.cmbScreenType.Name = "cmbScreenType";
            this.cmbScreenType.Size = new System.Drawing.Size(208, 24);
            this.cmbScreenType.TabIndex = 2;
            this.cmbScreenType.Text = "Main";
            // 
            // btnCaptureToFile
            // 
            this.btnCaptureToFile.Location = new System.Drawing.Point(221, 37);
            this.btnCaptureToFile.Name = "btnCaptureToFile";
            this.btnCaptureToFile.Size = new System.Drawing.Size(85, 23);
            this.btnCaptureToFile.TabIndex = 3;
            this.btnCaptureToFile.Text = "Capture";
            this.btnCaptureToFile.UseVisualStyleBackColor = true;
            this.btnCaptureToFile.Click += new System.EventHandler(this.btnCaptureToFile_Click);
            // 
            // timer10
            // 
            this.timer10.Enabled = true;
            this.timer10.Interval = 10000;
            this.timer10.Tick += new System.EventHandler(this.timer10_Tick);
            // 
            // btnScreenCheck
            // 
            this.btnScreenCheck.Location = new System.Drawing.Point(312, 37);
            this.btnScreenCheck.Name = "btnScreenCheck";
            this.btnScreenCheck.Size = new System.Drawing.Size(142, 23);
            this.btnScreenCheck.TabIndex = 4;
            this.btnScreenCheck.Text = "ScreenCheck Start";
            this.btnScreenCheck.UseVisualStyleBackColor = true;
            this.btnScreenCheck.Click += new System.EventHandler(this.btnScreenCheck_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.txtScreenStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 457);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(808, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // txtScreenStatus
            // 
            this.txtScreenStatus.Name = "txtScreenStatus";
            this.txtScreenStatus.Size = new System.Drawing.Size(793, 17);
            this.txtScreenStatus.Spring = true;
            this.txtScreenStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 2000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.utilsToolStripMenuItem,
            this.featuresToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(808, 28);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // utilsToolStripMenuItem
            // 
            this.utilsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadToolStripMenuItem});
            this.utilsToolStripMenuItem.Name = "utilsToolStripMenuItem";
            this.utilsToolStripMenuItem.Size = new System.Drawing.Size(50, 24);
            this.utilsToolStripMenuItem.Text = "Utils";
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(131, 26);
            this.reloadToolStripMenuItem.Text = "Reload";
            this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
            // 
            // featuresToolStripMenuItem
            // 
            this.featuresToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createToolStripMenuItem,
            this.verifyToolStripMenuItem,
            this.averageToolStripMenuItem,
            this.saveAutoPointsToolStripMenuItem});
            this.featuresToolStripMenuItem.Name = "featuresToolStripMenuItem";
            this.featuresToolStripMenuItem.Size = new System.Drawing.Size(76, 24);
            this.featuresToolStripMenuItem.Text = "Features";
            // 
            // createToolStripMenuItem
            // 
            this.createToolStripMenuItem.Name = "createToolStripMenuItem";
            this.createToolStripMenuItem.Size = new System.Drawing.Size(191, 26);
            this.createToolStripMenuItem.Text = "Create";
            this.createToolStripMenuItem.Click += new System.EventHandler(this.createToolStripMenuItem_Click);
            // 
            // verifyToolStripMenuItem
            // 
            this.verifyToolStripMenuItem.Name = "verifyToolStripMenuItem";
            this.verifyToolStripMenuItem.Size = new System.Drawing.Size(191, 26);
            this.verifyToolStripMenuItem.Text = "Verify";
            this.verifyToolStripMenuItem.Click += new System.EventHandler(this.verifyToolStripMenuItem_Click);
            // 
            // averageToolStripMenuItem
            // 
            this.averageToolStripMenuItem.Name = "averageToolStripMenuItem";
            this.averageToolStripMenuItem.Size = new System.Drawing.Size(191, 26);
            this.averageToolStripMenuItem.Text = "Average";
            this.averageToolStripMenuItem.Click += new System.EventHandler(this.averageToolStripMenuItem_Click);
            // 
            // saveAutoPointsToolStripMenuItem
            // 
            this.saveAutoPointsToolStripMenuItem.Name = "saveAutoPointsToolStripMenuItem";
            this.saveAutoPointsToolStripMenuItem.Size = new System.Drawing.Size(191, 26);
            this.saveAutoPointsToolStripMenuItem.Text = "Save AutoPoints";
            this.saveAutoPointsToolStripMenuItem.Click += new System.EventHandler(this.saveAutoPointsToolStripMenuItem_Click);
            // 
            // txtDebug
            // 
            this.txtDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDebug.Font = new System.Drawing.Font("Courier New", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDebug.Location = new System.Drawing.Point(7, 95);
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.Size = new System.Drawing.Size(789, 359);
            this.txtDebug.TabIndex = 8;
            this.txtDebug.Text = "";
            // 
            // chkAutoClick
            // 
            this.chkAutoClick.AutoSize = true;
            this.chkAutoClick.Checked = true;
            this.chkAutoClick.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoClick.Location = new System.Drawing.Point(460, 39);
            this.chkAutoClick.Name = "chkAutoClick";
            this.chkAutoClick.Size = new System.Drawing.Size(88, 21);
            this.chkAutoClick.TabIndex = 9;
            this.chkAutoClick.Text = "AutoClick";
            this.chkAutoClick.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(169, 66);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(250, 66);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 11;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(331, 66);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 12;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // cmbADBDevice
            // 
            this.cmbADBDevice.FormattingEnabled = true;
            this.cmbADBDevice.Location = new System.Drawing.Point(554, 36);
            this.cmbADBDevice.Name = "cmbADBDevice";
            this.cmbADBDevice.Size = new System.Drawing.Size(242, 24);
            this.cmbADBDevice.TabIndex = 13;
            this.cmbADBDevice.SelectedIndexChanged += new System.EventHandler(this.cmbADBDevice_SelectedIndexChanged);
            // 
            // Linage2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 479);
            this.Controls.Add(this.cmbADBDevice);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.chkAutoClick);
            this.Controls.Add(this.txtDebug);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.btnScreenCheck);
            this.Controls.Add(this.btnCaptureToFile);
            this.Controls.Add(this.cmbScreenType);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Linage2";
            this.Text = "Linage2";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox cmbScreenType;
        private System.Windows.Forms.Button btnCaptureToFile;
        private System.Windows.Forms.Timer timer10;
        private System.Windows.Forms.Button btnScreenCheck;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel txtScreenStatus;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem utilsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.RichTextBox txtDebug;
        private System.Windows.Forms.ToolStripMenuItem featuresToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verifyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem averageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAutoPointsToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkAutoClick;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.ComboBox cmbADBDevice;
    }
}
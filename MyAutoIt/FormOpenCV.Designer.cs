namespace MyAutoIt
{
    partial class FormOpenCV
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shoePointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pat2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pat1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(823, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem,
            this.shoePointsToolStripMenuItem,
            this.pat1ToolStripMenuItem,
            this.pat2ToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(66, 24);
            this.debugToolStripMenuItem.Text = "Debug";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.testToolStripMenuItem.Text = "Test";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // shoePointsToolStripMenuItem
            // 
            this.shoePointsToolStripMenuItem.Name = "shoePointsToolStripMenuItem";
            this.shoePointsToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.shoePointsToolStripMenuItem.Text = "ShowPoints";
            this.shoePointsToolStripMenuItem.Click += new System.EventHandler(this.shoePointsToolStripMenuItem_Click);
            // 
            // pat2ToolStripMenuItem
            // 
            this.pat2ToolStripMenuItem.Name = "pat2ToolStripMenuItem";
            this.pat2ToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.pat2ToolStripMenuItem.Text = "Pat2";
            this.pat2ToolStripMenuItem.Click += new System.EventHandler(this.pat2ToolStripMenuItem_Click);
            // 
            // pat1ToolStripMenuItem
            // 
            this.pat1ToolStripMenuItem.Name = "pat1ToolStripMenuItem";
            this.pat1ToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.pat1ToolStripMenuItem.Text = "Pat1";
            this.pat1ToolStripMenuItem.Click += new System.EventHandler(this.pat1ToolStripMenuItem_Click);
            // 
            // FormOpenCV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 573);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormOpenCV";
            this.Text = "FormOpenCV";
            this.SizeChanged += new System.EventHandler(this.FormOpenCV_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormOpenCV_Paint);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shoePointsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pat2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pat1ToolStripMenuItem;
    }
}
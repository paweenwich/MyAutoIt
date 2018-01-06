namespace MyAutoIt
{
    partial class CandyCrush
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
            this.button1 = new System.Windows.Forms.Button();
            this.txtDebug = new System.Windows.Forms.RichTextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.imgMain = new System.Windows.Forms.PictureBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.chkSlide = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imgMain)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Find Game";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imgMain_MouseDown);
            // 
            // txtDebug
            // 
            this.txtDebug.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtDebug.Location = new System.Drawing.Point(0, 576);
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.Size = new System.Drawing.Size(731, 63);
            this.txtDebug.TabIndex = 2;
            this.txtDebug.Text = "";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(3, 32);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.button2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imgMain_MouseDown);
            // 
            // imgMain
            // 
            this.imgMain.Location = new System.Drawing.Point(84, 3);
            this.imgMain.Name = "imgMain";
            this.imgMain.Size = new System.Drawing.Size(639, 567);
            this.imgMain.TabIndex = 4;
            this.imgMain.TabStop = false;
            this.imgMain.Click += new System.EventHandler(this.imgMain_Click);
            this.imgMain.Paint += new System.Windows.Forms.PaintEventHandler(this.imgMain_Paint);
            this.imgMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imgMain_MouseDown);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(41, 61);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(37, 23);
            this.btnNext.TabIndex = 5;
            this.btnNext.Text = "Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            this.btnNext.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imgMain_MouseDown);
            // 
            // btnPrev
            // 
            this.btnPrev.Location = new System.Drawing.Point(0, 61);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(37, 23);
            this.btnPrev.TabIndex = 6;
            this.btnPrev.Text = "Prev";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            this.btnPrev.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imgMain_MouseDown);
            // 
            // chkSlide
            // 
            this.chkSlide.AutoSize = true;
            this.chkSlide.Location = new System.Drawing.Point(3, 90);
            this.chkSlide.Name = "chkSlide";
            this.chkSlide.Size = new System.Drawing.Size(49, 17);
            this.chkSlide.TabIndex = 7;
            this.chkSlide.Text = "Slide";
            this.chkSlide.UseVisualStyleBackColor = true;
            this.chkSlide.CheckedChanged += new System.EventHandler(this.chkSlide_CheckedChanged);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(3, 380);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(3, 409);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 9;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // CandyCrush
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 639);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.chkSlide);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.imgMain);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtDebug);
            this.Controls.Add(this.button1);
            this.Name = "CandyCrush";
            this.Text = "CandyCrush";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CandyCrush_FormClosing);
            this.Load += new System.EventHandler(this.CandyCrush_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imgMain_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.imgMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox txtDebug;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.PictureBox imgMain;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.CheckBox chkSlide;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
    }
}
namespace WindowsControlApplication {
    partial class MainPage {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainPage));
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.SlowButton = new System.Windows.Forms.Button();
            this.MediumButton = new System.Windows.Forms.Button();
            this.FastButton = new System.Windows.Forms.Button();
            this.VFastButton = new System.Windows.Forms.Button();
            this.SensitivityBar = new System.Windows.Forms.TrackBar();
            this.Sensitivity = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ShiftSpeed = new System.Windows.Forms.TrackBar();
            this.StartA = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SensitivityBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShiftSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startButton.Location = new System.Drawing.Point(269, 128);
            this.startButton.Name = "startButton";
            this.startButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.startButton.Size = new System.Drawing.Size(158, 76);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // stopButton
            // 
            this.stopButton.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopButton.Location = new System.Drawing.Point(269, 266);
            this.stopButton.Name = "stopButton";
            this.stopButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.stopButton.Size = new System.Drawing.Size(158, 76);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Interval = 10;
            this.UpdateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
            // 
            // SlowButton
            // 
            this.SlowButton.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SlowButton.Location = new System.Drawing.Point(12, 40);
            this.SlowButton.Name = "SlowButton";
            this.SlowButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.SlowButton.Size = new System.Drawing.Size(87, 41);
            this.SlowButton.TabIndex = 3;
            this.SlowButton.Text = "Slow";
            this.SlowButton.UseVisualStyleBackColor = true;
            this.SlowButton.Click += new System.EventHandler(this.SlowButton_Click);
            // 
            // MediumButton
            // 
            this.MediumButton.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MediumButton.Location = new System.Drawing.Point(132, 40);
            this.MediumButton.Name = "MediumButton";
            this.MediumButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MediumButton.Size = new System.Drawing.Size(94, 41);
            this.MediumButton.TabIndex = 4;
            this.MediumButton.Text = "Medium";
            this.MediumButton.UseVisualStyleBackColor = true;
            this.MediumButton.Click += new System.EventHandler(this.MediumButton_Click);
            // 
            // FastButton
            // 
            this.FastButton.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FastButton.Location = new System.Drawing.Point(254, 40);
            this.FastButton.Name = "FastButton";
            this.FastButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.FastButton.Size = new System.Drawing.Size(87, 41);
            this.FastButton.TabIndex = 5;
            this.FastButton.Text = "Fast";
            this.FastButton.UseVisualStyleBackColor = true;
            this.FastButton.Click += new System.EventHandler(this.FastButton_Click);
            // 
            // VFastButton
            // 
            this.VFastButton.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VFastButton.Location = new System.Drawing.Point(364, 40);
            this.VFastButton.Name = "VFastButton";
            this.VFastButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.VFastButton.Size = new System.Drawing.Size(97, 41);
            this.VFastButton.TabIndex = 6;
            this.VFastButton.Text = "Very fast";
            this.VFastButton.UseVisualStyleBackColor = true;
            this.VFastButton.Click += new System.EventHandler(this.VFastButton_Click);
            // 
            // SensitivityBar
            // 
            this.SensitivityBar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SensitivityBar.Location = new System.Drawing.Point(25, 159);
            this.SensitivityBar.Maximum = 11;
            this.SensitivityBar.Minimum = 1;
            this.SensitivityBar.Name = "SensitivityBar";
            this.SensitivityBar.RightToLeftLayout = true;
            this.SensitivityBar.Size = new System.Drawing.Size(214, 45);
            this.SensitivityBar.TabIndex = 9;
            this.SensitivityBar.Value = 1;
            this.SensitivityBar.Scroll += new System.EventHandler(this.SensitivityBar_Scroll);
            // 
            // Sensitivity
            // 
            this.Sensitivity.AutoSize = true;
            this.Sensitivity.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Sensitivity.Location = new System.Drawing.Point(47, 119);
            this.Sensitivity.Name = "Sensitivity";
            this.Sensitivity.Size = new System.Drawing.Size(169, 37);
            this.Sensitivity.TabIndex = 10;
            this.Sensitivity.Text = "Sensitivity";
            this.Sensitivity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(39, 248);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 37);
            this.label1.TabIndex = 14;
            this.label1.Text = "Shift speed";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ShiftSpeed
            // 
            this.ShiftSpeed.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ShiftSpeed.Location = new System.Drawing.Point(25, 288);
            this.ShiftSpeed.Maximum = 9;
            this.ShiftSpeed.Minimum = 1;
            this.ShiftSpeed.Name = "ShiftSpeed";
            this.ShiftSpeed.RightToLeftLayout = true;
            this.ShiftSpeed.Size = new System.Drawing.Size(214, 45);
            this.ShiftSpeed.TabIndex = 13;
            this.ShiftSpeed.Value = 1;
            this.ShiftSpeed.Scroll += new System.EventHandler(this.ShiftSpeed_Scroll);
            // 
            // StartA
            // 
            this.StartA.Font = new System.Drawing.Font("Calibri", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartA.Location = new System.Drawing.Point(451, 159);
            this.StartA.Name = "StartA";
            this.StartA.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartA.Size = new System.Drawing.Size(219, 131);
            this.StartA.TabIndex = 15;
            this.StartA.Text = "Start Alternate";
            this.StartA.UseVisualStyleBackColor = true;
            this.StartA.Click += new System.EventHandler(this.StartA_Click);
            // 
            // MainPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(699, 369);
            this.Controls.Add(this.StartA);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ShiftSpeed);
            this.Controls.Add(this.Sensitivity);
            this.Controls.Add(this.SensitivityBar);
            this.Controls.Add(this.VFastButton);
            this.Controls.Add(this.FastButton);
            this.Controls.Add(this.MediumButton);
            this.Controls.Add(this.SlowButton);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainPage";
            this.Text = "LED Control";
            ((System.ComponentModel.ISupportInitialize)(this.SensitivityBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ShiftSpeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.Button SlowButton;
        private System.Windows.Forms.Button MediumButton;
        private System.Windows.Forms.Button FastButton;
        private System.Windows.Forms.Button VFastButton;
        private System.Windows.Forms.TrackBar SensitivityBar;
        private System.Windows.Forms.Label Sensitivity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar ShiftSpeed;
        private System.Windows.Forms.Button StartA;
    }
}


namespace WXsensorWebPage
{
    partial class WXSensor2WebPage
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblStartTime = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.lblRecCnt = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblNowTime = new System.Windows.Forms.Label();
            this.btnReadCorrections = new System.Windows.Forms.Button();
            this.txtLowTempCondition = new System.Windows.Forms.TextBox();
            this.txtHighTempcondition = new System.Windows.Forms.TextBox();
            this.txtHighWindCondition = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtWebUpdateCycle = new System.Windows.Forms.TextBox();
            this.chkTweet = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(152, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "WX Sensor Web Page Creator";
            // 
            // lblStartTime
            // 
            this.lblStartTime.AutoSize = true;
            this.lblStartTime.Location = new System.Drawing.Point(106, 31);
            this.lblStartTime.Name = "lblStartTime";
            this.lblStartTime.Size = new System.Drawing.Size(97, 13);
            this.lblStartTime.TabIndex = 1;
            this.lblStartTime.Text = "Program Start Time";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(11, 25);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
           // this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(181, 165);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(40, 23);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblRecCnt
            // 
            this.lblRecCnt.AutoSize = true;
            this.lblRecCnt.Location = new System.Drawing.Point(113, 52);
            this.lblRecCnt.Name = "lblRecCnt";
            this.lblRecCnt.Size = new System.Drawing.Size(38, 13);
            this.lblRecCnt.TabIndex = 5;
            this.lblRecCnt.Text = "recCnt";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Update Number";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Time Now";
            // 
            // lblNowTime
            // 
            this.lblNowTime.AutoSize = true;
            this.lblNowTime.Location = new System.Drawing.Point(116, 73);
            this.lblNowTime.Name = "lblNowTime";
            this.lblNowTime.Size = new System.Drawing.Size(52, 13);
            this.lblNowTime.TabIndex = 8;
            this.lblNowTime.Text = "NowTime";
            // 
            // btnReadCorrections
            // 
            this.btnReadCorrections.Location = new System.Drawing.Point(133, 113);
            this.btnReadCorrections.Name = "btnReadCorrections";
            this.btnReadCorrections.Size = new System.Drawing.Size(95, 23);
            this.btnReadCorrections.TabIndex = 9;
            this.btnReadCorrections.Text = "read Corrections";
            this.btnReadCorrections.UseVisualStyleBackColor = true;
            this.btnReadCorrections.Click += new System.EventHandler(this.btnReadCorrections_Click);
            // 
            // txtLowTempCondition
            // 
            this.txtLowTempCondition.Location = new System.Drawing.Point(68, 125);
            this.txtLowTempCondition.Name = "txtLowTempCondition";
            this.txtLowTempCondition.Size = new System.Drawing.Size(37, 20);
            this.txtLowTempCondition.TabIndex = 10;
            this.txtLowTempCondition.Text = "18";
            // 
            // txtHighTempcondition
            // 
            this.txtHighTempcondition.Location = new System.Drawing.Point(68, 147);
            this.txtHighTempcondition.Name = "txtHighTempcondition";
            this.txtHighTempcondition.Size = new System.Drawing.Size(37, 20);
            this.txtHighTempcondition.TabIndex = 11;
            this.txtHighTempcondition.Text = "30";
            // 
            // txtHighWindCondition
            // 
            this.txtHighWindCondition.Location = new System.Drawing.Point(68, 171);
            this.txtHighWindCondition.Name = "txtHighWindCondition";
            this.txtHighWindCondition.Size = new System.Drawing.Size(37, 20);
            this.txtHighWindCondition.TabIndex = 12;
            this.txtHighWindCondition.Text = "40";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "LowTemp";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 151);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "High Temp";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(8, 175);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "HighWind";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 109);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "House Conditions";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 197);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(170, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Web Update Frequency (seconds)";
            // 
            // txtWebUpdateCycle
            // 
            this.txtWebUpdateCycle.Location = new System.Drawing.Point(189, 194);
            this.txtWebUpdateCycle.Name = "txtWebUpdateCycle";
            this.txtWebUpdateCycle.Size = new System.Drawing.Size(32, 20);
            this.txtWebUpdateCycle.TabIndex = 18;
            this.txtWebUpdateCycle.Text = "120";
            // 
            // chkTweet
            // 
            this.chkTweet.AutoSize = true;
            this.chkTweet.Checked = true;
            this.chkTweet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTweet.Location = new System.Drawing.Point(137, 142);
            this.chkTweet.Name = "chkTweet";
            this.chkTweet.Size = new System.Drawing.Size(56, 17);
            this.chkTweet.TabIndex = 22;
            this.chkTweet.Text = "Tweet";
            this.chkTweet.UseVisualStyleBackColor = true;
            // 
            // WXSensor2WebPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 227);
            this.Controls.Add(this.chkTweet);
            this.Controls.Add(this.txtWebUpdateCycle);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtHighWindCondition);
            this.Controls.Add(this.txtHighTempcondition);
            this.Controls.Add(this.txtLowTempCondition);
            this.Controls.Add(this.btnReadCorrections);
            this.Controls.Add(this.lblNowTime);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblRecCnt);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lblStartTime);
            this.Controls.Add(this.label1);
            this.Name = "WXSensor2WebPage";
            this.Text = "WXWebPageWriter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStartTime;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label lblRecCnt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblNowTime;
        private System.Windows.Forms.Button btnReadCorrections;
        private System.Windows.Forms.TextBox txtLowTempCondition;
        private System.Windows.Forms.TextBox txtHighTempcondition;
        private System.Windows.Forms.TextBox txtHighWindCondition;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtWebUpdateCycle;
        private System.Windows.Forms.CheckBox chkTweet;
    }
}


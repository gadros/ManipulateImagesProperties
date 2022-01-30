namespace AssignDatePictureViewerApp
{
    partial class Form1
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.SelectFolderBtn = new System.Windows.Forms.Button();
            this.PreviousBtn = new System.Windows.Forms.Button();
            this.NextBtn = new System.Windows.Forms.Button();
            this.ImageDateMskTxtBox = new System.Windows.Forms.MaskedTextBox();
            this.SetDateBtn = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SetDateMoveNextBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85F));
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 93F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(887, 533);
            this.tableLayoutPanel1.TabIndex = 0;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tableLayoutPanel1.SetColumnSpan(this.pictureBox1, 2);
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(2, 2);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(883, 491);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.SelectFolderBtn);
            this.flowLayoutPanel1.Controls.Add(this.PreviousBtn);
            this.flowLayoutPanel1.Controls.Add(this.NextBtn);
            this.flowLayoutPanel1.Controls.Add(this.ImageDateMskTxtBox);
            this.flowLayoutPanel1.Controls.Add(this.SetDateMoveNextBtn);
            this.flowLayoutPanel1.Controls.Add(this.SetDateBtn);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(135, 497);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(750, 34);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // SelectFolderBtn
            // 
            this.SelectFolderBtn.Location = new System.Drawing.Point(2, 2);
            this.SelectFolderBtn.Margin = new System.Windows.Forms.Padding(2);
            this.SelectFolderBtn.Name = "SelectFolderBtn";
            this.SelectFolderBtn.Size = new System.Drawing.Size(84, 31);
            this.SelectFolderBtn.TabIndex = 0;
            this.SelectFolderBtn.Text = "Select Folder";
            this.toolTip1.SetToolTip(this.SelectFolderBtn, "Select the folder that has pictures");
            this.SelectFolderBtn.UseVisualStyleBackColor = true;
            this.SelectFolderBtn.Click += new System.EventHandler(this.SelectFolderBtn_Click);
            // 
            // PreviousBtn
            // 
            this.PreviousBtn.Location = new System.Drawing.Point(90, 2);
            this.PreviousBtn.Margin = new System.Windows.Forms.Padding(2);
            this.PreviousBtn.Name = "PreviousBtn";
            this.PreviousBtn.Size = new System.Drawing.Size(56, 31);
            this.PreviousBtn.TabIndex = 1;
            this.PreviousBtn.Text = "Previous";
            this.toolTip1.SetToolTip(this.PreviousBtn, "Previous picture");
            this.PreviousBtn.UseVisualStyleBackColor = true;
            this.PreviousBtn.Click += new System.EventHandler(this.PreviousBtn_Click);
            // 
            // NextBtn
            // 
            this.NextBtn.Location = new System.Drawing.Point(150, 2);
            this.NextBtn.Margin = new System.Windows.Forms.Padding(2);
            this.NextBtn.Name = "NextBtn";
            this.NextBtn.Size = new System.Drawing.Size(56, 31);
            this.NextBtn.TabIndex = 2;
            this.NextBtn.Text = "Next";
            this.toolTip1.SetToolTip(this.NextBtn, "Next picture");
            this.NextBtn.UseVisualStyleBackColor = true;
            this.NextBtn.Click += new System.EventHandler(this.NextBtn_Click);
            // 
            // ImageDateMskTxtBox
            // 
            this.ImageDateMskTxtBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ImageDateMskTxtBox.Location = new System.Drawing.Point(210, 2);
            this.ImageDateMskTxtBox.Margin = new System.Windows.Forms.Padding(2);
            this.ImageDateMskTxtBox.Mask = "00/00/0000";
            this.ImageDateMskTxtBox.Name = "ImageDateMskTxtBox";
            this.ImageDateMskTxtBox.Size = new System.Drawing.Size(65, 20);
            this.ImageDateMskTxtBox.TabIndex = 3;
            this.toolTip1.SetToolTip(this.ImageDateMskTxtBox, "The date the picture was taken");
            this.ImageDateMskTxtBox.ValidatingType = typeof(System.DateTime);
            // 
            // SetDateBtn
            // 
            this.SetDateBtn.Location = new System.Drawing.Point(410, 2);
            this.SetDateBtn.Margin = new System.Windows.Forms.Padding(2);
            this.SetDateBtn.Name = "SetDateBtn";
            this.SetDateBtn.Size = new System.Drawing.Size(78, 31);
            this.SetDateBtn.TabIndex = 5;
            this.SetDateBtn.Text = "Set Date";
            this.toolTip1.SetToolTip(this.SetDateBtn, "Save the date taken value to the picture");
            this.SetDateBtn.UseVisualStyleBackColor = true;
            this.SetDateBtn.Click += new System.EventHandler(this.SetDateBtn_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select folder with pictures";
            // 
            // SetDateMoveNextBtn
            // 
            this.SetDateMoveNextBtn.Location = new System.Drawing.Point(279, 2);
            this.SetDateMoveNextBtn.Margin = new System.Windows.Forms.Padding(2);
            this.SetDateMoveNextBtn.Name = "SetDateMoveNextBtn";
            this.SetDateMoveNextBtn.Size = new System.Drawing.Size(127, 31);
            this.SetDateMoveNextBtn.TabIndex = 4;
            this.SetDateMoveNextBtn.Text = "Set Date Move Next";
            this.toolTip1.SetToolTip(this.SetDateMoveNextBtn, "Save the date taken value to the picture");
            this.SetDateMoveNextBtn.UseVisualStyleBackColor = true;
            this.SetDateMoveNextBtn.Click += new System.EventHandler(this.SetDateMoveNextBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(887, 533);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Assign Date to Image";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button SelectFolderBtn;
        private System.Windows.Forms.Button PreviousBtn;
        private System.Windows.Forms.Button NextBtn;
        private System.Windows.Forms.Button SetDateBtn;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.MaskedTextBox ImageDateMskTxtBox;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button SetDateMoveNextBtn;
    }
}


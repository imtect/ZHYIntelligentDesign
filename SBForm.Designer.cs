namespace CadPlugins
{
    partial class SBForm
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
            this.OpenDBBtn = new System.Windows.Forms.Button();
            this.DB_pathTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.WritePositionInfoBtn = new System.Windows.Forms.Button();
            this.ReadBlockToDB = new System.Windows.Forms.Button();
            this.FloorcomboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OpenDBBtn
            // 
            this.OpenDBBtn.Location = new System.Drawing.Point(311, 51);
            this.OpenDBBtn.Name = "OpenDBBtn";
            this.OpenDBBtn.Size = new System.Drawing.Size(75, 23);
            this.OpenDBBtn.TabIndex = 0;
            this.OpenDBBtn.Text = "打开数据库";
            this.OpenDBBtn.UseVisualStyleBackColor = true;
            this.OpenDBBtn.Click += new System.EventHandler(this.OpenDBBtn_Click);
            // 
            // DB_pathTextBox
            // 
            this.DB_pathTextBox.Location = new System.Drawing.Point(122, 51);
            this.DB_pathTextBox.Name = "DB_pathTextBox";
            this.DB_pathTextBox.Size = new System.Drawing.Size(162, 21);
            this.DB_pathTextBox.TabIndex = 1;
            this.DB_pathTextBox.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "数据库的路径";
            // 
            // WritePositionInfoBtn
            // 
            this.WritePositionInfoBtn.Location = new System.Drawing.Point(12, 120);
            this.WritePositionInfoBtn.Name = "WritePositionInfoBtn";
            this.WritePositionInfoBtn.Size = new System.Drawing.Size(418, 23);
            this.WritePositionInfoBtn.TabIndex = 0;
            this.WritePositionInfoBtn.Text = "写入相对位置信息至数据库";
            this.WritePositionInfoBtn.UseVisualStyleBackColor = true;
            this.WritePositionInfoBtn.Click += new System.EventHandler(this.WritePositionInfoBtn_Click);
            // 
            // ReadBlockToDB
            // 
            this.ReadBlockToDB.Location = new System.Drawing.Point(12, 149);
            this.ReadBlockToDB.Name = "ReadBlockToDB";
            this.ReadBlockToDB.Size = new System.Drawing.Size(418, 23);
            this.ReadBlockToDB.TabIndex = 0;
            this.ReadBlockToDB.Text = "写入块位置信息至数据库";
            this.ReadBlockToDB.UseVisualStyleBackColor = true;
            this.ReadBlockToDB.Click += new System.EventHandler(this.ReadBlockToDB_Click);
            // 
            // FloorcomboBox
            // 
            this.FloorcomboBox.FormattingEnabled = true;
            this.FloorcomboBox.Location = new System.Drawing.Point(122, 88);
            this.FloorcomboBox.Name = "FloorcomboBox";
            this.FloorcomboBox.Size = new System.Drawing.Size(121, 20);
            this.FloorcomboBox.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 88);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "楼层";
            // 
            // SBForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 343);
            this.Controls.Add(this.FloorcomboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DB_pathTextBox);
            this.Controls.Add(this.ReadBlockToDB);
            this.Controls.Add(this.WritePositionInfoBtn);
            this.Controls.Add(this.OpenDBBtn);
            this.Name = "SBForm";
            this.Text = "自动翻模软件";
            this.Load += new System.EventHandler(this.SBForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OpenDBBtn;
        private System.Windows.Forms.TextBox DB_pathTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button WritePositionInfoBtn;
        private System.Windows.Forms.Button ReadBlockToDB;
        private System.Windows.Forms.ComboBox FloorcomboBox;
        private System.Windows.Forms.Label label2;
    }
}
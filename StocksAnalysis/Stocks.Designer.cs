namespace StocksAnalysis
{
    partial class Stocks
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabIndicatorAnalysis = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnStockAnalysis = new System.Windows.Forms.Button();
            this.InsertIntoDB = new System.Windows.Forms.Button();
            this.tabSetting = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabIndicatorAnalysis.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabIndicatorAnalysis);
            this.tabControl1.Controls.Add(this.tabSetting);
            this.tabControl1.Font = new System.Drawing.Font("Segoe Print", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1555, 628);
            this.tabControl1.TabIndex = 3;
            // 
            // tabIndicatorAnalysis
            // 
            this.tabIndicatorAnalysis.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.tabIndicatorAnalysis.Controls.Add(this.dataGridView1);
            this.tabIndicatorAnalysis.Controls.Add(this.btnStockAnalysis);
            this.tabIndicatorAnalysis.Controls.Add(this.InsertIntoDB);
            this.tabIndicatorAnalysis.Location = new System.Drawing.Point(4, 35);
            this.tabIndicatorAnalysis.Name = "tabIndicatorAnalysis";
            this.tabIndicatorAnalysis.Padding = new System.Windows.Forms.Padding(3);
            this.tabIndicatorAnalysis.Size = new System.Drawing.Size(1547, 589);
            this.tabIndicatorAnalysis.TabIndex = 0;
            this.tabIndicatorAnalysis.Text = "Indicator Analysis";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 67);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1535, 526);
            this.dataGridView1.TabIndex = 5;
            // 
            // btnStockAnalysis
            // 
            this.btnStockAnalysis.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.btnStockAnalysis.Location = new System.Drawing.Point(179, 14);
            this.btnStockAnalysis.Name = "btnStockAnalysis";
            this.btnStockAnalysis.Size = new System.Drawing.Size(130, 43);
            this.btnStockAnalysis.TabIndex = 4;
            this.btnStockAnalysis.Text = "Get Stock";
            this.btnStockAnalysis.UseVisualStyleBackColor = false;
            this.btnStockAnalysis.Click += new System.EventHandler(this.btnStockAnalysis_Click);
            // 
            // InsertIntoDB
            // 
            this.InsertIntoDB.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.InsertIntoDB.Location = new System.Drawing.Point(16, 13);
            this.InsertIntoDB.Name = "InsertIntoDB";
            this.InsertIntoDB.Size = new System.Drawing.Size(144, 43);
            this.InsertIntoDB.TabIndex = 1;
            this.InsertIntoDB.Text = "Insert Into Db";
            this.InsertIntoDB.UseVisualStyleBackColor = false;
            this.InsertIntoDB.Click += new System.EventHandler(this.InsertIntoDB_Click);
            // 
            // tabSetting
            // 
            this.tabSetting.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.tabSetting.Location = new System.Drawing.Point(4, 35);
            this.tabSetting.Name = "tabSetting";
            this.tabSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabSetting.Size = new System.Drawing.Size(1547, 589);
            this.tabSetting.TabIndex = 1;
            this.tabSetting.Text = "Setting";
            // 
            // Stocks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(1579, 652);
            this.Controls.Add(this.tabControl1);
            this.Name = "Stocks";
            this.Text = "Stock Analysis Tool";
            this.tabControl1.ResumeLayout(false);
            this.tabIndicatorAnalysis.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabIndicatorAnalysis;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnStockAnalysis;
        private System.Windows.Forms.Button InsertIntoDB;
        private System.Windows.Forms.TabPage tabSetting;
    }
}


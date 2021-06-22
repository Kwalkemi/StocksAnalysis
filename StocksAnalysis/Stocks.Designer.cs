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
            this.InsertIntoDB = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnStockAnalysis = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // InsertIntoDB
            // 
            this.InsertIntoDB.Location = new System.Drawing.Point(41, 24);
            this.InsertIntoDB.Name = "InsertIntoDB";
            this.InsertIntoDB.Size = new System.Drawing.Size(130, 31);
            this.InsertIntoDB.TabIndex = 0;
            this.InsertIntoDB.Text = "Insert Into Db";
            this.InsertIntoDB.UseVisualStyleBackColor = true;
            this.InsertIntoDB.Click += new System.EventHandler(this.InsertIntoDB_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 129);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1286, 511);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // btnStockAnalysis
            // 
            this.btnStockAnalysis.Location = new System.Drawing.Point(207, 24);
            this.btnStockAnalysis.Name = "btnStockAnalysis";
            this.btnStockAnalysis.Size = new System.Drawing.Size(130, 31);
            this.btnStockAnalysis.TabIndex = 2;
            this.btnStockAnalysis.Text = "Get Stock";
            this.btnStockAnalysis.UseVisualStyleBackColor = true;
            this.btnStockAnalysis.Click += new System.EventHandler(this.btnStockAnalysis_Click);
            // 
            // Stocks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1310, 652);
            this.Controls.Add(this.btnStockAnalysis);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.InsertIntoDB);
            this.Name = "Stocks";
            this.Text = "Stock Analysis Tool";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button InsertIntoDB;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnStockAnalysis;
    }
}


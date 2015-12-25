namespace cs_EVE_Arbitrage
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnFind = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txbSource = new System.Windows.Forms.TextBox();
            this.txbDestination = new System.Windows.Forms.TextBox();
            this.btnSwap = new System.Windows.Forms.Button();
            this.rtbDisplay = new System.Windows.Forms.RichTextBox();
            this.dgvDisplay = new System.Windows.Forms.DataGridView();
            this.chbExclude = new System.Windows.Forms.CheckBox();
            this.txbExclude = new System.Windows.Forms.TextBox();
            this.txbMaxVolume = new System.Windows.Forms.TextBox();
            this.chbMaxVolume = new System.Windows.Forms.CheckBox();
            this.itemname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.profitpervolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unitprofit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sellmin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buyprice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.unitvolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sellstation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.destinationstation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisplay)).BeginInit();
            this.SuspendLayout();
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(15, 90);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(75, 23);
            this.btnFind.TabIndex = 0;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Source Buy:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Destination Sell:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 137);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Results:";
            // 
            // txbSource
            // 
            this.txbSource.Location = new System.Drawing.Point(15, 25);
            this.txbSource.Name = "txbSource";
            this.txbSource.Size = new System.Drawing.Size(100, 20);
            this.txbSource.TabIndex = 4;
            this.txbSource.Text = "Curse";
            // 
            // txbDestination
            // 
            this.txbDestination.Location = new System.Drawing.Point(15, 64);
            this.txbDestination.Name = "txbDestination";
            this.txbDestination.Size = new System.Drawing.Size(100, 20);
            this.txbDestination.TabIndex = 5;
            this.txbDestination.Text = "Jita";
            // 
            // btnSwap
            // 
            this.btnSwap.Location = new System.Drawing.Point(121, 43);
            this.btnSwap.Name = "btnSwap";
            this.btnSwap.Size = new System.Drawing.Size(75, 23);
            this.btnSwap.TabIndex = 6;
            this.btnSwap.Text = "Swap";
            this.btnSwap.UseVisualStyleBackColor = true;
            this.btnSwap.Click += new System.EventHandler(this.btnSwap_Click);
            // 
            // rtbDisplay
            // 
            this.rtbDisplay.Location = new System.Drawing.Point(12, 153);
            this.rtbDisplay.Name = "rtbDisplay";
            this.rtbDisplay.ReadOnly = true;
            this.rtbDisplay.Size = new System.Drawing.Size(247, 25);
            this.rtbDisplay.TabIndex = 7;
            this.rtbDisplay.Text = "";
            // 
            // dgvDisplay
            // 
            this.dgvDisplay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDisplay.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.itemname,
            this.profitpervolume,
            this.unitprofit,
            this.sellmin,
            this.buyprice,
            this.unitvolume,
            this.sellstation,
            this.destinationstation});
            this.dgvDisplay.Location = new System.Drawing.Point(12, 184);
            this.dgvDisplay.Name = "dgvDisplay";
            this.dgvDisplay.Size = new System.Drawing.Size(1197, 272);
            this.dgvDisplay.TabIndex = 8;
            // 
            // chbExclude
            // 
            this.chbExclude.AutoSize = true;
            this.chbExclude.Checked = true;
            this.chbExclude.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbExclude.Location = new System.Drawing.Point(221, 27);
            this.chbExclude.Name = "chbExclude";
            this.chbExclude.Size = new System.Drawing.Size(108, 17);
            this.chbExclude.TabIndex = 9;
            this.chbExclude.Text = "Min. Profit per m³:";
            this.chbExclude.UseVisualStyleBackColor = true;
            this.chbExclude.CheckedChanged += new System.EventHandler(this.chbExclude_CheckedChanged);
            // 
            // txbExclude
            // 
            this.txbExclude.Location = new System.Drawing.Point(335, 25);
            this.txbExclude.Name = "txbExclude";
            this.txbExclude.Size = new System.Drawing.Size(100, 20);
            this.txbExclude.TabIndex = 10;
            this.txbExclude.Text = "10000";
            // 
            // txbMaxVolume
            // 
            this.txbMaxVolume.Enabled = false;
            this.txbMaxVolume.Location = new System.Drawing.Point(335, 51);
            this.txbMaxVolume.Name = "txbMaxVolume";
            this.txbMaxVolume.Size = new System.Drawing.Size(100, 20);
            this.txbMaxVolume.TabIndex = 11;
            // 
            // chbMaxVolume
            // 
            this.chbMaxVolume.AutoSize = true;
            this.chbMaxVolume.Location = new System.Drawing.Point(221, 53);
            this.chbMaxVolume.Name = "chbMaxVolume";
            this.chbMaxVolume.Size = new System.Drawing.Size(87, 17);
            this.chbMaxVolume.TabIndex = 12;
            this.chbMaxVolume.Text = "Max Volume:";
            this.chbMaxVolume.UseVisualStyleBackColor = true;
            this.chbMaxVolume.CheckedChanged += new System.EventHandler(this.chbMaxVolume_CheckedChanged);
            // 
            // itemname
            // 
            dataGridViewCellStyle1.NullValue = null;
            this.itemname.DefaultCellStyle = dataGridViewCellStyle1;
            this.itemname.Frozen = true;
            this.itemname.HeaderText = "Item Name";
            this.itemname.Name = "itemname";
            this.itemname.ReadOnly = true;
            this.itemname.Width = 200;
            // 
            // profitpervolume
            // 
            dataGridViewCellStyle2.Format = "N2";
            dataGridViewCellStyle2.NullValue = null;
            this.profitpervolume.DefaultCellStyle = dataGridViewCellStyle2;
            this.profitpervolume.Frozen = true;
            this.profitpervolume.HeaderText = "Profit per m³";
            this.profitpervolume.Name = "profitpervolume";
            this.profitpervolume.ReadOnly = true;
            // 
            // unitprofit
            // 
            dataGridViewCellStyle3.Format = "N2";
            dataGridViewCellStyle3.NullValue = null;
            this.unitprofit.DefaultCellStyle = dataGridViewCellStyle3;
            this.unitprofit.Frozen = true;
            this.unitprofit.HeaderText = "Unit Profit";
            this.unitprofit.Name = "unitprofit";
            this.unitprofit.ReadOnly = true;
            // 
            // sellmin
            // 
            dataGridViewCellStyle4.Format = "N2";
            dataGridViewCellStyle4.NullValue = null;
            this.sellmin.DefaultCellStyle = dataGridViewCellStyle4;
            this.sellmin.Frozen = true;
            this.sellmin.HeaderText = "Sell Min";
            this.sellmin.Name = "sellmin";
            this.sellmin.ReadOnly = true;
            // 
            // buyprice
            // 
            dataGridViewCellStyle5.Format = "N2";
            dataGridViewCellStyle5.NullValue = null;
            this.buyprice.DefaultCellStyle = dataGridViewCellStyle5;
            this.buyprice.Frozen = true;
            this.buyprice.HeaderText = "Buy Max";
            this.buyprice.Name = "buyprice";
            this.buyprice.ReadOnly = true;
            // 
            // unitvolume
            // 
            dataGridViewCellStyle6.Format = "N2";
            dataGridViewCellStyle6.NullValue = null;
            this.unitvolume.DefaultCellStyle = dataGridViewCellStyle6;
            this.unitvolume.Frozen = true;
            this.unitvolume.HeaderText = "Unit Volume";
            this.unitvolume.Name = "unitvolume";
            this.unitvolume.ReadOnly = true;
            // 
            // sellstation
            // 
            this.sellstation.Frozen = true;
            this.sellstation.HeaderText = "Source Station";
            this.sellstation.Name = "sellstation";
            this.sellstation.ReadOnly = true;
            this.sellstation.Width = 225;
            // 
            // destinationstation
            // 
            this.destinationstation.Frozen = true;
            this.destinationstation.HeaderText = "Destination Station";
            this.destinationstation.Name = "destinationstation";
            this.destinationstation.ReadOnly = true;
            this.destinationstation.Width = 225;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1221, 468);
            this.Controls.Add(this.chbMaxVolume);
            this.Controls.Add(this.txbMaxVolume);
            this.Controls.Add(this.txbExclude);
            this.Controls.Add(this.chbExclude);
            this.Controls.Add(this.dgvDisplay);
            this.Controls.Add(this.rtbDisplay);
            this.Controls.Add(this.btnSwap);
            this.Controls.Add(this.txbDestination);
            this.Controls.Add(this.txbSource);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFind);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "EVE Arbitrage Finder";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisplay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txbSource;
        private System.Windows.Forms.TextBox txbDestination;
        private System.Windows.Forms.Button btnSwap;
        public System.Windows.Forms.RichTextBox rtbDisplay;
        private System.Windows.Forms.DataGridView dgvDisplay;
        private System.Windows.Forms.CheckBox chbExclude;
        private System.Windows.Forms.TextBox txbExclude;
        private System.Windows.Forms.TextBox txbMaxVolume;
        private System.Windows.Forms.CheckBox chbMaxVolume;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemname;
        private System.Windows.Forms.DataGridViewTextBoxColumn profitpervolume;
        private System.Windows.Forms.DataGridViewTextBoxColumn unitprofit;
        private System.Windows.Forms.DataGridViewTextBoxColumn sellmin;
        private System.Windows.Forms.DataGridViewTextBoxColumn buyprice;
        private System.Windows.Forms.DataGridViewTextBoxColumn unitvolume;
        private System.Windows.Forms.DataGridViewTextBoxColumn sellstation;
        private System.Windows.Forms.DataGridViewTextBoxColumn destinationstation;
    }
}


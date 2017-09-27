namespace TipCalculator
{
    partial class TipCalculatorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TipCalculatorForm));
            this.totalBillLabel = new System.Windows.Forms.Label();
            this.totalBillTextBox = new System.Windows.Forms.TextBox();
            this.calculatedTipTextBox = new System.Windows.Forms.TextBox();
            this.tipPercentageTextBox = new System.Windows.Forms.TextBox();
            this.tipPercentageLabel = new System.Windows.Forms.Label();
            this.tipAmountLabel = new System.Windows.Forms.Label();
            this.totalToPayLabel = new System.Windows.Forms.Label();
            this.totalToPayTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // totalBillLabel
            // 
            this.totalBillLabel.AutoSize = true;
            this.totalBillLabel.BackColor = System.Drawing.Color.Transparent;
            this.totalBillLabel.Font = new System.Drawing.Font("Rockwell", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalBillLabel.ForeColor = System.Drawing.Color.White;
            this.totalBillLabel.Location = new System.Drawing.Point(12, 46);
            this.totalBillLabel.Name = "totalBillLabel";
            this.totalBillLabel.Size = new System.Drawing.Size(277, 42);
            this.totalBillLabel.TabIndex = 0;
            this.totalBillLabel.Text = "Enter Total Bill:";
            // 
            // totalBillTextBox
            // 
            this.totalBillTextBox.Location = new System.Drawing.Point(404, 57);
            this.totalBillTextBox.Name = "totalBillTextBox";
            this.totalBillTextBox.Size = new System.Drawing.Size(105, 31);
            this.totalBillTextBox.TabIndex = 0;
            this.totalBillTextBox.Text = "10.50";
            this.totalBillTextBox.TextChanged += new System.EventHandler(this.TotalBillTextBox_TextChanged);
            // 
            // calculatedTipTextBox
            // 
            this.calculatedTipTextBox.Location = new System.Drawing.Point(404, 175);
            this.calculatedTipTextBox.Name = "calculatedTipTextBox";
            this.calculatedTipTextBox.ReadOnly = true;
            this.calculatedTipTextBox.Size = new System.Drawing.Size(155, 31);
            this.calculatedTipTextBox.TabIndex = 2;
            // 
            // tipPercentageTextBox
            // 
            this.tipPercentageTextBox.Location = new System.Drawing.Point(404, 118);
            this.tipPercentageTextBox.Name = "tipPercentageTextBox";
            this.tipPercentageTextBox.Size = new System.Drawing.Size(66, 31);
            this.tipPercentageTextBox.TabIndex = 1;
            this.tipPercentageTextBox.Text = "20";
            this.tipPercentageTextBox.TextChanged += new System.EventHandler(this.TipPercentageTextBox_TextChanged);
            // 
            // tipPercentageLabel
            // 
            this.tipPercentageLabel.AutoSize = true;
            this.tipPercentageLabel.BackColor = System.Drawing.Color.Transparent;
            this.tipPercentageLabel.Font = new System.Drawing.Font("Rockwell", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tipPercentageLabel.ForeColor = System.Drawing.Color.White;
            this.tipPercentageLabel.Location = new System.Drawing.Point(12, 107);
            this.tipPercentageLabel.Name = "tipPercentageLabel";
            this.tipPercentageLabel.Size = new System.Drawing.Size(386, 42);
            this.tipPercentageLabel.TabIndex = 4;
            this.tipPercentageLabel.Text = "Enter Tip Percentage:";
            // 
            // tipAmountLabel
            // 
            this.tipAmountLabel.AutoSize = true;
            this.tipAmountLabel.BackColor = System.Drawing.Color.Transparent;
            this.tipAmountLabel.Font = new System.Drawing.Font("Rockwell", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tipAmountLabel.ForeColor = System.Drawing.Color.White;
            this.tipAmountLabel.Location = new System.Drawing.Point(12, 162);
            this.tipAmountLabel.Name = "tipAmountLabel";
            this.tipAmountLabel.Size = new System.Drawing.Size(225, 42);
            this.tipAmountLabel.TabIndex = 6;
            this.tipAmountLabel.Text = "Tip Amount:";
            // 
            // totalToPayLabel
            // 
            this.totalToPayLabel.AutoSize = true;
            this.totalToPayLabel.BackColor = System.Drawing.Color.Transparent;
            this.totalToPayLabel.Font = new System.Drawing.Font("Rockwell", 13.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalToPayLabel.ForeColor = System.Drawing.Color.White;
            this.totalToPayLabel.Location = new System.Drawing.Point(12, 223);
            this.totalToPayLabel.Name = "totalToPayLabel";
            this.totalToPayLabel.Size = new System.Drawing.Size(225, 42);
            this.totalToPayLabel.TabIndex = 7;
            this.totalToPayLabel.Text = "Total to Pay:";
            // 
            // totalToPayTextBox
            // 
            this.totalToPayTextBox.Location = new System.Drawing.Point(404, 234);
            this.totalToPayTextBox.Name = "totalToPayTextBox";
            this.totalToPayTextBox.ReadOnly = true;
            this.totalToPayTextBox.Size = new System.Drawing.Size(155, 31);
            this.totalToPayTextBox.TabIndex = 3;
            // 
            // TipCalculatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(573, 405);
            this.Controls.Add(this.totalToPayTextBox);
            this.Controls.Add(this.totalToPayLabel);
            this.Controls.Add(this.tipAmountLabel);
            this.Controls.Add(this.tipPercentageTextBox);
            this.Controls.Add(this.tipPercentageLabel);
            this.Controls.Add(this.calculatedTipTextBox);
            this.Controls.Add(this.totalBillTextBox);
            this.Controls.Add(this.totalBillLabel);
            this.Name = "TipCalculatorForm";
            this.Text = "Tip Calculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label totalBillLabel;
        private System.Windows.Forms.TextBox totalBillTextBox;
        private System.Windows.Forms.TextBox calculatedTipTextBox;
        private System.Windows.Forms.TextBox tipPercentageTextBox;
        private System.Windows.Forms.Label tipPercentageLabel;
        private System.Windows.Forms.Label tipAmountLabel;
        private System.Windows.Forms.Label totalToPayLabel;
        private System.Windows.Forms.TextBox totalToPayTextBox;
    }
}


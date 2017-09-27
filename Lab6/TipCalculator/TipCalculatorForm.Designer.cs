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
            this.totalBillLabel = new System.Windows.Forms.Label();
            this.calculateTipButton = new System.Windows.Forms.Button();
            this.totalBillTextBox = new System.Windows.Forms.TextBox();
            this.calculatedTipTextBox = new System.Windows.Forms.TextBox();
            this.tipPercentageTextBox = new System.Windows.Forms.TextBox();
            this.tipPercentageLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // totalBillLabel
            // 
            this.totalBillLabel.AutoSize = true;
            this.totalBillLabel.Location = new System.Drawing.Point(83, 49);
            this.totalBillLabel.Name = "totalBillLabel";
            this.totalBillLabel.Size = new System.Drawing.Size(158, 25);
            this.totalBillLabel.TabIndex = 0;
            this.totalBillLabel.Text = "Enter Total Bill:";
            // 
            // calculateTipButton
            // 
            this.calculateTipButton.Location = new System.Drawing.Point(88, 158);
            this.calculateTipButton.Name = "calculateTipButton";
            this.calculateTipButton.Size = new System.Drawing.Size(153, 39);
            this.calculateTipButton.TabIndex = 1;
            this.calculateTipButton.Text = "Calculate Tip";
            this.calculateTipButton.UseVisualStyleBackColor = true;
            this.calculateTipButton.Click += new System.EventHandler(this.CalculateTipButton_Click);
            // 
            // totalBillTextBox
            // 
            this.totalBillTextBox.Location = new System.Drawing.Point(271, 43);
            this.totalBillTextBox.Name = "totalBillTextBox";
            this.totalBillTextBox.Size = new System.Drawing.Size(105, 31);
            this.totalBillTextBox.TabIndex = 2;
            this.totalBillTextBox.Text = "10.50";
            // 
            // calculatedTipTextBox
            // 
            this.calculatedTipTextBox.Location = new System.Drawing.Point(271, 162);
            this.calculatedTipTextBox.Name = "calculatedTipTextBox";
            this.calculatedTipTextBox.ReadOnly = true;
            this.calculatedTipTextBox.Size = new System.Drawing.Size(105, 31);
            this.calculatedTipTextBox.TabIndex = 3;
            // 
            // tipPercentageTextBox
            // 
            this.tipPercentageTextBox.Location = new System.Drawing.Point(310, 104);
            this.tipPercentageTextBox.Name = "tipPercentageTextBox";
            this.tipPercentageTextBox.Size = new System.Drawing.Size(66, 31);
            this.tipPercentageTextBox.TabIndex = 5;
            this.tipPercentageTextBox.Text = "20";
            // 
            // tipPercentageLabel
            // 
            this.tipPercentageLabel.AutoSize = true;
            this.tipPercentageLabel.Location = new System.Drawing.Point(83, 110);
            this.tipPercentageLabel.Name = "tipPercentageLabel";
            this.tipPercentageLabel.Size = new System.Drawing.Size(221, 25);
            this.tipPercentageLabel.TabIndex = 4;
            this.tipPercentageLabel.Text = "Enter Tip Percentage:";
            // 
            // TipCalculatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(456, 275);
            this.Controls.Add(this.tipPercentageTextBox);
            this.Controls.Add(this.tipPercentageLabel);
            this.Controls.Add(this.calculatedTipTextBox);
            this.Controls.Add(this.totalBillTextBox);
            this.Controls.Add(this.calculateTipButton);
            this.Controls.Add(this.totalBillLabel);
            this.Name = "TipCalculatorForm";
            this.Text = "Tip Calculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label totalBillLabel;
        private System.Windows.Forms.Button calculateTipButton;
        private System.Windows.Forms.TextBox totalBillTextBox;
        private System.Windows.Forms.TextBox calculatedTipTextBox;
        private System.Windows.Forms.TextBox tipPercentageTextBox;
        private System.Windows.Forms.Label tipPercentageLabel;
    }
}


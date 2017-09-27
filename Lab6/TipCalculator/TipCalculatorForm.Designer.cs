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
            this.calculateTipButton.Location = new System.Drawing.Point(88, 117);
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
            this.totalBillTextBox.Size = new System.Drawing.Size(245, 31);
            this.totalBillTextBox.TabIndex = 2;
            // 
            // calculatedTipTextBox
            // 
            this.calculatedTipTextBox.Location = new System.Drawing.Point(271, 121);
            this.calculatedTipTextBox.Name = "calculatedTipTextBox";
            this.calculatedTipTextBox.ReadOnly = true;
            this.calculatedTipTextBox.Size = new System.Drawing.Size(245, 31);
            this.calculatedTipTextBox.TabIndex = 3;
            // 
            // TipCalculatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 210);
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
    }
}


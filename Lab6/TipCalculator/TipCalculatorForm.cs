using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TipCalculator
{
    public partial class TipCalculatorForm : Form
    {
        public TipCalculatorForm()
        {
            InitializeComponent();
            CalculateTip();
        }

        private void TotalBillTextBox_TextChanged(object sender, EventArgs e)
        {
            CalculateTip();
        }

        private void TipPercentageTextBox_TextChanged(object sender, EventArgs e)
        {
            CalculateTip();
        }

        /// <summary>
        /// Checks the input for errors.
        /// 
        /// Calculates the tip for the inputted total bill and tip percentage,
        /// and displays the tip and new total in the output fields.
        /// </summary>
        private void CalculateTip()
        {
            if (!double.TryParse(totalBillTextBox.Text, out var bill)
                || !double.TryParse(tipPercentageTextBox.Text, out var tip))
                return;

            // Calculate Tip
            var tipToPay = bill * (tip / 100);
            calculatedTipTextBox.Text = tipToPay.ToString("C2");

            // Calculate Total Payment
            totalToPayTextBox.Text = (bill + tipToPay).ToString("C2");
        }
    }
}
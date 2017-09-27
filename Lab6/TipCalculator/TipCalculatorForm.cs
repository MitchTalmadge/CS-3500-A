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
        }

        /// <summary>
        /// Calculates the tip for the inputted total bill and tip percentage,
        ///  and displays it in the output field.
        /// </summary>
        private void CalculateTipButton_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(totalBillTextBox.Text, out var bill))
            {
                calculatedTipTextBox.Text = "Invalid Total Bill Input";
                return;
            }

            if (!double.TryParse(tipPercentageTextBox.Text, out var tip))
            {
                calculatedTipTextBox.Text = "Invalid Tip Percentage Input";
                return;
            }

            calculatedTipTextBox.Text = (bill * (tip/100)).ToString("C2");
        }
    }
}
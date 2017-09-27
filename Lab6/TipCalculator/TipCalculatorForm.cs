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
        /// Calculates the tip at 20% for the inputted total bill and displays it in the output field.
        /// </summary>
        private void CalculateTipButton_Click(object sender, EventArgs e)
        {
            calculatedTipTextBox.Text =
                double.TryParse(totalBillTextBox.Text, out var bill) ? bill * 0.2 + "" : "Invalid Input";
        }
    }
}

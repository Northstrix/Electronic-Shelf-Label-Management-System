using ESL_Management_System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESL_Management_System
{
    public class SelectRecordForm : Form
    {
        private const int formMinWidth = 400;
        private const int formFixedHeight = 118;
        private const int comboBoxHeight = 30;
        private const int buttonHeight = 40;

        private ComboBox comboBox;
        private Button continueButton;
        private Button cancelButton;

        private string[,] dataArray;

        public SelectRecordForm(string[,] array, string form_title, string cont_button_inscription)
        {
            dataArray = array;
            InitializeForm(form_title);
            InitializeControls(cont_button_inscription);
        }

        private void InitializeForm(string form_title)
        {
            this.Text = form_title;
            this.BackColor = ColorTranslator.FromHtml("#142032"); // Color scheme changed

            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(formMinWidth, formFixedHeight); // Set fixed height
            this.MaximumSize = new Size(formMinWidth, formFixedHeight); // Set fixed height
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeControls(string cont_button_inscription)
        {
            TableLayoutPanel mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
            };

            comboBox = new ComboBox
            {
                BackColor = ColorTranslator.FromHtml("#142032"), // Color scheme changed
                ForeColor = ColorTranslator.FromHtml("#EEEEEE"),  // Foreground color set to #EEEEEE
                Dock = DockStyle.Fill,
                Height = comboBoxHeight,
                Font = new Font("Arial", 12, FontStyle.Regular),
                FlatStyle = FlatStyle.Flat,
            };

            // Populate ComboBox with the second column of the array
            for (int i = 0; i < dataArray.GetLength(0); i++)
            {
                comboBox.Items.Add(dataArray[i, 1]);
            }
            if (cont_button_inscription == "Delete")
                continueButton = CreateButton(cont_button_inscription, "#EC0000", "#142032");
            else
                continueButton = CreateButton(cont_button_inscription, "#00BD00", "#142032");
            continueButton.Click += (sender, e) => ContinueButtonClick();

            if (cont_button_inscription == "Delete")
                cancelButton = CreateButton("Cancel", "#00BD00", "#142032");
            else
                cancelButton = CreateButton("Cancel", "#EC0000", "#142032");
            cancelButton.Click += (sender, e) => CancelButtonClick();

            mainTable.Controls.Add(comboBox, 0, 0);
            mainTable.SetColumnSpan(comboBox, 2);
            mainTable.Controls.Add(continueButton, 0, 1);
            mainTable.Controls.Add(cancelButton, 1, 1);

            this.Controls.Add(mainTable);

            this.Resize += (sender, e) =>
            {
                mainTable.Width = this.ClientRectangle.Width;
                mainTable.Height = this.ClientRectangle.Height;
            };
        }

        private Button CreateButton(string buttonText, string backColor, string foreColor)
        {
            Button button = new Button()
            {
                Text = buttonText,
                BackColor = ColorTranslator.FromHtml(backColor),
                ForeColor = ColorTranslator.FromHtml("#EEEEEE"),  // Foreground color set to #EEEEEE
                Font = new Font("Arial", 12, FontStyle.Bold),
                Height = buttonHeight,
                FlatStyle = FlatStyle.Flat,
            };

            return button;
        }

        private void ContinueButtonClick()
        {
            if (comboBox.SelectedIndex != -1)
            {
                Form1.selected_id = dataArray[comboBox.SelectedIndex, 0];
                this.DialogResult = DialogResult.Continue;
            }
            else
            {
                this.DialogResult = DialogResult.Abort;
            }

            this.Close();
        }

        private void CancelButtonClick()
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
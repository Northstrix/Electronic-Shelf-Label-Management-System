using ESL_Management_System;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace ESL_Management_System
{
    public class ViewESLForm : Form
    {
        private const int formWidth = 760;
        private const int formHeight = 540;
        private const int fixedColumnWidth = 750;
        private const int emptyColumnWidth = 40;
        private const int increasedHeight = 12;
        private const int buttonHeight = 40;
        private const int reducedButtonHeight = 36;
        private string recId; // Record ID to be edited

        public ViewESLForm(string recId, string deviceName, string encryptionKey, string udpPort, string ipAddress, string resltn, Bitmap image)
        {
            this.recId = recId;
            InitializeForm(deviceName, encryptionKey, udpPort, ipAddress, resltn, image);
        }

        private void InitializeForm(string deviceName, string encryptionKey, string udpPort, string ipAddress, string resltn, Bitmap image)
        {
            this.Size = new Size(formWidth, formHeight);
            this.Text = "View ESL";
            this.BackColor = ColorTranslator.FromHtml("#131313"); // Set background color

            TableLayoutPanel mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                ColumnStyles =
            {
                new ColumnStyle(SizeType.Percent, 100),
            },
            };

            this.Controls.Add(mainTable);

            Button okButton = new Button()
            {
                Text = "OK",
                BackColor = ColorTranslator.FromHtml("#303030"),
                ForeColor = ColorTranslator.FromHtml("#EEEEEE"),
                Font = new Font("Arial", 12, FontStyle.Bold),
                Height = buttonHeight,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Margin = new Padding(0, 0, 8, 0),
                Anchor = AnchorStyles.Right
            };

            mainTable.Controls.Add(CreateTextBox("Device Name: " + deviceName, 1), 0, 0);
            mainTable.Controls.Add(CreateTextBox("Encryption Key: " + encryptionKey, 0), 0, 1);
            mainTable.Controls.Add(CreateTextBox("UDP Port: " + udpPort, 1), 0, 2);
            mainTable.Controls.Add(CreateTextBox("IP Address: " + ipAddress, 0), 0, 3);
            mainTable.Controls.Add(CreateTextBox("Resolution: " + resltn, 1), 0, 4);
            int pb_w = 0;
            int pb_h = 0;
            string[] parts = resltn.Split('x');
            if (int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height))
            {
                pb_w = width;
                pb_h = height;
            }
            PictureBox pictureBox = new PictureBox
            {
                Image = image,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Height = pb_h,
                Width = pb_w,
                Margin = new Padding(10, 10, 10, 10)
            };
            mainTable.Controls.Add(pictureBox, 0, 5);
            mainTable.Controls.Add(okButton, 0, 6);

            okButton.Click += (sender, e) =>
            {
                this.Close();
            };

            this.Resize += (sender, e) =>
            {
                okButton.Location = new Point((mainTable.Width - okButton.Width) / 2, mainTable.Height - okButton.Height - 20);
            };
        }

        private TextBox CreateTextBox(string text, int row)
        {
            TextBox textBox = new TextBox()
            {
                Text = text,
                BackColor = ColorTranslator.FromHtml("#131313"),
                ForeColor = row % 2 == 0 ? ColorTranslator.FromHtml("#6301A5") : ColorTranslator.FromHtml("#eeeeee"),
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Left,
                ReadOnly = true,
                Width = fixedColumnWidth,
                Height = reducedButtonHeight,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(10, 0, 0, 5)
            };

            return textBox;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESL_Management_System
{
    using Org.BouncyCastle.Ocsp;
    using System;
    using System.Data.SQLite;
    using System.Drawing;
    using System.Windows.Forms;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
    using System.Xml.Linq;
    using System.Security.Cryptography;
    using Org.BouncyCastle.Tsp;

    public class AddESL : Form
    {
        private const int formWidth = 540;
        private const int formHeight = 310;
        private const int fixedColumnWidth = 160;
        private const int emptyColumnWidth = 40;
        private const int increasedHeight = 12;
        private const int buttonHeight = 40;
        private const int reducedButtonHeight = 36;

        public AddESL()
        {
            InitializeForm();
        }

        private void InitializeForm()
        {
            this.Size = new Size(formWidth, formHeight);
            this.Text = "Add ESL";
            this.BackColor = ColorTranslator.FromHtml("#3188dc");

            TableLayoutPanel mainTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 7,
                ColumnStyles =
                {
                    new ColumnStyle(SizeType.Absolute, fixedColumnWidth),
                    new ColumnStyle(SizeType.Percent, 100),
                    new ColumnStyle(SizeType.Absolute, emptyColumnWidth)
                },
                RowStyles =
                {
                    new RowStyle(SizeType.AutoSize, increasedHeight),
                    new RowStyle(SizeType.AutoSize, increasedHeight),
                    new RowStyle(SizeType.AutoSize, increasedHeight),
                    new RowStyle(SizeType.AutoSize, increasedHeight),
                    new RowStyle(SizeType.AutoSize, increasedHeight),
                    new RowStyle(SizeType.AutoSize, reducedButtonHeight),
                    new RowStyle(SizeType.AutoSize, buttonHeight)
                }
            };

            this.Controls.Add(mainTable);
            mainTable.Controls.Add(CreateLabel("Device Name:", 1), 0, 0);
            mainTable.Controls.Add(CreateTextBox(1), 1, 0);

            mainTable.Controls.Add(CreateLabel("Encryption Key:", 0), 0, 1);
            mainTable.Controls.Add(CreateTextBox(0), 1, 1);

            mainTable.Controls.Add(CreateLabel("UDP Port:", 1), 0, 2);
            mainTable.Controls.Add(CreateTextBox(1), 1, 2);

            mainTable.Controls.Add(CreateLabel("IP Address:", 0), 0, 3);
            mainTable.Controls.Add(CreateTextBox(0), 1, 3);

            System.Windows.Forms.Label resolutionLabel = new System.Windows.Forms.Label()
            {
                Text = "Resolution:",
                BackColor = ColorTranslator.FromHtml("#3188dc"),
                ForeColor = ColorTranslator.FromHtml("#eeeeee"),
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                Width = fixedColumnWidth,
                Height = reducedButtonHeight
            };

            ComboBox resolutionComboBox = new ComboBox()
            {
                BackColor = ColorTranslator.FromHtml("#202020"),
                ForeColor = ColorTranslator.FromHtml("#eeeeee"),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                Height = reducedButtonHeight
            };

            int nonBlankLinesCount = 0;

            // Count the number of non-blank lines and print each non-blank line
            using (StreamReader reader = new StreamReader(Form1.resolutins_file_path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        resolutionComboBox.Items.Add(line);
                    }
                }
            }

            mainTable.Controls.Add(resolutionLabel, 0, 5);
            mainTable.Controls.Add(resolutionComboBox, 1, 5);

            TableLayoutPanel buttonTable = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Width = 200,
                ColumnCount = 2,
                ColumnStyles =
                {
                    new ColumnStyle(SizeType.Percent, 50),
                    new ColumnStyle(SizeType.Percent, 50)
                }
            };

            Button addRecordButton = new Button()
            {
                Text = "Add",
                BackColor = ColorTranslator.FromHtml("#1BCA00"),
                ForeColor = ColorTranslator.FromHtml("#202020"),
                Font = new Font("Arial", 12, FontStyle.Bold),
                Height = buttonHeight,
                Margin = new Padding(0, 0, 8, 0),
                Anchor = AnchorStyles.Right
            };

            Button cancelButton = new Button()
            {
                Text = "Cancel",
                BackColor = ColorTranslator.FromHtml("#EE0011"),
                ForeColor = ColorTranslator.FromHtml("#eeeeee"),
                Font = new Font("Arial", 12, FontStyle.Bold),
                Height = buttonHeight,
                Margin = new Padding(8, 0, 0, 0),
                Anchor = AnchorStyles.Left
            };

            buttonTable.Controls.Add(addRecordButton, 0, 0);
            buttonTable.Controls.Add(cancelButton, 1, 0);

            mainTable.Controls.Add(buttonTable, 0, 6);
            mainTable.SetColumnSpan(buttonTable, 3);

            addRecordButton.Click += (sender, e) =>
            {
                if (resolutionComboBox.SelectedItem != null)
                {

                    // Retrieve text from each entry
                    string device_name = ((TextBox)mainTable.GetControlFromPosition(1, 0)).Text;
                    string encryption_key = ((TextBox)mainTable.GetControlFromPosition(1, 1)).Text;
                    string udp_port = ((TextBox)mainTable.GetControlFromPosition(1, 2)).Text.Replace("0x", "");
                    string ip_addr = ((TextBox)mainTable.GetControlFromPosition(1, 3)).Text;
                    if (Form1.IsValidHexadecimalPort(udp_port))
                    {
                        if (Form1.IsValidIpAddress(ip_addr))
                        {
                            if (Form1.IsValidEncryptionKey(encryption_key))
                            {
                                AddESLRecord(device_name, encryption_key, udp_port, ip_addr, resolutionComboBox.SelectedItem.ToString());
                                this.Close();
                            }
                            else
                                Form1.ShowErrorMessageBox("Invalid Encryption Key", "Please Enter a Valid Encryption Key And Try Again");
                        }
                        else
                            Form1.ShowErrorMessageBox("Invalid IP Address", "Please Enter a Valid IP Address And Try Again");
                    }
                    else
                        Form1.ShowErrorMessageBox("Invalid UDP Port", "Please Enter a Valid UDP Port And Try Again");
                }
                else
                    Form1.ShowErrorMessageBox("Can't Add Device Without Screen Resolution", "Select the Screen Resolution And Try Again");
            };

            cancelButton.Click += (sender, e) =>
            {
                Form1.ShowLeashmoreMessageBox("Operation Was Cancelled By User");
                this.Close();
            };

            this.Resize += (sender, e) =>
            {
                buttonTable.Location = new Point((mainTable.Width - buttonTable.Width) / 2, mainTable.Height - buttonTable.Height - 20);
            };

        }
        private System.Windows.Forms.Label CreateLabel(string labelText, int row)
        {
            System.Windows.Forms.Label label = new System.Windows.Forms.Label()
            {
                Text = labelText,
                BackColor = ColorTranslator.FromHtml("#3188dc"),
                ForeColor = row % 2 == 0 ? ColorTranslator.FromHtml("#202020") : ColorTranslator.FromHtml("#eeeeee"),
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                Width = fixedColumnWidth,
                Height = reducedButtonHeight
            };

            return label;
        }

        private System.Windows.Forms.TextBox CreateTextBox(int row)
        {
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox()
            {
                BackColor = row % 2 == 0 ? ColorTranslator.FromHtml("#202020") : ColorTranslator.FromHtml("#eeeeee"),
                ForeColor = row % 2 == 0 ? ColorTranslator.FromHtml("#eeeeee") : ColorTranslator.FromHtml("#202020"),
                Dock = DockStyle.Fill,
                Height = increasedHeight,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };

            return textBox;
        }

        private static string GenerateRandomString(int length)
        {
            const string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                char[] chars = new char[length];
                for (int i = 0; i < length; i++)
                {
                    int index = randomBytes[i] % allowedChars.Length;
                    chars[i] = allowedChars[index];
                }

                return new string(chars);
            }
        }
        static void AddESLRecord(string device_name, string encryption_key, string udp_port, string ip_addr, string resolution)
        {
            string recId = GenerateRandomString(10); // Generate random ID
            while (Form1.CheckIfRecordExists(recId, "ESL") == true) // Check if the record with that ID is already in the database. If true, then keep generating new IDs until DB tells that record with such ID isn't present
                recId = GenerateRandomString(10); // If record with the generated ID already exists, then generate new ID and check again
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(Form1.SQLiteconnectionString))
                {
                    connection.Open();
                    string commandText = $@"INSERT INTO ESL 
                                       (Rec_id, device_name, encryption_key, udp_port, ip_addr, resolution, image) 
                                       VALUES 
                                       (@RecId, @device_name, @encryption_key, @udp_port, @ip_addr, @resolution, @image)";

                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@RecId", recId);
                        command.Parameters.AddWithValue("@device_name", Form1.Encrypt_string_with_aes_in_cbc(device_name));
                        command.Parameters.AddWithValue("@encryption_key", Form1.Encrypt_string_with_aes_in_cbc(encryption_key));
                        command.Parameters.AddWithValue("@udp_port", Form1.Encrypt_string_with_aes_in_cbc(udp_port));
                        command.Parameters.AddWithValue("@ip_addr", Form1.Encrypt_string_with_aes_in_cbc(ip_addr));
                        command.Parameters.AddWithValue("@resolution", Form1.Encrypt_string_with_aes_in_cbc(resolution));
                        command.Parameters.AddWithValue("@image", "-1");

                        // Execute the command
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Form1.ShowLeashmoreMessageBox("Record Added Successfully!");
                        }
                        else
                        {
                            Form1.ShowErrorMessageBox("Failed to Add Record", "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Form1.ShowErrorMessageBox("Something went wrong with the database", $"Error: {ex.Message}");
            }
        }
    }
}
using ESL_Management_System;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace ESL_Management_System
{
    public class EditESLForm : Form
    {
        private const int formWidth = 760;
        private const int formHeight = 340;
        private const int fixedColumnWidth = 250;
        private const int emptyColumnWidth = 40;
        private const int increasedHeight = 12;
        private const int buttonHeight = 40;
        private const int reducedButtonHeight = 36;
        private string recId; // Record ID to be edited

        public EditESLForm(string recId, string deviceName, string encryptionKey, string udpPort, string ipAddress, string resltn)
        {
            this.recId = recId;
            InitializeForm(deviceName, encryptionKey, udpPort, ipAddress, resltn);
        }

        private void InitializeForm(string deviceName, string encryptionKey, string udpPort, string ipAddress, string resltn)
        {
            this.Size = new Size(formWidth, formHeight);
            this.Text = "Edit ESL";
            this.BackColor = ColorTranslator.FromHtml("#131313"); // Set background color

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
            mainTable.Controls.Add(CreateTextBox(1, deviceName), 1, 0);

            mainTable.Controls.Add(CreateLabel("Encryption Key:", 0), 0, 1);
            mainTable.Controls.Add(CreateTextBox(0, encryptionKey), 1, 1);

            mainTable.Controls.Add(CreateLabel("UDP Port:", 1), 0, 2);
            mainTable.Controls.Add(CreateTextBox(1, udpPort), 1, 2);

            mainTable.Controls.Add(CreateLabel("IP Address:", 0), 0, 3);
            mainTable.Controls.Add(CreateTextBox(0, ipAddress), 1, 3);

            mainTable.Controls.Add(CreateLabel("Old Resolution: " + resltn, 1), 0, 4);

            System.Windows.Forms.Label resolutionLabel = new System.Windows.Forms.Label()
            {
                Text = "New Resolution:",
                BackColor = ColorTranslator.FromHtml("#131313"),
                ForeColor = ColorTranslator.FromHtml("#F92C5D"),
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                Width = fixedColumnWidth,
                Height = reducedButtonHeight
            };

            ComboBox resolutionComboBox = new ComboBox()
            {
                BackColor = ColorTranslator.FromHtml("#F92C5D"),
                ForeColor = ColorTranslator.FromHtml("#eeeeee"),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill,
                Height = reducedButtonHeight
            };

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

            Button updateRecordButton = new Button()
            {
                Text = "Update",
                BackColor = ColorTranslator.FromHtml("#EEEEEE"),
                ForeColor = ColorTranslator.FromHtml("#131313"),
                Font = new Font("Arial", 12, FontStyle.Bold),
                Height = buttonHeight,
                Margin = new Padding(0, 0, 8, 0),
                Anchor = AnchorStyles.Right
            };

            Button cancelButton = new Button()
            {
                Text = "Cancel",
                BackColor = ColorTranslator.FromHtml("#F92C5D"),
                ForeColor = ColorTranslator.FromHtml("#eeeeee"),
                Font = new Font("Arial", 12, FontStyle.Bold),
                Height = buttonHeight,
                Margin = new Padding(8, 0, 0, 0),
                Anchor = AnchorStyles.Left
            };

            buttonTable.Controls.Add(updateRecordButton, 0, 0);
            buttonTable.Controls.Add(cancelButton, 1, 0);

            mainTable.Controls.Add(buttonTable, 0, 6);
            mainTable.SetColumnSpan(buttonTable, 3);

            updateRecordButton.Click += (sender, e) =>
            {
                if (resolutionComboBox.SelectedItem != null)
                {

                    // Retrieve text from each entry
                    string editedDeviceName = ((TextBox)mainTable.GetControlFromPosition(1, 0)).Text;
                    string editedEncryptionKey = ((TextBox)mainTable.GetControlFromPosition(1, 1)).Text;
                    string editedUdpPort = ((TextBox)mainTable.GetControlFromPosition(1, 2)).Text.Replace("0x", "");
                    string editedIpAddress = ((TextBox)mainTable.GetControlFromPosition(1, 3)).Text;
                    string editedres = resolutionComboBox.SelectedItem.ToString();
                    if (Form1.IsValidHexadecimalPort(editedUdpPort))
                    {
                        if (Form1.IsValidIpAddress(editedIpAddress))
                        {
                            if (Form1.IsValidEncryptionKey(editedEncryptionKey))
                            {
                                UpdateESLRecord(recId, editedDeviceName, editedEncryptionKey, editedUdpPort, editedIpAddress, editedres);
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
                BackColor = ColorTranslator.FromHtml("#131313"),
                ForeColor = row % 2 == 0 ? ColorTranslator.FromHtml("#F92C5D") : ColorTranslator.FromHtml("#eeeeee"),
                Font = new Font("Arial", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                Width = fixedColumnWidth,
                Height = reducedButtonHeight
            };

            return label;
        }

        private System.Windows.Forms.TextBox CreateTextBox(int row, string defaultValue)
        {
            System.Windows.Forms.TextBox textBox = new System.Windows.Forms.TextBox()
            {
                BackColor = row % 2 == 0 ? ColorTranslator.FromHtml("#F92C5D") : ColorTranslator.FromHtml("#eeeeee"),
                ForeColor = row % 2 == 0 ? ColorTranslator.FromHtml("#eeeeee") : ColorTranslator.FromHtml("#F92C5D"),
                Dock = DockStyle.Fill,
                Height = increasedHeight,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Text = defaultValue
            };

            return textBox;
        }

        private void UpdateESLRecord(string recId, string deviceName, string encryptionKey, string udpPort, string ipAddress, string resolution)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(Form1.SQLiteconnectionString))
                {
                    connection.Open();

                    string commandText = $@"UPDATE ESL 
                                       SET 
                                       device_name = @DeviceName, 
                                       encryption_key = @EncryptionKey, 
                                       udp_port = @UdpPort, 
                                       ip_addr = @IpAddress, 
                                       resolution = @Resolution
                                       WHERE Rec_id = @RecId";

                    using (SQLiteCommand command = new SQLiteCommand(commandText, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@RecId", recId);
                        command.Parameters.AddWithValue("@DeviceName", Form1.Encrypt_string_with_aes_in_cbc(deviceName));
                        command.Parameters.AddWithValue("@EncryptionKey", Form1.Encrypt_string_with_aes_in_cbc(encryptionKey));
                        command.Parameters.AddWithValue("@UdpPort", Form1.Encrypt_string_with_aes_in_cbc(udpPort));
                        command.Parameters.AddWithValue("@IpAddress", Form1.Encrypt_string_with_aes_in_cbc(ipAddress));
                        command.Parameters.AddWithValue("@Resolution", Form1.Encrypt_string_with_aes_in_cbc(resolution));

                        // Execute the command
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Form1.ShowLeashmoreMessageBox("Record Updated Successfully!");
                        }
                        else
                        {
                            Form1.ShowErrorMessageBox("Failed to Update Record", "");
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

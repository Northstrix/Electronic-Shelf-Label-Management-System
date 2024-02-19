/*
DIY Electronic Shelf Label Management System
Distributed under the MIT License
© Copyright Maxim Bortnikov 2024
For more information please visit
https://sourceforge.net/projects/esl-management-system/
https://github.com/Northstrix/Electronic-Shelf-Label-Management-System
*/
using System;
using Org.BouncyCastle.Ocsp;
using System.Data.SQLite;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using Org.BouncyCastle.Crypto.Engines;
using System.Threading;
using System.IO;
using System.Reflection.Metadata;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Windows.Forms;

namespace ESL_Management_System
{
    public partial class Form1 : Form
    {
        protected static byte[] encryption_key = new byte[16];
        protected static byte[] verification_key = new byte[16];
        protected static byte[] decrypted_tag = new byte[32];
        public static string selected_id;
        private static string[] ESL_ids;
        public const string SQLiteconnectionString = "Data Source=esl_database.db;Version=3;";
        public static long selected_row;
        public const string resolutins_file_path = "resolutions.txt";
        private bool uploading = false;
        private string image_encryption_key;
        private string ipAddress;
        private int current_udp_port;
        protected static byte[] serp_key = { 128, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        protected static int decract;
        protected static byte[] array_for_CBC_mode = new byte[16];
        private static List<byte> globalByteList = new List<byte>();
        private static List<byte> full_image = new List<byte>();
        private static string current_id;
        private int pb_w;
        private int pb_h;
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            create_required_tables_if_not_exist();
            string encr_bash_of_mp = get_encr_hash_of_password_from_db();
            if (encr_bash_of_mp == "-1")
                set_password();
            else
                ask_user_for_password(encr_bash_of_mp);
            check_res_setups();
            DisplayESLInformation();
        }

        private static void check_res_setups()
        {
            if (!File.Exists(resolutins_file_path))
            {
                using (StreamWriter sw = File.CreateText(resolutins_file_path))
                {
                    sw.WriteLine("320x240");
                    sw.WriteLine("128x128");
                }
            }
        }

        private static string get_encr_hash_of_password_from_db()
        {
            StringBuilder enc_hash_to_ret = new StringBuilder();
            using (SQLiteConnection connection = new SQLiteConnection(SQLiteconnectionString))
            {
                connection.Open();
                int recId = 1;
                string query = $"SELECT * FROM Unlock WHERE Rec_id = {recId}";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Record found, you can access the values using reader["ColumnName"]
                            int foundRecId = reader.GetInt32(reader.GetOrdinal("Rec_id"));
                            enc_hash_to_ret.Append(reader.GetString(reader.GetOrdinal("Encrypted_hash_of_the_password")));
                        }
                        else
                        {
                            enc_hash_to_ret.Append("-1");
                        }
                    }
                }

                connection.Close();
            }
            return enc_hash_to_ret.ToString();
        }

        static void CenterLabelText(Label label, Control parent)
        {
            // Center the text horizontally in the label
            label.Location = new Point((parent.ClientSize.Width - label.Width) / 2, label.Location.Y);
        }

        public static bool CheckIfRecordExists(string recId, string table)
        {
            using (SQLiteConnection connection = new SQLiteConnection(SQLiteconnectionString))
            {
                connection.Open();

                try
                {
                    string query = $"SELECT 1 FROM {table} WHERE Rec_id = '{recId}' LIMIT 1";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        object result = command.ExecuteScalar();
                        return result != null && result != DBNull.Value;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private static string get_data_from_entry(string prompt, bool displayStars)
        {
            Form customForm = new Form
            {
                Text = "ESL Management System",
                Size = new Size(320, 170),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = ColorTranslator.FromHtml("#7B08A5")
            };

            // Create label
            Label label = new Label
            {
                Text = prompt,
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customForm.Controls.Add(label);

            // Create text field
            TextBox textField = new TextBox
            {
                Size = new Size(200, 30),
                Location = new Point((customForm.ClientSize.Width - 200) / 2, label.Bottom + 10),
                Font = new Font("Segoe UI", 12),
                BackColor = ColorTranslator.FromHtml("#2C2C2C"),
                ForeColor = ColorTranslator.FromHtml("#E4E3DF")
            };
            customForm.Controls.Add(textField);

            // If displayStars is true, set the UseSystemPasswordChar property to true
            if (displayStars)
            {
                textField.UseSystemPasswordChar = true;
            }

            // Create Continue button
            Button continueButton = new Button
            {
                Text = "Continue",
                Size = new Size(120, 38),
                BackColor = ColorTranslator.FromHtml("#7B08A5"),
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"),
                DialogResult = DialogResult.Yes,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 3, BorderColor = ColorTranslator.FromHtml("#E4E3DF") },
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            customForm.Controls.Add(continueButton);

            label.Location = new Point((customForm.ClientSize.Width) / 2, +10);
            textField.Location = new Point((customForm.ClientSize.Width - textField.Width) / 2, label.Bottom + 10);
            CenterLabelText(label, customForm);
            continueButton.Location = new Point((customForm.ClientSize.Width - continueButton.Width) / 2, textField.Bottom + 10);

            // Handle Resize event to adjust positions dynamically
            customForm.Resize += (sender, e) =>
            {
                textField.Location = new Point((customForm.ClientSize.Width - textField.Width) / 2, label.Bottom + 10);
                CenterLabelText(label, customForm);
                continueButton.Location = new Point((customForm.ClientSize.Width - continueButton.Width) / 2, textField.Bottom + 20);
            };

            textField.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    customForm.DialogResult = DialogResult.Yes;
                    customForm.Close();
                }
            };

            customForm.KeyPreview = true;
            customForm.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    customForm.DialogResult = DialogResult.Yes;
                    customForm.Close();
                }
            };

            // Show the form
            DialogResult result = customForm.ShowDialog();

            return textField.Text;
        }

        private void set_password()
        {
            string user_password = get_data_from_entry("Set Your Password", false);
            string hashed_password = HashStringWithSHA512(user_password, 91 * CalculateAsciiSum(HashStringWithSHA512(user_password, 9342)));
            byte[] source = StringToByteArray(hashed_password);
            for (int i = 0; i < 16; i++)
            {
                encryption_key[i] = source[i];
                verification_key[i] = source[i + 16];
            }
            byte[] to_be_hmaced = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                to_be_hmaced[i] = source[i + 32];
            }


            using (SQLiteConnection connection = new SQLiteConnection(SQLiteconnectionString))
            {
                connection.Open();

                try
                {
                    string query = $"INSERT INTO Unlock (Rec_id, Encrypted_hash_of_the_password) VALUES (1, '" + Encrypt_hash_with_aes_in_cbc(CalculateHMACSHA256(to_be_hmaced)) + "')";

                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        ShowLeashmoreMessageBox("Password Set Successfully");
                    }
                }
                catch (Exception ex)
                {
                    ShowErrorMessageBox("Something went wrong with the database", $"Error: {ex.Message}");
                }
                finally
                {
                    connection.Close();
                }
            }
            set_unlocked_status();
        }

        public static void ShowErrorMessageBox(string line1, string line2)
        {
            Form customMessageBox = new Form
            {
                Text = "ESL Management System Error",
                Size = new Size(640, 162),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(171, 49, 18)
            };

            // Create label for the first line
            Label label1 = new Label
            {
                Text = line1,
                ForeColor = Color.FromArgb(238, 238, 238),
                Font = new Font("Arial", 16, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customMessageBox.Controls.Add(label1);

            // Create label for the second line
            Label label2 = new Label
            {
                Text = line2,
                ForeColor = Color.FromArgb(238, 238, 238),
                Font = new Font("Arial", 14),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customMessageBox.Controls.Add(label2);

            // Create OK button
            Button okButton = new Button
            {
                Text = "OK",
                Size = new Size(60, 30),
                BackColor = Color.FromArgb(32, 32, 32), // "#202020"
                ForeColor = Color.FromArgb(238, 238, 238), // "#EEEEEE"
                DialogResult = DialogResult.OK,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            customMessageBox.Controls.Add(okButton);

            label1.Location = new Point((customMessageBox.ClientSize.Width) / 2, +10);
            label2.Location = new Point((customMessageBox.ClientSize.Width) / 2, label1.Bottom + 10);
            okButton.Location = new Point((customMessageBox.ClientSize.Width - okButton.Width) / 2, label2.Bottom + 12);

            CenterLabelText(label1, customMessageBox);
            CenterLabelText(label2, customMessageBox);

            // Handle Resize event to adjust positions dynamically
            customMessageBox.Resize += (sender, e) =>
            {
                CenterLabelText(label1, customMessageBox);
                CenterLabelText(label2, customMessageBox);
                okButton.Location = new Point((customMessageBox.ClientSize.Width - okButton.Width) / 2, label2.Bottom + 20);
            };

            // Show the message box
            customMessageBox.ShowDialog();
        }


        public static void ShowLeashmoreMessageBox(string line)
        {
            Form customMessageBox = new Form
            {
                Text = "Message",
                Size = new Size(700, 162),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = ColorTranslator.FromHtml("#08162F")
            };

            // Create label for the first line
            Label label1 = new Label
            {
                Text = line,
                ForeColor = Color.FromArgb(238, 238, 238),
                Font = new Font("Arial", 14, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customMessageBox.Controls.Add(label1);

            // Create OK button
            Button okButton = new Button
            {
                Text = "OK",
                Size = new Size(96, 32),
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(32, 32, 32),
                BackColor = Color.FromArgb(198, 198, 198),
                DialogResult = DialogResult.OK,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            customMessageBox.Controls.Add(okButton);

            label1.Location = new Point((customMessageBox.ClientSize.Width) / 2, 12);
            okButton.Location = new Point((customMessageBox.ClientSize.Width - okButton.Width) / 2, label1.Bottom + 20);

            CenterLabelText(label1, customMessageBox);

            // Handle Resize event to adjust positions dynamically
            customMessageBox.Resize += (sender, e) =>
            {
                CenterLabelText(label1, customMessageBox);
                okButton.Location = new Point((customMessageBox.ClientSize.Width - okButton.Width) / 2, label1.Bottom + 20);
            };

            // Show the message box
            customMessageBox.ShowDialog();
        }

        public static DialogResult ShowOrangeMessageBox(string line1)
        {
            Form customMessageBox = new Form
            {
                Text = "ESL Management System Warning",
                Size = new Size(840, 162),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = Color.FromArgb(237, 137, 40)
            };

            // Create label for the first line
            Label label1 = new Label
            {
                Text = line1,
                ForeColor = Color.FromArgb(238, 238, 238),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customMessageBox.Controls.Add(label1);

            // Create label for the second line
            Label label2 = new Label
            {
                Text = "Would you like to continue?",
                ForeColor = Color.FromArgb(238, 238, 238),
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customMessageBox.Controls.Add(label2);

            // Create Yes button
            Button yesButton = new Button
            {
                Text = "Yes",
                Size = new Size(60, 30),
                BackColor = Color.FromArgb(32, 32, 32), // "#202020"
                ForeColor = Color.FromArgb(238, 238, 238), // "#EEEEEE"
                DialogResult = DialogResult.Yes,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            customMessageBox.Controls.Add(yesButton);

            // Create No button
            Button noButton = new Button
            {
                Text = "No",
                Size = new Size(60, 30),
                BackColor = Color.FromArgb(32, 32, 32), // "#202020"
                ForeColor = Color.FromArgb(238, 238, 238), // "#EEEEEE"
                DialogResult = DialogResult.No,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 }
            };
            customMessageBox.Controls.Add(noButton);

            label1.Location = new Point((customMessageBox.ClientSize.Width) / 2, +10);
            label2.Location = new Point((customMessageBox.ClientSize.Width) / 2, label1.Bottom + 10);

            int buttonMargin = 30;
            int buttonWidth = (customMessageBox.ClientSize.Width - 3 * buttonMargin) / 2;

            yesButton.Size = new Size(buttonWidth, 30);
            noButton.Size = new Size(buttonWidth, 30);

            yesButton.Location = new Point(buttonMargin, label2.Bottom + 20);
            noButton.Location = new Point(yesButton.Right + buttonMargin, label2.Bottom + 20);

            CenterLabelText(label1, customMessageBox);
            CenterLabelText(label2, customMessageBox);

            // Handle Resize event to adjust positions dynamically
            customMessageBox.Resize += (sender, e) =>
            {
                CenterLabelText(label1, customMessageBox);
                CenterLabelText(label2, customMessageBox);

                buttonWidth = (customMessageBox.ClientSize.Width - 3 * buttonMargin) / 2;
                yesButton.Size = new Size(buttonWidth, 30);
                noButton.Size = new Size(buttonWidth, 30);

                yesButton.Location = new Point(buttonMargin, label2.Bottom + 20);
                noButton.Location = new Point(yesButton.Right + buttonMargin, label2.Bottom + 20);
            };

            // Show the message box
            DialogResult result = customMessageBox.ShowDialog();

            return result;
        }

        private void ask_user_for_password(string encr_bash_of_mp)
        {
            string user_password = get_data_from_entry("Enter Your Password", true);
            string hashed_password = HashStringWithSHA512(user_password, 91 * CalculateAsciiSum(HashStringWithSHA512(user_password, 9342)));
            byte[] source = StringToByteArray(hashed_password);
            for (int i = 0; i < 16; i++)
            {
                encryption_key[i] = source[i];
                verification_key[i] = source[i + 16];
            }
            byte[] to_be_hmaced = new byte[32];
            for (int i = 0; i < 32; i++)
            {
                to_be_hmaced[i] = source[i + 32];
            }

            if (!Decrypt_hash_with_aes_in_cbc(encr_bash_of_mp).SequenceEqual(CalculateHMACSHA256(to_be_hmaced)))
            {
                ShowErrorMessageBox("Wrong Password", "Please, Try Again");
                ask_user_for_password(encr_bash_of_mp);
            }
            else
                set_unlocked_status();
        }

        private static string Extract_value_from_record(string table_name, string Record_id, string column_name)
        {
            string extr_value = string.Empty;

            using (SQLiteConnection connection = new SQLiteConnection(SQLiteconnectionString))
            {
                connection.Open();
                string query = $"SELECT {column_name} FROM {table_name} WHERE Rec_id = @Record_id";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Record_id", Record_id);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            extr_value = reader[column_name].ToString();
                        }
                    }
                }
                connection.Close();
            }
            return extr_value;
        }

        public static int CalculateAsciiSum(string input)
        {
            int sum = 0;

            foreach (char character in input)
            {
                sum += (int)character;
            }

            return sum;
        }


        public static byte[] StringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] byteArray = new byte[length / 2];

            for (int i = 0; i < length; i += 2)
            {
                byteArray[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return byteArray;
        }

        private static byte[] CalculateHMACSHA256(byte[] data)
        {
            using (HMACSHA256 hmac = new HMACSHA256(verification_key))
            {
                return hmac.ComputeHash(data);
            }
        }

        public static string HashStringWithSHA512(string input, int iterations)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] data = Encoding.UTF8.GetBytes(input);

                for (int i = 0; i < iterations; i++)
                {
                    data = sha512.ComputeHash(data);
                }

                // Convert the final hash to a hexadecimal string
                StringBuilder builder = new StringBuilder();
                foreach (byte b in data)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private static void create_required_tables_if_not_exist()
        {
            using (SQLiteConnection connection = new SQLiteConnection(SQLiteconnectionString))
            {
                connection.Open();

                CreateTableIfNotExist(connection, "Unlock", "Rec_id INTEGER, Encrypted_hash_of_the_password TEXT");
                CreateTableIfNotExist(connection, "ESL", "Rec_id TEXT, device_name Text, encryption_key Text, udp_port Text, ip_addr Text, resolution Text, image Text");

                connection.Close();
            }
        }

        private static void CreateTableIfNotExist(SQLiteConnection connection, string tableName, string columns)
        {
            using (SQLiteCommand command = new SQLiteCommand($"CREATE TABLE IF NOT EXISTS {tableName} ({columns});", connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static byte[] GenerateRandomByteArray(int length)
        {
            byte[] randomBytes = new byte[length];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            return randomBytes;
        }

        public static string Encrypt_string_with_aes_in_cbc(string plaintext)
        {
            byte[] input = Encoding.ASCII.GetBytes(plaintext);
            byte[] iv = GenerateRandomByteArray(16);
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
            cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", encryption_key), iv));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CipherStream cipherStream = new CipherStream(memoryStream, null, cipher))
                {
                    cipherStream.Write(input, 0, input.Length);
                }

                return Encrypt_hash_with_aes_in_cbc(CalculateHMACSHA256(input)) + BitConverter.ToString(EncryptAES(iv)).Replace("-", "") + BitConverter.ToString(memoryStream.ToArray()).Replace("-", "");
            }
        }

        private static string Decrypt_string_with_aes_in_cbc(string ciphertext)
        {
            try
            {
                decrypted_tag = Decrypt_hash_with_aes_in_cbc(ciphertext.Substring(0, 96));
                byte[] encrypted_iv = StringToByteArray(ciphertext.Substring(96, 32));
                byte[] iv = DecryptAES(encrypted_iv);
                byte[] input = StringToByteArray(ciphertext.Substring(128));
                IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
                cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", encryption_key), iv));

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CipherStream cipherStream = new CipherStream(memoryStream, null, cipher))
                    {
                        cipherStream.Write(input, 0, input.Length);
                    }

                    if (!CalculateHMACSHA256(memoryStream.ToArray()).AsSpan().SequenceEqual(decrypted_tag))
                        ShowErrorMessageBox("Failed to Verify Integrity/Authenticity of a Ciphertext", "Decrypted and Computed Tags Don't Match");


                    return Encoding.ASCII.GetString(memoryStream.ToArray());
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox("Failed to Decrypt Ciphertext", "Error: " + ex.Message);
                return "\"Decryption Failed\"";

            }
        }

        private static string Encrypt_hash_with_aes_in_cbc(byte[] input)
        {
            byte[] iv = GenerateRandomByteArray(16);
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CBC/NoPadding");
            cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", encryption_key), iv));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CipherStream cipherStream = new CipherStream(memoryStream, null, cipher))
                {
                    cipherStream.Write(input, 0, input.Length);
                }

                return BitConverter.ToString(EncryptAES(iv)).Replace("-", "") + BitConverter.ToString(memoryStream.ToArray()).Replace("-", "");
            }
        }

        private static byte[] Decrypt_hash_with_aes_in_cbc(string ciphertext)
        {
            byte[] encrypted_iv = StringToByteArray(ciphertext.Substring(0, 32));
            byte[] iv = DecryptAES(encrypted_iv);
            byte[] input = StringToByteArray(ciphertext.Substring(32));
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CBC/NoPadding");
            cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", encryption_key), iv));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CipherStream cipherStream = new CipherStream(memoryStream, null, cipher))
                {
                    cipherStream.Write(input, 0, input.Length);
                }

                return memoryStream.ToArray();
            }
        }

        // Image Encryption/Decryption (Below)

        public static string EncryptImageWithAESInCBC(byte[] input)
        {
            byte[] iv = GenerateRandomByteArray(16);
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
            cipher.Init(true, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", encryption_key), iv));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CipherStream cipherStream = new CipherStream(memoryStream, null, cipher))
                {
                    cipherStream.Write(input, 0, input.Length);
                }

                byte[] encryptedHash = StringToByteArray(Encrypt_hash_with_aes_in_cbc(CalculateHMACSHA256(input)));
                byte[] encryptedIV = EncryptAES(iv);
                byte[] encryptedContent = memoryStream.ToArray();

                return BitConverter.ToString(CombineByteArrays(encryptedHash, encryptedIV, encryptedContent)).Replace("-", "");
            }
        }

        private static byte[] DecryptHashWithAESInCBC(byte[] data)
        {
            byte[] encryptedIV = data.Take(16).ToArray();
            byte[] iv = DecryptAES(encryptedIV);
            byte[] input = data.Skip(16).ToArray();

            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CBC/NoPadding");
            cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", encryption_key), iv));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CipherStream cipherStream = new CipherStream(memoryStream, null, cipher))
                {
                    cipherStream.Write(input, 0, input.Length);
                }

                return memoryStream.ToArray();
            }
        }

        private byte[] DecryptImageWithAESInCBC(string ciphertext)
        {
            byte[] data = StringToByteArray(ciphertext);
            try
            {
                byte[] tag = DecryptHashWithAESInCBC(data.Take(48).ToArray());
                byte[] iv = DecryptAES(data.Skip(48).Take(16).ToArray());
                byte[] encryptedContent = data.Skip(64).ToArray();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    IBufferedCipher cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
                    cipher.Init(false, new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", encryption_key), iv));

                    using (CipherStream cipherStream = new CipherStream(memoryStream, null, cipher))
                    {
                        cipherStream.Write(encryptedContent, 0, encryptedContent.Length);
                    }

                    if (!CalculateHMACSHA256(memoryStream.ToArray()).AsSpan().SequenceEqual(tag))
                    {
                        ShowErrorMessageBox("Failed to Verify Integrity/Authenticity of an image", "Decrypted and Computed Tags Don't Match");
                        return new byte[] { 0 };
                    }
                    else
                    {
                        return memoryStream.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox("Failed to decrypt image.", "Error: " + ex.Message);
                return new byte[] { 0 };
            }
        }

        private static byte[] CombineByteArrays(params byte[][] arrays)
        {
            byte[] rv = new byte[arrays.Sum(a => a.Length)];
            int offset = 0;
            foreach (byte[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }
            return rv;
        }

        //Image Encryption/Decryption (Above)

        private static byte[] EncryptAES(byte[] data)
        {
            // Create the AES cipher with ECB mode and no padding
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");
            cipher.Init(true, new KeyParameter(encryption_key));

            // Encrypt the data
            return cipher.DoFinal(data);
        }

        private static byte[] DecryptAES(byte[] encryptedData)
        {
            // Create the AES cipher with ECB mode and no padding
            IBufferedCipher cipher = CipherUtilities.GetCipher("AES/ECB/NoPadding");
            cipher.Init(false, new KeyParameter(encryption_key));

            // Decrypt the data
            return cipher.DoFinal(encryptedData);
        }

        private void set_unlocked_status()
        {
            sodtware_status_label.Text = "Unlocked";
            button6.Text = "Lock Software";
            button6.BackColor = Color.FromArgb(239, 0, 18);
        }

        private void set_locked_status()
        {
            sodtware_status_label.Text = "Locked";
            button6.Text = "Unlock Software";
            button6.BackColor = Color.FromArgb(0, 152, 136);
        }

        private void DisplayESLInformation()
        {
            selected_row = -1;
            dataGridView1.Rows.Clear(); // Clear the DataGridView
            dataGridView1.Columns.Clear(); // Clear existing columns

            // Define DataGridView columns
            DataGridViewTextBoxColumn ipclmn = new DataGridViewTextBoxColumn
            {
                Name = "IP",
                HeaderText = "IP",
                DataPropertyName = "IP",
                ReadOnly = true, // Set column as read-only
                Width = (int)(dataGridView1.Width * 0.18)
            };

            DataGridViewTextBoxColumn dn = new DataGridViewTextBoxColumn
            {
                Name = "Device_Name",
                HeaderText = "Device Name",
                DataPropertyName = "Device_Name",
                ReadOnly = true, // Set column as read-only
                Width = (int)(dataGridView1.Width * 0.81)
            };

            // Add columns to the DataGridView
            dataGridView1.Columns.AddRange(ipclmn, dn);

            // Set DataGridViewCellStyle properties for appearance
            DataGridViewCellStyle dataGridViewCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(16, 16, 16), // Background color
                ForeColor = Color.FromArgb(238, 238, 238), // Foreground color
                SelectionBackColor = ColorTranslator.FromHtml("#6301A5"), // Selected cell color
                SelectionForeColor = Color.FromArgb(238, 238, 238), // Selected text color
            };

            dataGridView1.DefaultCellStyle = dataGridViewCellStyle;

            dataGridView1.Dock = DockStyle.Fill;

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersVisible = false;

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dataGridView1.MultiSelect = false;

            dataGridView1.ReadOnly = true;

            dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;

            // Handle RowPostPaint event for zebra stripes
            dataGridView1.RowPostPaint += (sender, e) =>
            {
                if (e.RowIndex % 2 == 0)
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#EEEEEE");
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#2C2C2C");
                }
                else
                {
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = ColorTranslator.FromHtml("#2C2C2C");
                    dataGridView1.Rows[e.RowIndex].DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#EEEEEE");
                }
            };
            // Fill your DataGridView with data
            FillDataGridView();
        }

        private void FillDataGridView()
        {
            // Create a List to store Rec_id values
            List<string> recIdsList = new List<string>();

            dataGridView1.Rows.Add("IP Address", "Device Name");

            using (SQLiteConnection connection = new SQLiteConnection(SQLiteconnectionString))
            {
                connection.Open();
                // Assuming Worker is your table name
                string query = "SELECT Rec_id, ip_addr, device_name FROM ESL";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Extracting information
                            string recId = reader["Rec_id"].ToString();
                            string extr_ip = reader["ip_addr"].ToString();
                            string extr_dn = reader["device_name"].ToString();

                            // Add the information to dataGridView1
                            dataGridView1.Rows.Add(Decrypt_string_with_aes_in_cbc(extr_ip), Decrypt_string_with_aes_in_cbc(extr_dn));

                            // Add the Rec_id to the List
                            recIdsList.Add(recId);
                        }
                    }
                }

                connection.Close();
            }

            // Convert the List to an array at the end
            ESL_ids = recIdsList.ToArray();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (button6.Text == "Unlock Software")
            {
                ask_user_for_password(get_encr_hash_of_password_from_db());
                set_unlocked_status();
                DisplayESLInformation();
            }

            else if (button6.Text == "Lock Software")
            {
                for (int i = 0; i < 16; i++)
                {
                    encryption_key[i] = 0;
                    verification_key[i] = 0;
                }
                image_encryption_key = "";
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                set_locked_status();
            }
        }

        private bool software_unlocked()
        {
            if (button6.Text == "Lock Software")
                return true;
            else
            {
                ShowErrorMessageBox("The Software is Locked", "Unlock the Software to Continue");
                return false;
            }
        }

        public static bool IsValidHexadecimalPort(string portString)
        {
            return Regex.IsMatch(portString, @"^.{4}$");
        }

        public static bool IsValidEncryptionKey(string portString)
        {
            return Regex.IsMatch(portString, @"^.{64}$");
        }

        public static bool IsValidIpAddress(string ipAddress)
        {
            string ipAddressPattern = @"^\d+\.\d+\.\d+\.\d+$";
            return Regex.IsMatch(ipAddress, ipAddressPattern);
        }

        private void button1_Click(object sender, EventArgs e) // Add record to Database
        {
            add_record_to_esl();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            add_record_to_esl();

        }

        private void add_record_to_esl()
        {
            if (software_unlocked())
            {
                AddESL addesl = new AddESL();
                addesl.ShowDialog();
                DisplayESLInformation();
            }
        }

        private void SetPixelColor(int x, int y, Color color)
        {
            // Check if the picture box has an image
            if (pictureBox1.Image == null)
                return;

            // Get the bitmap from the picture box's image
            Bitmap bmp = (Bitmap)pictureBox1.Image;

            // Check if the coordinates are within the bounds of the image
            if (x >= 0 && x < bmp.Width && y >= 0 && y < bmp.Height)
            {
                // Set the color of the pixel
                bmp.SetPixel(x, y, color);

                // Update the picture box with the modified bitmap
                pictureBox1.Image = bmp;
            }
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (dataGridView1.SelectedRows[0].Index > 0 && dataGridView1.SelectedRows[0].Index < (ESL_ids.Length + 1))
                {
                    selected_row = dataGridView1.SelectedRows[0].Index - 1;
                    string res = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "resolution"));
                    string[] parts = res.Split('x');
                    if (int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height))
                    {
                        pb_w = width;
                        pb_h = height;
                        pictureBox1.Size = new System.Drawing.Size(pb_w, pb_h);
                        pictureBox2.Size = new System.Drawing.Size(pb_w, pb_h);
                        pictureBox3.Size = new System.Drawing.Size(pb_w, pb_h);
                        string image = Extract_value_from_record("ESL", ESL_ids[selected_row], "image");
                        if (image == "-1")
                        {
                            Bitmap bitmap = new Bitmap(width, height);
                            pictureBox1.Image = bitmap;
                            pictureBox2.Image = bitmap;
                            pictureBox3.Image = bitmap;
                        }

                        else
                        {
                            byte[] extr_image = DecryptImageWithAESInCBC(image);
                            int pixel_count = 0;
                            try
                            {
                                Bitmap bitmap = new Bitmap(width, height);

                                for (int y = 0; y < height; y++)
                                {
                                    for (int x = 0; x < width; x++)
                                    {
                                        bitmap.SetPixel(x, y, ConvertRgb565ToRgb888((ushort)((extr_image[pixel_count + 1] << 8) | extr_image[pixel_count])));
                                        pixel_count += 2;
                                    }
                                }
                                pictureBox1.Image = bitmap;
                            }
                            catch (IOException ex)
                            {
                                //ShowErrorMessageBox("Image Decryption Error", ex.Message);
                            }

                        }
                    }
                }
                else
                    selected_row = -1;

            }
        }

        private void button5_Click(object sender, EventArgs e) // Remove ESl from DB
        {
            if (software_unlocked())
            {
                delete_ESL();
            }
        }

        private void delete_ESL()
        {
            if (selected_row != -1)
            {
                string recId = ESL_ids[selected_row];
                string name = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "device_name"));
                string ESLIP = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "ip_addr"));
                DialogResult result = confirm_esl_deletion(name, ESLIP); // Assuming the confirmation dialog is still relevant

                if (result == DialogResult.Yes)
                {
                    using (SQLiteConnection connection = new SQLiteConnection(SQLiteconnectionString))
                    {
                        connection.Open();

                        using (SQLiteCommand command = new SQLiteCommand(connection))
                        {
                            // Construct the DELETE statement
                            command.CommandText = $"DELETE FROM ESL WHERE Rec_id = @id";
                            command.Parameters.AddWithValue("@id", recId);

                            try
                            {
                                // Execute the DELETE statement
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    ShowLeashmoreMessageBox("Record Deleted Successfully");
                                    DisplayESLInformation();
                                }
                                else
                                {
                                    ShowErrorMessageBox("Failed to Delete Record", $"Record \"{recId}\" isn't found");
                                }
                            }
                            catch (Exception ex)
                            {
                                ShowErrorMessageBox($"Failed to Delete Record \"{recId}\"", ex.Message);
                            }
                        }
                        connection.Close();
                        selected_id = "";
                    }
                }
                else if (result == DialogResult.No)
                {
                    ShowLeashmoreMessageBox("Operation Was Cancelled By User");
                }
                else
                {
                    ShowLeashmoreMessageBox("Operation Was Cancelled By User");
                }
            }
            else
                ShowErrorMessageBox("Record Selection Required", "Please select an ESL record first.");
        }

        public static DialogResult confirm_esl_deletion(string line1, string line2)
        {
            Form customMessageBox = new Form
            {
                Text = "Delete Record of \"" + line1 + "\"",
                Size = new Size(640, 242),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = ColorTranslator.FromHtml("#AE0031") // Set background color to "#AE0031"
            };

            Label attl = new Label
            {
                Text = "You are about to delete the following ESL record",
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"), // Set foreground color to "#E4E3DF"
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customMessageBox.Controls.Add(attl);

            // Create label for the first line
            Label label1 = new Label
            {
                Text = "Device Name: " + line1,
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"), // Set foreground color to "#E4E3DF"
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customMessageBox.Controls.Add(label1);

            // Create label for the second line
            Label label2 = new Label
            {
                Text = "Device IP: " + line2,
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"), // Set foreground color to "#E4E3DF"
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customMessageBox.Controls.Add(label2);

            Label label3 = new Label
            {
                Text = "This Can't be Undone! Would You Like to Delete that Record?",
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"), // Set foreground color to "#E4E3DF"
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customMessageBox.Controls.Add(label3);

            // Create Yes button
            Button yesButton = new Button
            {
                Text = "Yes, Delete it",
                Size = new Size(90, 48),
                BackColor = ColorTranslator.FromHtml("#AE0031"), // Set background color to "#AE0031"
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"), // Set foreground color to "#E4E3DF"
                DialogResult = DialogResult.Yes,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 3, BorderColor = ColorTranslator.FromHtml("#E4E3DF") }, // Set border properties
                Font = new Font("Segoe UI", 12, FontStyle.Bold) // Set bold font
            };
            customMessageBox.Controls.Add(yesButton);

            // Create No button
            Button noButton = new Button
            {
                Text = "No, Cancel",
                Size = new Size(90, 48),
                BackColor = ColorTranslator.FromHtml("#00844F"), // Set background color to "#00844F"
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"), // Set foreground color to "#E4E3DF"
                DialogResult = DialogResult.No,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 3, BorderColor = ColorTranslator.FromHtml("#E4E3DF") }, // Set border properties
                Font = new Font("Segoe UI", 12, FontStyle.Bold) // Set bold font
            };
            customMessageBox.Controls.Add(noButton);

            attl.Location = new Point((customMessageBox.ClientSize.Width) / 2, +11);
            label1.Location = new Point((customMessageBox.ClientSize.Width) / 2, attl.Bottom + 12);
            label2.Location = new Point((customMessageBox.ClientSize.Width) / 2, label1.Bottom + 8);
            label3.Location = new Point((customMessageBox.ClientSize.Width) / 2, label2.Bottom + 12);

            int buttonMargin = 30;
            int buttonWidth = (customMessageBox.ClientSize.Width - 3 * buttonMargin) / 2;

            yesButton.Size = new Size(buttonWidth, 41);
            noButton.Size = new Size(buttonWidth, 41);

            yesButton.Location = new Point(buttonMargin, label3.Bottom + 20);
            noButton.Location = new Point(yesButton.Right + buttonMargin, label3.Bottom + 20);
            CenterLabelText(attl, customMessageBox);
            CenterLabelText(label1, customMessageBox);
            CenterLabelText(label2, customMessageBox);
            CenterLabelText(label3, customMessageBox);

            // Handle Resize event to adjust positions dynamically
            customMessageBox.Resize += (sender, e) =>
            {
                CenterLabelText(attl, customMessageBox);
                CenterLabelText(label1, customMessageBox);
                CenterLabelText(label2, customMessageBox);
                CenterLabelText(label3, customMessageBox);

                buttonWidth = (customMessageBox.ClientSize.Width - 3 * buttonMargin) / 2;
                yesButton.Size = new Size(buttonWidth, 48);
                noButton.Size = new Size(buttonWidth, 48);

                yesButton.Location = new Point(buttonMargin, label3.Bottom + 20);
                noButton.Location = new Point(yesButton.Right + buttonMargin, label3.Bottom + 20);
            };

            // Show the message box
            DialogResult result = customMessageBox.ShowDialog();

            return result;
        }

        private void button2_Click(object sender, EventArgs e) // Edit ESL Record
        {
            if (software_unlocked())
            {
                if (selected_row != -1)
                {
                    string recId = ESL_ids[selected_row];
                    string deviceName = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "device_name"));
                    string encryption_key = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "encryption_key"));
                    string UDPport = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "udp_port"));
                    string ESLIP = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "ip_addr"));
                    string resltn = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "resolution"));
                    EditESLForm editesl = new EditESLForm(recId, deviceName, encryption_key, UDPport, ESLIP, resltn);
                    editesl.ShowDialog();
                    DisplayESLInformation();
                }
                else
                    ShowErrorMessageBox("Record Selection Required", "Please select an ESL record first.");
            }
        }

        private void button3_Click(object sender, EventArgs e) // Select Image
        {
            if (software_unlocked())
            {
                if (selected_row != -1)
                {
                    SelectAndLoadImage();
                }
                else
                    ShowErrorMessageBox("Record Selection Required", "Please select an ESL record first.");
            }
        }
        private void SelectAndLoadImage()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open Image";
            dialog.Filter = "Image Files|*.jpg; *.jpeg; *.gif; *.bmp; *.png";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Image srcImage = Image.FromFile(dialog.FileName);
                short angle;
                int img_x;
                int img_y;
                Bitmap lcdImage = new Bitmap(pb_w, pb_h);
                int width = srcImage.Width;
                int height = srcImage.Height;
                double ratio = (double)width / (double)height;

                int targetWidth = lcdImage.Width;
                int targetHeight = lcdImage.Height;
                double targetRatio = (double)targetWidth / (double)targetHeight;

                if (ratio > targetRatio) // Wide
                {
                    height = (targetWidth * height) / width;
                    width = targetWidth;
                }
                else if (ratio < targetRatio) // Tall
                {
                    width = (targetHeight * width) / height;
                    height = targetHeight;
                }
                else // Same
                {
                    width = targetWidth;
                    height = targetHeight;
                }

                int x = (width / 2) * -1;
                int y = (height / 2) * -1;

                Graphics g = Graphics.FromImage(lcdImage);
                g.Clear(Color.Black);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.TranslateTransform((targetWidth / 2), (targetHeight / 2));
                g.DrawImage(srcImage, x, y, width, height);
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                pictureBox1.Image = lcdImage;
                pictureBox1.Anchor = AnchorStyles.None;
            }
        }

        private void button4_Click(object sender, EventArgs e) // Send Image
        {
            if (software_unlocked())
            {
                if (selected_row != -1)
                {
                    image_encryption_key = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "encryption_key"));
                    current_udp_port = Convert.ToInt32(Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "udp_port")), 16);
                    ipAddress = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "ip_addr"));
                    current_id = ESL_ids[selected_row];
                    sendImagetoMCU();
                }
                else
                    ShowErrorMessageBox("Record Selection Required", "Please select an ESL record first.");
            }
        }

        private void stream_encrypted_image_over_udp(byte[] data)
        {
            try
            {
                // Create a UdpClient for sending the UDP packet
                using (UdpClient udpClient = new UdpClient())
                {
                    // Set the target device's IP address and port
                    udpClient.Connect(ipAddress, current_udp_port);

                    // Send the UDP packet
                    udpClient.Send(data, data.Length);

                    //MessageBox.Show("UDP packet sent successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessageBox("Image Streaming Error", ex.Message);
            }
        }

        static void encryptArrayWithSerpentInCbc(byte[] input, byte[] iv)
        {
            decract = 0;
            encrypt_iv_for_serpnt_in_cbc_mode(iv);
            int strLen = input.Length + 1;

            int p = 0;
            while (strLen > p + 1)
            {
                split_by_sixteen_for_encryption(input, p, strLen);
                p += 16;
            }
        }

        static void split_by_sixteen_for_encryption(byte[] plaintext, int k, int strLen)
        {
            byte[] res = new byte[16];

            for (int i = 0; i < 16; i++)
                res[i] = 0;

            for (int i = 0; i < 16; i++)
            {
                if (i + k > strLen - 1)
                    break;
                res[i] = plaintext[i + k];
            }

            for (int i = 0; i < 16; i++)
                res[i] ^= array_for_CBC_mode[i];

            encrypt_with_serpent(res);
        }

        private static void encrypt_iv_for_serpnt_in_cbc_mode(byte[] iv)
        {
            for (int i = 0; i < 16; i++)
            {
                array_for_CBC_mode[i] = iv[i];
            }

            encrypt_with_serpent(iv);
        }

        static void encrypt_with_serpent(byte[] passToSerp)
        {
            byte[] ct2b = EncryptSerpent(passToSerp);
            incr_serp_key();

            for (int i = 0; i < 16; i++)
            {
                if (decract > 0)
                {
                    if (i < 16)
                    {
                        array_for_CBC_mode[i] = ct2b[i];
                    }
                }
                globalByteList.Add(ct2b[i]);
            }
            decract++;
        }

        static byte[] EncryptSerpent(byte[] inputBytes)
        {

            SerpentEngine serpentEngine = new SerpentEngine();
            serpentEngine.Init(true, new KeyParameter(serp_key));

            byte[] encryptedData = new byte[inputBytes.Length];

            serpentEngine.ProcessBlock(inputBytes, 0, encryptedData, 0);

            return encryptedData;
        }

        protected static void incr_serp_key()
        {
            for (int i = 15; i >= 0; i--)
            {
                if (serp_key[i] == 255)
                {
                    serp_key[i] = 0;
                }
                else
                {
                    serp_key[i]++;
                    break;
                }
            }
        }

        private void sendImagetoMCU()
        {
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("No image loaded in the PictureBox.");
                return;
            }

            byte[] one_el = new byte[1];
            stream_encrypted_image_over_udp(one_el);
            full_image.Clear();
            Thread.Sleep(1);
            Bitmap bitmap = new Bitmap(pictureBox1.Image);
            int totalPixels = bitmap.Width * bitmap.Height;
            int pixelsSent = 0;

            try
            {

                int height = bitmap.Height;
                int width = bitmap.Width;
                Bitmap bitmap1 = new Bitmap(width, height);
                Bitmap bitmap2 = new Bitmap(width, height);

                for (int y = 0; y < height; y++)
                {
                    List<byte> byteList = new List<byte>();
                    for (int x = 0; x < width; x++)
                    {
                        Color pixelColor = bitmap.GetPixel(x, y);
                        ushort color16bit = ConvertColorTo16Bit(pixelColor);
                        byteList.Add((byte)(color16bit & 0xFF));
                        byteList.Add((byte)((color16bit >> 8) & 0xFF));
                        full_image.Add((byte)(color16bit & 0xFF));
                        full_image.Add((byte)((color16bit >> 8) & 0xFF));
                        pixelsSent++;
                    }
                    Thread.Sleep(160);
                    byte[] iv = new byte[16]; // Initialization vector

                    Random random = new Random();
                    for (int i = 0; i < 16; i++)
                    {
                        iv[i] = (byte)random.Next(256); // Fill iv array with random numbers.
                    }
                    SetKeyToSerpentFromString(image_encryption_key);
                    globalByteList.Clear();
                    encryptArrayWithSerpentInCbc(byteList.ToArray(), iv);
                    stream_encrypted_image_over_udp(globalByteList.ToArray());
                    byte[] encr_img = globalByteList.ToArray();

                    for (int j = 0; j < (width * 2); j += 2)
                    {
                        bitmap1.SetPixel(j / 2, y, ConvertRgb565ToRgb888((ushort)((encr_img[j + 16] << 8) | encr_img[j + 17])));
                    }

                    Invoke(new Action(() =>
                    {
                        pictureBox2.Image = bitmap1;
                    }));

                    byte[] dsp_565_img = byteList.ToArray();

                    for (int j = 0; j < width; j++)
                    {
                        bitmap2.SetPixel(j, y, bitmap.GetPixel(j, y));
                    }

                    Invoke(new Action(() =>
                    {
                        pictureBox3.Image = bitmap2;
                    }));

                    Thread.Sleep(160);
                    int progressPercentage = (int)((float)pixelsSent / totalPixels * 100);
                    progressBar1.Invoke(new Action(() => progressBar1.Value = progressPercentage));
                    globalByteList.Clear();
                }
                UpdateImage(EncryptImageWithAESInCBC(full_image.ToArray()));
                ShowLeashmoreMessageBox("Image Uploaded Successfully!");
            }
            catch (IOException ex)
            {
                ShowErrorMessageBox("Image Streaming Error", ex.Message);
            }
            progressBar1.Invoke(new Action(() => progressBar1.Value = 0));
        }

        public void UpdateImage(string newIMG)
        {
            using (SQLiteConnection connection = new SQLiteConnection(Form1.SQLiteconnectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    // Construct the UPDATE statement
                    command.CommandText = "UPDATE ESL SET image = @img WHERE Rec_id = @id";
                    command.Parameters.AddWithValue("@img", newIMG);
                    command.Parameters.AddWithValue("@id", current_id);
                    try
                    {
                        // Execute the UPDATE statement
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                        }
                        else
                        {
                            Form1.ShowErrorMessageBox("Failed to Add Image to Database", $"Record \"{current_id}\" isn't found");
                        }
                    }
                    catch (Exception ex)
                    {
                        Form1.ShowErrorMessageBox($"Failed to Add Image to Database", ex.Message);
                    }
                }
            }
        }

        static void SetKeyToSerpentFromString(string hexString)
        {
            // Check if the input string is a valid hexadecimal string
            if (hexString.Length % 2 != 0 || !System.Text.RegularExpressions.Regex.IsMatch(hexString, @"\A\b[0-9a-fA-F]+\b\Z"))
            {
                throw new ArgumentException("Invalid hexadecimal string.");
            }

            // Convert the hexadecimal string to a byte array
            byte[] byteArray = new byte[hexString.Length / 2];
            for (int i = 0; i < hexString.Length; i += 2)
            {
                serp_key[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            }
        }

        private ushort ConvertColorTo16Bit(Color color)
        {
            // Assuming 5 bits for red, 6 bits for green, and 5 bits for blue
            ushort red = (ushort)(color.R >> 3);
            ushort green = (ushort)(color.G >> 2);
            ushort blue = (ushort)(color.B >> 3);

            // Combine the bits to form a 16-bit color value
            ushort result = (ushort)((red << 11) | (green << 5) | blue);

            return result;
        }

        static Color ConvertRgb565ToRgb888(ushort color16bit)
        {
            int red5 = (color16bit >> 11) & 0x1F;
            int green6 = (color16bit >> 5) & 0x3F;
            int blue5 = color16bit & 0x1F;

            byte red8 = (byte)((red5 * 255) / 31);
            byte green8 = (byte)((green6 * 255) / 63);
            byte blue8 = (byte)((blue5 * 255) / 31);

            return Color.FromArgb(red8, green8, blue8);
        }

        static string GetSafeString(SQLiteDataReader reader, string columnName)
        {
            int columnIndex = reader.GetOrdinal(columnName);
            return reader.IsDBNull(columnIndex) ? null : reader.GetString(columnIndex);
        }

        static string[,] ExtractRecordTitlesAndIDs(SQLiteConnection connection, string table, string title_field)
        {
            // Define the SQL command to select records from the "Specialization" table
            string selectSql = "SELECT Rec_id, " + title_field + " FROM " + table + ";";

            // Create a list to store records
            List<string[]> recordsList = new List<string[]>();

            using (SQLiteCommand command = new SQLiteCommand(selectSql, connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Read values from the reader
                        string recId = GetSafeString(reader, "Rec_id");
                        string title = GetSafeString(reader, title_field);

                        // Add values to the list only if they are not null and not DBNull
                        if (recId != null && title != null)
                        {
                            recordsList.Add(new string[] { recId, Decrypt_string_with_aes_in_cbc(title) });
                        }
                    }
                }
            }

            // Convert the list to a 2D array
            string[,] recordsArray = new string[recordsList.Count, 2];
            for (int i = 0; i < recordsList.Count; i++)
            {
                recordsArray[i, 0] = recordsList[i][0]; // Rec_id
                recordsArray[i, 1] = recordsList[i][1]; // Title
            }

            return recordsArray;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (software_unlocked())
            {
                using (SQLiteConnection connection = new SQLiteConnection(SQLiteconnectionString))
                {
                    connection.Open();
                    string[,] extracted_recs = ExtractRecordTitlesAndIDs(connection, "ESL", "Device_name");
                    SelectRecordForm selectRecordForm = new SelectRecordForm(extracted_recs, "Remove ESL Record From Database", "Remove");
                    DialogResult result = selectRecordForm.ShowDialog();

                    if (result == DialogResult.Continue)
                    {
                        if (DeleteRecord("ESL", selected_id))
                        {
                            ShowLeashmoreMessageBox("ESL Record Removed Successfully");
                        }

                    }

                    if (result == DialogResult.Cancel)
                    {
                        ShowLeashmoreMessageBox("Operation Was Cancelled By User");
                    }

                    if (result == DialogResult.Abort)
                    {
                        ShowErrorMessageBox("No Record is Selected", "Please, Try Again");
                    }

                    connection.Close();
                    selected_id = "";

                }
                DisplayESLInformation();
            }
        }
        private bool DeleteRecord(string table, string id)
        {
            bool rec_deltd = false;
            using (SQLiteConnection connection = new SQLiteConnection(SQLiteconnectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    // Construct the DELETE statement
                    command.CommandText = $"DELETE FROM {table} WHERE Rec_id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    try
                    {
                        // Execute the DELETE statement
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            rec_deltd = true;
                        }
                        else
                        {
                            ShowErrorMessageBox("Failed to Delete Record", $"Record \"{id}\" isn't found");
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessageBox($"Failed to Delete Record \"{id}\"", ex.Message);
                    }
                }
                connection.Close();
            }
            return rec_deltd;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (software_unlocked())
            {
                using (SQLiteConnection connection = new SQLiteConnection(SQLiteconnectionString))
                {
                    connection.Open();
                    string[,] extracted_recs = ExtractRecordTitlesAndIDs(connection, "ESL", "Device_name");
                    SelectRecordForm selectRecordForm = new SelectRecordForm(extracted_recs, "Edit ESL Record", "Edit");
                    DialogResult result = selectRecordForm.ShowDialog();

                    if (result == DialogResult.Continue)
                    {
                        string deviceName = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", selected_id, "device_name"));
                        string encryption_key = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", selected_id, "encryption_key"));
                        string UDPport = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", selected_id, "udp_port"));
                        string ESLIP = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", selected_id, "ip_addr"));
                        string resltn = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", selected_id, "resolution"));
                        EditESLForm editesl = new EditESLForm(selected_id, deviceName, encryption_key, UDPport, ESLIP, resltn);
                        editesl.ShowDialog();
                        DisplayESLInformation();

                    }

                    if (result == DialogResult.Cancel)
                    {
                        ShowLeashmoreMessageBox("Operation Was Cancelled By User");
                    }

                    if (result == DialogResult.Abort)
                    {
                        ShowErrorMessageBox("No Record is Selected", "Please, Try Again");
                    }

                    connection.Close();
                    selected_id = "";

                }
                DisplayESLInformation();
            }
        }

        private void viewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (software_unlocked())
            {
                using (SQLiteConnection connection = new SQLiteConnection(SQLiteconnectionString))
                {
                    connection.Open();
                    string[,] extracted_recs = ExtractRecordTitlesAndIDs(connection, "ESL", "Device_name");
                    SelectRecordForm selectRecordForm = new SelectRecordForm(extracted_recs, "View ESL Record", "View");
                    DialogResult result = selectRecordForm.ShowDialog();

                    if (result == DialogResult.Continue)
                    {
                        string deviceName = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", selected_id, "device_name"));
                        string encryption_key = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", selected_id, "encryption_key"));
                        string UDPport = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", selected_id, "udp_port"));
                        string ESLIP = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", selected_id, "ip_addr"));
                        string resltn = Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", selected_id, "resolution"));
                        string[] parts = resltn.Split('x');
                        Bitmap bitmap = new Bitmap(320, 240);
                        if (int.TryParse(parts[0], out int width) && int.TryParse(parts[1], out int height))
                        {
                            pb_w = width;
                            pb_h = height;
                            string image = Extract_value_from_record("ESL", selected_id, "image");
                            if (image == "-1")
                            {
                                bitmap = new Bitmap(width, height);
                            }

                            else
                            {
                                byte[] extr_image = DecryptImageWithAESInCBC(image);
                                int pixel_count = 0;
                                try
                                {
                                    bitmap = new Bitmap(width, height);

                                    for (int y = 0; y < height; y++)
                                    {
                                        for (int x = 0; x < width; x++)
                                        {
                                            bitmap.SetPixel(x, y, ConvertRgb565ToRgb888((ushort)((extr_image[pixel_count + 1] << 8) | extr_image[pixel_count])));
                                            pixel_count += 2;
                                        }
                                    }
                                }
                                catch (IOException ex)
                                {
                                    //ShowErrorMessageBox("Image Decryption Error", ex.Message);
                                }

                            }

                            ViewESLForm viewesl = new ViewESLForm(selected_id, deviceName, encryption_key, UDPport, ESLIP, resltn, bitmap);
                            viewesl.ShowDialog();
                            DisplayESLInformation();

                        }

                        if (result == DialogResult.Cancel)
                        {
                            ShowLeashmoreMessageBox("Operation Was Cancelled By User");
                        }

                        if (result == DialogResult.Abort)
                        {
                            ShowErrorMessageBox("No Record is Selected", "Please, Try Again");
                        }

                        connection.Close();
                        selected_id = "";

                    }
                    DisplayESLInformation();
                }
            }
        }

        private void selectImageToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportESLDatabaseToolStripMenuItem_Click(object sender, EventArgs e) // Quit
        {

        }

        private void exportESLdb_Click(object sender, EventArgs e)
        {
            if (software_unlocked())
            {
                DialogResult result = ShowOrangeMessageBox("ESL Name, IP Address, UDP Port, and Encryption Key of Each Records Will Be Exported to .csv File");

                if (result == DialogResult.Yes)
                {
                    exportESLtocsv();
                }
                else if (result == DialogResult.No)
                {
                    ShowLeashmoreMessageBox("Record Export Was Cancelled By User");
                }
                else
                {
                    ShowLeashmoreMessageBox("Record Export Was Cancelled By User");
                }
            }
        }

        private void exportESLtocsv()
        {
            long selected_row_back = selected_row;
            int i = 0;
            List<string> stringList = new List<string>();
            foreach (string esl in ESL_ids)
            {
                selected_row = i;
                stringList.Add(Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "device_name")).Replace(",", ".") + "," + Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "ip_addr")) + "," + "0x" + Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "udp_port")) + "," + Decrypt_string_with_aes_in_cbc(Extract_value_from_record("ESL", ESL_ids[selected_row], "encryption_key")));
                i++;
            }

            // Ask the user for the file path to save the CSV file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog.Title = "Export ESL Records to...";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                ExportToCsv(stringList, saveFileDialog.FileName);
                ShowLeashmoreMessageBox($"Data has been successfully exported to {saveFileDialog.FileName}");
            }
            selected_row = selected_row_back;
        }

        private void ExportToCsv(List<string> list, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                string[] column_names = new string[4] { "Device Name", "IP Address", "UDP Port", "Encryption Key" };
                // Write header
                for (int i = 0; i < 4; i++)
                {
                    writer.Write(column_names[i]);
                    if (i < 3)
                    {
                        writer.Write(",");
                    }
                }
                writer.WriteLine();
                foreach (string item in list)
                {
                    writer.WriteLine(item);
                }
            }
        }

        private void quit_item_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about_eslms();
        }

        private void about_eslms()
        {
            Form customForm = new Form
            {
                Text = "About ESL Management System",
                Size = new Size(640, 520),
                MinimumSize = new Size(640, 520),
                StartPosition = FormStartPosition.CenterScreen,
                BackColor = ColorTranslator.FromHtml("#7B08A5")
            };

            Label label = new Label
            {
                Text = "This software is a part of the ESL Management System\n" +
                       "You are free to modify and distribute copies of this software.\n" +
                       "You can use this software, as well as firmware for ESP8266\n" +
                       "in commercial applications.\n\n" +
                       "Source code and user guide can be found on:\n\n" +
                       "SourceForge",
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customForm.Controls.Add(label);

            TextBox textField = new TextBox
            {
                Size = new Size(620, 30),
                Text = "sourceforge.net/projects/esl-management-system/",
                Location = new Point((customForm.ClientSize.Width - 200) / 2, label.Bottom + 12),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ReadOnly = true,
                BackColor = customForm.BackColor, // Set the background color to match the form's
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"),
                BorderStyle = BorderStyle.None, // Make the textbox borderless
                TextAlign = HorizontalAlignment.Center // Center the text horizontally
            };

            customForm.Controls.Add(textField);

            Label label1 = new Label
            {
                Location = new Point((customForm.ClientSize.Width - 200) / 2, textField.Bottom + 15),
                Text = "Github",
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customForm.Controls.Add(label1);

            TextBox textField1 = new TextBox
            {
                Size = new Size(620, 30),
                Text = "github.com/Northstrix/Electronic-Shelf-Label-Management-System",
                Location = new Point((customForm.ClientSize.Width - 200) / 2, label.Bottom + 12),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ReadOnly = true,
                BackColor = customForm.BackColor, // Set the background color to match the form's
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"),
                BorderStyle = BorderStyle.None, // Make the textbox borderless
                TextAlign = HorizontalAlignment.Center // Center the text horizontally
            };
            customForm.Controls.Add(textField1);

            Label label11 = new Label
            {
                Location = new Point((customForm.ClientSize.Width - 200) / 2, textField.Bottom + 15),
                Text = "Instructables",
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customForm.Controls.Add(label11);

            TextBox textField11 = new TextBox
            {
                Size = new Size(620, 30),
                Text = "instructables.com/DIY-Electronic-Shelf-Label-Management-System/",
                Location = new Point((customForm.ClientSize.Width - 200) / 2, label.Bottom + 12),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ReadOnly = true,
                BackColor = customForm.BackColor, // Set the background color to match the form's
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"),
                BorderStyle = BorderStyle.None, // Make the textbox borderless
                TextAlign = HorizontalAlignment.Center // Center the text horizontally
            };
            customForm.Controls.Add(textField11);

            Label label2 = new Label
            {
                Location = new Point((customForm.ClientSize.Width - 200) / 2, textField1.Bottom + 20),
                Text = "Copyright " + "\u00a9" + " 2024 Maxim Bortnikov",
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            customForm.Controls.Add(label2);

            Button continueButton = new Button
            {
                Text = "Got It",
                Size = new Size(120, 38),
                Location = new Point((customForm.ClientSize.Width - 200) / 2, label2.Bottom + 30),
                BackColor = ColorTranslator.FromHtml("#4113AA"),
                ForeColor = ColorTranslator.FromHtml("#E4E3DF"),
                DialogResult = DialogResult.Yes,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            customForm.Controls.Add(continueButton);

            CenterLabelText(label, customForm);
            CenterLabelText(label1, customForm);
            CenterLabelText(label2, customForm);
            CenterLabelText(label11, customForm);
            label.Location = new Point((customForm.ClientSize.Width - label.Width) / 2, +12);
            textField.Location = new Point((customForm.ClientSize.Width - textField.Width) / 2, label.Bottom + 10);
            label1.Location = new Point((customForm.ClientSize.Width - label1.Width) / 2, textField.Bottom + 15);
            textField1.Location = new Point((customForm.ClientSize.Width - textField.Width) / 2, label1.Bottom + 6);
            label11.Location = new Point((customForm.ClientSize.Width - label11.Width) / 2, textField1.Bottom + 15);
            textField11.Location = new Point((customForm.ClientSize.Width - textField.Width) / 2, label11.Bottom + 6);
            label2.Location = new Point((customForm.ClientSize.Width - label2.Width) / 2, textField11.Bottom + 20);
            continueButton.Location = new Point((customForm.ClientSize.Width - continueButton.Width) / 2, label2.Bottom + 20);

            // Handle Resize event to adjust positions dynamically
            customForm.Resize += (sender, e) =>
            {
                CenterLabelText(label, customForm);
                CenterLabelText(label1, customForm);
                CenterLabelText(label2, customForm);
                CenterLabelText(label11, customForm);
                label.Location = new Point((customForm.ClientSize.Width - label.Width) / 2, +12);
                textField.Location = new Point((customForm.ClientSize.Width - textField.Width) / 2, label.Bottom + 10);
                label1.Location = new Point((customForm.ClientSize.Width - label1.Width) / 2, textField.Bottom + 15);
                textField1.Location = new Point((customForm.ClientSize.Width - textField.Width) / 2, label1.Bottom + 6);
                label11.Location = new Point((customForm.ClientSize.Width - label11.Width) / 2, textField1.Bottom + 15);
                textField11.Location = new Point((customForm.ClientSize.Width - textField.Width) / 2, label11.Bottom + 6);
                label2.Location = new Point((customForm.ClientSize.Width - label2.Width) / 2, textField11.Bottom + 20);
                continueButton.Location = new Point((customForm.ClientSize.Width - continueButton.Width) / 2, label2.Bottom + 20);
            };
            customForm.ShowDialog();
        }
    }
}
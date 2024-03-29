namespace ESL_Management_System
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exportESLdb = new ToolStripMenuItem();
            quit_item = new ToolStripMenuItem();
            eSLToolStripMenuItem = new ToolStripMenuItem();
            addToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            dataGridView1 = new DataGridView();
            label4 = new Label();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            label5 = new Label();
            sodtware_status_label = new Label();
            button6 = new Button();
            progressBar1 = new ProgressBar();
            label7 = new Label();
            toolStripContainer1 = new ToolStripContainer();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            toolStripContainer1.ContentPanel.SuspendLayout();
            toolStripContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Black;
            pictureBox1.Location = new Point(908, 75);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(320, 240);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = Color.Black;
            pictureBox2.Location = new Point(908, 364);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(320, 240);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.Black;
            pictureBox3.Location = new Point(908, 653);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(320, 240);
            pictureBox3.TabIndex = 3;
            pictureBox3.TabStop = false;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = Color.FromArgb(32, 32, 32);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, eSLToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1259, 24);
            menuStrip1.TabIndex = 4;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportESLdb, quit_item });
            fileToolStripMenuItem.ForeColor = Color.FromArgb(238, 238, 238);
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            fileToolStripMenuItem.Click += fileToolStripMenuItem_Click;
            // 
            // exportESLdb
            // 
            exportESLdb.Name = "exportESLdb";
            exportESLdb.Size = new Size(180, 22);
            exportESLdb.Text = "Export ESL Database";
            exportESLdb.Click += exportESLdb_Click;
            // 
            // quit_item
            // 
            quit_item.Name = "quit_item";
            quit_item.Size = new Size(180, 22);
            quit_item.Text = "Quit";
            quit_item.Click += quit_item_Click;
            // 
            // eSLToolStripMenuItem
            // 
            eSLToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addToolStripMenuItem, editToolStripMenuItem, deleteToolStripMenuItem, viewToolStripMenuItem });
            eSLToolStripMenuItem.ForeColor = Color.FromArgb(238, 238, 238);
            eSLToolStripMenuItem.Name = "eSLToolStripMenuItem";
            eSLToolStripMenuItem.Size = new Size(37, 20);
            eSLToolStripMenuItem.Text = "ESL";
            // 
            // addToolStripMenuItem
            // 
            addToolStripMenuItem.Name = "addToolStripMenuItem";
            addToolStripMenuItem.Size = new Size(117, 22);
            addToolStripMenuItem.Text = "Add";
            addToolStripMenuItem.Click += addToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(117, 22);
            editToolStripMenuItem.Text = "Edit";
            editToolStripMenuItem.Click += editToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(117, 22);
            deleteToolStripMenuItem.Text = "Remove";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(117, 22);
            viewToolStripMenuItem.Text = "View";
            viewToolStripMenuItem.Click += viewToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.ForeColor = Color.FromArgb(238, 238, 238);
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.FromArgb(238, 238, 238);
            label1.Location = new Point(908, 331);
            label1.Name = "label1";
            label1.Size = new Size(116, 30);
            label1.TabIndex = 5;
            label1.Text = "Encrypted:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.FromArgb(238, 238, 238);
            label2.Location = new Point(908, 620);
            label2.Name = "label2";
            label2.Size = new Size(111, 30);
            label2.TabIndex = 6;
            label2.Text = "Displayed:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = Color.FromArgb(238, 238, 238);
            label3.Location = new Point(908, 42);
            label3.Name = "label3";
            label3.Size = new Size(92, 30);
            label3.TabIndex = 7;
            label3.Text = "Preview:";
            // 
            // dataGridView1
            // 
            dataGridView1.BackgroundColor = Color.FromArgb(45, 45, 45);
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.GridColor = Color.FromArgb(99, 1, 165);
            dataGridView1.Location = new Point(0, 0);
            dataGridView1.Margin = new Padding(0);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowTemplate.Height = 25;
            dataGridView1.Size = new Size(567, 859);
            dataGridView1.TabIndex = 8;
            dataGridView1.CurrentCellChanged += dataGridView1_CurrentCellChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = Color.FromArgb(238, 238, 238);
            label4.Location = new Point(34, 42);
            label4.Name = "label4";
            label4.Size = new Size(59, 30);
            label4.TabIndex = 9;
            label4.Text = "ESLs:";
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(238, 238, 238);
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            button1.ForeColor = Color.FromArgb(44, 44, 44);
            button1.Location = new Point(649, 75);
            button1.Name = "button1";
            button1.Size = new Size(211, 38);
            button1.TabIndex = 10;
            button1.Text = "Add ESL";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(0, 152, 136);
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            button2.ForeColor = Color.FromArgb(44, 44, 44);
            button2.Location = new Point(649, 119);
            button2.Name = "button2";
            button2.Size = new Size(211, 38);
            button2.TabIndex = 11;
            button2.Text = "Edit ESL Settings";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(162, 231, 0);
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            button3.ForeColor = Color.FromArgb(44, 44, 44);
            button3.Location = new Point(649, 163);
            button3.Name = "button3";
            button3.Size = new Size(211, 38);
            button3.TabIndex = 12;
            button3.Text = "Select Image for ESL";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.FromArgb(28, 203, 0);
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            button4.ForeColor = Color.FromArgb(44, 44, 44);
            button4.Location = new Point(649, 207);
            button4.Name = "button4";
            button4.Size = new Size(211, 38);
            button4.TabIndex = 13;
            button4.Text = "Send Image to ESL";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.BackColor = Color.FromArgb(239, 0, 18);
            button5.FlatStyle = FlatStyle.Flat;
            button5.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            button5.ForeColor = Color.FromArgb(44, 44, 44);
            button5.Location = new Point(649, 364);
            button5.Name = "button5";
            button5.Size = new Size(211, 38);
            button5.TabIndex = 14;
            button5.Text = "Remove ESL from DB";
            button5.UseVisualStyleBackColor = false;
            button5.Click += button5_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = Color.FromArgb(238, 238, 238);
            label5.Location = new Point(649, 789);
            label5.Name = "label5";
            label5.Size = new Size(167, 30);
            label5.TabIndex = 15;
            label5.Text = "Software Status:";
            // 
            // sodtware_status_label
            // 
            sodtware_status_label.AutoSize = true;
            sodtware_status_label.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            sodtware_status_label.ForeColor = Color.FromArgb(238, 238, 238);
            sodtware_status_label.Location = new Point(649, 822);
            sodtware_status_label.Name = "sodtware_status_label";
            sodtware_status_label.Size = new Size(0, 30);
            sodtware_status_label.TabIndex = 16;
            // 
            // button6
            // 
            button6.BackColor = Color.FromArgb(44, 44, 44);
            button6.FlatStyle = FlatStyle.Flat;
            button6.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            button6.ForeColor = Color.FromArgb(44, 44, 44);
            button6.Location = new Point(649, 855);
            button6.Name = "button6";
            button6.Size = new Size(211, 38);
            button6.TabIndex = 17;
            button6.UseVisualStyleBackColor = false;
            button6.Click += button6_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(649, 578);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(211, 26);
            progressBar1.TabIndex = 18;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI Semibold", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            label7.ForeColor = Color.FromArgb(238, 238, 238);
            label7.Location = new Point(649, 545);
            label7.Name = "label7";
            label7.Size = new Size(177, 30);
            label7.TabIndex = 19;
            label7.Text = "Upload Progress:";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            toolStripContainer1.ContentPanel.Controls.Add(dataGridView1);
            toolStripContainer1.ContentPanel.Size = new Size(566, 793);
            toolStripContainer1.Location = new Point(34, 75);
            toolStripContainer1.Name = "toolStripContainer1";
            toolStripContainer1.Size = new Size(566, 818);
            toolStripContainer1.TabIndex = 20;
            toolStripContainer1.Text = "toolStripContainer1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(99, 1, 165);
            ClientSize = new Size(1259, 925);
            Controls.Add(toolStripContainer1);
            Controls.Add(label7);
            Controls.Add(progressBar1);
            Controls.Add(button6);
            Controls.Add(sodtware_status_label);
            Controls.Add(label5);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Electronic Shelf Label Management System";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            toolStripContainer1.ContentPanel.ResumeLayout(false);
            toolStripContainer1.ResumeLayout(false);
            toolStripContainer1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private MenuStrip menuStrip1;
        private Label label1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private Label label2;
        private Label label3;
        private ToolStripMenuItem eSLToolStripMenuItem;
        private DataGridView dataGridView1;
        private Label label4;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Label label5;
        private Label sodtware_status_label;
        private Button button6;
        private ProgressBar progressBar1;
        private Label label7;
        private ToolStripMenuItem exportESLdb;
        private ToolStripMenuItem quit_item;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripContainer toolStripContainer1;
    }
}
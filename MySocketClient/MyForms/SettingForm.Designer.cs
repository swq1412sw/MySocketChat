namespace MySocketClient
{
    partial class SettingForm
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
            label1 = new Label();
            textBox1 = new TextBox();
            comboBox1 = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            textBox2 = new TextBox();
            numericUpDown1 = new NumericUpDown();
            label4 = new Label();
            label5 = new Label();
            button1 = new Button();
            label6 = new Label();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            label7 = new Label();
            label8 = new Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(24, 21);
            label1.Name = "label1";
            label1.Size = new Size(64, 24);
            label1.TabIndex = 0;
            label1.Text = "用户名";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(106, 20);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(150, 30);
            textBox1.TabIndex = 1;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(106, 65);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(182, 32);
            comboBox1.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(24, 68);
            label2.Name = "label2";
            label2.Size = new Size(82, 24);
            label2.TabIndex = 3;
            label2.Text = "绑定类型";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(306, 67);
            label3.Name = "label3";
            label3.Size = new Size(26, 24);
            label3.TabIndex = 4;
            label3.Text = "IP";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(347, 64);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(150, 30);
            textBox2.TabIndex = 5;
            // 
            // numericUpDown1
            // 
            numericUpDown1.Location = new Point(576, 64);
            numericUpDown1.Maximum = new decimal(new int[] { 49151, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 1024, 0, 0, 0 });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(122, 30);
            numericUpDown1.TabIndex = 6;
            numericUpDown1.Value = new decimal(new int[] { 1024, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(515, 67);
            label4.Name = "label4";
            label4.Size = new Size(46, 24);
            label4.TabIndex = 7;
            label4.Text = "端口";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(24, 130);
            label5.Name = "label5";
            label5.Size = new Size(118, 24);
            label5.TabIndex = 8;
            label5.Text = "对方消息颜色";
            // 
            // button1
            // 
            button1.Location = new Point(148, 125);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 9;
            button1.TabStop = false;
            button1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(306, 130);
            label6.Name = "label6";
            label6.Size = new Size(118, 24);
            label6.TabIndex = 10;
            label6.Text = "我方消息颜色";
            // 
            // button2
            // 
            button2.Location = new Point(430, 125);
            button2.Name = "button2";
            button2.Size = new Size(112, 34);
            button2.TabIndex = 11;
            button2.TabStop = false;
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(265, 186);
            button3.Name = "button3";
            button3.Size = new Size(112, 34);
            button3.TabIndex = 12;
            button3.Text = "保存";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(449, 186);
            button4.Name = "button4";
            button4.Size = new Size(112, 34);
            button4.TabIndex = 13;
            button4.Text = "取消";
            button4.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(597, 123);
            label7.Name = "label7";
            label7.Size = new Size(63, 24);
            label7.TabIndex = 14;
            label7.Text = "label7";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(595, 161);
            label8.Name = "label8";
            label8.Size = new Size(63, 24);
            label8.TabIndex = 15;
            label8.Text = "label8";
            // 
            // SettingForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(818, 224);
            ControlBox = false;
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label6);
            Controls.Add(button1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(numericUpDown1);
            Controls.Add(textBox2);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(comboBox1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingForm";
            ShowIcon = false;
            Text = "设置";
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox textBox1;
        private ComboBox comboBox1;
        private Label label2;
        private Label label3;
        private TextBox textBox2;
        private NumericUpDown numericUpDown1;
        private Label label4;
        private Label label5;
        private Button button1;
        private Label label6;
        private Button button2;
        private Button button3;
        private Button button4;
        private Label label7;
        private Label label8;
    }
}
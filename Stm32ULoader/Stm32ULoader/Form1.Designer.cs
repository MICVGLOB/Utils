namespace Stm32ULoader
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.SerialPortSelector = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ReadSerialButton = new System.Windows.Forms.Button();
            this.CancelOperationButton = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.ModbusAddress = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.Progress = new System.Windows.Forms.ProgressBar();
            this.SerialPortStateConnection = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SerialNumber = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ChangeAddressButton = new System.Windows.Forms.Button();
            this.ChangeSerialButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.EnableIdChange = new System.Windows.Forms.CheckBox();
            this.NewModbusAddress = new System.Windows.Forms.NumericUpDown();
            this.NewModbusLabel = new System.Windows.Forms.Label();
            this.NewSerialNumber = new System.Windows.Forms.NumericUpDown();
            this.NewSerialLabel = new System.Windows.Forms.Label();
            this.TextWindow = new System.Windows.Forms.RichTextBox();
            this.FileName = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.McuType = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.LoadFileButton = new System.Windows.Forms.Button();
            this.OpenFlashFileButton = new System.Windows.Forms.Button();
            this.BaudrateForLoad = new System.Windows.Forms.ComboBox();
            this.OpenFlashFile = new System.Windows.Forms.OpenFileDialog();
            this.label13 = new System.Windows.Forms.Label();
            this.SerialPort = new System.IO.Ports.SerialPort(this.components);
            this.RewriteTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModbusAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SerialNumber)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NewModbusAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NewSerialNumber)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // SerialPortSelector
            // 
            this.SerialPortSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SerialPortSelector.FormattingEnabled = true;
            this.SerialPortSelector.Location = new System.Drawing.Point(114, 138);
            this.SerialPortSelector.Name = "SerialPortSelector";
            this.SerialPortSelector.Size = new System.Drawing.Size(72, 21);
            this.SerialPortSelector.TabIndex = 0;
            this.SerialPortSelector.SelectedIndexChanged += new System.EventHandler(this.SerialPortSelector_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ReadSerialButton);
            this.groupBox1.Controls.Add(this.CancelOperationButton);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.ModbusAddress);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.Progress);
            this.groupBox1.Controls.Add(this.SerialPortStateConnection);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.SerialNumber);
            this.groupBox1.Controls.Add(this.SerialPortSelector);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(391, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(210, 341);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Связь с блоком";
            // 
            // ReadSerialButton
            // 
            this.ReadSerialButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ReadSerialButton.Location = new System.Drawing.Point(20, 100);
            this.ReadSerialButton.Name = "ReadSerialButton";
            this.ReadSerialButton.Size = new System.Drawing.Size(166, 23);
            this.ReadSerialButton.TabIndex = 7;
            this.ReadSerialButton.Text = "Читать серийный номер";
            this.ReadSerialButton.UseVisualStyleBackColor = true;
            this.ReadSerialButton.Click += new System.EventHandler(this.ReadSerialButton_Click);
            // 
            // CancelOperationButton
            // 
            this.CancelOperationButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelOperationButton.Location = new System.Drawing.Point(20, 301);
            this.CancelOperationButton.Name = "CancelOperationButton";
            this.CancelOperationButton.Size = new System.Drawing.Size(166, 23);
            this.CancelOperationButton.TabIndex = 5;
            this.CancelOperationButton.Text = "Отменить";
            this.CancelOperationButton.UseVisualStyleBackColor = true;
            this.CancelOperationButton.Click += new System.EventHandler(this.CancelOperationButton_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 66);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(83, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Адрес MODBUS";
            // 
            // ModbusAddress
            // 
            this.ModbusAddress.Location = new System.Drawing.Point(114, 64);
            this.ModbusAddress.Maximum = new decimal(new int[] {
            126,
            0,
            0,
            0});
            this.ModbusAddress.Name = "ModbusAddress";
            this.ModbusAddress.Size = new System.Drawing.Size(72, 20);
            this.ModbusAddress.TabIndex = 8;
            this.ModbusAddress.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 234);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Выполнение команды";
            // 
            // Progress
            // 
            this.Progress.Location = new System.Drawing.Point(20, 261);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(166, 23);
            this.Progress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.Progress.TabIndex = 5;
            // 
            // SerialPortStateConnection
            // 
            this.SerialPortStateConnection.BackColor = System.Drawing.SystemColors.Control;
            this.SerialPortStateConnection.Location = new System.Drawing.Point(20, 198);
            this.SerialPortStateConnection.Name = "SerialPortStateConnection";
            this.SerialPortStateConnection.ReadOnly = true;
            this.SerialPortStateConnection.Size = new System.Drawing.Size(166, 20);
            this.SerialPortStateConnection.TabIndex = 2;
            this.SerialPortStateConnection.Text = "Связь не установлена";
            this.SerialPortStateConnection.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 172);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Статус связи с COM-портом";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 141);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "COM-порт";
            // 
            // SerialNumber
            // 
            this.SerialNumber.Location = new System.Drawing.Point(114, 30);
            this.SerialNumber.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.SerialNumber.Name = "SerialNumber";
            this.SerialNumber.Size = new System.Drawing.Size(72, 20);
            this.SerialNumber.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Серийный номер";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ChangeAddressButton);
            this.groupBox2.Controls.Add(this.ChangeSerialButton);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.EnableIdChange);
            this.groupBox2.Controls.Add(this.NewModbusAddress);
            this.groupBox2.Controls.Add(this.NewModbusLabel);
            this.groupBox2.Controls.Add(this.NewSerialNumber);
            this.groupBox2.Controls.Add(this.NewSerialLabel);
            this.groupBox2.Location = new System.Drawing.Point(12, 359);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(403, 100);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Перезапись идентификаторов";
            // 
            // ChangeAddressButton
            // 
            this.ChangeAddressButton.Enabled = false;
            this.ChangeAddressButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChangeAddressButton.Location = new System.Drawing.Point(217, 62);
            this.ChangeAddressButton.Name = "ChangeAddressButton";
            this.ChangeAddressButton.Size = new System.Drawing.Size(88, 23);
            this.ChangeAddressButton.TabIndex = 5;
            this.ChangeAddressButton.Text = "Изменить";
            this.ChangeAddressButton.UseVisualStyleBackColor = true;
            this.ChangeAddressButton.Click += new System.EventHandler(this.ChangeAddressButton_Click);
            // 
            // ChangeSerialButton
            // 
            this.ChangeSerialButton.Enabled = false;
            this.ChangeSerialButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ChangeSerialButton.Location = new System.Drawing.Point(217, 27);
            this.ChangeSerialButton.Name = "ChangeSerialButton";
            this.ChangeSerialButton.Size = new System.Drawing.Size(88, 23);
            this.ChangeSerialButton.TabIndex = 4;
            this.ChangeSerialButton.Text = "Изменить";
            this.ChangeSerialButton.UseVisualStyleBackColor = true;
            this.ChangeSerialButton.Click += new System.EventHandler(this.ChangeSerialButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(320, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 7;
            this.label7.Text = "Разрешить";
            // 
            // EnableIdChange
            // 
            this.EnableIdChange.AutoSize = true;
            this.EnableIdChange.Location = new System.Drawing.Point(356, 66);
            this.EnableIdChange.Name = "EnableIdChange";
            this.EnableIdChange.Size = new System.Drawing.Size(15, 14);
            this.EnableIdChange.TabIndex = 5;
            this.EnableIdChange.UseVisualStyleBackColor = true;
            this.EnableIdChange.CheckedChanged += new System.EventHandler(this.EnableIdChange_CheckedChanged);
            // 
            // NewModbusAddress
            // 
            this.NewModbusAddress.Enabled = false;
            this.NewModbusAddress.Location = new System.Drawing.Point(119, 64);
            this.NewModbusAddress.Maximum = new decimal(new int[] {
            126,
            0,
            0,
            0});
            this.NewModbusAddress.Name = "NewModbusAddress";
            this.NewModbusAddress.Size = new System.Drawing.Size(72, 20);
            this.NewModbusAddress.TabIndex = 4;
            this.NewModbusAddress.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // NewModbusLabel
            // 
            this.NewModbusLabel.AutoSize = true;
            this.NewModbusLabel.Enabled = false;
            this.NewModbusLabel.Location = new System.Drawing.Point(20, 67);
            this.NewModbusLabel.Name = "NewModbusLabel";
            this.NewModbusLabel.Size = new System.Drawing.Size(83, 13);
            this.NewModbusLabel.TabIndex = 3;
            this.NewModbusLabel.Text = "Адрес MODBUS";
            // 
            // NewSerialNumber
            // 
            this.NewSerialNumber.Enabled = false;
            this.NewSerialNumber.Location = new System.Drawing.Point(119, 30);
            this.NewSerialNumber.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NewSerialNumber.Name = "NewSerialNumber";
            this.NewSerialNumber.Size = new System.Drawing.Size(72, 20);
            this.NewSerialNumber.TabIndex = 3;
            this.NewSerialNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // NewSerialLabel
            // 
            this.NewSerialLabel.AutoSize = true;
            this.NewSerialLabel.Enabled = false;
            this.NewSerialLabel.Location = new System.Drawing.Point(20, 32);
            this.NewSerialLabel.Name = "NewSerialLabel";
            this.NewSerialLabel.Size = new System.Drawing.Size(91, 13);
            this.NewSerialLabel.TabIndex = 2;
            this.NewSerialLabel.Text = "Серийный номер";
            // 
            // TextWindow
            // 
            this.TextWindow.BackColor = System.Drawing.SystemColors.Control;
            this.TextWindow.Location = new System.Drawing.Point(23, 104);
            this.TextWindow.Name = "TextWindow";
            this.TextWindow.ReadOnly = true;
            this.TextWindow.Size = new System.Drawing.Size(320, 169);
            this.TextWindow.TabIndex = 3;
            this.TextWindow.Text = "";
            // 
            // FileName
            // 
            this.FileName.BackColor = System.Drawing.SystemColors.Control;
            this.FileName.Location = new System.Drawing.Point(23, 68);
            this.FileName.Name = "FileName";
            this.FileName.ReadOnly = true;
            this.FileName.Size = new System.Drawing.Size(320, 20);
            this.FileName.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.McuType);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.LoadFileButton);
            this.groupBox3.Controls.Add(this.OpenFlashFileButton);
            this.groupBox3.Controls.Add(this.FileName);
            this.groupBox3.Controls.Add(this.TextWindow);
            this.groupBox3.Controls.Add(this.BaudrateForLoad);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(363, 341);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Программирование";
            // 
            // McuType
            // 
            this.McuType.AutoCompleteCustomSource.AddRange(new string[] {
            "1"});
            this.McuType.BackColor = System.Drawing.SystemColors.Window;
            this.McuType.DisplayMember = "0";
            this.McuType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.McuType.FormattingEnabled = true;
            this.McuType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.McuType.Location = new System.Drawing.Point(181, 279);
            this.McuType.Name = "McuType";
            this.McuType.Size = new System.Drawing.Size(162, 21);
            this.McuType.TabIndex = 8;
            this.McuType.SelectedIndexChanged += new System.EventHandler(this.McuType_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(20, 308);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(155, 13);
            this.label12.TabIndex = 10;
            this.label12.Text = "Скорость загрузки ПО, бит/с";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.BackColor = System.Drawing.SystemColors.Control;
            this.label11.Location = new System.Drawing.Point(20, 282);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(124, 13);
            this.label11.TabIndex = 10;
            this.label11.Text = "Тип микроконтроллера";
            // 
            // LoadFileButton
            // 
            this.LoadFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoadFileButton.Location = new System.Drawing.Point(255, 30);
            this.LoadFileButton.Name = "LoadFileButton";
            this.LoadFileButton.Size = new System.Drawing.Size(88, 23);
            this.LoadFileButton.TabIndex = 8;
            this.LoadFileButton.Text = "Загрузить";
            this.LoadFileButton.UseVisualStyleBackColor = true;
            this.LoadFileButton.Click += new System.EventHandler(this.LoadFileButton_Click);
            // 
            // OpenFlashFileButton
            // 
            this.OpenFlashFileButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.OpenFlashFileButton.Location = new System.Drawing.Point(23, 30);
            this.OpenFlashFileButton.Name = "OpenFlashFileButton";
            this.OpenFlashFileButton.Size = new System.Drawing.Size(88, 23);
            this.OpenFlashFileButton.TabIndex = 8;
            this.OpenFlashFileButton.Text = "Файл";
            this.OpenFlashFileButton.UseVisualStyleBackColor = true;
            this.OpenFlashFileButton.Click += new System.EventHandler(this.OpenFlashFileButton_Click);
            // 
            // BaudrateForLoad
            // 
            this.BaudrateForLoad.BackColor = System.Drawing.SystemColors.Window;
            this.BaudrateForLoad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BaudrateForLoad.FormattingEnabled = true;
            this.BaudrateForLoad.Items.AddRange(new object[] {
            "9600",
            "19200",
            "38400",
            "57600"});
            this.BaudrateForLoad.Location = new System.Drawing.Point(181, 305);
            this.BaudrateForLoad.Name = "BaudrateForLoad";
            this.BaudrateForLoad.Size = new System.Drawing.Size(162, 21);
            this.BaudrateForLoad.TabIndex = 1;
            // 
            // OpenFlashFile
            // 
            this.OpenFlashFile.FileName = "openFileDialog1";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(431, 409);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(138, 13);
            this.label13.TabIndex = 7;
            this.label13.Text = "(с) ЗАО \"Техно-Т\", г. Тула";
            // 
            // RewriteTimer
            // 
            this.RewriteTimer.Interval = 2000;
            this.RewriteTimer.Tick += new System.EventHandler(this.RewriteTimer_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(618, 476);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "Универсальный загрузчик STM32F0X, 1X  (v.1.0, апрель 2016 г.)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ModbusAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SerialNumber)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NewModbusAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NewSerialNumber)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox SerialPortSelector;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox SerialPortStateConnection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown SerialNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ProgressBar Progress;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox EnableIdChange;
        private System.Windows.Forms.NumericUpDown NewModbusAddress;
        private System.Windows.Forms.Label NewModbusLabel;
        private System.Windows.Forms.NumericUpDown NewSerialNumber;
        private System.Windows.Forms.Label NewSerialLabel;
        private System.Windows.Forms.RichTextBox TextWindow;
        private System.Windows.Forms.Button ChangeSerialButton;
        private System.Windows.Forms.Button CancelOperationButton;
        private System.Windows.Forms.Button ChangeAddressButton;
        private System.Windows.Forms.Button ReadSerialButton;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown ModbusAddress;
        private System.Windows.Forms.TextBox FileName;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button LoadFileButton;
        private System.Windows.Forms.Button OpenFlashFileButton;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox BaudrateForLoad;
        private System.Windows.Forms.OpenFileDialog OpenFlashFile;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox McuType;
        private System.IO.Ports.SerialPort SerialPort;
        private System.Windows.Forms.Timer RewriteTimer;
    }
}


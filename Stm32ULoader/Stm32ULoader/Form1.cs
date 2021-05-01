using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;


namespace Stm32ULoader {
    public partial class Form1 : Form {
        private string ComPortWasOpen = "";
        private bool ComPortState = false;
        string CurrentCommand = "";
        Int32 CommandProgressTimeout = 0;
        Int32 CounterByteOld = 0;
        BootCoreType BootCore = new BootCoreType();
        string[] SupportedDevices = new string[] { "STM32F051C8 (IUG-5)", "STM32F103CB (GPRS-02)" };

        /*-------------------------------------- */
        /*-------------- Init form1 -------------*/
        /*-------------------------------------- */
        public Form1() {


            InitializeComponent();

            foreach(var item in SupportedDevices)
                this.McuType.Items.Add(item);
            this.McuType.SelectedItem = this.McuType.Items[0];
            this.BaudrateForLoad.SelectedItem = this.BaudrateForLoad.Items[this.BaudrateForLoad.Items.Count - 1];
            SetDefaultModbusAddress();

            string[] ports = SerialPort.GetPortNames();
            IEnumerable<string> PortsSort = from port in ports
                                            orderby port
                                            select port;
            if(ports.Length == 0) {
                SerialPortStateConnection.ForeColor = Color.Red;
                SerialPortStateConnection.Text = "Порты отсутствуют!";
                SerialPortSelector.Enabled = false;
            } else {
                foreach(string s in PortsSort)
                    SerialPortSelector.Items.Add(s);
            }

            try {
                StreamReader sr = new StreamReader("PathToFlashFile.txt", Encoding.Unicode);
                FileName.Text = sr.ReadLine();
                sr.Close();
            } catch(Exception) {
                return;
            }
        }

        void McuType_SelectedIndexChanged(object sender, EventArgs e) {
            SetDefaultModbusAddress();
        }

        void SetDefaultModbusAddress() {
            string localTypeName = McuType.SelectedItem.ToString();
            if(localTypeName.Contains("IUG-5")) {
                ModbusAddress.Value = 10;
            } else if(localTypeName.Contains("GPRS-02")) {
                ModbusAddress.Value = 60;
            }
        }

        /*-------------------------------------- */
        /*-------------- Gui getting data--------*/
        /*-------------------------------------- */

        void GuiGettingData() {
            BootCore.SerialNumber = (UInt32)SerialNumber.Value;
            BootCore.NewSerialNumber = (UInt32)NewSerialNumber.Value;
            BootCore.ModbusAddress = (byte)ModbusAddress.Value;
            BootCore.NewModbusAddress = (byte)NewModbusAddress.Value;
            BootCore.FlashBaseAddress = 0x08000000;
            BootCore.ModbusBitrate = 9600;
            BootCore.NewModbusBitrate = Convert.ToUInt16(BaudrateForLoad.Items[BaudrateForLoad.SelectedIndex].ToString());
            string localTypeName = McuType.SelectedItem.ToString();

            if(localTypeName.Contains("IUG-5")) {
                BootCore.FlashPageSize = 1024;
                BootCore.ModbusAddressFlashAddress = BootCore.FlashBaseAddress + 17 * (UInt32)BootCore.FlashPageSize;
                // page 18 - for full writing flag!
                // page 19 - for coefficients!
                BootCore.FlashBaseAddressForMainProgram = BootCore.FlashBaseAddress + 20 * (UInt32)BootCore.FlashPageSize;
            } else if(localTypeName.Contains("GPRS-02")) {
                BootCore.FlashPageSize = 1024;
                BootCore.ModbusAddressFlashAddress = BootCore.FlashBaseAddress + 32 * (UInt32)BootCore.FlashPageSize; // page 33 - for full writing flag!
                BootCore.FlashBaseAddressForMainProgram = BootCore.FlashBaseAddress + 34 * (UInt32)BootCore.FlashPageSize;
            } else {
                BootCore.FlashPageSize = 2048;
                BootCore.ModbusAddressFlashAddress = BootCore.FlashBaseAddress + 16 * (UInt32)BootCore.FlashPageSize;
                BootCore.FlashBaseAddressForMainProgram = BootCore.FlashBaseAddress + 18 * (UInt32)BootCore.FlashPageSize;  // page 17 - for full writing flag!
            }

            BootCore.SerialNumberFlashAddress = BootCore.ModbusAddressFlashAddress + 4;
            BootCore.FlashPageForErasing = 0;
            BootCore.FlashFragmentNumber = 0;
        }


        bool SimpleTRSerial(float Time_Wait = (float)1.0)   //Выполнение простой команды записи/чтения RS485
        {
            // N_TX - количество байт на передачу, c учетом CRC
            // N_RX - количество байт, которые должны быть приняты (c учетом CRC). Если 
            // N_RX = 65535, то длину принятого сообщения не проверять.
            // Time_Wait -  максимальное время, за которое должны быть прочитаны данные

            UInt16 w = 0, w_old = 0;
            UInt16 count_0 = 0;

            SerialPort.DiscardOutBuffer();
            SerialPort.DiscardInBuffer();
            BootCore.ReceiveDataLength = 0;

            SerialPort.Write(BootCore.TransmitData, 0, BootCore.TransmitDataLength);

            while(true) // Ждем приема данных по COM-порту
          {
                Thread.Sleep((int)(100 * Time_Wait));   // задержка 100 мс
                w = (UInt16)SerialPort.BytesToRead;		   //Количество байт в буфере
                if(w > 4) {
                    if(w_old == w)
                        break;
                    w_old = w;
                }
                if((++count_0) > 10) {
                    return true;
                }
            }

            if(w > 4) {
                SerialPort.Read(BootCore.ReceiveData, 0, w);
                BootCore.ReceiveDataLength = w;
                return false;
            }
            return true;
        }
        /*-------------------------------------- */
        /*------------Open file-----------------*/
        /*-------------------------------------- */

        private void OpenFlashFileButton_Click(object sender, EventArgs e) {
            GuiGettingData(); // for test only!!
            string defaultDirectoryName = "";
            try {
                using(StreamReader sr = new StreamReader("PathToFlashFile.txt", Encoding.Unicode)) {
                    defaultDirectoryName = Path.GetDirectoryName(sr.ReadLine());
                }
            } catch(Exception) { }

            OpenFileDialog OpenFlashFileDialog = new OpenFileDialog();
            OpenFlashFileDialog.InitialDirectory = string.IsNullOrEmpty(defaultDirectoryName) ? "c:\\" : defaultDirectoryName;
            OpenFlashFileDialog.Filter = "Binary files (*.bin)|*.bin";
            OpenFlashFileDialog.FilterIndex = 2;
            OpenFlashFileDialog.RestoreDirectory = true;

            if(OpenFlashFileDialog.ShowDialog() == DialogResult.OK) {
                try {
                    FileName.Text = OpenFlashFileDialog.FileName;
                } catch(Exception) {
                    TextWindow.Text = "Чтение невозможно! Файл не существует, защищен от чтения или поврежден.\n";
                    return;
                }

                try {
                    StreamWriter sw = new StreamWriter("PathToFlashFile.txt", false, Encoding.Unicode);
                    sw.WriteLine(FileName.Text);
                    sw.Close();
                } catch(Exception) {
                    return;
                }
            }
        }

        /*-------------------------------------- */
        /*-------------- Enable ID Change--------*/
        /*-------------------------------------- */

        private void EnableIdChange_CheckedChanged(object sender, EventArgs e) {
            if(EnableIdChange.Checked) {
                NewModbusAddress.Enabled = true;
                NewSerialNumber.Enabled = true;
                NewSerialLabel.Enabled = true;
                NewModbusLabel.Enabled = true;
                ChangeAddressButton.Enabled = true;
                ChangeSerialButton.Enabled = true;
            } else {
                NewModbusAddress.Enabled =

                NewModbusAddress.Enabled = false;
                NewSerialNumber.Enabled = false;
                NewSerialLabel.Enabled = false;
                NewModbusLabel.Enabled = false;
                ChangeAddressButton.Enabled = false;
                ChangeSerialButton.Enabled = false;
            }
        }

        private void SerialConfig(string PName, int BaudR = 9600, StopBits Stb = StopBits.Two, Parity Prt = Parity.None) {
            SerialPort.BaudRate = BaudR;
            SerialPort.PortName = PName;
            SerialPort.Parity = Prt;
            SerialPort.StopBits = Stb;
        }


        private void SetStateOfConnect(string s, Color cl) {
            SerialPortStateConnection.ForeColor = cl;
            SerialPortStateConnection.Text = s;
        }

        /*-------------------------------------- */
        /*------- Serial port selector ----------*/
        /*-------------------------------------- */

        private void SerialPortSelector_SelectedIndexChanged(object sender, EventArgs e) {
            string cmpsel = SerialPortSelector.Items[SerialPortSelector.SelectedIndex].ToString();
            TextWindow.Clear();


            if(ComPortState)
                try {
                    SerialPort.Close();
                } catch(Exception) {
                    SetStateOfConnect("Откройте порт еще раз!", Color.Red);
                    ComPortWasOpen = "";
                    ComPortState = false;
                    return;
                }

            SerialConfig(cmpsel);
            try {
                SerialPort.Open();
            } catch(Exception) {
                SetStateOfConnect("Невозможно открыть порт!", Color.Red);
                ComPortWasOpen = "";
                ComPortState = false;
                return;
            }

            SetStateOfConnect("Порт открыт!", Color.Green);
            ComPortState = true;
            ComPortWasOpen = cmpsel;
        }

        /*-------------------------------------- */
        /*-------------- Load file button--------*/
        /*-------------------------------------- */
        private void LoadFileButton_Click(object sender, EventArgs e) {
            if(CurrentCommand != "")
                return;
            if(!ComPortState) {
                TextWindow.Text = "Последовательный порт не выбран (закрыт)!";
                return;
            }
            String FlashFileName = FileName.Text; // Open file
            if(FlashFileName == "")
                return; // no file!

            TextWindow.Clear();
            TextWindow.Text = "Чтение файла прошивки... \n";
            try {
                using(FileStream fs = new FileStream(FlashFileName, FileMode.Open, FileAccess.Read)) {
                    BootCore.FlashArray = new byte[fs.Length];
                    BootCore.FlashArrayLength = (Int32)fs.Length;
                    fs.Read(BootCore.FlashArray, 0, (int)fs.Length);
                }
                TextWindow.Text += "Файл успешно прочитан!\n";
                TextWindow.Text += "Размер прошивки " + BootCore.FlashArrayLength.ToString() + " байт.\n";
            } catch(Exception) {
                TextWindow.Text += "Чтение невозможно! Файл не существует, защищен от чтения или поврежден. \n";
                return;
            }

            GuiGettingData();
            BootCore.FlashPageForErasing = (UInt16)(((UInt32)BootCore.FlashArrayLength / (UInt32)BootCore.FlashPageSize) + 1);
            BootCore.LoaderMessageCreator("SET_PROGRAM_PARAMETERS");
            CurrentCommand = "SET_PROGRAM_PARAMETERS";
            TextWindow.Text += "Идет поиск микроконтроллера... \n";
            SendNewMessage();
            SetProgressBar(100, Color.Green);
            RewriteTimer.Interval = 400;
            RewriteTimer.Start();
        }

        private void SetProgressBar(byte ProgressLevel, Color ProgressColor) {
            Progress.ForeColor = ProgressColor;
            Progress.Value = ProgressLevel;
        }

        private void ReadSerialButton_Click(object sender, EventArgs e) {
            if(CurrentCommand != "")
                return;
            if(!ComPortState) {
                TextWindow.Text = "Последовательный порт не выбран (закрыт)!";
                return;
            }
            string localTypeName = McuType.SelectedItem.ToString();
            if(localTypeName.Contains("IUG-5")) {
                BootCore.ModbusReadMessageCreator((byte)ModbusAddress.Value, "03001e0001");
            } else if(localTypeName.Contains("GPRS-02")) {
                BootCore.ModbusReadMessageCreator((byte)ModbusAddress.Value, "0300000001");
            }
            SetProgressBar(50, Color.Green);
            UInt16 Stemp = 555;
            if(!SimpleTRSerial()) {
                if(!BootCore.ModbusOneRegReadVeryfier((byte)ModbusAddress.Value, ref Stemp)) {
                    SerialNumber.Value = Stemp;
                    NewSerialNumber.Value = Stemp;
                    NewModbusAddress.Value = ModbusAddress.Value;
                    GuiGettingData();
                    SetProgressBar(100, Color.Green);
                    return;
                }
            }
            SetProgressBar(100, Color.Red);
        }

        private void SendNewMessage() {
            SerialPort.DiscardOutBuffer();
            SerialPort.DiscardInBuffer();
            BootCore.ReceiveDataLength = 0;
            SerialPort.Write(BootCore.TransmitData, 0, BootCore.TransmitDataLength);
        }

        private void ChangeSerialButton_Click(object sender, EventArgs e) {
            if(CurrentCommand != "")
                return;
            if(!ComPortState) {
                TextWindow.Text = "Последовательный порт не выбран (закрыт)!";
                return;
            }
            GuiGettingData();
            BootCore.LoaderMessageCreator("SET_NEW_SERIAL");
            CurrentCommand = "SET_NEW_SERIAL";
            TextWindow.Text = "Идет поиск микроконтроллера... \n";
            SendNewMessage();

            SerialPort.DiscardInBuffer();

            SetProgressBar(100, Color.Green);
            RewriteTimer.Interval = 400;
            RewriteTimer.Start();
        }

        private void ChangeAddressButton_Click(object sender, EventArgs e) {
            if(CurrentCommand != "")
                return;
            if(!ComPortState) {
                TextWindow.Text = "Последовательный порт не выбран (закрыт)!";
                return;
            }
            GuiGettingData();
            BootCore.LoaderMessageCreator("SET_NEW_MODBUS_ADDRESS");
            CurrentCommand = "SET_NEW_MODBUS_ADDRESS";
            TextWindow.Text = "Идет поиск микроконтроллера... \n";
            SendNewMessage();
            SetProgressBar(100, Color.Green);
            RewriteTimer.Interval = 400;
            RewriteTimer.Start();
        }

        private void RewriteTimer_Tick(object sender, EventArgs e) {
            switch(CurrentCommand) {
                case "SET_NEW_MODBUS_ADDRESS":
                case "SET_NEW_SERIAL":
                    AddressAndSerialProgress();
                    break;
                case "SET_PROGRAM_PARAMETERS":
                    SetProgramParameters();
                    break;
                default:
                    FlashRewriteProgress();
                    break;
            }
        }

        private void SerialSpeedReconfig(int spd) {
            try {
                SerialPort.BaudRate = spd;
            } catch(Exception) {
                SetStateOfConnect("Ошибка изменения скорости!", Color.Red);
                ComPortWasOpen = "";
                ComPortState = false;
                return;
            }
        }

        private void CancelOperationButton_Click(object sender, EventArgs e) {
            if(CurrentCommand == "")
                return;
            if(SerialPort.BaudRate != 9600) {
                SerialSpeedReconfig(9600);
            }
            CurrentCommand = "";
            RewriteTimer.Stop();
            SetProgressBar(100, Color.Yellow);
            TextWindow.Text = "Операция остановлена...\n ";
        }

        private void AddressAndSerialProgress() {
            UInt16 w = (UInt16)SerialPort.BytesToRead;
            if(Progress.ForeColor == Color.Green) {
                Progress.ForeColor = Color.Yellow;
            } else {
                Progress.ForeColor = Color.Green;
            }
            if(w >= 4) {
                SerialPort.Read(BootCore.ReceiveData, 0, w);
                BootCore.ReceiveDataLength = w;
                if(!BootCore.CompactAnswerVeryfier(BootCore.CommandIdFinder(CurrentCommand))) {
                    UInt16 tmp2 = (UInt16)(BootCore.RLoaderVersion / 10);
                    TextWindow.Text += "Микроконтроллер обнаружен!\n";
                    TextWindow.Text += "Версия ПО загрузчика: " + (tmp2).ToString() + "."
                        + (BootCore.RLoaderVersion - tmp2 * 10).ToString() + ".\nПерезапись";
                    TextWindow.Text += (CurrentCommand == "SET_NEW_MODBUS_ADDRESS") ? " адреса Modbus" : " серийного номера";
                    TextWindow.Text += " успешно завершена!";
                    if(CurrentCommand == "SET_NEW_MODBUS_ADDRESS") {
                        ModbusAddress.Value = NewModbusAddress.Value;
                    } else {
                        SerialNumber.Value = NewSerialNumber.Value;
                    }
                    CurrentCommand = "";
                    RewriteTimer.Stop();
                    SetProgressBar(100, Color.Green);
                } else {
                    SendNewMessage();
                }
            } else {
                SendNewMessage();
            }
        }

        private void SetProgramParameters() {
            UInt16 w = (UInt16)SerialPort.BytesToRead;
            if(Progress.ForeColor == Color.Green) {
                Progress.ForeColor = Color.Yellow;
            } else {
                Progress.ForeColor = Color.Green;
            }
            if(w >= 4) {
                SerialPort.Read(BootCore.ReceiveData, 0, w);
                BootCore.ReceiveDataLength = w;
                if(!BootCore.ExpandedAnswerVeryfier(BootCore.CommandIdFinder(CurrentCommand))) {
                    UInt16 tmp2 = (UInt16)(BootCore.RLoaderVersion / 10);
                    TextWindow.Text += "Микроконтроллер обнаружен!\n";
                    TextWindow.Text += "Версия ПО загрузчика: " + (tmp2).ToString() + "."
                        + (BootCore.RLoaderVersion - tmp2 * 10).ToString() + ".\n";
                    TextWindow.Text += "Уникальный номер микроконтроллера: 0x" + BootCore.RIDMcuHi.ToString("X8")
                        + BootCore.RIDMcuLo.ToString("X16") + ".\n";
                    TextWindow.Text += "Адрес Modbus: " + BootCore.NewModbusAddress.ToString() + ".\n";
                    TextWindow.Text += "Текущая скорость обмена данными: " + BootCore.RModbusBitrate.ToString() + " бит/с.\n";
                    TextWindow.Text += "Размер страницы Flash-памяти: " + BootCore.RFlashPageSize.ToString() + " байт.\n";
                    TextWindow.Text += "Адрес начала рабочей программы: 0x" + BootCore.RFlashBaseAddressForMainProgram.ToString("X") + ".\n";
                    TextWindow.Text += "Будет стерто страниц памяти: " + BootCore.RFlashPageForErasing.ToString() + ".\n";
                    BootCore.LoaderMessageCreator("ERASE_SOME_PAGES");
                    CurrentCommand = "ERASE_SOME_PAGES";
                    TextWindow.Text += "Стирание памяти... \n";
                    TextWindow.SelectionStart = TextWindow.Text.Length;
                    TextWindow.ScrollToCaret();
                    SendNewMessage();
                    CommandProgressTimeout = 30; //timeout 3 sec (timer). 
                    RewriteTimer.Interval = 100;
                    SetProgressBar(10, Color.Yellow);
                } else {
                    SendNewMessage();
                    return;
                }
            } else {
                SendNewMessage();
                return;
            }
        }
        void ErrorProcess() {
            RewriteTimer.Stop();
            SetProgressBar(100, Color.Red);
            if(SerialPort.BaudRate != 9600)
                SerialPort.BaudRate = 9600; // Baudrate to default value
            CurrentCommand = "";
            CounterByteOld = 0;
            TextWindow.Text += "- - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n";
            TextWindow.Text += "Ошибка выполнения операции!\n";
            TextWindow.Text += "- - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n";
            TextWindow.SelectionStart = TextWindow.Text.Length;
            TextWindow.ScrollToCaret();
        }

        private void FlashRewriteProgress() {
            UInt16 w = (UInt16)SerialPort.BytesToRead;
            CommandProgressTimeout--;

            if((w <= 4) && (CommandProgressTimeout == 0)) {
                ErrorProcess();
                return;
            }
            if(w <= 4) {
                CounterByteOld = w;
                return;
            }
            if(CounterByteOld != w) {
                CounterByteOld = w;
                return;
            }

            switch(CurrentCommand) {
                case "ERASE_SOME_PAGES":
                    SerialPort.Read(BootCore.ReceiveData, 0, w);
                    BootCore.ReceiveDataLength = w;
                    if(!BootCore.CompactAnswerVeryfier(BootCore.CommandIdFinder(CurrentCommand))) {
                        TextWindow.Text += "Стирание успешно завершено! \n";
                        if(BootCore.NewModbusBitrate != BootCore.ModbusBitrate) {
                            BootCore.LoaderMessageCreator("SET_NEW_BAUDRATE");
                            CurrentCommand = "SET_NEW_BAUDRATE";
                            TextWindow.Text += "Установка новой скорости загрузки... \n";
                            CounterByteOld = 0;
                            SendNewMessage();
                            CommandProgressTimeout = 10; //timeout 1 sec (timer). 
                            SetProgressBar(20, Color.Yellow);
                        } else {
                            BootCore.FlashFragmentNumber = 0;
                            BootCore.LoaderFragmentMessageCreator();
                            CurrentCommand = "WRITE_FLASH_FRAGMENT";

                            TextWindow.Text += "Загрузка рабочей программы... \n";
                            CounterByteOld = 0;
                            SendNewMessage();
                            CommandProgressTimeout = 40; //timeout 4 sec (timer). 
                            SetProgressBar(30, Color.Yellow);
                        }
                        TextWindow.SelectionStart = TextWindow.Text.Length;
                        TextWindow.ScrollToCaret();
                    } else {
                        ErrorProcess();
                    }
                    break;

                case "SET_NEW_BAUDRATE":
                    SerialPort.Read(BootCore.ReceiveData, 0, w);
                    BootCore.ReceiveDataLength = w;
                    if(!BootCore.CompactAnswerVeryfier(BootCore.CommandIdFinder(CurrentCommand))) {
                        SerialSpeedReconfig(BootCore.NewModbusBitrate); // Setting new baudrate
                        TextWindow.Text += "Новая скорость загрузки ПО установлена! \n";

                        BootCore.FlashFragmentNumber = 0;
                        BootCore.LoaderFragmentMessageCreator();
                        CurrentCommand = "WRITE_FLASH_FRAGMENT";

                        TextWindow.Text += "Загрузка рабочей программы... \n";
                        TextWindow.SelectionStart = TextWindow.Text.Length;
                        TextWindow.ScrollToCaret();

                        CounterByteOld = 0;
                        SendNewMessage();
                        CommandProgressTimeout = (SerialPort.BaudRate < 9600) ? 120 : 40;
                        SetProgressBar(30, Color.Yellow);
                    } else {
                        ErrorProcess();
                    }
                    break;
                case "WRITE_FLASH_FRAGMENT":
                    SerialPort.Read(BootCore.ReceiveData, 0, w);
                    BootCore.ReceiveDataLength = w;
                    if(!BootCore.CompactAnswerVeryfier(BootCore.CommandIdFinder(CurrentCommand))) {
                        if(BootCore.FlashPageSize * BootCore.FlashFragmentNumber >= BootCore.FlashArrayLength) {
                            BootCore.LoaderMessageCreator("SET_SUCCESSFUL_FLAG");
                            CurrentCommand = "SET_SUCCESSFUL_FLAG";
                            TextWindow.Text += "Запись признака завершения обновления... \n";
                            TextWindow.SelectionStart = TextWindow.Text.Length;
                            TextWindow.ScrollToCaret();
                            CounterByteOld = 0;
                            SendNewMessage();
                            CommandProgressTimeout = 10; //timeout 1 sec (timer). 
                            SetProgressBar(100, Color.Yellow);
                        } else {

                            BootCore.LoaderFragmentMessageCreator();
                            CurrentCommand = "WRITE_FLASH_FRAGMENT";

                            CounterByteOld = 0;
                            SendNewMessage();
                            CommandProgressTimeout = (SerialPort.BaudRate < 9600) ? 120 : 40;
                            SetProgressBar((byte)(30 + 70 * (((float)BootCore.FlashPageSize
                                * ((float)BootCore.FlashFragmentNumber - 1)) / (float)BootCore.FlashArrayLength)), Color.Yellow);
                        }

                    } else {
                        ErrorProcess();
                    }
                    break;
                case "SET_SUCCESSFUL_FLAG":
                    SerialPort.Read(BootCore.ReceiveData, 0, w);
                    BootCore.ReceiveDataLength = w;
                    if(!BootCore.CompactAnswerVeryfier(BootCore.CommandIdFinder(CurrentCommand))) {
                        TextWindow.Text += "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n";
                        TextWindow.Text += "Обновление прошивки успешно завершено!\n";
                        TextWindow.Text += "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n";
                        if(SerialPort.BaudRate != 9600) // Baudrate to default value
                        {
                            SerialSpeedReconfig(9600);
                        }
                        TextWindow.SelectionStart = TextWindow.Text.Length;
                        TextWindow.ScrollToCaret();
                        CurrentCommand = "";
                        SetProgressBar(100, Color.Green);
                        RewriteTimer.Stop();
                        CounterByteOld = 0;
                    } else {
                        ErrorProcess();
                    }
                    break;
            }
        }
    };


}


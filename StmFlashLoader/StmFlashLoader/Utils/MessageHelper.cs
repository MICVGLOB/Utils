using StmFlashLoader.LoaderCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StmFlashLoader.Utils {
    public class MessageHelper {
        Action<string> sendMessageAction;

        public MessageHelper(Action<string> sendMessageAction) {
            this.sendMessageAction = sendMessageAction;
        }
        public void SendUpdateFirmwareMessage(List<byte> data, int erasedPagesCount, bool displayBaudrate) {
            string message = "Микроконтроллер обнаружен!\n";
            message += string.Format("Версия ПО загрузчика: {0}.\n", LConverters.LoaderVersionToView(data));
            message += string.Format("Уникальный ID микроконтроллера: {0}.\n", LConverters.McuIdToView(data));
            message += string.Format("Адрес MODBUS: {0}.\n", LConverters.ModbusAddressToView(data));
            if(displayBaudrate)
                message += string.Format("Текущая скорость обмена данными: {0} бит/с.\n", LConverters.BaudrateToView(data));
            if(LConverters.PageSizeToView(data) > 0)
                message += string.Format("Размер страницы Flash-памяти: {0} байт.\n", LConverters.PageSizeToView(data));
            message += string.Format("Адрес начала рабочей программы: {0}.\n", LConverters.BaseAddressToView(data));
            message += string.Format("Будет стерто страниц (секторов) памяти: {0}.\n", erasedPagesCount);
            sendMessageAction.Invoke(message);
        }
        public void SendCreateIdUpdateMessage(List<byte> data, CommandIds commandId) {
            string message = "Микроконтроллер обнаружен!\n";
            message += string.Format("Версия ПО загрузчика: {0}.\n", LConverters.LoaderVersionToView(data));
            message += Commands.Info[commandId].ExecuteMessage;
            sendMessageAction.Invoke(message);
        }
        public void SendSearchMessage() {
            sendMessageAction.Invoke("Поиск микроконтроллера...\n");
        }
        public void SendTerminateMessage() {
            sendMessageAction.Invoke("Выполнение операции прервано!\n");
        }
        public void SendBeginReadingFileMessage() {
            sendMessageAction.Invoke("Чтение файла прошивки... \n");
        }
        public void SendSuccessfulReadFileMessage(int fileSize) {
            string message = "Файл успешно прочитан!\n";
            message += string.Format("Размер прошивки {0} байт.\n", fileSize);
            sendMessageAction.Invoke(message);
        }
        public void SendFailReadFileMessage() {
            sendMessageAction.Invoke("Чтение невозможно! Файл не существует, защищен от\nчтения или поврежден.\n");
        }
        public void SendPreMessage(bool isModbusScenario = false) {
            SetCommandMessage(id => Commands.Info[id].PreMessage, isModbusScenario);
        }
        public void SendExecuteMessage(bool isModbusScenario = false) {
            SetCommandMessage(id => Commands.Info[id].ExecuteMessage, isModbusScenario);
        }
        public void SendReconnectUsbMessage() {
            string message = "Устройство уже подключено.\n";
            message += "Отключите его и повторите попытку!\n";
            sendMessageAction.Invoke(message);
        }
        public void SendConnectUsbMessage() {
            sendMessageAction.Invoke("Подключите кабель USB к устройству и подайте\nна него питание!...\n");
        }
        public void SendUpdateProgressMesssage() {
            sendMessageAction.Invoke("Обновление прошивки...\n");
        }
        void SetCommandMessage(Func<CommandIds, string> getMessage, bool isModbusScenario) {
            if(isModbusScenario)
                return;
            var commandId = LoaderScenario.GetCommandId();
            var message = getMessage(commandId);
            if(!string.IsNullOrEmpty(message))
                sendMessageAction.Invoke(message);
        }
    }
}

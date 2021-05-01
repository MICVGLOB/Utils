using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace StmFlashLoader.LoaderCore {
    public static class Commands {
        static Dictionary<CommandIds, CommandInfo> commandInfo = CreateCommandInfo();

        public static Dictionary<CommandIds, CommandInfo> Info { get { return commandInfo; } }

        static Dictionary<CommandIds, CommandInfo> CreateCommandInfo() {
            var result = new Dictionary<CommandIds, CommandInfo>();
            result.Add(CommandIds.ReadLoaderSettings, new CommandInfo(0x01, 2, AnswerFormat.Extended));
            result.Add(CommandIds.SetProgramParameters, new CommandInfo(0x03, 12, AnswerFormat.Extended));
            result.Add(CommandIds.EraseSomePages, new CommandInfo(0x06, 2, AnswerFormat.Compact,
                "Стирание памяти...\n", "Стирание памяти успешно завершено!\n"));
            result.Add(CommandIds.SetNewBaudrate, new CommandInfo(0x02, 2, AnswerFormat.Compact,
                "Установка новой скорости загрузки...\n", "Новая скорость загрузки ПО установлена!\n"));
            result.Add(CommandIds.WriteFlashFragment, new CommandInfo(0x07, null, AnswerFormat.Compact));
            result.Add(CommandIds.SetSuccessfulFlag, new CommandInfo(0x08, 2, AnswerFormat.Compact,
                "Установка флага завершения обновления...\n", "Обновление ПО успешно завершено!\n"));
            result.Add(CommandIds.SetNewSerial, new CommandInfo(0x04, 12, AnswerFormat.Compact,
                executeMessage: "Перезапись серийного номера успешно завершена!\n"));
            result.Add(CommandIds.SetNewAddress, new CommandInfo(0x05, 10, AnswerFormat.Compact, 
                executeMessage: "Перезапись адреса MODBUS успешно завершена!\n"));
            return result;
        }
    }
    public class CommandInfo {
        public byte ByteId { get; private set; }
        public int? DataLength { get; private set; }
        public AnswerFormat AnswerFormat { get; private set; }
        public string PreMessage { get; private set; }
        public string ExecuteMessage { get; private set; }

        public CommandInfo(byte byteId, int? dataLength, AnswerFormat answerFormat, 
            string preMessege = "", string executeMessage = "") {
            ByteId = byteId;
            DataLength = dataLength;
            AnswerFormat = answerFormat;
            PreMessage = preMessege;
            ExecuteMessage = executeMessage;
        }
    }
    public enum AnswerFormat {
        Compact,
        Extended
    }
    public enum CommandIds {
        ReadLoaderSettings,
        SetProgramParameters,
        EraseSomePages,
        SetNewBaudrate,
        WriteFlashFragment,
        SetSuccessfulFlag,
        SetNewSerial,
        SetNewAddress
    }
}

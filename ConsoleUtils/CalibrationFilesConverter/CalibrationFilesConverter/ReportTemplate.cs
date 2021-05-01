using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalibrationFilesConverter {
    partial class Converter {
        string calibrationProtocolTemplate = @"
        ПРОТОКОЛ КАЛИБРОВКИ ИЗМЕРИТЕЛЯ(ДАТЧИКА) ПО УГЛУ


        Дата калибровки:            {0}

        Время калибровки:           {1}

        Оператор:                   {2}


        Серийный номер датчика:     {3}

        Адрес MODBUS:               {4}

        Тип датчика:                {5}


        Значения кодов АЦП после фильтрации:


        Ось Х:      Минимум:    {6}         Максимум:   {7}

        Ось Y:      Минимум:    {8}         Максимум:   {9}


        Калибровочные коэффициенты измерителя:


        Ось Х:      По нулю:    {10}        По шкале:   {11}

        Ось Y:      По нулю:    {12}        По шкале:   {13}


        Подпись оператора:
";
    }
}

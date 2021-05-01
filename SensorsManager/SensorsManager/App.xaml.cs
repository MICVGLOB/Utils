using SensorsManager.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

namespace SensorsManager {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            if(FolderHelper.IsReadOnly) {
                MessageBox.Show("Диск или каталог с программой защищены от записи. Сохранение отчетов невозможно!",
                    "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }








            //string path = Path.Combine(currentDirectory, "IUG_CAL_SN351.txt");
            //var exist = File.Exists(path);

            //var result = System.Windows.MessageBox.Show("Файл отчета уже существует. Перезаписать?", "Внимание!",
            //         System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);

        }



    }
}

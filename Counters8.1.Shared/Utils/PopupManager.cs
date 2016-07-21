using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Counters.Utils
{
    public class PopupManager
    {
        /// <summary>
        /// Показать диалоговое окно с выбором ответа
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns></returns>
        public async Task<bool> ShowDialogPopupAsync(string message, bool defaultValue = false)
        {
            MessageDialog msgbox = new MessageDialog(message);
            msgbox.Commands.Add(new UICommand("Да", null, 0));
            msgbox.Commands.Add(new UICommand("Нет", null, 1));

            var result = await msgbox.ShowAsync();

            if (result == null)
                return defaultValue;
            else
                return (int)result.Id == 0;
        }
    }
}

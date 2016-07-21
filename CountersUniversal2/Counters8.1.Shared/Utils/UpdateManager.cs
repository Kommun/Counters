﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Counters
{
    public static class UpdateManager
    {
        public const int CurrentVersion = 5;
        private static AppSettings _settings = new AppSettings();

        /// <summary>
        /// Отобразить новости изменений
        /// </summary>
        /// <param name="fromCurrentVersion">С текущей версии (false - за все время)</param>
        /// <returns></returns>
        public static async Task ShowUpdateMessage(bool fromCurrentVersion = true)
        {
            StringBuilder message = new StringBuilder();

            var toVersion = fromCurrentVersion ? _settings.AppVersion : 0;
            int i = CurrentVersion;
            while (i > toVersion)
            {
                switch (i--)
                {
                    case 5:
                        message.AppendLine("V 4.4.3.3");
                        message.AppendLine("- На странице добавления счета появилась вкладка 'Оплата', на которой можно указать сумму перерасчета (в том числе отрицательную), пени и увидеть итоговую сумму к оплате.");
                        break;
                    case 4:
                        message.AppendLine("V 4.4.3.2");
                        message.AppendLine("- В раздел 'О программе' добавлена справка.");
                        break;
                    case 3:
                        message.AppendLine("V 4.4.3.1");
                        message.AppendLine("- Добавлен раздел 'О программе'.");
                        break;
                    case 2:
                        message.AppendLine("V 4.4.3");
                        message.AppendLine("- Увеличены возможности по редактированию счета. Теперь можно добавлять и удалять услуги, менять дату. Чтобы изменения счета вступили в силу, его нужно сохранить.");
                        break;
                    case 1:
                        message.AppendLine("V 4.4.2");
                        message.AppendLine("- Добавлена возможность изменять дату начальных показаний.");
                        break;
                }
                message.AppendLine();
            }
            _settings.AppVersion = CurrentVersion;

            var res = message.ToString().Trim();
            if (!string.IsNullOrEmpty(res))
                await new MessageDialog(res, "Новости обновлений").ShowAsync();
        }
    }
}

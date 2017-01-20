using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Counters
{
    public static class UpdateManager
    {
        public const int CurrentVersion = 9;
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
                    case 9:
                        message.AppendLine("V 4.4.3.7");
                        message.AppendLine("- Добавлена возможность сортировать счетчики и услуги в произвольном порядке. Для этого нужно перейти в режим сортировки с помощью кнопки на нижней панели.");
                        message.AppendLine("- Редактирование и удаление услуг вынесено в контекстное меню.");
                        message.AppendLine();
                       message.AppendLine("Уважаемые пользователи! Периодически в приложении случаются ошибки, при этом мгновенно в магазине приложений появляется большое количество негативных отзывов. Это нормальная практика, они помогают быстрее заметить неполадки в работе приложения. Я стараюсь реагировать на ваши замечания как можно оперативнее. Однако негативные отзывы в магазине остаются и отпугивают потенциальных пользователей. Убедительная просьба пересматривать отзыв, если ваша проблема была решена. Спасибо за понимание!");
                        break;
                    case 8:
                        message.AppendLine("V 4.4.3.6");
                        message.AppendLine("- Открыть счет для редактирования теперь можно только с помощью контекстного меню. В режиме просмотра невозможно изменить данные.");
                        message.AppendLine("- На странице добавления счета появилось поле 'Комментарий', которое можно заполнить произвольным текстом.");
                        break;
                    case 7:
                        message.AppendLine("V 4.4.3.5");
                        message.AppendLine("- По многочисленным просьбам пользователей изменен логотип приложения.");
                        message.AppendLine("- Округление суммы платежей теперь работает корректно(по математическим правилам).");
                        message.AppendLine("- В настройках экспорта теперь можно установить пользовательский шаблон. Правила его формирования добавлены в справку.");
                        break;
                    case 6:
                        message.AppendLine("V 4.4.3.4");
                        message.AppendLine("- В настройках появилась возможность включить отображение списка квартир при запуске приложения.");
                        message.AppendLine("- Добавлена возможность закреплять плитку, соответствующую определенной квартире, на рабочем столе (для быстрого доступа).");
                        message.AppendLine("- Исправлены ошибки.");
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

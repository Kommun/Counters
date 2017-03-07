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
		public const int CurrentVersion = 11;
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
					case 11:
						message.AppendLine("V 4.4.3.9");
						message.AppendLine("- В пользовательском шаблоне экспорта единицы измерения счетчика вынесены в отдельное подстановочное значение.");
						message.AppendLine("- На странице графиков теперь отображаются временные рамки, заданные в настройках.");
						break;
					case 10:
						message.AppendLine("V 4.4.3.8");
						message.AppendLine("- В свойствах счетчиков тепловой энергии добавлено поле 'Коэффициент перевода единиц измерения'. Данный коэффициент позволяет переводить показания счетчика из одних единиц измерения в другие (например, кВт*ч в Гкал).");
						break;
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

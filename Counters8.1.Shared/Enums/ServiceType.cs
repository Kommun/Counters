using System;
using System.Collections.Generic;
using System.Text;

namespace Counters.Enums
{
	public enum ServiceType
	{
		/// <summary>
		/// Фиксированная стоимость
		/// </summary>
		Fix = 0,

		/// <summary>
		/// Коэффициент
		/// </summary>
		Coefficient = 1,

		/// <summary>
		/// Водоотведение
		/// </summary>
		Sewerage = 2,

		/// <summary>
		/// Подогрев воды
		/// </summary>
		WaterHeating = 3
	}
}

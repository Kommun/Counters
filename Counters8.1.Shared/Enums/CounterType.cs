using System;
using System.Collections.Generic;
using System.Text;

namespace Counters.Enums
{
	public enum CounterType
	{
		/// <summary>
		/// Холодная вода
		/// </summary>
		ColdWater = 1,

		/// <summary>
		/// Горячая вода
		/// </summary>
		HotWater = 2,

		/// <summary>
		/// Электрическая
		/// </summary>
		Electricity = 3,

		/// <summary>
		/// Газ
		/// </summary>
		Gas = 4,

		/// <summary>
		/// Отопление
		/// </summary>
		Heating = 5,

		/// <summary>
		/// Моторесурс
		/// </summary>
		EngineLifespan = 6
	}
}

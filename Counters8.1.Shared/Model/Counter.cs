﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Counters
{
	public class Counter
	{
		[PrimaryKey, AutoIncrement]
		public int CounterId { get; set; }
		public int FlatId { get; set; }
		public int TypeId { get; set; }
		public string Name { get; set; }
		public int TarifId { get; set; }
		public bool EnableODN { get; set; }

		/// <summary>
		/// Порядок сортировки
		/// </summary>
		public int SortOrder { get; set; } = -1;

		/// <summary>
		/// Коэффициент преобразования единиц измерения
		/// </summary>
		public double UnitConvertCoefficient { get; set; }
	}
}

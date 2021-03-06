﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;


namespace Counters
{
	public enum ChartType { Delta, Summ }

	public sealed partial class Chart : MyToolkit.Paging.MtPage
	{
		MenuItem selectedChart;
		Counter selectedCounter;
		ChartSettingsParameter settings = new ChartSettingsParameter();

		public Chart()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
		{
			try
			{
				if (e.NavigationMode == NavigationMode.New)
				{
					if (e.Parameter != null)
						selectedChart = (MenuItem)e.Parameter;

					var counters = App.QueryManager.Connection.Table<Counter>().ToList();
					if (selectedChart.Type == 1 || selectedChart.Type == 2)
						counters.Insert(0, new Counter() { CounterId = 0, Name = "Все" });
					tbCounters.ItemsSource = counters;
					tbCounters.SelectedIndex = 0;
				}

				tbPeriod.Text = $"{settings.BeginDate:dd MMMM yyyy} - {settings.EndDate:dd MMMM yyyy}";

				tbCounters_SelectionChanged();
				Tag = selectedChart.Title;
			}
			catch (Exception ex)
			{
				new MessageDialog(ex.Message).ShowAsync();
				Frame.GoBackAsync();
			}
		}

		private void makeChart()
		{
			switch (selectedChart.Type)
			{
				case 0:
					btnCounters.Visibility = Visibility.Collapsed;
					makePieChart();
					break;
				case 1:
					parametrizedQuery(ChartType.Summ);
					break;
				case 2:
					parametrizedQuery(ChartType.Delta);
					break;
				case 3:
					var query = App.QueryManager.TarifsChart(selectedCounter.CounterId, settings.bDate, settings.eDate);
					makeSingleChart(query.Select(q => new { Date = q.Date, Value = q.Tarif3 }));
					break;
				case 4:
					btnCounters.Visibility = Visibility.Collapsed;
					var scores = App.QueryManager.ScoresChart(settings.bDate, settings.eDate);
					makeSingleChart(scores.Select(s => new { Date = s.Date, Value = s.Data }), "MMM yyyy");
					break;
			}
		}

		private void parametrizedQuery(ChartType type)
		{
			if (selectedCounter.CounterId == 0)
				makeMultiChart(type);
			else
			{
				var query = App.QueryManager.CounterDataChart(selectedCounter.CounterId, settings.bDate, settings.eDate);
				if (type == ChartType.Delta)
					makeSingleChart(query.Select(q => new { Date = q.Date, Value = q.Delta }));
				else
					makeSingleChart(query.Select(q => new { Date = q.Date, Value = q.Summ }));
			}
		}

		private void makeMultiChart(ChartType type)
		{
			var query = App.QueryManager.ExportData();
			IEnumerable<object> data;
			if (type == ChartType.Delta)
				data = from q in query
					   where !q.IsFirst
					   orderby q.Date
					   group q by q.Date.ToString("MM yyyy") into g
					   select new
					   {
						   Date = new DateTime(g.First().Date.Year, g.First().Date.Month, 1),
						   Cold = (from c in query where c.TypeId == 1 && c.Date.ToString("MM yyyy") == g.Key select c.Delta).Sum(),
						   Hot = (from c in query where c.TypeId == 2 && c.Date.ToString("MM yyyy") == g.Key select c.Delta).Sum(),
						   Energy = (from c in query where c.TypeId == 3 && c.Date.ToString("MM yyyy") == g.Key select c.Delta).Sum(),
						   Gas = (from c in query where c.TypeId == 4 && c.Date.ToString("MM yyyy") == g.Key select c.Delta).Sum(),
						   Heat = (from c in query where c.TypeId == 5 && c.Date.ToString("MM yyyy") == g.Key select c.Delta).Sum(),
					   };
			else
				data = from q in query
					   where !q.IsFirst
					   orderby q.Date
					   group q by q.Date.ToString("MM yyyy") into g
					   select new
					   {
						   Date = new DateTime(g.First().Date.Year, g.First().Date.Month, 1),
						   Cold = (from c in query where c.TypeId == 1 && c.Date.ToString("MM yyyy") == g.Key select c.Summ).Sum(),
						   Hot = (from c in query where c.TypeId == 2 && c.Date.ToString("MM yyyy") == g.Key select c.Summ).Sum(),
						   Energy = (from c in query where c.TypeId == 3 && c.Date.ToString("MM yyyy") == g.Key select c.Summ).Sum(),
						   Gas = (from c in query where c.TypeId == 4 && c.Date.ToString("MM yyyy") == g.Key select c.Summ).Sum(),
						   Heat = (from c in query where c.TypeId == 5 && c.Date.ToString("MM yyyy") == g.Key select c.Summ).Sum(),
					   };

			var source = data.ToList();

			var model = new PlotModel();
			model.Axes.Add(new DateTimeAxis()
			{
				StringFormat = "MMM yyyy",
				Angle = 45,
				Minimum = DateTimeAxis.ToDouble(settings.BeginDate),
				Maximum = DateTimeAxis.ToDouble(settings.EndDate),
			});
			model.Series.Add(new LineSeries() { DataFieldX = "Date", DataFieldY = "Cold", ItemsSource = source, MarkerType = MarkerType.Circle, Title = "Холодная вода", Color = OxyColors.Blue });
			model.Series.Add(new LineSeries() { DataFieldX = "Date", DataFieldY = "Hot", ItemsSource = source, MarkerType = MarkerType.Circle, Title = "Горячая вода", Color = OxyColors.Red });
			model.Series.Add(new LineSeries() { DataFieldX = "Date", DataFieldY = "Energy", ItemsSource = source, MarkerType = MarkerType.Circle, Title = "Электричество", Color = OxyColors.Orange });
			model.Series.Add(new LineSeries() { DataFieldX = "Date", DataFieldY = "Gas", ItemsSource = source, MarkerType = MarkerType.Circle, Title = "Газ", Color = OxyColors.SkyBlue });
			model.Series.Add(new LineSeries() { DataFieldX = "Date", DataFieldY = "Heat", ItemsSource = source, MarkerType = MarkerType.Circle, Title = "Отопление", Color = OxyColors.IndianRed });
			chart.Model = model;
		}

		private void makePieChart()
		{
			var model = new PlotModel { PlotMargins = new OxyThickness(50) };
			var query = App.QueryManager.ExportData();
			var ps = new PieSeries { InsideLabelFormat = "{1}({0} " + new AppSettings().Currency + ")" };
			ps.Slices = query.GroupBy(q => q.Name).Select(g => new PieSlice(g.Key, g.Sum(q => q.Summ))).ToList();
			model.Series.Add(ps);
			chart.Model = model;
		}

		private void makeSingleChart(IEnumerable source, string dateFormat = "dd MM yyyy")
		{
			var model = new PlotModel();
			if (new AppSettings().PlotType == 0)
			{
				model.Axes.Add(new DateTimeAxis()
				{
					StringFormat = dateFormat,
					Angle = 45,
					Minimum = DateTimeAxis.ToDouble(settings.BeginDate),
					Maximum = DateTimeAxis.ToDouble(settings.EndDate),
				});
				model.Series.Add(new LineSeries() { DataFieldX = "Date", DataFieldY = "Value", ItemsSource = source, MarkerType = MarkerType.Circle });
			}
			else
			{
				model.Axes.Add(new CategoryAxis { ItemsSource = source, LabelField = "Date", StringFormat = dateFormat, Angle = 45 });
				model.Axes.Add(new LinearAxis { MaximumPadding = 0.05, AbsoluteMinimum = 0 });
				model.Series.Add(new ColumnSeries() { ItemsSource = source, ValueField = "Value", LabelFormatString = "{0:F2}", ColumnWidth = 50 });
			}
			chart.Model = model;
		}

		private void tbCounters_ItemsPicked(ListPickerFlyout sender, ItemsPickedEventArgs args)
		{
			tbCounters_SelectionChanged();
		}

		private void tbCounters_SelectionChanged()
		{
			selectedCounter = tbCounters.SelectedItem as Counter;
			makeChart();
		}

		private async void btnSettings_Click(object sender, RoutedEventArgs e)
		{
			settings.ChartType = selectedChart.Type;
			await Frame.NavigateAsync(typeof(ChartSettings), settings);
		}
	}
}

﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

namespace Counters
{
    public sealed partial class AddFlat : MyToolkit.Paging.MtPage
    {
        Flat currentFlat;

        public AddFlat()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(MyToolkit.Paging.MtNavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                currentFlat = e.Parameter as Flat;
                spFlat.DataContext = currentFlat;

                Tag = currentFlat.Name;
            }
            else
                Tag = "Новая квартира";
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (tbName.Text == "")
                invalidData("Введите название", tbName);
            else
            {
                var newFlat = new Flat()
                {
                    Name = tbName.Text,
                    Adress = tbAdress.Text
                };

                if (currentFlat != null)
                {
                    newFlat.FlatId = currentFlat.FlatId;
                    App.QueryManager.Connection.Update(newFlat);
                }
                else
                    App.QueryManager.Connection.Insert(newFlat);

                await Frame.GoBackAsync();
            }
        }

        private async void invalidData(string message, Control tbToFocus)
        {
            await new MessageDialog(message).ShowAsync();
            tbToFocus.Focus(FocusState.Programmatic);
        }
    }
}

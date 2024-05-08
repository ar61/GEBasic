﻿// Copyright (c) Abhinav Rathod. All rights reserved.

using GEBasicEditor.GameProjects;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GEBasicEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;
            OptionProjectBrowserDialog();
            Closing += OnMainWindowClosing;
        }

        private void OnMainWindowClosing(object? sender, CancelEventArgs e)
        {
            Closing -= OnMainWindowClosing;
            Project.Current?.Unload();
        }

        private void OptionProjectBrowserDialog()
        {
            var projectBowser = new ProjectBrowserDialog();
            if(projectBowser.ShowDialog() == false || projectBowser.DataContext == null)
            {
                Application.Current.Shutdown();
            } else
            {
                Project.Current?.Unload();
                DataContext = projectBowser.DataContext;
            }
        }
    }
}
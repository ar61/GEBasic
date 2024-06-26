﻿// Copyright (c) Abhinav Rathod. All rights reserved.

using GEBasicEditor.Components;
using GEBasicEditor.GameProjects;
using GEBasicEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GEBasicEditor.Editors
{
    /// <summary>
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddGameEntity_Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var vm = btn?.DataContext as Scene;
            vm?.AddGameEntityCommand.Execute(new GameEntity(vm) { Name = "Empty Game Entity"});
        }

        private void OnGameEntities_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameEntityView.Instance!.DataContext = null;
            var listBox = sender as ListBox;
            var newSelection = listBox!.SelectedItems.Cast<GameEntity>().ToList();
            var prevSelection = newSelection.Except(e.AddedItems.Cast<GameEntity>()).Concat(e.RemovedItems.Cast<GameEntity>()).ToList();

            Project.UndoRedo.Add(new UndoRedoAction(
                () => // undo
                {
                    listBox.UnselectAll();
                    prevSelection.ForEach(item => (listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem)!.IsSelected = true );
                },
                () => // redo
                {
                    listBox.UnselectAll();
                    newSelection.ForEach(item => (listBox.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem)!.IsSelected = true );
                },
                "Selection Changed"
                ));

            MSGameEntity? msEntity = null;
            if (newSelection.Any())
            {
                msEntity = new MSGameEntity(newSelection);
            }
            GameEntityView.Instance.DataContext = msEntity;
        }
    }
}

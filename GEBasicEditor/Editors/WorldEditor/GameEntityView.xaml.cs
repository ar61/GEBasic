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
using GEBasicEditor.Components;
using GEBasicEditor.GameProjects;
using GEBasicEditor.Utilities;

namespace GEBasicEditor.Editors
{
    /// <summary>
    /// Interaction logic for GameEntityView.xaml
    /// </summary>
    public partial class GameEntityView : UserControl
    {
        private Action? _undoAction;
        private string? _propertyName;
        public static GameEntityView? Instance { get; private set; }
        public GameEntityView()
        {
            InitializeComponent();
            DataContext = null;
            Instance = this;
            DataContextChanged += (_, __) =>
            {
                if(DataContext != null)
                {
                    (DataContext as MSEntity)!.PropertyChanged += (s, e) => _propertyName = e.PropertyName;
                }
            };
        }

        private Action? getRenameAction() 
        {
            var vm = DataContext as MSEntity;
            if (vm == null) return null;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.entity.Name = item.Name);
                if (DataContext != null)
                {
                    (DataContext as MSEntity)!.Refresh();
                }
            });
        }
        private Action? getIsEnabledAction()
        {
            var vm = DataContext as MSEntity;
            if (vm == null) return null;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();
            return new Action(() =>
            {
                selection.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
                if (DataContext != null)
                {
                    (DataContext as MSEntity)!.Refresh();
                }
            });
        }

        private void OnName_Textbox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _undoAction = getRenameAction();
        }

        private void OnName_Textbox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if(_propertyName == nameof(MSEntity.Name) && _undoAction != null)
            {
                var redoAction = getRenameAction();
                if (redoAction != null)
                {
                    Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Rename game entity"));
                }
                _propertyName = null;
            }
            _undoAction = null;
        }

        private void OnIsEnabled_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var undoAction = getIsEnabledAction();
            var vm = DataContext as MSEntity;
            if (vm == null) { return; }
            var checkBox = sender as CheckBox;
            if(checkBox == null) { return; }
            vm.IsEnabled = checkBox.IsChecked == true;
            var redoAction = getIsEnabledAction();
            if(undoAction == null || redoAction == null) { return; }
            Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction,
                vm.IsEnabled == true ? "Enable game entity" : "Disable game entity"));
        }
    }
}

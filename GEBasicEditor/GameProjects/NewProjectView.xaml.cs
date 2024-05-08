// Copyright (c) Abhinav Rathod. All rights reserved.

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

namespace GEBasicEditor.GameProjects
{
    /// <summary>
    /// Interaction logic for NewProjectView.xaml
    /// </summary>
    public partial class NewProjectView : UserControl
    {
        public NewProjectView()
        {
            InitializeComponent();
        }

        private void OnClick_Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as NewProject;
            var projectTemplate = templateListBox.SelectedItem as ProjectTemplate;
            string projectPath;
            if (vm != null && projectTemplate != null)
            {
                projectPath = vm.CreateProject(projectTemplate);
            } 
            else
            {
                Debug.WriteLine("NewProjectView.xaml.cs.OnClick_Button_Click: Invalid Template, or invalid Data Context");
                return;
            }
            bool dialogResult = false;
            var win = Window.GetWindow(this);
            if(!string.IsNullOrEmpty(projectPath) )
            {
                dialogResult = true;
                var project = OpenProject.Open(new ProjectData() { ProjectName = vm.ProjectName, ProjectPath = projectPath });
                win.DataContext = project;
            }
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}

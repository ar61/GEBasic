// Copyright (c) Abhinav Rathod. All rights reserved.

using GEBasicEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Xps;

namespace GEBasicEditor.GameProjects
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string? ProjectType { get; set; }
        [DataMember]
        public string? ProjectFile { get; set; }
        [DataMember]
        public List<string>? Folders { get; set; }

        public byte[]? Icon { get; set; }
        public byte[]? Screenshot{ get; set; }
        public string? IconFilePath {  get; set; }
        public string? ScreenshotFilePath { get; set;}
        public string? ProjectFilePath { get; set;}
    }
    public class NewProject : ViewModelBase
    {
        // TODO: Get Path from installation location
        private readonly string _templatePath = @"..\..\GEBasicEditor\ProjectTemplates";
        private string _projectName = "New Project";
        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (_projectName != value)
                {
                    _projectName = value;
                    ValidateProjectsPath();
                    OnPropertyChanged(nameof(ProjectName));
                }
            }
        }

        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\GEBasicProject\";
        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath != value)
                {
                    _projectPath = value;
                    ValidateProjectsPath();
                    OnPropertyChanged( nameof(ProjectPath) );
                }
            }
        }

        private bool _isValid;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        private string? _errorMsg;
        public string? ErrorMsg
        {
            get => _errorMsg;
            set
            {
                if (_errorMsg != value)
                {
                    _errorMsg = value;
                    OnPropertyChanged(nameof(ErrorMsg));
                }
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        private bool ValidateProjectsPath()
        {
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path))
            {
                path += @"\";
            }
            path += $@"{ProjectName}\";

            IsValid = false;
            if(string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMsg = "Type in a project name.";
            }
            else if(ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMsg = "Invalid character(s) used in project name.";
            }
            else if(string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMsg = "Select a valid project folder.";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMsg = "Invalid character(s) used in project path.";
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMsg = "Select project folder already exists and is not empty.";
            }
            else
            {
                ErrorMsg = "";
                IsValid = true;
            }
            return IsValid;
        }

        public string CreateProject(ProjectTemplate template)
        {
            ValidateProjectsPath();
            if(!IsValid)
            {
                return string.Empty;
            }

            if (!Path.EndsInDirectorySeparator(ProjectPath))
            {
                ProjectPath += @"\";
            }
            var path = $@"{ProjectPath}{ProjectName}\";

            try
            {
                if(!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                foreach (var folder in template.Folders!)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path)!, folder)));
                }
                var dirInfo = new DirectoryInfo(path + @".GEBasic\");
                dirInfo.Attributes |= FileAttributes.Hidden;
                File.Copy(template.IconFilePath!, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotFilePath!, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

                var projectXml = File.ReadAllText(template.ProjectFilePath!);
                projectXml = string.Format(projectXml, ProjectName, ProjectPath);
                var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(projectPath, projectXml);
                return path;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Unable create project at {ProjectName}");
                return string.Empty;
            }
        }

        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try
            {
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                foreach (var file in templateFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);

                    if (template != null)
                    {
                        template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file) ?? "", "Icon.png"));
                        template.Icon = File.ReadAllBytes(template.IconFilePath);
                    
                        template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file) ?? "", "Screenshot.png"));
                        template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);

                        template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file) ?? "", template.ProjectFile ?? ""));

                        _projectTemplates.Add(template);
                    }
                    ValidateProjectsPath();
                    /*
                     * To Write to template.txt:
                    var folders = file.Split("\\");
                    var parentFolderName = folders[folders.Length - 2].ToString();
                    int idx = parentFolderName.IndexOf("Project");
                    var projectType = parentFolderName.Substring(0, idx) + " Project";
                    var template = new ProjectTemplate()
                    {
                        ProjectType = projectType,
                        ProjectFile = "project.gebasic",
                        Folders = new List<string>() { ".GEBasic", "Content", "GameCode"}
                    };

                    Serializer.ToFile( template, file );*/
                }
            }
            catch(Exception ex) 
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Failed to read project templates");
            }
        }
    }
}


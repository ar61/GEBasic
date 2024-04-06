using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace GEBasicEditor.GameProjects
{
    [DataContract]
    public class Project : ViewModelBase
    {
        public static string Extension { get; } = ".gebasic";
		public string? Name { get; private set; }
        public string? Path { get; private set; }

        public string FullPath => $"{Path}{Name}{Extension}";

        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene> ();
        public ReadOnlyObservableCollection<Scene> Scenes
        { get; }
    }
}

using GEBasicEditor.GameProjects;
using GEBasicEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GEBasicEditor.Components
{
	[DataContract]
	[KnownType(typeof(Transform))]
    public class GameEntity : ViewModelBase
    {

		private bool _isEnabled;
		public bool IsEnabled
		{
			get => _isEnabled;
			set
			{
				if (_isEnabled != value)
				{
					_isEnabled = value;
					OnPropertyChanged(nameof(IsEnabled));
				}
			}
		}
		private string _name;
		[DataMember]
		public string Name
		{
			get => _name;
			set
			{
				if (_name != value)
				{
					_name = value;
					OnPropertyChanged(nameof(Name));
				}
			}
		}
		[DataMember]
		public Scene ParentScene { get; private set; }

		[DataMember(Name = nameof(Components))]
		private readonly ObservableCollection<Component> _components = [];
		public ReadOnlyObservableCollection<Component> Components { get; private set; }

		public ICommand RenameCommand { get; private set; }
		public ICommand EnableCommand { get; private set; }

		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
			if(_components != null)
			{
				Components = new ReadOnlyObservableCollection<Component>(_components);
				OnPropertyChanged(nameof(Components));
			}

			RenameCommand = new RelayCommand<string>(x =>
			{
				var oldName = _name;
				Name = x;

				Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, oldName, x, $"Entity renamed from {oldName} to {x}"));
			}, x => x != Name
			);

			EnableCommand = new RelayCommand<bool>(x =>
			{
				var oldVal = _isEnabled;
				IsEnabled = x;

				Project.UndoRedo.Add(new UndoRedoAction(nameof(IsEnabled), this, oldVal, x, x ? $"Enable: {Name}" : $"Disabled: {Name}"));
			}
			);
		}
		public GameEntity(Scene scene)
		{
			Debug.Assert(scene != null);
			ParentScene = scene;
			IsEnabled = false;
			Name = "Default Game Entity";
			_components.Add(new Transform(this));
			OnDeserialized(new StreamingContext());
			Debug.Assert(Components != null);
			Debug.Assert(_name != null);
			Debug.Assert(RenameCommand != null);
			//Debug.Assert(EnableCommand != null);
		}
    }
}

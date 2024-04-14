using GEBasicEditor.Components;
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

namespace GEBasicEditor.GameProjects
{
	[DataContract]
    class Scene : ViewModelBase
    {

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
        public Project Project { get; private set; }

        private bool _isActive;
        [DataMember]
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        public ICommand AddGameEntityCommand { get; private set; }
        public ICommand RemoveGameEntityCommand { get; private set; }

        private void AddGameEntity(GameEntity gameEntity)
        {
            Debug.Assert(!_gameEntities.Contains(gameEntity));
            _gameEntities.Add(gameEntity);
        }

        private void RemoveGameEntity(GameEntity gameEntity)
        {
            Debug.Assert(_gameEntities.Contains(gameEntity));
            _gameEntities.Remove(gameEntity);
        }

        [DataMember(Name = nameof(GameEntity))]
		private readonly ObservableCollection<GameEntity> _gameEntities = [];
		public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_gameEntities != null)
            {
                GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
                OnPropertyChanged(nameof(GameEntities));
            }

            AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                AddGameEntity(x);
                var gameEntityIndex = _gameEntities!.Count - 1;
                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveGameEntity(x),
                    () => _gameEntities!.Insert(gameEntityIndex, x),
                    $"Add {x.Name} to {Name}"));
            });

            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                var gameEntityIndex = _gameEntities!.IndexOf(x);
                RemoveGameEntity(x);

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => _gameEntities.Insert(gameEntityIndex, x),
                    () => RemoveGameEntity(x),
                    $"Remove {x.Name} from {Name}"));
            });
        }

        public Scene(Project project, string name)
		{
			Debug.Assert(project != null);
			Project = project;
			Name = name;
            OnDeserialized(new StreamingContext());
			Debug.Assert(_name != null);
            Debug.Assert(AddGameEntityCommand != null);
            Debug.Assert(RemoveGameEntityCommand != null);
            Debug.Assert(GameEntities != null);
        }
    }
}

// Copyright (c) Abhinav Rathod. All rights reserved.

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
using System.Transactions;
using System.Windows.Input;

namespace GEBasicEditor.Components
{
	[DataContract]
	[KnownType(typeof(Transform))]
    class GameEntity : ViewModelBase
    {

		private bool _isEnabled = true;
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

		[OnDeserialized]
		void OnDeserialized(StreamingContext context)
		{
			if(_components != null)
			{
				Components = new ReadOnlyObservableCollection<Component>(_components);
				OnPropertyChanged(nameof(Components));
			}
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
		}
    }

	abstract class MSEntity : ViewModelBase
	{
		// enables update to its properties
		private bool _enableUpdates = true;
        private bool? _isEnabled;
        public bool? IsEnabled
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


		private string? _name;
		public string? Name
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

		private readonly ObservableCollection<IMSComponent> _components = [];
		public ReadOnlyObservableCollection<IMSComponent> Components { get; private set; }

		public List<GameEntity> SelectedEntities { get; }


		public static float? GetMixedValue(List<GameEntity> entities, Func<GameEntity, float> getProperty)
		{
			var value = getProperty(entities.First());
			foreach (var entity in entities.Skip(1))
			{
				if(value.IsSameAs(getProperty(entity)))
				{
					return null;
				}
			}
			return value;
        }
        public static bool? GetMixedValue(List<GameEntity> entities, Func<GameEntity, bool> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1))
            {
                if (value != getProperty(entity))
                {
                    return null;
                }
            }
            return value;
        }
        public static string? GetMixedValue(List<GameEntity> entities, Func<GameEntity, string> getProperty)
        {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1))
            {
                if (value != getProperty(entity))
                {
                    return null;
                }
            }
            return value;
        }
        protected virtual bool UpdateGameEntities(string? propertyName)
        {
			if(propertyName == null) return false;
            switch (propertyName)
            {
                case nameof(IsEnabled): SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled!.Value); return true;
                case nameof(Name): 
					if(Name == null)
					{ return false; }
					SelectedEntities.ForEach(x => x.Name = Name); 
					return true;
            }
            return false;
        }

        protected virtual bool UpdateMSGameEntity()
		{
			IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));
			Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));
			return true;
		}

		public void Refresh()
		{
			_enableUpdates = false;
			UpdateMSGameEntity();
			_enableUpdates = true;
		}

        public MSEntity(List<GameEntity> entities)
        {
			Debug.Assert(entities?.Any() == true);
			Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
			SelectedEntities = entities;
			PropertyChanged += (s, e) => { if(_enableUpdates) UpdateGameEntities(e.PropertyName); };
        }
    }

	class MSGameEntity : MSEntity
	{
        public MSGameEntity(List<GameEntity> entities) : base(entities)
		{
			Refresh();
		}
    }
}

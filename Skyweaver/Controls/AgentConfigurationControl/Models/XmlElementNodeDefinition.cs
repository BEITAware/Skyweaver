using System.Collections.ObjectModel;
using Skyweaver.Infrastructure.Mvvm;

namespace Skyweaver.Controls.AgentConfigurationControl.Models
{
    public sealed class XmlElementNodeDefinition : ObservableObject
    {
        private string _name = string.Empty;

        public XmlElementNodeDefinition(string name, bool isRoot = false)
        {
            _name = name ?? string.Empty;
            IsRoot = isRoot;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value ?? string.Empty);
        }

        public bool IsRoot { get; }

        public XmlElementNodeDefinition? Parent { get; private set; }

        public ObservableCollection<XmlElementNodeDefinition> Children { get; } = new();

        public XmlElementNodeDefinition AddChild(string name)
        {
            var child = new XmlElementNodeDefinition(name);
            AddChild(child);
            return child;
        }

        public void AddChild(XmlElementNodeDefinition child)
        {
            ArgumentNullException.ThrowIfNull(child);

            if (ReferenceEquals(child, this))
            {
                throw new InvalidOperationException("A node cannot be added as its own child.");
            }

            child.SetParent(this);
            Children.Add(child);
        }

        public bool RemoveChild(XmlElementNodeDefinition child)
        {
            ArgumentNullException.ThrowIfNull(child);

            if (!Children.Remove(child))
            {
                return false;
            }

            child.SetParent(null);
            return true;
        }

        public XmlElementNodeDefinition DeepClone()
        {
            var clone = new XmlElementNodeDefinition(Name, IsRoot);
            foreach (var child in Children)
            {
                clone.AddChild(child.DeepClone());
            }

            return clone;
        }

        private void SetParent(XmlElementNodeDefinition? parent)
        {
            if (ReferenceEquals(Parent, parent))
            {
                return;
            }

            Parent = parent;
            OnPropertyChanged(nameof(Parent));
        }
    }
}

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Ferrita.Commands;
using Ferrita.Controls.PersonaSettingsControl.Models;
using Ferrita.Controls.PersonaSettingsControl.Services;
using Ferrita.Infrastructure.Mvvm;

namespace Ferrita.Controls.PersonaSettingsControl.ViewModels
{
    public sealed class PersonaSettingsControlViewModel : ObservableObject
    {
        private const string DefaultOrbColor = "#FF808080";
        private const string DefaultBackgroundColor = "#FF505050";

        private readonly PersonaConfigurationRepository _repository;
        private readonly PersonaColorService _colorService;
        private int _suspendPersistenceCounter;
        private bool _isGeneratingMissingColors;
        private PersonaModel? _selectedPersona;
        private bool _isConfigPanelExpanded;
        private string _statusMessage = string.Empty;

        public PersonaSettingsControlViewModel()
            : this(new PersonaConfigurationRepository(), new PersonaColorService())
        {
        }

        internal PersonaSettingsControlViewModel(
            PersonaConfigurationRepository repository,
            PersonaColorService colorService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _colorService = colorService ?? throw new ArgumentNullException(nameof(colorService));

            NextPersonaCommand = new RelayCommand(SelectNext);
            PrevPersonaCommand = new RelayCommand(SelectPrev);
            ActivateSelectedPersonaCommand = new RelayCommand(ActivateSelectedPersona);
            DeleteSelectedPersonaCommand = new RelayCommand(
                DeleteSelectedPersona,
                () => SelectedPersona is { IsAddPlaceholder: false });
            ToggleConfigPanelCommand = new RelayCommand(ToggleConfigPanel);
            RefreshSelectedPersonaColorCommand = new AsyncRelayCommand(
                RefreshSelectedPersonaColorAsync,
                () => SelectedPersona is { IsAddPlaceholder: false });

            Personas.CollectionChanged += OnPersonasCollectionChanged;
            LoadPersonas();
        }

        public ObservableCollection<PersonaModel> Personas { get; } = new();

        public PersonaModel? SelectedPersona
        {
            get => _selectedPersona;
            set
            {
                if (SetProperty(ref _selectedPersona, value))
                {
                    OnPropertyChanged(nameof(SelectedPersonaIndex));
                    OnPropertyChanged(nameof(IsEditablePersonaSelected));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public int SelectedPersonaIndex
        {
            get => SelectedPersona == null ? -1 : Personas.IndexOf(SelectedPersona);
            set
            {
                if (value >= 0 && value < Personas.Count)
                {
                    SelectedPersona = Personas[value];
                }
            }
        }

        public bool IsEditablePersonaSelected => SelectedPersona is { IsAddPlaceholder: false };

        public bool IsConfigPanelExpanded
        {
            get => _isConfigPanelExpanded;
            set
            {
                var wasExpanded = _isConfigPanelExpanded;
                if (SetProperty(ref _isConfigPanelExpanded, value) && wasExpanded && !value)
                {
                    _ = GenerateMissingPersonaColorsAsync();
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetProperty(ref _statusMessage, value ?? string.Empty);
        }

        public ICommand NextPersonaCommand { get; }

        public ICommand PrevPersonaCommand { get; }

        public ICommand ActivateSelectedPersonaCommand { get; }

        public ICommand DeleteSelectedPersonaCommand { get; }

        public ICommand ToggleConfigPanelCommand { get; }

        public ICommand RefreshSelectedPersonaColorCommand { get; }

        private void LoadPersonas()
        {
            try
            {
                using (SuspendPersistence())
                {
                    Personas.Clear();
                    foreach (var persona in _repository.Load())
                    {
                        Personas.Add(persona);
                    }

                    EnsureAddPlaceholder();
                    SelectedPersona = Personas.FirstOrDefault(persona => !persona.IsAddPlaceholder) ?? Personas.FirstOrDefault();
                }

                StatusMessage = "Persona 配置已加载。";
            }
            catch (Exception ex)
            {
                using (SuspendPersistence())
                {
                    Personas.Clear();
                    var fallback = PersonaConfigurationRepository.CreateBlankPersona(0);
                    fallback.Name = "Persona 1";
                    Personas.Add(fallback);
                    EnsureAddPlaceholder();
                    SelectedPersona = fallback;
                }

                StatusMessage = $"Persona 配置加载失败：{ex.Message}";
            }
        }

        private void SelectNext()
        {
            if (Personas.Count == 0)
            {
                return;
            }

            var currentIndex = SelectedPersonaIndex;
            var nextIndex = currentIndex < 0 ? 0 : (currentIndex + 1) % Personas.Count;
            SelectedPersonaIndex = nextIndex;
        }

        private void SelectPrev()
        {
            if (Personas.Count == 0)
            {
                return;
            }

            var currentIndex = SelectedPersonaIndex;
            var prevIndex = currentIndex < 0 ? 0 : (currentIndex - 1 + Personas.Count) % Personas.Count;
            SelectedPersonaIndex = prevIndex;
        }

        private void ToggleConfigPanel()
        {
            IsConfigPanelExpanded = !IsConfigPanelExpanded;
        }

        private void ActivateSelectedPersona()
        {
            if (SelectedPersona is not { IsAddPlaceholder: true } placeholder)
            {
                return;
            }

            SelectedPersona = ConvertPlaceholderToPersona(placeholder);
        }

        private void DeleteSelectedPersona()
        {
            if (SelectedPersona is not { IsAddPlaceholder: false } persona)
            {
                return;
            }

            var removedIndex = Personas.IndexOf(persona);
            if (removedIndex < 0)
            {
                return;
            }

            PersonaModel? nextSelection;
            using (SuspendPersistence())
            {
                Personas.RemoveAt(removedIndex);
                EnsureAddPlaceholder();
                nextSelection = PickSelectionAfterDelete(removedIndex);
                SelectedPersona = nextSelection;
            }

            PersistAll("已删除 Persona。");
        }

        private PersonaModel? PickSelectionAfterDelete(int removedIndex)
        {
            if (Personas.Count == 0)
            {
                return null;
            }

            var targetIndex = Math.Min(removedIndex, Personas.Count - 1);
            if (Personas[targetIndex] is { IsAddPlaceholder: false } persona)
            {
                return persona;
            }

            return Personas
                .Take(targetIndex)
                .LastOrDefault(item => !item.IsAddPlaceholder)
                ?? Personas.FirstOrDefault();
        }

        private PersonaModel ConvertPlaceholderToPersona(PersonaModel placeholder)
        {
            var placeholderIndex = Personas.IndexOf(placeholder);
            if (placeholderIndex < 0)
            {
                return placeholder;
            }

            PersonaModel persona;
            using (SuspendPersistence())
            {
                DetachPersona(placeholder);
                persona = PersonaConfigurationRepository.CreateBlankPersona(CountEditablePersonas());
                Personas[placeholderIndex] = persona;
                AttachPersona(persona);
                EnsureAddPlaceholder();
            }

            PersistAll("已添加 Persona。");
            return persona;
        }

        private async Task RefreshSelectedPersonaColorAsync()
        {
            if (SelectedPersona is not { IsAddPlaceholder: false } persona)
            {
                return;
            }

            var refreshed = await _colorService.TryUpdatePersonaColorsAsync(persona).ConfigureAwait(true);
            PersistAll(refreshed ? "Persona 颜色已更新。" : "未找到可用嵌入模型，Persona 颜色保持当前值。");
        }

        private async Task GenerateMissingPersonaColorsAsync()
        {
            if (_isGeneratingMissingColors)
            {
                return;
            }

            var targets = Personas
                .Where(persona => !persona.IsAddPlaceholder && IsMissingColor(persona))
                .ToArray();
            if (targets.Length == 0)
            {
                return;
            }

            _isGeneratingMissingColors = true;
            StatusMessage = $"正在并行生成 {targets.Length} 个 Persona 颜色...";
            try
            {
                using (SuspendPersistence())
                {
                    var results = await Task
                        .WhenAll(targets.Select(persona => _colorService.TryUpdatePersonaColorsAsync(persona)))
                        .ConfigureAwait(true);
                    var refreshedCount = results.Count(result => result);

                    if (refreshedCount > 0)
                    {
                        PersistAllAfterSuspension($"已并行生成 {refreshedCount} 个 Persona 颜色。");
                    }
                    else
                    {
                        StatusMessage = "未找到可用嵌入模型，Persona 颜色保持灰色。";
                    }
                }
            }
            finally
            {
                _isGeneratingMissingColors = false;
            }
        }

        private static bool IsMissingColor(PersonaModel persona)
        {
            return IsDefaultColor(persona.ColorHex, DefaultOrbColor) ||
                   IsDefaultColor(persona.BackgroundColorHex, DefaultBackgroundColor);
        }

        private static bool IsDefaultColor(string? value, string defaultColor)
        {
            return string.IsNullOrWhiteSpace(value) ||
                   string.Equals(value.Trim(), defaultColor, StringComparison.OrdinalIgnoreCase);
        }

        private void MarkPersonaColorMissing(PersonaModel persona)
        {
            if (IsMissingColor(persona))
            {
                return;
            }

            using (SuspendPersistence())
            {
                persona.ColorHex = DefaultOrbColor;
                persona.BackgroundColorHex = DefaultBackgroundColor;
            }

            PersistAll("Persona 配置已保存，颜色将在收回设置页面后重新生成。");
        }

        private void OnPersonasCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var persona in e.NewItems.OfType<PersonaModel>())
                {
                    AttachPersona(persona);
                }
            }

            if (e.OldItems != null)
            {
                foreach (var persona in e.OldItems.OfType<PersonaModel>())
                {
                    DetachPersona(persona);
                }
            }

            OnPropertyChanged(nameof(SelectedPersonaIndex));
        }

        private void AttachPersona(PersonaModel persona)
        {
            persona.PropertyChanged -= OnPersonaPropertyChanged;
            persona.PropertyChanged += OnPersonaPropertyChanged;
        }

        private void DetachPersona(PersonaModel persona)
        {
            persona.PropertyChanged -= OnPersonaPropertyChanged;
        }

        private void OnPersonaPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is not PersonaModel persona || persona.IsAddPlaceholder)
            {
                return;
            }

            if (IsDerivedVisualProperty(e.PropertyName))
            {
                return;
            }

            if (IsColorAffectingProperty(e.PropertyName))
            {
                MarkPersonaColorMissing(persona);
                return;
            }

            if (!IsColorStorageProperty(e.PropertyName))
            {
                PersistAll("Persona 配置已保存。");
            }
        }

        private static bool IsColorAffectingProperty(string? propertyName)
        {
            return propertyName is nameof(PersonaModel.Name)
                or nameof(PersonaModel.Description)
                or nameof(PersonaModel.AgentName)
                or nameof(PersonaModel.UserName)
                or nameof(PersonaModel.Tone)
                or nameof(PersonaModel.ModalParticles)
                or nameof(PersonaModel.Prompt);
        }

        private static bool IsColorStorageProperty(string? propertyName)
        {
            return propertyName is nameof(PersonaModel.ColorHex)
                or nameof(PersonaModel.BackgroundColorHex);
        }

        private static bool IsDerivedVisualProperty(string? propertyName)
        {
            return propertyName is nameof(PersonaModel.OrbBrush)
                or nameof(PersonaModel.BackgroundBrush);
        }

        private void EnsureAddPlaceholder()
        {
            var placeholders = Personas.Where(persona => persona.IsAddPlaceholder).ToArray();
            foreach (var extra in placeholders.Skip(1))
            {
                Personas.Remove(extra);
            }

            var placeholder = Personas.FirstOrDefault(persona => persona.IsAddPlaceholder);
            if (placeholder == null)
            {
                Personas.Add(PersonaConfigurationRepository.CreateAddPlaceholder());
                return;
            }

            var targetIndex = Personas.Count - 1;
            var currentIndex = Personas.IndexOf(placeholder);
            if (currentIndex >= 0 && currentIndex != targetIndex)
            {
                Personas.Move(currentIndex, targetIndex);
            }
        }

        private int CountEditablePersonas()
        {
            return Personas.Count(persona => !persona.IsAddPlaceholder);
        }

        private void PersistAllAfterSuspension(string successMessage)
        {
            var resumeCount = _suspendPersistenceCounter;
            _suspendPersistenceCounter = 0;
            try
            {
                PersistAll(successMessage);
            }
            finally
            {
                _suspendPersistenceCounter = resumeCount;
            }
        }

        private void PersistAll(string successMessage)
        {
            if (_suspendPersistenceCounter > 0)
            {
                return;
            }

            try
            {
                _repository.Save(Personas);
                StatusMessage = successMessage;
            }
            catch (Exception ex)
            {
                StatusMessage = $"保存 Persona 配置失败：{ex.Message}";
            }
        }

        private IDisposable SuspendPersistence()
        {
            _suspendPersistenceCounter++;
            return new DelegateDisposable(() => _suspendPersistenceCounter--);
        }

        private sealed class DelegateDisposable : IDisposable
        {
            private readonly Action _disposeAction;
            private bool _isDisposed;

            public DelegateDisposable(Action disposeAction)
            {
                _disposeAction = disposeAction;
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }

                _isDisposed = true;
                _disposeAction();
            }
        }
    }
}

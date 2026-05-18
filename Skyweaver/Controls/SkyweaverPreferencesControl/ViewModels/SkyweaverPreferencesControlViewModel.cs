using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Skyweaver.Commands;
using Skyweaver.Controls.SkyweaverPreferencesControl.Models;
using Skyweaver.Controls.SkyweaverPreferencesControl.Services;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Panels.MultiFunctionArea.ViewModels;
using Skyweaver.Services.Localization;

namespace Skyweaver.Controls.SkyweaverPreferencesControl.ViewModels
{
    public sealed class SkyweaverPreferencesControlViewModel : ObservableObject
    {
        private readonly Action<string>? _openTabByType;
        private readonly Dictionary<string, UserControl> _viewCache = new(StringComparer.Ordinal);
        private SelectablePreferencePageViewModel? _selectedPageViewModel;
        private UserControl? _currentView;
        private string? _currentPageName;

        public SkyweaverPreferencesControlViewModel()
            : this(null)
        {
        }

        public SkyweaverPreferencesControlViewModel(Action<string>? openTabByType)
        {
            _openTabByType = openTabByType;
            SkyweaverPreferencesRegistration.EnsureRegistered();

            var modelGroups = PreferenceRegistry.Instance.Groups;
            Groups = new ObservableCollection<PreferenceGroupViewModel>(
                modelGroups.Select(group => new PreferenceGroupViewModel(group)));

            modelGroups.CollectionChanged += (_, args) =>
            {
                if (args.NewItems != null)
                {
                    foreach (PreferenceGroup group in args.NewItems)
                    {
                        Groups.Add(new PreferenceGroupViewModel(group));
                    }
                }

                if (args.OldItems == null)
                {
                    return;
                }

                foreach (PreferenceGroup group in args.OldItems)
                {
                    var existing = Groups.FirstOrDefault(candidate => ReferenceEquals(candidate.Group, group));
                    if (existing != null)
                    {
                        Groups.Remove(existing);
                    }
                }
            };

            SelectPageCommand = new RelayCommand<SelectablePreferencePageViewModel>(
                page => SelectedPageViewModel = page,
                page => page is not null);
            OpenLanguageModelConfigurationCommand = new RelayCommand(
                () => OpenTab(MultiFunctionAreaPanelViewModel.TabTypes.LanguageModelConfiguration),
                CanOpenExternalPanel);
            OpenEmbeddingModelConfigurationCommand = new RelayCommand(
                () => OpenTab(MultiFunctionAreaPanelViewModel.TabTypes.EmbeddingModelConfiguration),
                CanOpenExternalPanel);
            OpenToolConfigurationCommand = new RelayCommand(
                () => OpenTab(MultiFunctionAreaPanelViewModel.TabTypes.ToolConfiguration),
                CanOpenExternalPanel);
            OpenAgentConfigurationCommand = new RelayCommand(
                () => OpenTab(MultiFunctionAreaPanelViewModel.TabTypes.AgentConfiguration),
                CanOpenExternalPanel);
            OpenWorkflowEditorCommand = new RelayCommand(
                () => OpenTab(MultiFunctionAreaPanelViewModel.TabTypes.WorkflowEditor),
                CanOpenExternalPanel);

            LocalizationRuntime.Instance.LanguageChanged += (_, _) => RefreshLocalizedText();
        }

        public string Title => LocalizationRuntime.Instance.GetString("Preferences.Welcome.Title", "Skyweaver 首选项");

        public string Description => LocalizationRuntime.Instance.GetString("Preferences.Welcome.Description", "从左侧栏目中选择要查看的配置项。");

        public string Hint => LocalizationRuntime.Instance.GetString("Preferences.Welcome.Hint", "目前已提供文件系统、呈现界面与上下文管理相关配置。");

        public ObservableCollection<PreferenceGroupViewModel> Groups { get; }

        public ICommand SelectPageCommand { get; }

        public ICommand OpenLanguageModelConfigurationCommand { get; }

        public ICommand OpenEmbeddingModelConfigurationCommand { get; }

        public ICommand OpenToolConfigurationCommand { get; }

        public ICommand OpenAgentConfigurationCommand { get; }

        public ICommand OpenWorkflowEditorCommand { get; }

        public SelectablePreferencePageViewModel? SelectedPageViewModel
        {
            get => _selectedPageViewModel;
            set
            {
                if (!SetProperty(ref _selectedPageViewModel, value))
                {
                    return;
                }

                foreach (var group in Groups)
                {
                    foreach (var page in group.Pages)
                    {
                        page.IsSelected = ReferenceEquals(page, value);
                    }
                }

                if (value == null)
                {
                    CurrentView = null;
                    CurrentPageName = null;
                    return;
                }

                ActivatePage(value.Id);
            }
        }

        public UserControl? CurrentView
        {
            get => _currentView;
            private set => SetProperty(ref _currentView, value);
        }

        public string? CurrentPageName
        {
            get => _currentPageName;
            private set => SetProperty(ref _currentPageName, value);
        }

        private void ActivatePage(string pageId)
        {
            var pageInfo = PreferenceRegistry.Instance.GetPageInfo(pageId);
            if (pageInfo == null)
            {
                CurrentView = null;
                CurrentPageName = null;
                return;
            }

            CurrentPageName = GetLocalizedPageName(pageInfo);

            if (_viewCache.TryGetValue(pageId, out var cachedView))
            {
                CurrentView = cachedView;
                return;
            }

            var view = CreateView(pageInfo);
            if (view == null)
            {
                CurrentView = null;
                return;
            }

            _viewCache[pageId] = view;
            CurrentView = view;
        }

        private static UserControl? CreateView(PreferencePageInfo pageInfo)
        {
            if (pageInfo.ViewType == null)
            {
                return null;
            }

            try
            {
                if (Activator.CreateInstance(pageInfo.ViewType) is not UserControl view)
                {
                    return null;
                }

                if (pageInfo.ViewModelType != null)
                {
                    view.DataContext = Activator.CreateInstance(pageInfo.ViewModelType);
                }

                return view;
            }
            catch
            {
                return null;
            }
        }

        private bool CanOpenExternalPanel()
        {
            return _openTabByType != null;
        }

        private void OpenTab(string typeKey)
        {
            _openTabByType?.Invoke(typeKey);
        }

        private void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Hint));

            if (_selectedPageViewModel != null)
            {
                CurrentPageName = GetLocalizedPageName(_selectedPageViewModel.PageInfo);
            }
        }

        private static string GetLocalizedPageName(PreferencePageInfo pageInfo)
        {
            return LocalizationRuntime.Instance.GetString(pageInfo.DisplayNameResourceKey, pageInfo.DisplayName);
        }
    }
}

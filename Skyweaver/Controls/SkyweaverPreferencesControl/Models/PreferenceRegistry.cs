using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Skyweaver.Controls.SkyweaverPreferencesControl.Models
{
    public sealed class PreferenceRegistry
    {
        private static readonly Lazy<PreferenceRegistry> s_instance = new(() => new PreferenceRegistry());

        private PreferenceRegistry()
        {
            Groups = new ObservableCollection<PreferenceGroup>
            {
                new()
                {
                    Id = "files-system",
                    DisplayName = "文件与系统",
                    IsExpanded = true
                },
                new()
                {
                    Id = "presentation-ui",
                    DisplayName = "呈现与界面",
                    IsExpanded = true
                },
                new()
                {
                    Id = "context-management",
                    DisplayName = "上下文管理",
                    IsExpanded = true
                },
                new()
                {
                    Id = "about-skyweaver",
                    DisplayName = "关于Skyweaver",
                    IsExpanded = true
                }
            };
        }

        public static PreferenceRegistry Instance => s_instance.Value;

        public ObservableCollection<PreferenceGroup> Groups { get; }

        public PreferenceGroup? GetGroup(string groupId)
        {
            return Groups.FirstOrDefault(group => string.Equals(group.Id, groupId, StringComparison.Ordinal));
        }

        public PreferencePageInfo? GetPageInfo(string pageId)
        {
            foreach (var group in Groups)
            {
                var page = group.Pages.FirstOrDefault(candidate => string.Equals(candidate.Id, pageId, StringComparison.Ordinal));
                if (page != null)
                {
                    return page;
                }
            }

            return null;
        }

        public void RegisterPage(string groupId, PreferencePageInfo pageInfo)
        {
            var group = GetGroup(groupId);
            if (group == null)
            {
                throw new ArgumentException($"Unknown preference group '{groupId}'.", nameof(groupId));
            }

            if (group.Pages.Any(existing => string.Equals(existing.Id, pageInfo.Id, StringComparison.Ordinal)))
            {
                return;
            }

            var insertIndex = group.Pages.Count;
            for (var index = 0; index < group.Pages.Count; index++)
            {
                if (group.Pages[index].Order > pageInfo.Order)
                {
                    insertIndex = index;
                    break;
                }
            }

            group.Pages.Insert(insertIndex, pageInfo);
        }
    }
}

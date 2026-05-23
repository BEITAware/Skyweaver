using System;
using System.Collections.ObjectModel;
using Skyweaver.Infrastructure.Mvvm;
using Skyweaver.Services.Localization;

namespace Skyweaver.Services.Notifications
{
    /// <summary>
    /// 提供应用内的全局通知服务
    /// </summary>
    public class NotificationService : ObservableObject
    {
        private static readonly Lazy<NotificationService> _instance = 
            new Lazy<NotificationService>(() => new NotificationService());

        public static NotificationService Instance => _instance.Value;

        private string _currentTransientMessage = string.Empty;
        private string _userTransientMessage = string.Empty;
        
        /// <summary>
        /// 获取当前的一过性通知内容
        /// </summary>
        public string CurrentTransientMessage
        {
            get => _currentTransientMessage;
            private set => SetProperty(ref _currentTransientMessage, value);
        }

        /// <summary>
        /// 活动的常驻通知列表
        /// </summary>
        public ObservableCollection<PermanentNotificationItem> ActivePermanentNotifications { get; } 
            = new ObservableCollection<PermanentNotificationItem>();

        private NotificationService()
        {
            LocalizationRuntime.Instance.LanguageChanged += OnLanguageChanged;
            UpdateTransientMessage();
        }

        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            UpdateTransientMessage();
        }

        private void UpdateTransientMessage()
        {
            RunOnUi(() =>
            {
                if (string.IsNullOrEmpty(_userTransientMessage))
                {
                    CurrentTransientMessage = LocalizationRuntime.Instance.GetString("MainWindow.Status.Welcome", "欢迎使用Skyweaver！");
                }
                else
                {
                    CurrentTransientMessage = _userTransientMessage;
                }
            });
        }

        /// <summary>
        /// 创建或更新一过性通知
        /// </summary>
        /// <param name="message">通知内容</param>
        public void ShowTransient(string message)
        {
            RunOnUi(() =>
            {
                _userTransientMessage = message;
                UpdateTransientMessage();
            });
        }

        /// <summary>
        /// 清除当前的一过性通知
        /// </summary>
        public void ClearTransient()
        {
            RunOnUi(() =>
            {
                _userTransientMessage = string.Empty;
                UpdateTransientMessage();
            });
        }

        /// <summary>
        /// 创建一条新的常驻通知
        /// </summary>
        /// <param name="message">初始内容</param>
        /// <param name="progress">初始进度（若为null表示不确定进度）</param>
        /// <returns>通知ID，用于后续更新或移除</returns>
        public string CreatePermanent(string message, double? progress = null)
        {
            var id = Guid.NewGuid().ToString();
            var item = new PermanentNotificationItem(id, message, progress);
            
            RunOnUi(() =>
            {
                ActivePermanentNotifications.Add(item);
            });
            
            return id;
        }

        /// <summary>
        /// 更新指定ID的常驻通知
        /// </summary>
        /// <param name="id">常驻通知ID</param>
        /// <param name="message">新的通知内容</param>
        /// <param name="progress">新的进度值</param>
        public void UpdatePermanent(string id, string message, double? progress = null)
        {
            RunOnUi(() =>
            {
                foreach (var item in ActivePermanentNotifications)
                {
                    if (item.Id == id)
                    {
                        item.Message = message;
                        item.Progress = progress;
                        break;
                    }
                }
            });
        }

        /// <summary>
        /// 移除/结束指定ID的常驻通知
        /// </summary>
        /// <param name="id">常驻通知ID</param>
        public void RemovePermanent(string id)
        {
            RunOnUi(() =>
            {
                for (int i = 0; i < ActivePermanentNotifications.Count; i++)
                {
                    if (ActivePermanentNotifications[i].Id == id)
                    {
                        ActivePermanentNotifications.RemoveAt(i);
                        break;
                    }
                }
            });
        }

        private void RunOnUi(Action action)
        {
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }

    /// <summary>
    /// 常驻通知项的数据实体
    /// </summary>
    public class PermanentNotificationItem : ObservableObject
    {
        public string Id { get; }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private double? _progress;
        public double? Progress
        {
            get => _progress;
            set => SetProperty(ref _progress, value);
        }

        public PermanentNotificationItem(string id, string message, double? progress)
        {
            Id = id;
            _message = message;
            _progress = progress;
        }
    }
}

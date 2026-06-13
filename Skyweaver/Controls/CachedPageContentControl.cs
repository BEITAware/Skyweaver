using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Skyweaver.Controls
{
    public sealed class CachedPageContentControl : ContentControl
    {
        public static readonly DependencyProperty PageContentProperty =
            DependencyProperty.Register(
                nameof(PageContent),
                typeof(object),
                typeof(CachedPageContentControl),
                new PropertyMetadata(null, OnPageContentChanged));

        private readonly Dictionary<object, object> _viewCache = new(ReferenceEqualityComparer.Instance);

        public object? PageContent
        {
            get => GetValue(PageContentProperty);
            set => SetValue(PageContentProperty, value);
        }

        public object? GetOrCreateCachedView(object? pageContent)
        {
            if (pageContent == null)
            {
                return null;
            }

            if (!_viewCache.TryGetValue(pageContent, out var cachedView))
            {
                cachedView = CreateView(pageContent);
                _viewCache[pageContent] = cachedView;
            }

            return cachedView;
        }

        private static void OnPageContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CachedPageContentControl)d).ShowPageContent(e.NewValue);
        }

        private void ShowPageContent(object? pageContent)
        {
            if (pageContent == null)
            {
                Content = null;
                return;
            }

            var cachedView = GetOrCreateCachedView(pageContent);
            DetachFromCurrentParent(cachedView);
            Content = cachedView;
        }

        private object CreateView(object pageContent)
        {
            var template = ResolveTemplate(pageContent);
            if (template?.LoadContent() is object view)
            {
                ApplyDataContext(view, pageContent);
                return view;
            }

            return pageContent;
        }

        private DataTemplate? ResolveTemplate(object pageContent)
        {
            if (ContentTemplate != null)
            {
                return ContentTemplate;
            }

            return TryFindResource(new DataTemplateKey(pageContent.GetType())) as DataTemplate;
        }

        private static void ApplyDataContext(object view, object pageContent)
        {
            switch (view)
            {
                case FrameworkElement frameworkElement:
                    frameworkElement.DataContext = pageContent;
                    break;
                case FrameworkContentElement frameworkContentElement:
                    frameworkContentElement.DataContext = pageContent;
                    break;
            }
        }

        private static void DetachFromCurrentParent(object? view)
        {
            if (view is not FrameworkElement frameworkElement)
            {
                return;
            }

            if (frameworkElement.Parent is ContentControl parentContentControl &&
                ReferenceEquals(parentContentControl.Content, view))
            {
                parentContentControl.Content = null;
            }
        }
    }
}

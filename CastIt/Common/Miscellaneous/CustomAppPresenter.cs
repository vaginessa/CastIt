﻿using CastIt.Common.Utils;
using MvvmCross.Platforms.Wpf.Presenters;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.Presenters;
using MvvmCross.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CastIt.Common.Miscellaneous
{
    public class CustomAppPresenter : MvxWpfViewPresenter
    {
        public CustomAppPresenter(ContentControl root) : base(root)
        {
        }

        public override void RegisterAttributeTypes()
        {
            base.RegisterAttributeTypes();
            AttributeTypesToActionsDictionary.Add(typeof(CustomMvxContentPresentationAttribute), new MvxPresentationAttributeAction
            {
                ShowAction = (viewType, attribute, request) =>
                {
                    var view = WpfViewLoader.CreateView(request);
                    return ShowContentView(view, (CustomMvxContentPresentationAttribute)attribute, request);
                },
                CloseAction = (viewModel, attribute) => CloseContentView(viewModel)
            });
        }

        protected override Task<bool> ShowContentView(
            FrameworkElement element,
            MvxContentPresentationAttribute attribute,
            MvxViewModelRequest request)
        {
            if (!(attribute is CustomMvxContentPresentationAttribute customMvxViewForAttribute))
                return base.ShowContentView(element, attribute, request);

            var contentControl = FrameworkElementsDictionary.Keys
                .FirstOrDefault(w => (w as MvxWindow)?.Identifier == attribute.WindowIdentifier)
                ?? FrameworkElementsDictionary.Keys.Last();

            if (!attribute.StackNavigation && FrameworkElementsDictionary[contentControl].Any())
                FrameworkElementsDictionary[contentControl].Pop(); // Close previous view

            if (WindowsUtils.GetDescendantFromName(contentControl, customMvxViewForAttribute.ContentFrame) is Frame frame)
            {
                frame.Content = element;
            }

            //FrameworkElementsDictionary[contentControl].Push(element);
            //contentControl.Content = element;
            return Task.FromResult(true);
        }
    }
}

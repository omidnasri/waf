﻿using Jbe.NewsReader.Applications.Services;
using Jbe.NewsReader.Applications.Views;
using Jbe.NewsReader.Domain;
using Jbe.NewsReader.Domain.Foundation;
using System;
using System.ComponentModel;
using System.Composition;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Input;

namespace Jbe.NewsReader.Applications.ViewModels
{
    [Export, Shared]
    public class FeedItemListViewModel : ViewModel<IFeedItemListView>
    {
        private ObservableListView<FeedItem> itemsListView;
        private string searchText = "";


        [ImportingConstructor]
        public FeedItemListViewModel(IFeedItemListView view, SelectionService selectionService) : base(view)
        {
            SelectionService = selectionService;
            SelectionService.PropertyChanged += SelectionServicePropertyChanged;
            UpdateItemsListView();
        }

        
        public SelectionService SelectionService { get; }

        public ICommand RefreshCommand { get; set; }

        public ICommand ReadUnreadCommand { get; set; }

        public ICommand ShowFeedItemViewCommand { get; set; }

        public IReadOnlyObservableList<FeedItem> ItemsListView => itemsListView;
        
        public string SearchText
        {
            get { return searchText; }
            set
            {
                if (SetProperty(ref searchText, value))
                {
                    itemsListView.Refresh();
                }
            }
        }
        

        private bool FilterFeedItems(FeedItem item)
        {
            return string.IsNullOrEmpty(SearchText)
                || (item.Name ?? "").IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0
                || (item.Description ?? "").IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0
                || (item.Author ?? "").IndexOf(SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private void UpdateItemsListView()
        {
            itemsListView = new ObservableListView<FeedItem>(SelectionService.SelectedFeed?.Items ?? CollectionHelper.Empty<FeedItem>()) { Filter = FilterFeedItems };
            RaisePropertyChanged(nameof(ItemsListView));
        }

        private void SelectionServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SelectionService.SelectedFeed))
            {
                UpdateItemsListView();
            }
        }
    }
}

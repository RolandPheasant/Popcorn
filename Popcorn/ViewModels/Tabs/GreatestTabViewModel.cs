﻿using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Popcorn.Helpers;
using Popcorn.Messaging;
using GalaSoft.MvvmLight.CommandWpf;

namespace Popcorn.ViewModels.Tabs
{
    /// <summary>
    /// The greatest movies tab
    /// </summary>
    public sealed class GreatestTabViewModel : TabsViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the GreatestTabViewModel class.
        /// </summary>
        private GreatestTabViewModel()
        {
            RegisterMessages();
            RegisterCommands();
            TabName = LocalizationProviderHelper.GetLocalizedValue<string>("GreatestTitleTab");
        }

        #endregion

        #region Methods

        #region Method -> InitializeAsync
        /// <summary>
        /// Load asynchronously the greatest movies for the current instance
        /// </summary>
        /// <returns>Instance of TrailerViewModel</returns>
        private async Task<GreatestTabViewModel> InitializeAsync()
        {
            await LoadNextPageAsync();
            return this;
        }
        #endregion

        #region Method -> CreateAsync
        /// <summary>
        /// Initialize asynchronously an instance of the GreatestTabViewModel class
        /// </summary>
        /// <returns>Instance of GreatestTabViewModel</returns>
        public static Task<GreatestTabViewModel> CreateAsync()
        {
            var ret = new GreatestTabViewModel();
            return ret.InitializeAsync();
        }
        #endregion

        #region Method -> RegisterMessages

        /// <summary>
        /// Register messages
        /// </summary>
        private void RegisterMessages()
        {
            Messenger.Default.Register<ChangeLanguageMessage>(
                this,
                language => { TabName = LocalizationProviderHelper.GetLocalizedValue<string>("GreatestTitleTab"); });
        }

        #endregion

        #region Method -> RegisterCommands

        /// <summary>
        /// Register commands
        /// </summary>
        private void RegisterCommands()
        {
            ReloadMovies = new RelayCommand(async () =>
            {
                await LoadNextPageAsync();
            });
        }

        #endregion

        #region Method -> LoadNextPageAsync

        /// <summary>
        /// Load next page with an optional search parameter
        /// </summary>
        public async Task LoadNextPageAsync()
        {
            Page++;
            IsLoadingMovies = true;
            try
            {
                var movieResults =
                    await MovieService.GetTopRatedMoviesAsync(Page,
                        MaxMoviesPerPage,
                        CancellationLoadNextPageToken.Token);
                var movies = movieResults.Item1.ToList();
                MaxNumberOfMovies = movieResults.Item2;

                foreach (var movie in movies)
                {
                    Movies.Add(movie);
                }

                await UserService.ComputeMovieHistoryAsync(movies);
                await MovieService.DownloadCoverImageAsync(movies);
            }
            catch
            {
                Page--;
            }
            finally
            {
                IsLoadingMovies = false;
                IsMovieFound = Movies.Any();
                CurrentNumberOfMovies = Movies.Count();
            }
        }

        #endregion

        #endregion
    }
}
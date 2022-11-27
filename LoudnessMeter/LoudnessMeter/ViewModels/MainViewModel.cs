using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LoudnessMeter.DataModel;
using LoudnessMeter.Services;
using System.Linq;
using System.Threading.Tasks;

namespace LoudnessMeter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        #region Private Members

        private IAudioInterfaceService _audioInterfaceService;

        #endregion

        #region Public Properties

        [ObservableProperty]
        private string _boldTitle = "AVALONIA";

        [ObservableProperty]
        private string _regularTitle = "LOUDNESS METER";

        [ObservableProperty]
        private bool _channelConfigurationListIsOpen = false;

        [ObservableProperty]
        private ObservableGroupedCollection<string, ChannelConfigurationItem>? _channelConfigurations = default;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ChannelConfigurationButtonText))]
        private ChannelConfigurationItem? _selectedChannelConfiguration;

        public string ChannelConfigurationButtonText => SelectedChannelConfiguration?.ShortText ?? "Select Channel";

        #endregion

        #region Public Commands

        [RelayCommand]
        private void ChannelConfigurationButtonPressed() => ChannelConfigurationListIsOpen ^= true;

        [RelayCommand]
        private void ChannelConfigurationItemPressed(ChannelConfigurationItem item)
        {
            // Update the selected item
            SelectedChannelConfiguration = item;

            // Close the menu
            ChannelConfigurationListIsOpen = false;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="audioInterfaceService">The audio interface service</param>
        public MainViewModel(IAudioInterfaceService audioInterfaceService)
        {
            _audioInterfaceService = audioInterfaceService;
        }

        /// <summary>
        /// Design-time constructor
        /// </summary>
        public MainViewModel()
        {
            _audioInterfaceService = new DummyAudioInterfaceService();
        }

        [RelayCommand]
        private async Task LoadSettingsAsync()
        {
            // Get the channel configuration data
            var channelConfigurations = await _audioInterfaceService.GetChannelConfigurationsAsync();

            // Create a grouping from the flat data
            ChannelConfigurations =
                new ObservableGroupedCollection<string, ChannelConfigurationItem>(
                    channelConfigurations.GroupBy(item => item.Group));
        }

        #endregion
    }
}

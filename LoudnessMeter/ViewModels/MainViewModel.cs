using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace LoudnessMeter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string boldTitle = "AVALONIA";

        [ObservableProperty]
        private string regularTitle = "LOUDNESS METER";

        [ObservableProperty]
        private bool channelConfigurationListIsOpen = false;

        [RelayCommand]
        private void ChannelConfigurationButtonPressed() => ChannelConfigurationListIsOpen ^= true;
    }
}

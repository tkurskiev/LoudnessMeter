using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;

namespace LoudnessMeter.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string boldTitle = "AVALONIA";

        [ObservableProperty]
        private string regularTitle = "LOUDNESS METER";
    }
}

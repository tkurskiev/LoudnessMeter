using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using LoudnessMeter.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace LoudnessMeter.Views
{
    public partial class MainView : UserControl
    {
        #region Private Members

        private Control _channelConfigPopup;
        private Control _channelConfigButton;
        private Control _mainGrid;

        #endregion

        protected override async void OnLoaded()
        {
            //await ((MainViewModel?)DataContext)?.LoadSettingsCommand?.ExecuteAsync(null);

            var command = ((MainViewModel?)DataContext)?.LoadSettingsCommand;

            if (command is not null)
                await command.ExecuteAsync(null);

            base.OnLoaded();
        }

        public MainView()
        {
            InitializeComponent();

            _channelConfigButton = this.FindControl<Control>("ChannelConfigurationButton") ?? throw new Exception("Cannot find channel configuration button by name");
            _channelConfigPopup = this.FindControl<Control>("ChannelConfigurationPopup") ?? throw new Exception("Cannot find channel configuration popup by name");
            _mainGrid = this.FindControl<Control>("MainGrid") ?? throw new Exception("Cannot find channel main grid by name");
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            // Render can stuck in a loop infinitely if we change UI element on the UI thread, because this change requires another
            // render call, which leads to another render call, etc...
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                // Get relative position of button, in relation to main grid
                var position = _channelConfigButton.TranslatePoint(new Point(), _mainGrid) ??
                               throw new Exception("Cannot get TranslatePoint from Configuration Button");

                // Set margin on popup, so it appears bottom left of button
                _channelConfigPopup.Margin = new Thickness(
                    position.X,
                    0,
                    0,
                    _mainGrid.Bounds.Height - position.Y - _channelConfigButton.Bounds.Height);
            });
        }

        private void InputElement_OnPointerPressed(object sender, PointerPressedEventArgs e) =>
            ((MainViewModel?)DataContext)?.ChannelConfigurationButtonPressedCommand.Execute(null);
    }
}
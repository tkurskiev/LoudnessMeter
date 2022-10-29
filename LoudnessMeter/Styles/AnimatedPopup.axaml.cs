﻿using System;
using System.Diagnostics;
using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;

namespace LoudnessMeter
{
    public partial class AnimatedPopup : ContentControl
    {
        #region Private members

        /// <summary>
        /// The underlay control for closing this popup
        /// </summary>
        private Control _underlayControl;

        /// <summary>
        /// Indicates if this is the first time we are animating
        /// </summary>
        private bool _isFirstAnimation = true;

        /// <summary>
        /// Indicates if we have captured the opacity value yet
        /// </summary>
        private bool _opacityCaptured = false;

        /// <summary>
        /// Store the control's original Opacity value at startup
        /// </summary>
        private double _originalOpacity;

        /// <summary>
        /// The speed of animation if FPS
        /// </summary>
        private TimeSpan _framerate = TimeSpan.FromSeconds(1 / 60.0);

        // Calculate the total ticks that make up the animation time
        private int TotalTicks => (int)(_animationTime.TotalSeconds / _framerate.TotalSeconds);

        /// <summary>
        /// Store the controls desired size
        /// </summary>
        private Size _desiredSize;

        /// <summary>
        /// A flag for when we are animating
        /// </summary>
        private bool _animating;

        /// <summary>
        /// Keeps track of if we have found the desired 100% width/height auto size
        /// </summary>
        private bool _sizeFound;

        /// <summary>
        /// The animation UI timer
        /// </summary>
        private readonly DispatcherTimer _animationTimer;

        /// <summary>
        /// The timeout timer to detect when auto-sizing has finished firing
        /// </summary>
        private Timer _sizingTimer;

        /// <summary>
        /// The current position in the animation
        /// </summary>
        private int _animationCurrentTick;

        #endregion

        #region Public Properties

        /// <summary>
        /// Indicates if the control is currently opened
        /// </summary>
        public bool IsOpened => _animationCurrentTick >= TotalTicks;

        #region Open

        private bool _open;

        public static readonly DirectProperty<AnimatedPopup, bool> OpenProperty =
            AvaloniaProperty.RegisterDirect<AnimatedPopup, bool>(nameof(Open), o => o.Open,
                (o, v) => o.Open = v);

        /// <summary>
        /// Property to sets whether the control should be opened or closed
        /// </summary>
        public bool Open
        {
            get => _open;
            set
            {
                // If we are opening...
                if(value)
                {
                    // If the parent is a grid...
                    if (Parent is Grid grid)
                    {
                        // Set grid row/column span
                        if (grid.RowDefinitions?.Count > 0)
                            _underlayControl.SetValue(Grid.RowSpanProperty, grid.RowDefinitions?.Count);

                        if (grid.ColumnDefinitions?.Count > 0)
                            _underlayControl.SetValue(Grid.ColumnSpanProperty, grid.ColumnDefinitions?.Count);

                        // Insert the underlay control
                        grid.Children.Insert(0, _underlayControl);
                    }
                }

                SetAndRaise(OpenProperty, ref _open, value);
            }
        }

        #endregion

        #region Animation Time

        // Fix for 3 seconds
        private TimeSpan _animationTime = TimeSpan.FromSeconds(0.17);

        public static readonly DirectProperty<AnimatedPopup, TimeSpan> AnimationTimeProperty =
            AvaloniaProperty.RegisterDirect<AnimatedPopup, TimeSpan>(nameof(AnimationTime), o => o.AnimationTime,
                (o, v) => o.AnimationTime = v);

        public TimeSpan AnimationTime
        {
            get => _animationTime;
            set => SetAndRaise(AnimationTimeProperty, ref _animationTime, value);
        }

        #endregion

        #region Underlay Opacity

        private double _underlayOpacity = 0.2;

        public static readonly DirectProperty<AnimatedPopup, double> UnderlayOpacityProperty = AvaloniaProperty.RegisterDirect<AnimatedPopup, double>(
            "UnderlayOpacity", o => o.UnderlayOpacity, (o, v) => o.UnderlayOpacity = v);

        public double UnderlayOpacity
        {
            get => _underlayOpacity;
            set => SetAndRaise(UnderlayOpacityProperty, ref _underlayOpacity, value);
        }

        #endregion

        #endregion

        #region Public Commands

        [RelayCommand]
        public void BeginOpen()
        {
            Open = true;

            // Update Animation
            UpdateAnimation();
        }

        [RelayCommand]
        public void BeginClose()
        {
            Open = false;

            // Update Animation
            UpdateAnimation();
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AnimatedPopup()
        {
            // Make a new underlay control
            _underlayControl = new Border
            {
                Background = Brushes.Black,
                Opacity = 0,
                ZIndex = 9
            };

            // On press, close popup
            _underlayControl.PointerPressed += (sender, args) =>
            {
                BeginClose();
            };

            // Set to invisible
            //Opacity = 0;

            // Make a new dispatch timer
            _animationTimer = new DispatcherTimer
            {
                // Set the timer to run 60 times a second
                Interval = _framerate
            };

            _sizingTimer = new Timer((t) =>
            {
                // If we have already calculated the size
                if (_sizeFound)
                {
                    // No longer accept new sizes...
                    return;
                }

                // We have found our desired size
                _sizeFound = true;

                // DesiredSize is a UI-threaded property, and _sizingTimer is a generic-threaded timer
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    // Set the desired size
                    _desiredSize = DesiredSize - Margin;

                    // Update Animation
                    UpdateAnimation();
                });
            });

            // Callback on every tick
            _animationTimer.Tick += (s, e) => AnimationTick();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Calculate and start any new required animations
        /// </summary>
        private void UpdateAnimation()
        {
            // Do nothing if we still haven't found our initial size
            if(!_sizeFound)
                return;

            // Start the animation thread again
            _animationTimer.Start();
        }

        /// <summary>
        /// Should be called when an open or close transition has complete
        /// </summary>
        private void AnimationComplete()
        {
            // If opened...
            if(_open)
            {
                // Set size to desired size...
                Width = _desiredSize.Width;
                Height = _desiredSize.Height;
            }
            // If closed...
            else
            {
                // Set size to 0...
                Width = 0;
                Height = 0;

                // If the parent is a grid...
                if(Parent is Grid grid)
                {
                    // Reset opacity
                    _underlayControl.Opacity = 0;

                    if (grid.Children.Contains(_underlayControl))
                        // Remove the underlay
                        grid.Children.Remove(_underlayControl);
                }
            }

            Width = _open ? _desiredSize.Width : 0;
            Height = _open ? _desiredSize.Height : 0;
        }

        /// <summary>
        /// Update control's sizes based on the next tick of an animation
        /// </summary>
        private void AnimationTick()
        {
            // If this is the first call after calculating the desired size
            if (_isFirstAnimation)
            {
                // Clear the flag
                _isFirstAnimation = false;

                // Stop this animation timer
                _animationTimer.Stop();

                // Reset opacity
                Opacity = _originalOpacity;

                // Set the final size (final percentage can add up to less than 100%, it could be 99.97%,
                // because of how we are calculating the easing and the percentage)
                // Bypass all animation and set size
                AnimationComplete();

                // Done on this tick
                return;
            }

            // If we have reached the end of our animation
            if ((_open && _animationCurrentTick >= TotalTicks) ||
                (!_open && _animationCurrentTick == 0))
            {
                // Stop this animation timer
                _animationTimer.Stop();

                // Set the final size (final percentage can add up to less than 100%, it could be 99.97%,
                // because of how we are calculating the easing and the percentage)
                // Bypass all animation and set size
                AnimationComplete();

                // Clear animating flag
                _animating = false;

                // Break out of code
                return;
            }

            // Set animating flag
            _animating = true;

            // TODO: 012. Avalonia UI - Direct Properties On Control - 37:14
            // Move the tick in the right direction
            _animationCurrentTick += _open ? 1 : -1;

            // Get percentage of the way through the current animation
            var percentageAnimated = (float)_animationCurrentTick / TotalTicks;

            // Make an animation easing
            var easing = new QuadraticEaseIn();

            // Calculate final width and height
            var finalWidth = _desiredSize.Width * easing.Ease(percentageAnimated);
            var finalHeight = _desiredSize.Height * easing.Ease(percentageAnimated);

            // Do our animation
            Width = finalWidth;
            Height = finalHeight;
            Debug.WriteLine($"Current popup width: {Width}, height: {Height}");

            // Animate underlay
            _underlayControl.Opacity = _underlayOpacity * easing.Ease(percentageAnimated);
            Debug.WriteLine($"Current underlay opacity: {_underlayControl.Opacity}");

            Debug.WriteLine($"\nCurrent tick: {_animationCurrentTick}");
        }

        #endregion

        public override void Render(DrawingContext context)
        {
            // If we have not yet found the desired size...
            if (!_sizeFound)
            {
                // If we have not yet captured the opacity...
                if (!_opacityCaptured)
                {
                    // Set flag to true
                    _opacityCaptured = true;

                    // Remember original control's opacity
                    _originalOpacity = Opacity;

                    // Hide control
                    Opacity = 0;
                }

                

                _sizingTimer.Change(100, int.MaxValue);
            }

            base.Render(context);
        }
    }
}

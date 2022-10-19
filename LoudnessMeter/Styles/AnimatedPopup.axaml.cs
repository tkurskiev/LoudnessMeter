using System;
using System.Diagnostics;
using System.Threading;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace LoudnessMeter
{
    public class AnimatedPopup : ContentControl
    {
        #region Private members

        /// <summary>
        /// Store the controls desired size
        /// </summary>
        private Size _desiredSize;

        /// <summary>
        /// A flag for when we are animating
        /// </summary>
        private bool _animating;

        /// <summary>
        /// The animation UI timer
        /// </summary>
        private readonly DispatcherTimer _animationTimer;

        /// <summary>
        /// The current position in the animation
        /// </summary>
        private int _animationCurrentTick;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AnimatedPopup()
        {
            // Get a 60 fps TimeSpan
            var framerate = TimeSpan.FromSeconds(1 / 60.0);

            // Make a new dispatch timer
            _animationTimer = new DispatcherTimer
            {
                // Set the timer to run 60 times a second
                Interval = framerate
            };

            // Fix for 3 seconds
            var animationTime = TimeSpan.FromSeconds(0.17);

            // Calculate the total ticks that make up the animation time
            var totalTicks = (int) (animationTime.TotalSeconds / framerate.TotalSeconds);

            // Keep track of current tick
            _animationCurrentTick = 0;

            // Callback on every tick
            _animationTimer.Tick += (sender, e) =>
            {
                // Increment current tick
                _animationCurrentTick++;

                // Set animating flag
                _animating = true;
                
                // If we have reached total ticks...
                if (_animationCurrentTick > totalTicks)
                {
                    // Stop this animation timer
                    _animationTimer.Stop();

                    // Clear animating flag
                    _animating = false;

                    // Break out of code
                    return;
                }

                // Get percentage of the way through the current animation
                var percentageAnimated = (float)_animationCurrentTick / totalTicks;

                // Make an animation easing
                var easing = new QuadraticEaseIn();


                // Calculate final width and height
                var finalWidth = _desiredSize.Width * easing.Ease(percentageAnimated);
                var finalHeight = _desiredSize.Height * easing.Ease(percentageAnimated);

                // Do our animation
                Width = finalWidth;
                Height = finalHeight;

                Debug.WriteLine($"Current tick: {_animationCurrentTick}");
            };
        }

        #endregion

        public override void Render(DrawingContext context)
        {
            // If we are not animating
            if (!_animating)
            {
                #region Commented out

                // Update the new desired size only once
                // NOTE: unsure of what the second measure pass adds to the size
                //       but for now ignore it
                //if (DesiredSize != Size.Empty && _desiredSize == Size.Empty)
                //{
                    
                //}

                #endregion

                // Update the new desired size (which includes the margin, so remove that from our calculation)
                _desiredSize = DesiredSize - Margin;

                // Reset animation position
                _animationCurrentTick = 0;

                // Start timer
                _animationTimer.Start();

                Debug.WriteLine($"Desired size: {_desiredSize}");
            }

            base.Render(context);
        }
    }
}

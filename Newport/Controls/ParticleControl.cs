using System;
using System.Windows.Input;

namespace Newport.Controls
{
    public class Particle
    {
        public Point Position;
        public Point Velocity;
        public double StartLife;
        public double Life;
        public double Decay;
        public double StartSize;
        public double Size;
        public Color Color;
    }

    public class ParticleControl : GraphicsView, IDrawable
    {
        private readonly IDispatcherTimer _timer;
        private readonly List<Particle> _particles = new();
        private readonly List<Particle> _deadList = new();
        private readonly double _elapsed;

        public ParticleControl()
        {
            BackgroundColor = Colors.Transparent;
            InputTransparent = true;
            Drawable = this;

            _elapsed = 0.1;
            Speed = 20;
            OriginVariance = 5;
            ParticleSize = 12;
            ParticleSizeVariance = 5;
            MaxParticleCount = 100;
            OffsetX = 400;
            OffsetY = 200;
            Life = 10;
            LifeVariance = 10;
            Color = Colors.Pink;

            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000 / 20);
            _timer.Tick += (s, e) =>
            {
                UpdateParticles();
                Invalidate();
            };
        }

        public static readonly BindableProperty OnNewParticleCommandProperty =
            BindableProperty.Create(nameof(OnNewParticleCommand), typeof(ICommand), typeof(ParticleControl));

        public ICommand OnNewParticleCommand
        {
            get => (ICommand)GetValue(OnNewParticleCommandProperty);
            set => SetValue(OnNewParticleCommandProperty, value);
        }

        public static readonly BindableProperty IsRunningProperty =
            BindableProperty.Create(nameof(IsRunning), typeof(bool), typeof(ParticleControl), false, propertyChanged: OnIsRunningChanged);

        private static void OnIsRunningChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var ctl = (ParticleControl)bindable;
            if ((bool)newValue)
            {
                ctl._timer.Start();
            }
            else
            {
                ctl._timer.Stop();
            }
        }

        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        public bool DrawOutline { get; set; }

        public int Speed { get; set; }

        public int OriginVariance { get; set; }

        public int ParticleSize { get; set; }

        public int ParticleSizeVariance { get; set; }

        public int MaxParticleCount { get; set; }

        public int OffsetX { get; set; }

        public int OffsetY { get; set; }

        public int Life { get; set; }

        public int LifeVariance { get; set; }

        public Color Color { get; set; }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (DrawOutline)
            {
                canvas.StrokeColor = Colors.Magenta;
                canvas.StrokeSize = 5;
                canvas.StrokeDashPattern = new[] { 2f, 2f };
                canvas.DrawRectangle(0, 0, dirtyRect.Width, dirtyRect.Height);
            }

            if (IsRunning)
            {
                foreach (var p in _particles)
                {
                    canvas.FillColor = p.Color;
                    canvas.FillEllipse((float)p.Position.X, (float)p.Position.Y, (float)p.Size, (float)p.Size);
                }
            }
        }

        private void UpdateParticles()
        {
            if (IsRunning)
            {
                //
                // Update exsting particles
                //
                _deadList.Clear();

                foreach (var p in _particles)
                {
                    // calculate the "life" of the particle
                    p.Life -= p.Decay * _elapsed;

                    if (p.Life <= 0.0)
                    {
                        _deadList.Add(p);
                    }
                    else
                    {
                        // update size
                        p.Size = p.StartSize * (p.Life / p.StartLife);
                        double scale = p.Size / p.StartSize;

                        // update position
                        p.Position.X = p.Position.X + (p.Velocity.X * _elapsed);
                        p.Position.Y = p.Position.Y + (p.Velocity.Y * _elapsed);
                    }
                }
            }

            // create new particles
            for (var i = 0; _particles.Count < MaxParticleCount; i++)
            {
                SpawnParticle();
            }

            foreach (var p in _deadList)
            {
                _particles.Remove(p);
            }
        }

        private static double RandomWithVariance(double midvalue, double variance)
        {
            var min = Math.Max(midvalue - (variance / 2), 0);
            var max = midvalue + (variance / 2);
            var value = min + ((max - min) * RandomData.GetDouble());
            return value;
        }

        private void SpawnParticle()
        {
            double x = RandomWithVariance(OffsetX, OriginVariance);
            double y = RandomWithVariance(OffsetY, OriginVariance);
            double z = 10 * (RandomData.GetDouble() * OriginVariance);
            double life = RandomWithVariance(this.Life, this.LifeVariance);
            double size = RandomWithVariance(this.ParticleSize, this.ParticleSizeVariance);

            var p = new Particle();
            p.Color = Color;
            p.Position = new Point(x, y);
            p.StartLife = life;
            p.Life = life;
            p.StartSize = size;
            p.Size = size;
            p.Decay = 1.0;

            var velocityMultiplier = (RandomData.GetDouble() + 0.25) * Speed;
            var vX = (1.0 - (RandomData.GetDouble() * 2.0)) * velocityMultiplier;
            var vY = (1.0 - (RandomData.GetDouble() * 2.0)) * velocityMultiplier;
            p.Velocity = new Point(vX, vY);

            var command = OnNewParticleCommand;
            if (command != null)
            {
                command.Execute(p);
            }

            _particles.Add(p);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.IO;
using System.Text.Json;


namespace WpfApp3
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private DispatcherTimer animationTimer;
        private double animationPhase = 0;

        private double amplitudeX = 100;
        private double amplitudeY = 100;

        private double frequencyX = 1;
        private double frequencyY = 1;

        private double phaseX = 0;
        private double phaseY = 0;

        private int pointsCount = 1000;

        private string errorMessage = "";

        private readonly LissajousModel model;

        private void SaveSettings()
        {
            var settings = new LissajousSettings
            {
                AmplitudeX2 = AmplitudeX,
                AmplitudeY2 = AmplitudeY,

                FrequencyX2 = FrequencyX,
                FrequencyY2 = FrequencyY,

                PhaseX2 = PhaseX,
                PhaseY2 = PhaseY,

                PointsCount2 = PointsCount
            };

            string json = JsonSerializer.Serialize(
                settings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

            File.WriteAllText("lissajous.json", json);

            ErrorMessage = "Параметри збережено!";
        }

        private void LoadSettings()
        {
            string fileName = "lissajous.json";

            if (!File.Exists(fileName))
            {
                ErrorMessage = "Файл параметрів не знайдено.";
                return;
            }

            string json = File.ReadAllText(fileName);

            var settings = JsonSerializer.Deserialize<LissajousSettings>(json);

            if (settings == null)
            {
                ErrorMessage = "Не вдалося завантажити параметри.";
                return;
            }

            AmplitudeX = settings.AmplitudeX2;
            AmplitudeY = settings.AmplitudeY2;

            FrequencyX = settings.FrequencyX2;
            FrequencyY = settings.FrequencyY2;

            PhaseX = settings.PhaseX2;
            PhaseY = settings.PhaseY2;

            PointsCount = settings.PointsCount2;

            BuildFigure();

            ErrorMessage = "Параметри завантажено!";
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            animationPhase += 2;

            PhaseX = animationPhase;
            PhaseY = animationPhase;

            BuildFigure();
        }

        public void StartAnimation()
        {
            animationTimer.Start();
        }

        public void StopAnimation()
        {
            animationTimer.Stop();
        }

        public PointCollection ScreenPoints { get; set; }

        public ICommand BuildCommand { get; }

        public ICommand StartAnimationCommand { get; }
        public ICommand StopAnimationCommand { get; }

        public ICommand SaveCommand { get; }
        public ICommand LoadCommand { get; }

        public MainViewModel()
        {
            model = new LissajousModel();

            ScreenPoints = new PointCollection();

            BuildCommand = new RelayCommand(
                _ => BuildFigure(),
                _ => Validate()
            );

            SaveCommand = new RelayCommand(
                _ => SaveSettings()
);

            LoadCommand = new RelayCommand(
                _ => LoadSettings()
            );

            StartAnimationCommand = new RelayCommand(
                _ => StartAnimation()
            );

            StopAnimationCommand = new RelayCommand(
                _ => StopAnimation()
            );

            animationTimer = new DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(20);
            animationTimer.Tick += AnimationTimer_Tick;

            BuildFigure();
        }

        public double AmplitudeX
        {
            get => amplitudeX;
            set
            {
                if (amplitudeX != value)
                {
                    amplitudeX = value;
                    OnPropertyChanged(nameof(AmplitudeX));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public double AmplitudeY
        {
            get => amplitudeY;
            set
            {
                if (amplitudeY != value)
                {
                    amplitudeY = value;
                    OnPropertyChanged(nameof(AmplitudeY));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public double FrequencyX
        {
            get => frequencyX;
            set
            {
                if (frequencyX != value)
                {
                    frequencyX = value;
                    OnPropertyChanged(nameof(FrequencyX));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public double FrequencyY
        {
            get => frequencyY;
            set
            {
                if (frequencyY != value)
                {
                    frequencyY = value;
                    OnPropertyChanged(nameof(FrequencyY));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public double PhaseX
        {
            get => phaseX;
            set
            {
                if (phaseX != value)
                {
                    phaseX = value;
                    OnPropertyChanged(nameof(PhaseX));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public double PhaseY
        {
            get => phaseY;
            set
            {
                if (phaseY != value)
                {
                    phaseY = value;
                    OnPropertyChanged(nameof(PhaseY));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public int PointsCount
        {
            get => pointsCount;
            set
            {
                if (pointsCount != value)
                {
                    pointsCount = value;
                    OnPropertyChanged(nameof(PointsCount));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        private bool Validate()
        {
            if (AmplitudeX <= 0)
            {
                ErrorMessage = "Амплітуда Ax повинна бути > 0.";
                return false;
            }

            if (AmplitudeY <= 0)
            {
                ErrorMessage = "Амплітуда Ay повинна бути > 0.";
                return false;
            }

            if (FrequencyX <= 0)
            {
                ErrorMessage = "Частота fx повинна бути > 0.";
                return false;
            }

            if (FrequencyY <= 0)
            {
                ErrorMessage = "Частота fy повинна бути > 0.";
                return false;
            }

            if (PointsCount < 20)
            {
                ErrorMessage = "Кількість точок повинна бути не менше 20.";
                return false;
            }

            ErrorMessage = "";
            return true;
        }

        private void BuildFigure()
        {
            if (!Validate())
                return;

            var newPoints = model.GeneratePoints(
                AmplitudeX,
                AmplitudeY,
                FrequencyX,
                FrequencyY,
                PhaseX,
                PhaseY,
                PointsCount
            );

            double canvasSize = 600;

            double centerX = canvasSize / 2;
            double centerY = canvasSize / 2;

            double maxAmplitude = Math.Max(AmplitudeX, AmplitudeY);

            double scale = (canvasSize * 0.45) / maxAmplitude;

            var screenPoints = new PointCollection();

            foreach (var point in newPoints)
            {
                double screenX = centerX + point.X * scale;
                double screenY = centerY - point.Y * scale;

                screenPoints.Add(new Point(screenX, screenY));
            }

            ScreenPoints = screenPoints;

            OnPropertyChanged(nameof(ScreenPoints));

            ErrorMessage = $"Побудовано точок: {ScreenPoints.Count}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }
    }
}

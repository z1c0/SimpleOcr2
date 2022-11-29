using Newport;
using Newport.Commands;
using Newport.ViewModels;
using DotNeuralNet;
using System.Diagnostics;
using Elastic.Apm;

namespace SimpleOcr2.ViewModels
{
	public class MainViewModel : ViewModelBase
	{
        private DotNeuralNet.Network _network;
        private List<BackPropagationTrainingRow> _trainingRows;
        private int _selectedNumber;
        private bool _isTraining;

        public MainViewModel()
        {
            InitNeuralNetwork();

            Text = "Simple OCR";
            Rows = Cols = 10;
            IsTraining = true;

            Cells = Enumerable.Range(0, Rows * Cols).Select(_ => new CellViewModel()).ToList();
            Numbers = Enumerable.Range(0, 10).ToList();

            ResetCommmand = new ActionCommand(_ => Reset());
            OkCommmand = new ActionCommand(_ =>
            {
                _network.Invalidate();
                if (IsTraining)
                {
                    var row = BackPropagationTrainingRow.Create(_network);
                    Cells.ForEach((i, c) => row.Inputs[i] = c.IsChecked ? 1 : 0);
                    for (var i = 0; i < 10; i++)
                    {
                        row.Outputs[i] = (SelectedNumber == i) ? 1.0 : 0.0;
                    }
                    _trainingRows.Add(row);
                }
                else
                {
                    // Feed input data to neural network.
                    Cells.ForEach((i, c) => _network.InputNodes[i++].Value = c.IsChecked ? 1 : 0);
                    // Find node with highest activation.
                    var node = _network.OutputNodes.First(n => n.Value == _network.OutputNodes.Max(nn => nn.Value));
                    SelectedNumber = _network.OutputNodes.IndexOf(node);
                }
                Reset();
            });
        }

        private async void InitNeuralNetwork()
        {
            _trainingRows = new List<BackPropagationTrainingRow>();
            _network = new(100, 60, 10);
            var weights = await IsolatedStorageHelper.Load<double[]>("weights.xml");
            if (weights != null)
            {
                _network.SetWeights(weights);
            }
        }

        public int Cols { get; set; }

        public int Rows { get; set; }

        public bool IsTraining
        {
            get
            {
                return _isTraining;
            }
            set
            {
                if (!value)
                {
                    Train();
                }
                SetProperty(ref _isTraining, value);
            }
        }

        private async void Train()
        {
            if (_trainingRows.Count > 0)
            {
                await Agent.Tracer.CaptureTransaction("Training", "Custom", async t =>
                {
                    using (BusyScope())
                    {
                        t.SetLabel("nr_of_training_rows", _trainingRows.Count);
                        await Task.Factory.StartNew(async () =>
                        {
                            var trainer = new BackPropagationTrainer(_network);
                            trainer.Train(_trainingRows, 0.5, 50);
                            await IsolatedStorageHelper.Save("weights.xml", _network.GetWeights());
                        });
                    }
                });
            }
        }

        public IEnumerable<CellViewModel> Cells { get; private set; }

        public ActionCommand ResetCommmand { get; private set; }

        public ActionCommand OkCommmand { get; private set; }

        public List<int> Numbers { get; private set; }

        public int SelectedNumber
        {
            get
            {
                return _selectedNumber;
            }
            set
            {
                SetProperty(ref _selectedNumber, value);
            }
        }

        private void Reset()
        {
            Cells.ForEach(c => c.IsChecked = false);
        }
    }
}


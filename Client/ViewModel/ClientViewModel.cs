using Client.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;

namespace Client
{
    class ClientViewModel : BaseViewModel
    {

        public bool _isActionPerforming = false;
        public bool IsActionPerforming
        {
            get
            {
                return _isActionPerforming;
            }

            set
            {
                _isActionPerforming = value;
                OnPropertiesChanged(nameof(IsActionPerforming));
            }
        }



        #region InputFile

        private string _inputFile;
        public string InputFile
        {
            get
            {

                return _inputFile;
            }

            set
            {
                _inputFile = value;
                OnPropertyChanged(nameof(InputFile));
            }
        }

        public RelayCommand _inputFileCommand;
        public ICommand Input
        {
            get
            {
                if (_inputFileCommand == null)
                    _inputFileCommand = new RelayCommand(ExecuteInputCommand, CanExecuteInputCommand);
                return _inputFileCommand;
            }
        }

        public void ExecuteInputCommand(object parameter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.ShowDialog();
            InputFile = dialog.FileName;
            OnPropertyChanged(nameof(InputFile));

        }

        public bool CanExecuteInputCommand(object parameter)
        {
            return !IsActionPerforming;
        }

        #endregion



        private string _key;
        public string Key
        {
            get
            {
                return _key;
            }

            set
            {
                _key = value;
                OnPropertiesChanged("Key");
            }
        }



        #region Utils

        public static List<byte[]> SplitFile(string fileName, int batchSize)
        {

            List<byte[]> result = new List<byte[]>();
            byte[] batch = new byte[batchSize];
            using (var stream = File.OpenRead(fileName))
            {
                int count = 0;
                while ((count = stream.Read(batch)) != 0)
                {
                    result.Add(batch);
                    batch = new byte[batchSize];
                }
            }
            return result;

        }

        #endregion
    }
}


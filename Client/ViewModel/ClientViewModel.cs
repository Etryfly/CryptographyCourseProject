using Client.Command;
using Microsoft.Win32;
using Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
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


        #region Encrypt
        private RelayCommand _encryptCommand;
        public ICommand Encrypt
        {
            get
            {
                if (_encryptCommand == null)
                    _encryptCommand = new RelayCommand(ExecuteEncryptCommand, CanExecutEncryptCommand);
                return _encryptCommand;
            }
        }



        public async void ExecuteEncryptCommand(object parameter)
        {

            TcpClient client = new TcpClient("localhost", 8888);
            using (NetworkStream stream = client.GetStream())
            {
               
                    List<byte> result = new List<byte>();
                    BinaryFormatter formatter = new BinaryFormatter();
                    while (true)
                    {

                        Package package = (Package)formatter.Deserialize(stream);
                    if (package.message == Message.END) break;
                    result.AddRange(package.data);
                   
                }

                    BigInteger e = new BigInteger(result.ToArray());
                    MessageBox.Show(e.ToString());
                
            }
            /* try
             {
                 IsActionPerforming = true;



                 string tmpPath = InputFile;

                 foreach (CryptoItemModel item in Algorithms)
                 {



                     try
                     {
                         IEnumerable<byte[]> result = Encryption(tmpPath, item.Key, item.Algorithm, cancelTokenSource);
                         if (tmpPath == InputFile)
                         {

                             tmpPath = Path.GetTempFileName();
                         }
                         try
                         {
                             using (var outputStream = File.OpenWrite(tmpPath))
                             {
                                 Task thread = new Task(new Action(() =>
                                 {
                                     foreach (var line in result)
                                     {
                                         outputStream.Write(line);
                                     }
                                 }));
                                 thread.Start();
                                 await thread;

                             }
                         }
                         catch (AggregateException agex)
                         {
                             agex.Handle(ex =>
                             {
                                 MessageBox.Show(ex.Message);
                                 return true;
                             });
                         }
                     }
                     catch (KeyLengthException e)
                     {
                         MessageBox.Show("Key length error, expected " + e.ExpectedLength);
                         return;
                     }



                 }

                 using (var input = File.OpenRead(tmpPath))
                 {
                     using (var output = File.OpenWrite(OutputFile))
                     {
                         byte[] buffer = new byte[1024];
                         int count = 0;
                         while ((count = input.Read(buffer)) != 0)
                         {

                             output.Write(buffer, 0, count);

                         }
                     }
                 }

                 File.Delete(tmpPath);




                 MessageBox.Show("Encrypted");
             }
             catch (OperationCanceledException e)
             {
                 MessageBox.Show("Canceled");
             }
             IsActionPerforming = false;
             */
        }


        public bool CanExecutEncryptCommand(object parameter)
        {
            return true;
            if (IsActionPerforming) return false;
            if (!String.IsNullOrEmpty(InputFile)) return true;
            return false;
        }

        public void Encryption(string fileName, string password)
        {







            //return encrypted;

        }

        #endregion


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


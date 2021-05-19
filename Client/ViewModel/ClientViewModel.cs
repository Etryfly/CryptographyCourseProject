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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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


       

        #region Mode
        private Ciphers.FileEncrypter.MODE _mode = Ciphers.FileEncrypter.MODE.ECB;
        public Ciphers.FileEncrypter.MODE SelectedMode
        {
            get
            {

                return _mode;
            }

            set
            {
                _mode = value;
                OnPropertyChanged(nameof(SelectedMode));
            }
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
            IsActionPerforming = true;
            Task task = new Task(new Action(() =>
            {
                
                TcpClient client = new TcpClient("localhost", 8888);
                using (NetworkStream stream = client.GetStream())
                {


                    byte[] eBytes = NetworkStreamUtils.ReadBytesFromStream(stream);
                    BigInteger e = new BigInteger(eBytes);

                    byte[] nBytes = NetworkStreamUtils.ReadBytesFromStream(stream);
                    BigInteger n = new BigInteger(nBytes);

                    BigInteger m = new BigInteger(Encoding.ASCII.GetBytes(Key));

                    BigInteger c = CryptographyCourseProject.RSA.Encrypt(m, e, n);

                    byte[] IV = new byte[16];
                    Random random = new Random();
                    random.NextBytes(IV);
                    NetworkStreamUtils.WriteDataIntoStream(Message.KEY, c.ToByteArray(), stream);
                    NetworkStreamUtils.WriteDataIntoStream(Message.IV, IV, stream);
                    string output = Path.GetTempFileName();
                    Ciphers.FileEncrypter.Encrypt(InputFile, output, Encoding.ASCII.GetBytes(Key), SelectedMode, IV);
                    MessageBox.Show("Encrypted");
                    NetworkStreamUtils.WriteDataIntoStream(Message.FILE, Encoding.ASCII.GetBytes(Path.GetFileName(InputFile)), stream);
                    NetworkStreamUtils.WriteDataIntoStream(Message.FILE, Encoding.ASCII.GetBytes(SelectedMode.ToString()), stream);

                   

                    NetworkStreamUtils.WriteFileIntoStream(output, stream);
                    if (NetworkStreamUtils.RecieveDecryptedPackage(stream))
                    {
                        MessageBox.Show("Decrypted");
                    } else
                    {
                        MessageBox.Show("Error");
                    }
                   
                }
            }));
             task.Start();
            await task;

            IsActionPerforming = false;
        }


        public bool CanExecutEncryptCommand(object parameter)
        {
            
            if (IsActionPerforming) return false;
            if (!String.IsNullOrEmpty(InputFile)) return true;
            return false;
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


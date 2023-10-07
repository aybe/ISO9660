using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CDROMTools;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using TinyTree;

namespace CDROMToolsDemo.Classes
{
    public sealed class Model : ViewModelBase, IDisposable
    {
        public Model()
        {
            _progressHandler = new Progress<double>(ProgressHandler);
        }

        #region CD-ROM related

        private CDROM _cdrom;
        private DriveInfo _drive;
        private DriveInfo[] _drives;

        public CDROM CDROM
        {
            get { return _cdrom; }
            private set
            {
                Set(ref _cdrom, value);
                RaisePropertyChanged(() => Tracks);

                if (CDROM?.Files != null)
                {
                     RaisePropertyChanged(()=> Files);
                }
            }
        } 

        public DriveInfo Drive
        {
            get { return _drive; }
            set
            {
                Set(ref _drive, value);
                UpdateDrive();
            }
        }

        public DriveInfo[] Drives => _drives ?? (_drives = CDROM.GetCDROMDrives());

        public CDROMTrack[] Tracks => CDROM?.Tracks;

        private void UpdateDrive()
        {
            DisposeCDROM();

            if (Drive == null) return;
            CDROM cdrom = null;
            string message = null;
            try
            {
                cdrom = new CDROM(Drive);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            if (cdrom == null)
            {
                SetLastMessage(message, MessageType.Error);
                return;
            }

            SetLastMessage("Device opened succesfully !");
            CDROM = cdrom;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            DisposeCDROM();
        }

        private void DisposeCDROM()
        {
            if (CDROM != null)
            {
                CDROM.Dispose();
                CDROM = null;
            }
        }

        #endregion

        #region Progress

        private readonly IProgress<double> _progressHandler;

        private double _progress;

        public double Progress
        {
            get { return _progress; }
            set { Set(ref _progress, value); }
        }

        private void ProgressHandler(double d)
        {
            Progress = d*100.0d;
        }

        #endregion

        #region Extract Track

        private RelayCommand<CDROMTrack> _extractTrack;

        public RelayCommand<CDROMTrack> ExtractTrack
            =>
                _extractTrack ??
                (_extractTrack = new RelayCommand<CDROMTrack>(ExtractTrackExecute, ExtractTrackCanExecute));

        public bool IsExtracting { get; private set; }

        private bool ExtractTrackCanExecute(CDROMTrack arg)
        {
            return !IsExtracting;
        }

        private async void ExtractTrackExecute(CDROMTrack track)
        {
            string fileName = null;
            string name = $"Track{track.Index:D2}.bin";
            using (var dialog = new CommonSaveFileDialog {DefaultFileName = name})
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    fileName = dialog.FileName;
            }

            if (fileName == null) return;

            await Task.Run(() =>
            {
                IsExtracting = true;
                SetLastMessage($"Extracting track {track.Index} to \"{fileName}\" ...");
                using (var stream = File.Create(fileName))
                {
                    CDROM.ReadTrackRaw(track, stream, _progressHandler);
                }
                SetLastMessage("Track extracted succesfully !");
                IsExtracting = false;
            });
        }

        #endregion

        public IEnumerable<DirectoryNode> Files
        {
            get
            {// todo move
                if (CDROM?.Files == null) return null;
                var files = CDROM.Files;
                var nodes = IsoTreeHelper.ToNodes(files);
                return nodes;
            }
        }

        #region Last Message

        private string _lastMessage;
        private MessageType _lastMessageType;

        public string LastMessage
        {
            get { return _lastMessage; }
            set { Set(ref _lastMessage, value); }
        }

        public MessageType LastMessageType
        {
            get { return _lastMessageType; }
            set { Set(ref _lastMessageType, value); }
        }

        private void SetLastMessage(string message, MessageType messageType = MessageType.Normal)
        {
            LastMessage = message;
            LastMessageType = messageType;
        }

        #endregion

       
    }
}
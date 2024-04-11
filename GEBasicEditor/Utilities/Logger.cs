using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Shapes;

namespace GEBasicEditor.Utilities
{
    public enum MessageType
    {
        Info = 0x01,
        Warning = 0x02,
        Error = 0x04,
    }

    public class LogMessage
    {
        public DateTime Time { get; }
        public MessageType MessageType { get; }
        public string MessageString { get; }
        public string FileName { get; }
        public string Caller { get; }
        public int Line { get; }
        public string MetaData => $"{FileName}: {Caller} ({Line})";

        public LogMessage(MessageType type, string msg, string file, string caller, int line)
        {
            Time = DateTime.Now;
            MessageType = type;
            MessageString = msg;
            FileName = file;
            Caller = caller;
            Line = line;
        }
    }

    public static class Logger
    {
        private static int _messageFilter = (int)(MessageType.Info | MessageType.Warning | MessageType.Error);
        private readonly static ObservableCollection<LogMessage> _messages = new ObservableCollection<LogMessage> ();
        public static ReadOnlyObservableCollection<LogMessage> Messages
        { get; } = new ReadOnlyObservableCollection<LogMessage>(_messages);
        public static CollectionViewSource FilteredMessages
        { get; } = new CollectionViewSource() { Source = Messages! };

        public static async void Log(MessageType type, string msg, [CallerFilePath]string file="", 
            [CallerMemberName]string caller="", [CallerLineNumber]int line = 0)
        {
            await Application.Current.Dispatcher.BeginInvoke(new Action(() => 
            {
                _messages.Add(new LogMessage(type, msg, file, caller, line));
            }));
        }

        public static async void Clear()
        {

            await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _messages.Clear();
            }));
        }

        public static void SetMessageFilter(int mask)
        {
            _messageFilter = mask;
            FilteredMessages.View.Refresh();
        }

        static Logger()
        {
            FilteredMessages.Filter += (s, e) =>
            {
                var type = (int)(e.Item as LogMessage)!.MessageType;
                e.Accepted = (type & _messageFilter) != 0;
            };
        }
    }
}

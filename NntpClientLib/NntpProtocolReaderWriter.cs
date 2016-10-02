using System;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace NntpClientLib
{
    internal class NntpProtocolReaderWriter : IDisposable
    {
        private TcpClient _connection;
        private NetworkStream _network;
        private NntpStreamReader _reader;
        private StreamWriter _writer;

        private readonly Encoding _enc = Rfc977NntpClient.DefaultEncoding;

        internal NntpProtocolReaderWriter(TcpClient connection)
        {
            _connection = connection;
            _network = _connection.GetStream();
            _writer = new StreamWriter(_network, DefaultTextEncoding);
            _writer.AutoFlush = true;
            _reader = new NntpStreamReader(_network);
        }

        public TextWriter LogWriter { get; set; }

        internal Encoding DefaultTextEncoding
        {
            get { return _enc; }
        }

        internal string LastResponse { get; private set; }

        internal int LastResponseCode
        {
            get
            {
                if (string.IsNullOrEmpty(LastResponse))
                {
                    throw new InvalidOperationException(Resource.ErrorMessage41);
                }
                if (LastResponse.Length > 2)
                {
                    return Convert.ToInt32(LastResponse.Substring(0, 3), CultureInfo.InvariantCulture);
                }
                throw new InvalidOperationException(Resource.ErrorMessage42);
            }
        }

        internal string LastCommand { get; private set; }

        internal string ReadLine()
        {
            var s = _reader.ReadLine();
            if (LogWriter != null)
            {
                LogWriter.Write(">> ");
                LogWriter.WriteLine(s);
            }
            return s;
        }

        internal string ReadResponse()
        {
            LastResponse = _reader.ReadLine();
            if (LogWriter != null)
            {
                LogWriter.WriteLine("< " + LastResponse);
            }
            return LastResponse;
        }

        internal void WriteCommand(string line)
        {
            if (LogWriter != null)
            {
                LogWriter.WriteLine("> " + line);
            }
            LastCommand = line;
            _writer.WriteLine(line);
        }

        internal void WriteLine(string line)
        {
            if (LogWriter != null)
            {
                LogWriter.WriteLine("> " + line);
            }
            _writer.WriteLine(line);
        }

        internal void Write(string line)
        {
            if (LogWriter != null)
            {
                LogWriter.Write("> " + line);
            }
            _writer.Write(line);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_connection == null)
            {
                return;
            }
            try
            {
                _writer.Close();
            }
            catch
            {
            }
            _writer = null;

            try
            {
                _reader.Close();
            }
            catch
            {
            }
            _reader = null;

            if (_connection != null)
            {
                try
                {
                    _connection.GetStream().Close();
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}
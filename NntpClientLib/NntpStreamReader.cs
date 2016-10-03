using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace NntpClientLib
{
    internal class NntpStreamReader : TextReader
    {
        private const int DefaultBufferSize = 1024;
        private const int DefaultFileBufferSize = 4096;
        private const int MinimumBufferSize = 512;
        private Stream _baseStream;
        private int _bufferSize;
        private int _currentDecodePosition;

        //
        // The input array
        //

        //
        // The decoded array from the above input array
        //
        private char[] _decodedBuffer;

        //
        // Decoded bytes in _decodedBuffer.
        //
        private int _decodedCount;

        //
        // Current position in the _decodedBuffer
        //
        private Decoder _decoder;
        private Encoding _encoding;
        private bool _foundCarriageReturn;
        private byte[] _inputBuffer;

        private StringBuilder _lineBuilder;
        private bool _mayBlock;

        public NntpStreamReader(Stream stream)
            : this(stream, Rfc977NntpClient.DefaultEncoding, DefaultBufferSize)
        {
        }

        public NntpStreamReader(Stream stream, Encoding encoding, int bufferSize)
        {
            Initialize(stream, encoding, bufferSize);
        }

        public virtual Stream BaseStream
        {
            get { return _baseStream; }
        }

        public virtual Encoding CurrentEncoding
        {
            get
            {
                if (_encoding == null)
                {
                    throw new InvalidOperationException();
                }
                return _encoding;
            }
        }

        public bool EndOfStream
        {
            get { return Peek() < 0; }
        }

        internal void Initialize(Stream stream, Encoding encoding, int bufferSize)
        {
            if (null == stream)
            {
                throw new ArgumentNullException("stream");
            }
            if (null == encoding)
            {
                throw new ArgumentNullException("encoding");
            }
            if (!stream.CanRead)
            {
                throw new ArgumentException(Resource.ErrorMessage44);
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentException(Resource.ErrorMessage43, "bufferSize");
            }

            if (bufferSize < MinimumBufferSize)
            {
                bufferSize = MinimumBufferSize;
            }

            _baseStream = stream;
            _inputBuffer = new byte[bufferSize];
            _bufferSize = bufferSize;
            _encoding = encoding;
            _decoder = encoding.GetDecoder();

            _decodedBuffer = new char[encoding.GetMaxCharCount(bufferSize)];
            _decodedCount = 0;
            _currentDecodePosition = 0;
        }

        public void Close()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && _baseStream != null)
                {
                    _baseStream.Dispose();
                }

                _inputBuffer = null;
                _decodedBuffer = null;
                _encoding = null;
                _decoder = null;
                _baseStream = null;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public void DiscardBufferedData()
        {
            _decoder = _encoding.GetDecoder();
            _currentDecodePosition = 0;
            _decodedCount = 0;
            _mayBlock = false;
        }

        private int ReadBuffer()
        {
            _currentDecodePosition = 0;

            _decodedCount = 0;
            var parseStart = 0;
            do
            {
                var cbEncoded = _baseStream.Read(_inputBuffer, 0, _bufferSize);

                if (cbEncoded <= 0)
                {
                    return 0;
                }

                _mayBlock = (cbEncoded < _bufferSize);

                _decodedCount += _decoder.GetChars(_inputBuffer, parseStart, cbEncoded, _decodedBuffer, 0);
                parseStart = 0;
            } while (_decodedCount == 0);

            return _decodedCount;
        }

        public override int Peek()
        {
            CheckObjectState();

            if (_currentDecodePosition >= _decodedCount && (_mayBlock || ReadBuffer() == 0))
            {
                return -1;
            }

            return _decodedBuffer[_currentDecodePosition];
        }

        public override int Read()
        {
            CheckObjectState();

            if (_currentDecodePosition >= _decodedCount && ReadBuffer() == 0)
            {
                return -1;
            }

            return _decodedBuffer[_currentDecodePosition++];
        }

        public override int Read([In, Out] char[] destinationBuffer, int index, int count)
        {
            CheckObjectState();

            if (destinationBuffer == null)
            {
                throw new ArgumentNullException("destinationBuffer");
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            if (index > (destinationBuffer.Length - count))
            {
                throw new ArgumentOutOfRangeException("index");
            }

            var charsRead = 0;
            while (count > 0)
            {
                if (_currentDecodePosition >= _decodedCount && ReadBuffer() == 0)
                {
                    return charsRead > 0 ? charsRead : 0;
                }

                var cch = Math.Min(_decodedCount - _currentDecodePosition, count);
                Array.Copy(_decodedBuffer, _currentDecodePosition, destinationBuffer, index, cch);
                _currentDecodePosition += cch;
                index += cch;
                count -= cch;
                charsRead += cch;
            }
            return charsRead;
        }

        private int FindNextEndOfLine()
        {
            var c = '\0';
            for (; _currentDecodePosition < _decodedCount; _currentDecodePosition++)
            {
                c = _decodedBuffer[_currentDecodePosition];
                if (c == '\n' && _foundCarriageReturn)
                {
                    _currentDecodePosition++;
                    var res = (_currentDecodePosition - 2);
                    if (res < 0)
                    {
                        res = 0; // if a new array starts with a \n and there was a \r at the end of the previous one, we get here.
                    }
                    _foundCarriageReturn = false;
                    return res;
                }

                _foundCarriageReturn = (c == '\r');
            }

            return -1;
        }

        public override string ReadLine()
        {
            CheckObjectState();

            if (_currentDecodePosition >= _decodedCount && ReadBuffer() == 0)
            {
                return null;
            }

            var begin = _currentDecodePosition;
            var end = FindNextEndOfLine();
            if (end < _decodedCount && end >= begin)
            {
                return new string(_decodedBuffer, begin, end - begin);
            }

            if (_lineBuilder == null)
            {
                _lineBuilder = new StringBuilder();
            }
            else
            {
                _lineBuilder.Length = 0;
            }

            while (true)
            {
                if (_foundCarriageReturn) // don't include the trailing CR if present
                {
                    _decodedCount--;
                }

                _lineBuilder.Append(_decodedBuffer, begin, _decodedCount - begin);
                if (ReadBuffer() == 0)
                {
                    if (_lineBuilder.Capacity > 32768)
                    {
                        var sb = _lineBuilder;
                        _lineBuilder = null;
                        return sb.ToString(0, sb.Length);
                    }
                    return _lineBuilder.ToString(0, _lineBuilder.Length);
                }

                begin = _currentDecodePosition;
                end = FindNextEndOfLine();
                if (end < _decodedCount && end >= begin)
                {
                    _lineBuilder.Append(_decodedBuffer, begin, end - begin);
                    if (_lineBuilder.Capacity > 32768)
                    {
                        var sb = _lineBuilder;
                        _lineBuilder = null;
                        return sb.ToString(0, sb.Length);
                    }
                    return _lineBuilder.ToString(0, _lineBuilder.Length);
                }
            }
        }

        public override string ReadToEnd()
        {
            CheckObjectState();

            var text = new StringBuilder();

            var size = _decodedBuffer.Length;
            var buffer = new char[size];
            int len;

            while ((len = Read(buffer, 0, size)) > 0)
            {
                text.Append(buffer, 0, len);
            }

            return text.ToString();
        }

        private void CheckObjectState()
        {
            if (_baseStream == null)
            {
                throw new InvalidOperationException(Resource.ErrorMessage45);
            }
        }
    }
}
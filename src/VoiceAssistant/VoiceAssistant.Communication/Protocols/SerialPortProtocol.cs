using System.IO.Ports;

namespace VoiceAssistant.Communication.Protocols
{
    public class SerialPortProtocol : IDisposable
    {
        private readonly SerialPort _serialPort;
        private Thread _readThread;
        private bool _running;

        // 外部事件
        public Action OnConnected { get; set; }
        public Action OnDisconnected { get; set; }
        public Action<string> OnDataReceived { get; set; }
        public Action<Exception> OnError { get; set; }

        public SerialPortProtocol(string portName, int baudRate = 9600)
        {
            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500
            };

            try
            {
                _serialPort.Open();
                OnConnected?.Invoke();
                // 启动读取线程
                _running = true;
                _readThread = new Thread(ReadLoop) { IsBackground = true };
                _readThread.Start();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
            }
        }

        private void ReadLoop()
        {
            try
            {
                while (_running)
                {
                    try
                    {
                        string line = _serialPort.ReadLine(); // 按行读取
                        OnDataReceived?.Invoke(line);
                    }
                    catch (TimeoutException)
                    {
                        // 超时不处理，继续循环
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke(ex);
                    }
                }
            }
            finally
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.Close();
                    OnDisconnected?.Invoke();
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public void Send(string data)
        {
            if (_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.WriteLine(data);
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(ex);
                }
            }
        }

        /// <summary>
        /// 发送字节数组
        /// </summary>
        public void SendBytes(byte[] data)
        {
            if (_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Write(data, 0, data.Length);
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(ex);
                }
            }
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void Close()
        {
            _running = false;
            _readThread?.Join(500);
            if (_serialPort.IsOpen)
                _serialPort.Close();
            OnDisconnected?.Invoke();
        }

        public void Dispose()
        {
            Close();
            _serialPort?.Dispose();
        }
    }
}
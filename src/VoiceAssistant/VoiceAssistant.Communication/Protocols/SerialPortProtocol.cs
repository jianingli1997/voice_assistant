using System.IO.Ports;

namespace VoiceAssistant.Communication.Protocols
{
    public class SerialPortProtocol : IDisposable
    {
        private readonly SerialPort _serialPort;
        private Thread _readThread;
        private bool _running;

        // 外部事件
        public Action OnConnected;
        public Action OnDisconnected;
        public Action<string> OnDataReceived;
        public Action<Exception> OnError;
        public Action<byte[]> OnDataReceivedBytes;
        public SerialPortProtocol(string portName, int baudRate = 9600)
        {
            _serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One)
            {
                ReadTimeout = 500
            };
        }
        public void Connect()
        {
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

        // private void ReadLoop()
        // {
        //     try
        //     {
        //         while (_running)
        //         {
        //             try
        //             {
        //                 string line = _serialPort.ReadLine(); // 按行读取
        //                 OnDataReceived?.Invoke(line);
        //             }
        //             catch (TimeoutException)
        //             {
        //                 // 超时不处理，继续循环
        //             }
        //             catch (Exception ex)
        //             {
        //                 OnError?.Invoke(ex);
        //             }
        //         }
        //     }
        //     finally
        //     {
        //         if (_serialPort.IsOpen)
        //         {
        //             _serialPort.Close();
        //             OnDisconnected?.Invoke();
        //         }
        //     }
        // }
        private List<byte> _receiveBuffer = new List<byte>();

        private void ReadLoop()
        {
            try
            {
                byte[] readBuffer = new byte[1024]; // 每次最多读1KB

                while (_running)
                {
                    try
                    {
                        int bytesRead = _serialPort.Read(readBuffer, 0, readBuffer.Length);
                        if (bytesRead <= 0)
                            continue;

                        // 1. 先对原始字节做「反向转义」，还原协议原始字节流
                        byte[] unescaped = UnescapeData(readBuffer, bytesRead);

                        // 2. 加入接收缓冲区
                        lock (_receiveBuffer) // 线程安全（如果多线程访问）
                        {
                            _receiveBuffer.AddRange(unescaped);
                            ProcessReceiveBuffer();
                        }
                    }
                    catch (TimeoutException)
                    {
                        // 串口 Read 超时，正常现象，继续循环
                        continue;
                    }
                    catch (InvalidOperationException)
                    {
                        // 串口可能已关闭
                        break;
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke(ex);
                        // 可选：短暂休眠避免疯狂报错
                        System.Threading.Thread.Sleep(10);
                    }
                }
            }
            finally
            {
                if (_serialPort?.IsOpen == true)
                {
                    try
                    {
                        _serialPort.Close();
                    }
                    catch { /* ignore */ }
                }
                OnDisconnected?.Invoke();
            }
        }

        /// <summary>
        /// 对收到的字节流进行反向转义（还原 0xA5, 0xA6, 0x1B）
        /// 规则：遇到 0x1B，则跳过它并取下一个字节（即使它不是特殊字节）
        /// </summary>
        private byte[] UnescapeData(byte[] data, int length)
        {
            List<byte> result = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                if (data[i] == 0x1B)
                {
                    // 转义符：跳过 0x1B，取下一个字节（即使越界也尽量处理）
                    if (i + 1 < length)
                    {
                        result.Add(data[++i]);
                    }
                    // 如果 0x1B 是最后一个字节，按协议应丢弃或报错，这里保守丢弃
                }
                else
                {
                    result.Add(data[i]);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// 从接收缓冲区中提取并处理所有完整帧（0xA5 ... 0xA6）
        /// </summary>
        private void ProcessReceiveBuffer()
        {
            while (true)
            {
                // 查找帧头 0xA5
                int headIndex = _receiveBuffer.IndexOf(0xA5);
                if (headIndex == -1)
                {
                    // 没有帧头，清空无效前缀（可选优化）
                    _receiveBuffer.Clear();
                    break;
                }

                // 如果帧头不在开头，说明前面是脏数据，丢弃
                if (headIndex > 0)
                {
                    _receiveBuffer.RemoveRange(0, headIndex);
                }

                // 此时 _receiveBuffer[0] == 0xA5
                if (_receiveBuffer.Count < 2)
                {
                    // 至少要有头+尾，但数据不够，等下一轮
                    break;
                }

                // 从位置 1 开始找帧尾 0xA6（因为头在0）
                int tailIndex = _receiveBuffer.IndexOf(0xA6, 1);
                if (tailIndex == -1)
                {
                    // 帧尾还没收到，退出等待更多数据
                    break;
                }

                // 提取完整帧 [0 ... tailIndex]（包含 0xA5 和 0xA6）
                byte[] frame = _receiveBuffer.Take(tailIndex + 1).ToArray();

                // 移除已处理的帧
                _receiveBuffer.RemoveRange(0, tailIndex + 1);

                // 校验最小长度（至少：头+用户ID+类型+长度2+ID2+校验+尾 = 9字节，你协议要求>=10）
                if (frame.Length >= 10)
                {
                    // 这里传递原始 byte[] 给上层，不要转 string！
                    // 修改你的 VoiceModuleService.HandleReceivedData 为接收 byte[]
                    OnDataReceivedBytes?.Invoke(frame);
                }
                // 如果帧太短，直接丢弃（不处理）
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
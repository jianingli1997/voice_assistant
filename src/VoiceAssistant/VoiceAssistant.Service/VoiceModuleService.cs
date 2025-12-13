// /*
//  * Class: VoiceModuleService.cs
//  * Author: lijianing
//  * Date: 2025-12-13
//  * Description: 
//  * Version: 1.0
//  */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceAssistant.Communication.Protocols;

namespace VoiceAssistant.Service
{
    public class VoiceModuleService : IDisposable
    {
        private readonly SerialPortProtocol _protocol;
        private readonly byte _userId = 0x01;
        private ushort _messageIdCounter = 0;

        public Action OnHandshakeSuccess { get; set; }
        public Action<byte> OnHeartbeatReceived { get; set; }
        public Action<string> OnError { get; set; }

        public VoiceModuleService(string portName, int baudRate = 115200)
        {
            _protocol = new SerialPortProtocol(portName, baudRate);
            _protocol.OnConnected = () => Console.WriteLine("串口已连接");
            _protocol.OnDisconnected = () => Console.WriteLine("串口已断开");
            _protocol.OnError = ex => OnError?.Invoke(ex.Message);
            _protocol.OnDataReceived = HandleReceivedData;
        }

        private void HandleReceivedData(string data)
        {
            // data 是串口收到的原始字符串
            // 实际上我们需要按字节处理
            byte[] bytes = Encoding.Default.GetBytes(data);

            if (bytes.Length < 10) return; // 长度不足，丢弃

            // 检查头尾
            if (bytes[0] != 0xA5 || bytes[^1] != 0xA6) return;

            byte msgType = bytes[2];
            ushort messageId = BitConverter.ToUInt16(bytes, 5); // 小端模式
            ushort dataLen = BitConverter.ToUInt16(bytes, 3);

            switch (msgType)
            {
                case 0x02: // 心跳消息
                    if (dataLen >= 1)
                        OnHeartbeatReceived?.Invoke(bytes[7]);
                    break;
                case 0xFF: // 确认消息
                    Console.WriteLine("收到确认消息，消息ID=" + messageId);
                    break;
                default:
                    Console.WriteLine("收到未知消息类型: " + msgType);
                    break;
            }
        }

        /// <summary>
        /// 发送握手请求
        /// </summary>
        public void SendHandshake(byte key)
        {
            byte[] msgData = new byte[4]; // 4字节消息数据
            msgData[0] = key;             // key
            msgData[1] = 0x00;            // 保留
            msgData[2] = 0x00;
            msgData[3] = 0x00;

            SendMessage(0x01, msgData);
        }

        /// <summary>
        /// 发送确认消息
        /// </summary>
        public void SendConfirm(ushort messageId)
        {
            byte[] msgData = new byte[4]; // 示例
            msgData[0] = 0x00;
            msgData[1] = 0x00;
            msgData[2] = 0x00;
            msgData[3] = 0x00;

            SendMessage(0xFF, msgData, messageId);
        }

        private void SendMessage(byte msgType, byte[] msgData, ushort? messageId = null)
        {
            ushort dataLen = (ushort)msgData.Length;
            ushort msgId = messageId ?? _messageIdCounter++;

            List<byte> buffer = new List<byte>();
            buffer.Add(0xA5);         // 消息头
            buffer.Add(_userId);      // 用户ID
            buffer.Add(msgType);       // 消息类型
            buffer.AddRange(BitConverter.GetBytes(dataLen)); // 数据长度，小端
            buffer.AddRange(BitConverter.GetBytes(msgId));   // 消息ID，小端
            buffer.AddRange(msgData);  // 消息数据

            // 计算校验码
            byte checkCode = CalcCheckCode(buffer.ToArray());
            buffer.Add(checkCode);

            buffer.Add(0xA6);         // 消息尾

            // 处理转义符
            buffer = EscapeBuffer(buffer);

            _protocol.SendBytes(buffer.ToArray());
        }

        private byte CalcCheckCode(byte[] data)
        {
            int sum = 0;
            for (int i = 0; i < data.Length; i++)
            {
                if (i == data.Length - 1) break; // 校验码位置不参与
                if (data[i] != 0x1B) sum += data[i];
            }
            byte check = (byte)((~sum + 1) & 0xFF);
            return check;
        }

        private List<byte> EscapeBuffer(List<byte> buffer)
        {
            List<byte> escaped = new List<byte>();
            escaped.Add(buffer[0]); // 头不转义

            for (int i = 1; i < buffer.Count; i++)
            {
                byte b = buffer[i];
                if (b == 0xA5 || b == 0xA6 || b == 0x1B)
                {
                    escaped.Add(0x1B);
                }
                escaped.Add(b);
            }

            return escaped;
        }

        public void Dispose()
        {
            _protocol?.Dispose();
        }
    }
}

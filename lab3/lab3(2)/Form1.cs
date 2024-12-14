using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        private TcpListener tcpListener;
        private List<TcpClient> connectedClients = new List<TcpClient>();
        private NetworkStream networkStream;
        private const int defaultPort = 8080;

        public Form1()
        {
            InitializeComponent();
        }

        private void StartServerButton_Click(object sender, EventArgs e)
        {
            try
            {
                string serverIp = serverIpTextBox.Text;
                int port = int.TryParse(portTextBox.Text, out var parsedPort) ? parsedPort : defaultPort;

                if (string.IsNullOrWhiteSpace(serverIp) || !IPAddress.TryParse(serverIp, out IPAddress ipAddress))
                {
                    MessageBox.Show("Неправильный Айпи.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                tcpListener = new TcpListener(ipAddress, port);
                tcpListener.Start();
                this.Text = $"Сервер - {ipAddress}:{port}"; // Обновляем заголовок формы
                AppendLog($"Сервер запустился на {ipAddress}:{port}");

                Thread listenerThread = new Thread(() =>
                {
                    while (true)
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();
                        lock (connectedClients)
                        {
                            connectedClients.Add(client);
                        }
                        AppendLog($"Клиент подключился: {client.Client.RemoteEndPoint}");
                        Thread clientThread = new Thread(() => HandleClient(client));
                        clientThread.IsBackground = true;
                        clientThread.Start();
                    }
                });
                listenerThread.IsBackground = true;
                listenerThread.Start();
            }
            catch (Exception ex)
            {
                AppendLog($"Ошибка запуска сервера: {ex.Message}");
            }
        }

        private void ConnectClientButton_Click(object sender, EventArgs e)
        {
            try
            {
                string serverIp = serverIpTextBox.Text;
                int port = int.TryParse(portTextBox.Text, out var parsedPort) ? parsedPort : defaultPort;

                if (string.IsNullOrWhiteSpace(serverIp) || !IPAddress.TryParse(serverIp, out _))
                {
                    MessageBox.Show("Неправильный Айпи.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                TcpClient tcpClient = new TcpClient(serverIp, port);
                networkStream = tcpClient.GetStream();
                this.Text = $"Клиент подключен к {serverIp}:{port}"; // Обновляем заголовок формы
                AppendLog($"Подключился к серверу {serverIp}:{port}");

                Thread clientThread = new Thread(() =>
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    while (true)
                    {
                        try
                        {
                            bytesRead = networkStream.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0) break;

                            byte[] receivedBytes = new byte[bytesRead];
                            Array.Copy(buffer, receivedBytes, bytesRead);

                            string message = KOI8RCodec.Decode(receivedBytes);

                            // Извлекаем IP-адрес сервера
                            IPEndPoint remoteEndPoint = tcpClient.Client.RemoteEndPoint as IPEndPoint;
                            string remoteAddress = remoteEndPoint?.ToString() ?? "Неизвестный адрес";

                            AppendLog($"Получил от {remoteAddress}: {message}");

                            string binaryString = ByteConverter.ToBinaryString(receivedBytes);
                            string hexString = ByteConverter.ToHexString(receivedBytes);
                            AppendLog($"Кодировка: {hexString}");
                            AppendLog($"Двоичный вид: {binaryString}");
                        }
                        catch (Exception ex)
                        {
                            AppendLog($"Error receiving data: {ex.Message}");
                            break;
                        }
                    }
                });
                clientThread.IsBackground = true;
                clientThread.Start();
            }
            catch (Exception ex)
            {
                AppendLog($"Ошибка подключения к серверу: {ex.Message}");
            }
        }


        private void SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                string message = inputTextBox.Text;
                if (string.IsNullOrEmpty(message))
                {
                    MessageBox.Show("Сообщение не может быть пустым.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (networkStream == null)
                {
                    MessageBox.Show("Не подключено ни к какому серверу.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

              
                byte[] encodedBytes = KOI8RCodec.Encode(message);

               
                networkStream.Write(encodedBytes, 0, encodedBytes.Length);
                networkStream.Flush();

              
                string binaryString = ByteConverter.ToBinaryString(encodedBytes);
             
                string hexString = ByteConverter.ToHexString(encodedBytes);

            
                AppendLog($"Вы: {message}");
                AppendLog($"Кодировка: {hexString}");
                AppendLog($"Двоичная форма: {binaryString}");

                inputTextBox.Clear();
            }
            catch (Exception ex)
            {
                AppendLog($"Ошибка отправления сообщения: {ex.Message}");
            }
        }

       
        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while (true)
            {
                try
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; 

             
                    byte[] receivedBytes = new byte[bytesRead];
                    Array.Copy(buffer, receivedBytes, bytesRead);

                  
                    string message = KOI8RCodec.Decode(receivedBytes);

                    AppendLog($"Получил от {client.Client.RemoteEndPoint}: {message}");

                 
                    string binaryString = ByteConverter.ToBinaryString(receivedBytes);
                    string hexString = ByteConverter.ToHexString(receivedBytes);
                    AppendLog($"Кодировка: {hexString}");
                    AppendLog($"Двоичная форма: {binaryString}");

                    BroadcastMessage(message, client);
                }
                catch (Exception ex)
                {
                    AppendLog($"Клиент отключился: {ex.Message}");
                    break;
                }
            }

            lock (connectedClients)
            {
                connectedClients.Remove(client);
            }
            client.Close();
        }

  
        private void BroadcastMessage(string message, TcpClient sender)
        {
            byte[] encodedBytes = KOI8RCodec.Encode(message);
            lock (connectedClients)
            {
                foreach (TcpClient client in connectedClients)
                {
                    if (client != sender)
                    {
                        try
                        {
                            NetworkStream clientStream = client.GetStream();
                            clientStream.Write(encodedBytes, 0, encodedBytes.Length);
                            clientStream.Flush();
                        }
                        catch
                        {
                           
                        }
                    }
                }
            }
        }

    
        private void AppendLog(string message)
        {
            this.Invoke((MethodInvoker)delegate
            {
                logTextBox.AppendText($"{DateTime.Now}: {message}\r\n");
            });
        }
    }

    public static class KOI8RCodec
    {
     
        private static readonly char[] koi8rTable = new char[]
        {
            '\u2500', '\u2502', '\u250C', '\u2510', '\u2514', '\u2518', '\u251C', '\u2524',
            '\u252C', '\u2534', '\u253C', '\u2580', '\u2584', '\u2588', '\u258C', '\u2590',
            '\u2591', '\u2592', '\u2593', '\u2320', '\u25A0', '\u2219', '\u221A', '\u2248',
            '\u2264', '\u2265', '\u00A0', '\u2321', '\u00B0', '\u00B2', '\u00B7', '\u00F7',
            '\u2550', '\u2551', '\u2552', '\u0451', '\u2553', '\u2554', '\u2555', '\u2556',
            '\u2557', '\u2558', '\u2559', '\u255A', '\u255B', '\u255C', '\u255D', '\u255E',
            '\u255F', '\u2560', '\u2561', '\u0401', '\u2562', '\u2563', '\u2564', '\u2565',
            '\u2566', '\u2567', '\u2568', '\u2569', '\u256A', '\u256B', '\u256C', '\u00A9',
            '\u044E', '\u0430', '\u0431', '\u0446', '\u0434', '\u0435', '\u0444', '\u0433',
            '\u0445', '\u0438', '\u0439', '\u043A', '\u043B', '\u043C', '\u043D', '\u043E',
            '\u043F', '\u044F', '\u0440', '\u0441', '\u0442', '\u0443', '\u0436', '\u0432',
            '\u044C', '\u044B', '\u0437', '\u0448', '\u044D', '\u0449', '\u0447', '\u044A',
            '\u042E', '\u0410', '\u0411', '\u0426', '\u0414', '\u0415', '\u0424', '\u0413',
            '\u0425', '\u0418', '\u0419', '\u041A', '\u041B', '\u041C', '\u041D', '\u041E',
            '\u041F', '\u042F', '\u0420', '\u0421', '\u0422', '\u0423', '\u0416', '\u0412',
            '\u042C', '\u042B', '\u0417', '\u0428', '\u042D', '\u0429', '\u0427', '\u042A'
        };

      
        public static byte[] Encode(string input)
        {
            byte[] bytes = new byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch < 128)
                {
                    bytes[i] = (byte)ch; 
                }
                else
                {
                 
                    int index = Array.IndexOf(koi8rTable, ch);
                    if (index != -1)
                    {
                        bytes[i] = (byte)(index + 0x80); 
                    }
                    else
                    {
                        bytes[i] = (byte)'?'; 
                    }
                }
            }
            return bytes;
        }

      
        public static string Decode(byte[] byteArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteArray)
            {
                if (b < 128)
                {
                    sb.Append((char)b); 
                }
                else
                {
                
                    sb.Append(koi8rTable[b - 0x80]); 
                }
            }
            return sb.ToString();
        }
    }

    public static class ByteConverter
    {
       
        public static string ToBinaryString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0')).Append(" ");
            }
            return sb.ToString().Trim();
        }

      
        public static string ToHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2")).Append(" ");
            }
            return sb.ToString().Trim();
        }
    }
}





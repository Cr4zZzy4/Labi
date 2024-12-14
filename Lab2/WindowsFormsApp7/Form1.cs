using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp5
{
    public partial class Form1 : Form
    {
        private TextBox inputTextBox;
        private Button sendButton;
        private TextBox dialogTextBox;
        private TextBox encodedTextBox;
        private TextBox binaryTextBox;
        private UDPChat udpChat;
        private string chatFilePath = "chat.txt"; // Путь к файлу для записи переписки

        public Form1()
        {
            InitializeComponent();
            udpChat = new UDPChat(localPort: 5001, remotePort: 5000);
            Task.Run(() => ListenForMessages());
            CreateChatFile(); // Создаем файл для переписки при старте приложения
            LoadChatHistory(); // Загружаем историю чата в текстовое поле
        }

        private void InitializeComponent()
        {
            inputTextBox = new TextBox() { Location = new System.Drawing.Point(20, 20), Width = 300 };
            sendButton = new Button() { Text = "Отправить", Location = new System.Drawing.Point(330, 20), Width = 100 };
            sendButton.Click += sendButton_Click;

            dialogTextBox = new TextBox() { Location = new System.Drawing.Point(20, 60), Width = 410, Height = 150, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical };
            encodedTextBox = new TextBox() { Location = new System.Drawing.Point(20, 220), Width = 410, ReadOnly = true };
            binaryTextBox = new TextBox() { Location = new System.Drawing.Point(20, 260), Width = 410, ReadOnly = true };

            Controls.Add(inputTextBox);
            Controls.Add(sendButton);
            Controls.Add(dialogTextBox);
            Controls.Add(encodedTextBox);
            Controls.Add(binaryTextBox);

            this.Text = "UDP Chat";
            this.Size = new System.Drawing.Size(460, 350);
        }

        private void CreateChatFile()
        {
            if (!File.Exists(chatFilePath))
            {
                using (var fs = File.Create(chatFilePath))
                {
                    // Создаем файл, ничего не записывая
                }
            }
        }

        private void LoadChatHistory()
        {
            if (File.Exists(chatFilePath))
            {
                dialogTextBox.Text = File.ReadAllText(chatFilePath); // Загружаем текст из файла в диалоговое поле
            }
        }

        // Обработчик для кнопки отправки
        private async void sendButton_Click(object sender, EventArgs e)
        {
            string message = inputTextBox.Text; // Берем текст из поля ввода
            byte[] encodedMessage = KOI8RCodec.Encode(message); // Кодируем текст в KOI8-R

            await udpChat.SendMessage(encodedMessage, "127.0.0.1"); // Асинхронно отправляем сообщение на локальный хост

            string byteRepresentation = BitConverter.ToString(encodedMessage); // Получаем байтовое представление
            string binary = string.Join(" ", Array.ConvertAll(encodedMessage, b => Convert.ToString(b, 2).PadLeft(8, '0'))); // Преобразование в двоичный формат

            AppendToChatFile(message, byteRepresentation, binary, udpChat.LocalPort); // Записываем сообщение в файл
            DisplayMessage(message, encodedMessage, udpChat.LocalPort); // Отображаем сообщение
        }


        // Метод для прослушивания входящих UDP сообщений
        private async Task ListenForMessages()
        {
            while (true)
            {
                UdpReceiveResult receivedMessage = await udpChat.ReceiveMessage(); // Асинхронно получаем сообщение
                string decodedMessage = KOI8RCodec.Decode(receivedMessage.Buffer); // Декодируем его

                // Получаем порт отправителя
                int senderPort = receivedMessage.RemoteEndPoint.Port;

                string byteRepresentation = BitConverter.ToString(receivedMessage.Buffer); // Получаем байтовое представление
                string binary = string.Join(" ", Array.ConvertAll(receivedMessage.Buffer, b => Convert.ToString(b, 2).PadLeft(8, '0'))); // Преобразование в двоичный формат

                AppendToChatFile(decodedMessage, byteRepresentation, binary, senderPort); // Записываем сообщение в файл
                DisplayMessage(decodedMessage, receivedMessage.Buffer, senderPort); // Отображаем
            }
        }

        // Метод для добавления сообщения в файл
        private void AppendToChatFile(string message, string byteRepresentation, string binary, int senderPort)
        {
            using (StreamWriter sw = File.AppendText(chatFilePath))
            {
                sw.WriteLine($"{DateTime.Now}: Отправлено с порта {senderPort}: {message}");
                sw.WriteLine($"Байтовое представление: {byteRepresentation}");
                sw.WriteLine($"Двоичный код: {binary}");
                sw.WriteLine(); // Пустая строка для разделения сообщений
            }
        }

        // Метод для отображения сообщения в разных форматах
        private void DisplayMessage(string original, byte[] encoded, int senderPort)
        {
            string binary = string.Join(" ", Array.ConvertAll(encoded, b => Convert.ToString(b, 2).PadLeft(8, '0'))); // Преобразование в двоичный формат
            string byteRepresentation = BitConverter.ToString(encoded); // Байтовое представление

            // Отображаем оригинальный текст с информацией о порте
            dialogTextBox.AppendText($"Отправлено с порта {senderPort}: {original}\r\n"); // Отображаем оригинальный текст с портом
            dialogTextBox.AppendText($"Байтовое представление: {byteRepresentation}\r\n"); // Отображаем байтовое представление
            dialogTextBox.AppendText($"Двоичный код: {binary}\r\n"); // Двоичный код
            encodedTextBox.Text = byteRepresentation; // Закодированный текст
            binaryTextBox.Text = binary; // Двоичный код
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                udpChat?.Dispose(); // Освобождаем ресурсы UDP-клиента
            }
            base.Dispose(disposing);
        }
    }

    // Класс для работы с UDP
    public class UDPChat
    {
        private UdpClient udpClient;
        private int remotePort;
        private IPEndPoint remoteEndPoint;

        public int LocalPort { get; } // Добавлено свойство для локального порта

        public UDPChat(int localPort, int remotePort)
        {
            udpClient = new UdpClient(localPort);
            this.remotePort = remotePort;
            LocalPort = localPort; // Сохраняем локальный порт
            remoteEndPoint = new IPEndPoint(IPAddress.Loopback, remotePort); // Локальный адрес (127.0.0.1)
        }

        // Отправка сообщения
        public async Task SendMessage(byte[] message, string ipAddress)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), remotePort);
            await udpClient.SendAsync(message, message.Length, endPoint); // Асинхронно отправляем UDP сообщение
        }

        // Прием сообщения
        public async Task<UdpReceiveResult> ReceiveMessage()
        {
            return await udpClient.ReceiveAsync(); // Ожидание получения данных
        }

        public void Dispose()
        {
            udpClient?.Close(); // Закрываем UDP-клиент
        }
    }

    // Класс для кодировки и декодировки текста в KOI8-R
    public static class KOI8RCodec
    {
        // Таблица кодировки KOI8-R
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

        // Кодирование строки в KOI8-R
        public static byte[] Encode(string input)
        {
            byte[] bytes = new byte[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if (ch < 128)
                {
                    bytes[i] = (byte)ch; // ASCII символы остаются теми же
                }
                else
                {
                    // Поиск символа в таблице KOI8-R
                    int index = Array.IndexOf(koi8rTable, ch);
                    if (index != -1)
                    {
                        bytes[i] = (byte)(index + 0x80); // +0x80 для диапазона KOI8-R
                    }
                    else
                    {
                        bytes[i] = (byte)'?'; // Неизвестные символы заменяем на "?"
                    }
                }
            }
            return bytes;
        }

        // Декодирование строки из KOI8-R
        public static string Decode(byte[] byteArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteArray)
            {
                if (b < 128)
                {
                    sb.Append((char)b); // ASCII символы
                }
                else
                {
                    // Восстановление символа из таблицы KOI8-R
                    sb.Append(koi8rTable[b - 0x80]); // -0x80 для диапазона KOI8-R
                }
            }
            return sb.ToString();
        }
    }
}

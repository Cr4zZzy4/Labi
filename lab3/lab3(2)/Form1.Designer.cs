namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Очистка всех используемых ресурсов.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов Windows Forms

        /// <summary>
        /// Метод для инициализации компонентов формы.
        /// </summary>
        private void InitializeComponent()
        {
            startServerButton = new Button();
            sendButton = new Button();
            inputTextBox = new TextBox();
            encodedTextBox = new TextBox();
            binaryTextBox = new TextBox();
            serverIpTextBox = new TextBox();
            clientIpTextBox = new TextBox();
            portTextBox = new TextBox();
            connectClientButton = new Button();
            logTextBox = new TextBox();
            SuspendLayout();
            // 
            // startServerButton
            // 
            startServerButton.Location = new Point(228, 12);
            startServerButton.Name = "startServerButton";
            startServerButton.Size = new Size(108, 35);
            startServerButton.TabIndex = 9;
            startServerButton.Text = "Запустить сервер";
            startServerButton.Click += StartServerButton_Click;
            // 
            // sendButton
            // 
            sendButton.Location = new Point(104, 12);
            sendButton.Margin = new Padding(4, 3, 4, 3);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(117, 35);
            sendButton.TabIndex = 1;
            sendButton.Text = "Отправить";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += SendButton_Click;
            // 
            // inputTextBox
            // 
            inputTextBox.Location = new Point(14, 69);
            inputTextBox.Margin = new Padding(4, 3, 4, 3);
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(240, 23);
            inputTextBox.TabIndex = 2;
            inputTextBox.Text = "Вводить текст сюда";
            // 
            // encodedTextBox
            // 
            encodedTextBox.Location = new Point(0, 0);
            encodedTextBox.Name = "encodedTextBox";
            encodedTextBox.Size = new Size(100, 23);
            encodedTextBox.TabIndex = 0;
            // 
            // binaryTextBox
            // 
            binaryTextBox.Location = new Point(0, 0);
            binaryTextBox.Name = "binaryTextBox";
            binaryTextBox.Size = new Size(100, 23);
            binaryTextBox.TabIndex = 0;
            // 
            // serverIpTextBox
            // 
            serverIpTextBox.Location = new Point(342, 87);
            serverIpTextBox.Name = "serverIpTextBox";
            serverIpTextBox.PlaceholderText = "Введите айпи сервера";
            serverIpTextBox.Size = new Size(150, 23);
            serverIpTextBox.TabIndex = 8;
            // 
            // clientIpTextBox
            // 
            clientIpTextBox.Location = new Point(0, 0);
            clientIpTextBox.Name = "clientIpTextBox";
            clientIpTextBox.Size = new Size(100, 23);
            clientIpTextBox.TabIndex = 0;
            // 
            // portTextBox
            // 
            portTextBox.Location = new Point(342, 130);
            portTextBox.Name = "portTextBox";
            portTextBox.PlaceholderText = "Введите порт сервера";
            portTextBox.Size = new Size(150, 23);
            portTextBox.TabIndex = 10;
            // 
            // connectClientButton
            // 
            connectClientButton.Location = new Point(342, 12);
            connectClientButton.Name = "connectClientButton";
            connectClientButton.Size = new Size(103, 35);
            connectClientButton.TabIndex = 1;
            connectClientButton.Text = "Подключить клиент";
            connectClientButton.Click += ConnectClientButton_Click;
            // 
            // logTextBox
            // 
            logTextBox.Location = new Point(14, 100);
            logTextBox.Multiline = true;
            logTextBox.Name = "logTextBox";
            logTextBox.ReadOnly = true;
            logTextBox.ScrollBars = ScrollBars.Vertical;
            logTextBox.Size = new Size(236, 200);
            logTextBox.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(599, 471);
            Controls.Add(logTextBox);
            Controls.Add(connectClientButton);
            Controls.Add(portTextBox);
            Controls.Add(serverIpTextBox);
            Controls.Add(inputTextBox);
            Controls.Add(sendButton);
            Controls.Add(startServerButton);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Form1";
            Text = "TCP Communication";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button startServerButton;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.TextBox encodedTextBox;
        private System.Windows.Forms.TextBox binaryTextBox;
        private TextBox serverIpTextBox;
        private TextBox clientIpTextBox;
        private TextBox portTextBox;
        private Button connectClientButton;
        private TextBox logTextBox;
    }
}

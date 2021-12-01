using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace serverTCP
{
    /// <summary>
    /// Класс ClientObject
    /// Для описания клиента, подключенного к серверу
    /// </summary>
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        
        private TcpClient client;
        private ServerObject server; // объект сервера


        public string UserName { get; private set; }

        /// <summary>
        /// Констурктор класса ClientObject
        /// </summary>
        /// <param name="tcpClient">Аргумент метода ClientObject()</param>
        /// <param name="serverObject">Аргумент метода ClientObject()</param>
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        /// <summary>
        /// Метод Process()
        /// Обрабатывает запросы клиента
        /// </summary>
        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                string message = GetMessage();
                UserName = message;
                if (!server.IsTrueName(UserName, Id))
                {
                    message = String.Format("{1}| {0}: Такое имя уже существует", UserName, DateTime.Now.ToString("f"));
                    Console.WriteLine(message); 
                    server.BroadcastMessageOneClient("Такое имя уже существует", this.Id);
                    
                    throw new Exception("Такое имя пользователя уже существует");
                }
                message = DateTime.Now.ToString("f") + "| " + UserName + " вошел в игру";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message); // сообщение о входе на сервер

                // Посылаем правила игры вошедшему клиенту
                
                server.BroadcastMessageOneClient(server.GameServer.Rool1 + server.GameServer.Rool2, this.Id);
                string tableStrOne = "Игровая таблица:\n" + server.GameServer.GetTableStr();
                server.BroadcastMessageOneClient(tableStrOne, this.Id);

                // Посылаем кол-во играков вошедшему игроку
                server.BroadcastMessageOneClient("Всего человек в игре на данный момент (включая вас): " + server.GetCountOfClients() + "\n", this.Id);
                //server.BroadcastMessageOneClient("Ваш ход: ", this.Id);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    Thread.Sleep(2000);
                    if (server.GameServer.IsMyStep(server.GetNumberClient(Id)))
                    {
                        try
                        {
                            server.BroadcastMessageAllClient("Сейчас ходит игрок "+ UserName +"\n");
                            server.BroadcastMessageOneClient("Ваш ход: ", this.Id);
                            message = GetMessage(); // Получаем сообщение (номер ячейки и напрвление через пробел)
                            string[] step = message.Trim().Split(" ");
                            try
                            {
                                if (step.Length != 2)
                                    throw new ArgumentException();

                                int numCell;
                                if (!int.TryParse(step[0], out numCell))
                                    throw new ArgumentException();

                                if (numCell >= 25 || numCell < 0)
                                    throw new ArgumentException();

                                Direction dr;
                                if (step[1] == "л")
                                    dr = Direction.LEFT;
                                else if (step[1] == "п")
                                    dr = Direction.RIGHT;
                                else if (step[1] == "в")
                                    dr = Direction.TOP;
                                else if (step[1] == "н")
                                    dr = Direction.BOTTOM;
                                else throw new ArgumentException();

                                server.GameServer.SetTableStep(numCell, dr);

                                message = String.Format("{2}| {0}: {1}", UserName, message, DateTime.Now.ToString("f"));
                                Console.WriteLine(message); // Пишем в консоли сервера сообщение переденнаное клиентом (номер ячейки и напрвление движения)
                                server.BroadcastMessage(message, this.Id); // Транслируем сообщение полученным клиентам

                                BroadcastAllGameTable();

                                if (server.GameServer.TheWinGame())
                                {
                                    string winStr = "!!!! Игра закончена, вы победили !!!! (Кол-во выигранных игр: "
                                        + server.GameServer.WinCount.ToString() + ")";
                                    Console.WriteLine(winStr);
                                    server.BroadcastMessage(winStr, this.Id); // Транслируем сообщение полученным клиентам
                                    server.BroadcastMessageOneClient(winStr, this.Id);

                                    server.GameServer.StartNewGame();

                                    BroadcastAllGameTable();
                                }
                            }

                            catch (ArgumentException)
                            {
                                message = "Некорректно записан ход";
                                server.BroadcastMessageOneClient(message, this.Id);
                                server.BroadcastMessageOneClient(server.GameServer.Rool2, this.Id);
                                message = String.Format("{2}| {0}: {1}", UserName, message, DateTime.Now.ToString("f"));
                                Console.WriteLine(message);
                            }
                            catch (Exception ex)
                            {
                                server.BroadcastMessageOneClient(ex.Message, this.Id);
                                message = String.Format("{2}| {0}: {1}", UserName, ex.Message, DateTime.Now.ToString("f"));
                                Console.WriteLine(message);
                            }
                        }
                        catch
                        {
                            message = String.Format("{1}| {0}: покинул игру", UserName, DateTime.Now.ToString("f"));
                            Console.WriteLine(message); // Пишем в консоле сервера о исключении из игры клиента
                            server.BroadcastMessage(message, this.Id); // Транслируем это для всех пользователей
                            break;
                        }
                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        /// <summary>
        /// Метод BroadcastAllGameTable()
        /// Транслирует всем клиентам игровую таблицу
        /// </summary>
        private void BroadcastAllGameTable()
        {
            string tableStrNew = "Игровая таблица:\n" + server.GameServer.GetTableStr();
            Console.WriteLine(tableStrNew);
            server.BroadcastMessageAllClient(tableStrNew);
        }

        /// <summary>
        /// Метод GetMessage()
        /// чтение входящего сообщения и преобразование его в строку
        /// </summary>
        private string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        /// <summary>
        /// Метод Close()
        /// закрывает подключение
        /// </summary>
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
        
    }
}

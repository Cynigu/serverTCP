using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace serverTCP
{
    public class ClientObject
    {

        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        TcpClient client;
        ServerObject server; // объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                // получаем имя пользователя
                string message = GetMessage();
                userName = message;

                message = DateTime.Now.ToString("f") + "| " + userName + " вошел в игру";
                // посылаем сообщение о входе в чат всем подключенным пользователям
                server.BroadcastMessage(message, this.Id);
                Console.WriteLine(message); // сообщение о входе на сервер

                // Посылаем правила игры вошедшему клиенту
                server.BroadcastMessageOneClient(server.rool1 + server.rool2, this.Id);
                string tableStrOne = "Игровая таблица:\n" + server.GameServer.GetTableStr();
                server.BroadcastMessageOneClient(tableStrOne, this.Id);

                //server.BroadcastMessageOneClient("Ваш ход: ", this.Id);
                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    try
                    {
                        server.BroadcastMessageOneClient("Ваш ход: ", this.Id);
                        message = GetMessage(); // Получаем сообщение (номер ячейки и напрвление через пробел)
                        string[] step = message.Trim().Split(" ");
                        try
                        {
                            if (step.Length != 2) 
                                throw new ArgumentException();

                            int numCell;
                            if (!int.TryParse(step[0],out numCell))
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

                            message = String.Format("{2}| {0}: {1}", userName, message, DateTime.Now.ToString("f"));
                            Console.WriteLine(message); // Пишем в консоле сервера сообщение переденнаное клиентом (номер ячейки и напрвление движения)
                            server.BroadcastMessage(message, this.Id); // Транслируем сообщение полученным клиентам

                            BroadcastAllGameTable();

                            if (server.GameServer.TheWinGame())
                            {
                                string winStr = "!!!! Игра закончена, вы победили !!!! (Кол-во выигранных игр: " 
                                    + Game.WinCount.ToString() + ")";
                                Console.WriteLine(winStr);
                                server.BroadcastMessage(winStr, this.Id); // Транслируем сообщение полученным клиентам
                                server.BroadcastMessageOneClient(winStr, this.Id);

                                server.GameServer.StartNewGame();

                                BroadcastAllGameTable();
                            }
                        }

                        catch(ArgumentException)
                        {
                            message = "Некорректно записан ход";
                            server.BroadcastMessageOneClient(message, this.Id);
                            server.BroadcastMessageOneClient(server.rool2, this.Id);
                            message = String.Format("{2}| {0}: {1}", userName, message, DateTime.Now.ToString("f"));
                            Console.WriteLine(message);
                        }
                        catch (Exception ex)
                        {
                            server.BroadcastMessageOneClient(ex.Message, this.Id);
                            message = String.Format("{2}| {0}: {1}", userName, ex.Message, DateTime.Now.ToString("f"));
                            Console.WriteLine(message);
                        }
                    }
                    catch
                    {
                        message = String.Format("{1}| {0}: покинул игру", userName, DateTime.Now.ToString("f"));
                        Console.WriteLine(message); // Пишем в консоле сервера о исключении из игры клиента
                        server.BroadcastMessage(message, this.Id); // Транслируем это для всех пользователей
                        break;
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

        private void BroadcastAllGameTable()
        {
            string tableStrNew = "Игровая таблица:\n" + server.GameServer.GetTableStr();
            Console.WriteLine(tableStrNew);
            server.BroadcastMessageAllClient(tableStrNew);
        }

        // чтение входящего сообщения и преобразование в строку
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

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
        
    }
}

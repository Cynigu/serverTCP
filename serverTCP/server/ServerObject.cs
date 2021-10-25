using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace serverTCP
{
    public class ServerObject
    {
        private static TcpListener tcpListener; // сервер для прослушивания
        private List<ClientObject> clients = new List<ClientObject>(); // все подключения
        private Game game = new Game();

        public Game GameServer => game;

        public string rool1 = "Правила: \nДано поле 5*5 клеток и 15 фишек трех цветов, по пять каждого цвета.\n" +
                    "Каждая клетка поля может быть либо блокирована, либо занята одной фишкой любого цвета, либо свободна.\n" +
                    "На поле выставлены все фишки, 6 клеток блокированы и 4 клетки свободны.\n" +
                    "Блокированные клетки остаются таковыми всегда.Фишки мы можем передвигать на \n" +
                    "соседнее свободное место по горизонтали или вертикали. Требуется, передвигая фишки,\n" +
                    "выставить их в три вертикальных ряда соответственно цветам, стоящим над полем.\n";
        public string rool2 = "\nЧтобы сделать ход нужно написать номер ячеки (от 0 до 24 (включая))\nи напрвление движения куда двинуть ячейку " +
                    "(влево - л, вправо - п, вверх - в, вниз - н) через пробел. \nПример: \"2 л\" \n";

            

        // Добавление подключения
        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }

        // Исключения клиента из списка подключенных
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null)
                clients.Remove(client);
        }

        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                Console.WriteLine(rool1 + rool2);
                Console.WriteLine("Игровая таблица:");
                Console.WriteLine(GameServer.GetTableStr());

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();

                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // трансляция сообщения подключенным клиентам
        protected internal void BroadcastMessage(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id) // если id клиента не равно id отправляющего
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }

        // трансляция сообщения конкретному клиентаму
        protected internal void BroadcastMessageOneClient(string message, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id == id) // если id клиента не равно id отправляющего
                {
                    clients[i].Stream.Write(data, 0, data.Length); //передача данных
                }
            }
        }

        // трансляция сообщения всем клиентам
        protected internal void BroadcastMessageAllClient(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Stream.Write(data, 0, data.Length); //передача данных
            }
        }

        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}

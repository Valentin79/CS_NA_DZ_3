using System.Net.Sockets;
using System.Net;
using System.Text;

namespace DZ_Sem_3
{
    internal class Program
    {
        public static bool flag = true;

        static private CancellationTokenSource cts = new CancellationTokenSource();
        static private CancellationToken token = cts.Token;

        static void Main(string[] args)
        {
            UdpServer("Hello");

        }

        public static void UdpServer(string name)
        {
            
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            using (UdpClient udpClient = new UdpClient(12345))
            {

                Console.WriteLine("Сервер ждет сообщение от клиента");

                ThreadPool.QueueUserWorkItem(obj =>
                {
                    Console.ReadKey(true);
                    cts.Cancel();
                });


                while (!token.IsCancellationRequested)
                {
                    
                    ThreadPool.QueueUserWorkItem(obj =>
                    {
                    try // иногда выскакивала непонятная ошибка
                    {
                        byte[] buffer = udpClient.Receive(ref iPEndPoint);
                            var messageText = Encoding.UTF8.GetString(buffer);
                            Message message = Message.DeserializeFromJson(messageText);
                            message.Print();

                            byte[] answer = Encoding.UTF8.GetBytes("Сообщение получено");
                            udpClient.Send(answer, answer.Length, iPEndPoint);
                        }
                        catch (Exception ex) { Console.WriteLine(ex); }
                    }, token);
                }
            }
        }
    }
}


using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Cliente_Servicio
{
    [Serializable]
    public class Cliente
    {
        TcpClient client = new TcpClient();

        public ICommand ConectarCommand
        { get;
            set;
        }
        public ICommand EnviarCommand
        {
            get;
            set;
        }

        private bool conectado = false;

        public bool Conectado
        {
            get { return conectado; }
            set { conectado = value; }
        }

        private void Conectar()
        {
            if (Conectado == false)
            {
                client.Connect(new IPEndPoint(IPAddress.Loopback, 2800));
                Task.Delay(10);
                Conectado = client.Connected;
            }
        }

        private void Enviar(string cuestionario)
        {
            
                if (conectado == true)
            {
                if (!string.IsNullOrWhiteSpace(cuestionario))
            {
                NetworkStream ns = client.GetStream();
                var buffer = Encoding.UTF8.GetBytes(cuestionario);
                ns.Write(buffer, 0, buffer.Length);

            }
        }
             else
                MessageBox.Show("Error de conexión");
        }
       
        public Cliente()
        {
            ConectarCommand = new RelayCommand(Conectar);
            EnviarCommand = new RelayCommand<string>(Enviar);
        }

    }
}

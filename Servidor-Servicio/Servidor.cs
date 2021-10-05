using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Servidor_Servicio
{
    public class Servidor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        List<TcpClient> clientes = new List<TcpClient>();
        List<string> cuestionario = new List<string>();
        TcpListener listener;
        Dispatcher dispatcher;
        public ICommand IniciarCommand 
        {
            get;
            set;
        }

        public ICommand DetenerCommand
        {
            get;
            set;
        }

        public int CuestionarioResultado
        {
            get { return cuestionario.Count; }
        }

            public float Pporcentaje
        {
            get
            {
                int tp = cuestionario.Where(x => x == "Pesimo").Count();
                if (cuestionario.Count > 0)
                    return tp * 100 / cuestionario.Count;
                else
                    return 0;

            }
        }

        public float Mporcentaje
        {
            get
            {
                int tm = cuestionario.Where(x => x == "Malo").Count();
                if (cuestionario.Count > 0)
                    return tm * 100 / cuestionario.Count;
                else
                    return 0;

            }
        }

        public float Nporcentaje
        {
            get
            {
                int tn = cuestionario.Where(x => x == "Normal").Count();
                if (cuestionario.Count > 0)
                    return tn * 100 / cuestionario.Count;
                else
                    return 0;

            }
        }

        public float Bporcentaje
        {
            get
            {
                int tb = cuestionario.Where(x => x == "Bueno").Count();
                if (cuestionario.Count > 0)
                    return tb * 100 / cuestionario.Count;
                else
                    return 0;

            }
        }

        public float SBporcentaje
        {
            get
            {
                int ts = cuestionario.Where(x => x == "SpBien").Count();
                if (cuestionario.Count > 0)
                    return ts * 100 / cuestionario.Count;
                else
                    return 0;

            }
        }

        public void onServidor()
        {
            if (listener == null)
            {
                Task.Run(() =>
                {
                    try
                    {
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 2800);
                        listener = new TcpListener(endPoint);
                        listener.Start();

                        while (listener != null)
                        {
                            TcpClient tcp = listener.AcceptTcpClient();
                            clientes.Add(tcp);
                            Recibir(tcp);
                        }
                    }
                    catch (Exception) { }
                });       
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }

        public void Recibir(TcpClient client)
        {
            Task.Run(() =>
           {
               NetworkStream ns = client.GetStream();
               while (client.Connected)
               {
                   if (client.Available > 0)
                   {
                       byte[] buffer = new byte[client.Available];
                       ns.Read(buffer, 0, buffer.Length);
                       string cuestionarios = Encoding.UTF8.GetString(buffer);

                       dispatcher.Invoke(() =>
                       {
                           cuestionario.Add(cuestionarios);
                           Save();
                       });
                   }
                   Task.Delay(50);
                   PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
               }
           });
        }

        public void offServidor()
        {
            if (listener != null)
            {
                listener.Stop();
                listener = null;

                foreach (var client in clientes)
                {
                    client.Close();
                }
                clientes.Clear();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));   
            }
        }

        public Servidor()
        {      
            dispatcher = Dispatcher.CurrentDispatcher;
            IniciarCommand = new RelayCommand(onServidor);
            DetenerCommand = new RelayCommand(offServidor);
            Load();
        }


        private void Load()
        {
            try
            {
                XmlReader archivo = XmlReader.Create("Resultados.xml");
                XmlSerializer seralizer = new XmlSerializer(typeof(List<string>));
                cuestionario = (List<string>)seralizer.Deserialize(archivo);
                archivo.Close();
            }
            catch (Exception)
            {
                cuestionario = new List<string>();
            }
        }

        private void Save ()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            XmlWriter archivo = XmlWriter.Create("Resultados.xml");
            serializer.Serialize(archivo, cuestionario);
            archivo.Close();
        }

    }    



}



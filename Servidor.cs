using System ;
using System.Net.Sockets ;
using System.IO ;
using System.Net;
using System.Threading.Tasks;

public class Servidor
{
    public static void Main(){
        try{
            bool status = true;
            string mensagemEnviada = "";
            string mensagemRecebida = "";
            TcpListener servidor = new TcpListener(IPAddress.Any, 8090);

            //iniciando servidor
            Console.WriteLine("********************************");
            Console.WriteLine("*********** WSERVIDOR **********");
            Console.WriteLine("********************************\n");
            Console.WriteLine("Iniciando Servidor");
            servidor.Start();
            Console.WriteLine("Servidor Iniciado com Sucesso");

            string nome;
            Console.Write("\n\nDigite seu nome: ");
            nome = Console.ReadLine();

            //aguardando conexao
            Console.WriteLine("\n\nAguardando Conexão...");
            Socket cliente = servidor.AcceptSocket();

            Console.WriteLine("Cliente Conectado");
            Console.WriteLine("\nPara Sair digite \"/sair\"");
            NetworkStream conexao = new NetworkStream(cliente);
            StreamWriter escritor = new StreamWriter(conexao);
            escritor.AutoFlush = true;
            StreamReader leitor = new StreamReader(conexao);
            
            
            if(cliente.Connected){

                FuncLer(mensagemRecebida, leitor, escritor, cliente, status, conexao);

                while(status){
                    mensagemEnviada = Console.ReadLine();
                    if (!String.IsNullOrEmpty(mensagemEnviada)){
                        escritor.WriteLine(nome + ": " + mensagemEnviada);
                        escritor.Flush();
                        Console.WriteLine("Voce: " + mensagemEnviada);

                        if((mensagemEnviada == "/sair" || mensagemEnviada == "/SAIR")){
                            status = false;
                            leitor.Close();
                            conexao.Close();
                            escritor.Close();
                        }

                        mensagemEnviada = null;
                    }   
                }
                
                
            }

        }catch(Exception e){
            Console.WriteLine("\nNão pode Enviar ou Ler a mensagem. Conexão foi perdida\n");
            Console.WriteLine("\nO Servidor será fechado\n");
            Console.ReadKey(true);
            Console.WriteLine(e.StackTrace);
        }
    }

    //Criando uma Thread para ler as mensagem do Cliente
    public static void FuncLer(string mensagemRecebida, StreamReader leitor, StreamWriter escritor, Socket cliente, bool status, NetworkStream conexao){
        
        Task.Run(() => {
            status = true;
            while(status){
                mensagemRecebida = leitor.ReadLine();
                if (!String.IsNullOrEmpty(mensagemRecebida)){
                    Console.WriteLine(mensagemRecebida);

                    if((mensagemRecebida == "/sair" || mensagemRecebida == "/SAIR")){
                        status = false;
                        leitor.Close();
                        conexao.Close();
                        escritor.Close();
                    }

                    mensagemRecebida = "";
                }
            }

            leitor.Close();
            conexao.Close();
            escritor.Close();
            cliente.Close();
            Console.WriteLine("Cliente saiu");
        });
    }
} 
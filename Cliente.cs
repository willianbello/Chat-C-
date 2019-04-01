using System ; 
using System.Net.Sockets ; 
using System.IO ;
using System.Net;
using System.Threading.Tasks;

public class Cliente 
{ 
    static void Main(string[] args){

        TcpClient servidor; 
        bool status = true;
        
        //iniciando servidor
        Console.WriteLine("********************************");
        Console.WriteLine("*********** WCLIENTE ***********");
        Console.WriteLine("********************************\n");

        string nome;
        Console.Write("Digite seu nome: ");
        nome = Console.ReadLine();

        try{
            Console.Write("Digite o IP do servidor: ");
            string IPServidor = Console.ReadLine();
            servidor = new TcpClient(IPServidor, 8090);
            if (servidor.Connected){
                Console.WriteLine("Voce foi conectado no servidor");
                Console.WriteLine("\nPara Sair digite \"/sair\"");
            }else{
                Console.WriteLine("O servidor esta ocupado...");
            }

        }catch{
            Console.WriteLine("Falha de Conexao");
            return;
        }

        NetworkStream conexao = servidor.GetStream();
        StreamReader leitor = new StreamReader(conexao);
        StreamWriter escritor = new StreamWriter(conexao);
        escritor.AutoFlush = true; 
        string mensagemEnviada = "";
        string mensagemRecebida = "";

        try{

            FuncLer(mensagemRecebida, leitor, escritor, servidor, status, conexao);

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
            
        }catch (Exception e){

            Console.WriteLine("\nNão pode Enviar ou Ler a mensagem. Conexão foi perdida\n");
            Console.WriteLine("\nO Cliente será fechado\n");
            Console.ReadKey(true);
            Console.WriteLine(e.StackTrace);
        }

        leitor.Close(); 
        conexao.Close(); 
        escritor.Close(); 
    }

    public static void FuncLer(string mensagemRecebida, StreamReader leitor, StreamWriter escritor, TcpClient servidor, bool status, NetworkStream conexao){
        status = true;
        Task.Run(() => {
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

                    mensagemRecebida = null;
                }
            }

            leitor.Close();
            conexao.Close();
            escritor.Close();
            servidor.Close();
            Console.WriteLine("Servidor saiu");
        });
    }

} 


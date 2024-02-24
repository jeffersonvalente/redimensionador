using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;



namespace didaticos.redimensionador
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando redimensionador");

            // Inicia uma nova thread para o processo de redimensionamento
            Thread thread = new Thread(Redimensionar);
            thread.Start();
            

        }
        
        static void Redimensionar()
        {
            #region "diretorios"
            // Diretórios para entrada, redimensionamento e arquivos finalizados
            string diretorio_entrada = "Arquivos_Entrada";
            string diretorio_redimensionado = "Arquivos_Redimensionado";
            string diretorio_finalizados = "Arquivos_Finalizado";

            // Cria os diretórios se eles não existirem
            if (!Directory.Exists(diretorio_entrada))
            {
                Directory.CreateDirectory(diretorio_entrada);
                Console.WriteLine($"Criando {diretorio_entrada}");
            }

            if (!Directory.Exists(diretorio_redimensionado))
            {
                Directory.CreateDirectory(diretorio_redimensionado);
                Console.WriteLine($"Criando {diretorio_redimensionado}");
            }

            if (!Directory.Exists(diretorio_finalizados))
            {
                Directory.CreateDirectory(diretorio_finalizados);
                Console.WriteLine($"Criando {diretorio_finalizados}");
            }
            #endregion

            FileStream fileStream;
            FileInfo fileInfo;

            while (true)
            {
                // Obtém a lista de arquivos no diretório de entrada
                var arquivosEntrada = Directory.EnumerateFiles(diretorio_entrada);

                int novaAltura = 200;

                // Verifica se existem arquivos no diretório de entrada
                if (!arquivosEntrada.Any())
                {
                    Console.WriteLine("Pasta de entrada vazia.");
                    Thread.Sleep(new TimeSpan(0, 0, 5)); // Aguarda antes de verificar novamente
                    continue; // Pula para a próxima iteração do loop
                }

                // Itera sobre cada arquivo no diretório de entrada
                foreach (var arquivo in arquivosEntrada)

                {
                    // Abre o arquivo em modo de leitura e escrita
                    fileStream = new FileStream(arquivo, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fileInfo = new FileInfo(arquivo);

                    // Caminho para salvar o arquivo redimensionado
                    string caminho = Environment.CurrentDirectory + @"\" + diretorio_redimensionado
                        + @"\" + DateTime.Now.Millisecond.ToString() + "_" + fileInfo.Name;

                    // Chama a função para redimensionar a imagem
                    Redimensionador(Image.FromStream(fileStream), novaAltura, caminho);

                    fileStream.Close();

                    // Move o arquivo redimensionado para o diretório de arquivos finalizados
                    string caminhoFinalizado = Environment.CurrentDirectory + @"\" + diretorio_finalizados + @"\" + DateTime.Now.Millisecond.ToString() + "_" + fileInfo.Name;
                    fileInfo.MoveTo(caminhoFinalizado);

                    Console.WriteLine("Imagem redimensionada");
                }

                // Aguarda antes de verificar novamente os arquivos de entrada
                Thread.Sleep(new TimeSpan(0, 0, 10));
                Console.WriteLine("Buscando Arquivos");
            }

        }
        
        // Função para redimensionar a imagem
        static void Redimensionador(Image imagem, int altura, string caminho)
        {
            
            // Calcula a proporção para redimensionar a imagem mantendo sua proporção
            double ratio = (double)altura / imagem.Height;
            int novaLargura = (int)(imagem.Width * ratio);
            int novaAltura= (int)(imagem.Height * ratio);

            // Cria uma nova imagem com as dimensões redimensionadas
            Bitmap novaImage = new Bitmap(novaLargura, novaAltura);

            using (Graphics g = Graphics.FromImage(novaImage))
            {
                g.DrawImage(imagem, 0, 0, novaLargura, novaAltura);
            }
            
            // Salva a nova imagem no caminho especificado
            novaImage.Save(caminho);
            imagem.Dispose();
        }
    }
}

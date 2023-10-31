
using System.Resources;
using intCodeProgram;
using Parte1.Properties;

namespace Parte1 {

    internal class App{
        public static void Main(string[] args){
   

            ProcessIntCode processIntCode = new ProcessIntCode();
            
            //Cargamos los valores del fichero
            string[] contentFile = Resources.Input.Split(',');
            
            //Añadimos sustantivo 12 y verbo 2
            contentFile[1] = "12";
            contentFile[2] = "2";

            //Procesamos la cadena
            string[] result = processIntCode.run(contentFile);
            
            //Imprimos por consola el resultado
            System.Console.WriteLine(string.Join(", ", result));
        }
    }
}
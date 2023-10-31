using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace intCodeProgram
{
    public class FileManager
    {

        //Carga los datos del fichero en la ruta (fileName) y los
        //devuelve como una array de valores string
        public static String[] ReadFile(string fileName)
        {
            String content;
            StreamReader reader = new StreamReader(fileName);
            content = reader.ReadToEnd();
            reader.Close();
            return content.Split(','); ;
        }

    }
}

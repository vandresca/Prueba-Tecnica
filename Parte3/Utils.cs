using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Parte3.Properties;

namespace intCodeProgram
{
    public class Utils
    {

        //Metodo que dado un array de enteros fragmenta el array en trozos de 4
        public static List<int[]> getFragments(int[] intCode)
        {

            List<int[]> fragments = new List<int[]>();

            // Recorremos el array en fragmentos de 4
            // números
            for (int i = 0; i < intCode.Length; i += 4)
            {
                // Obtenemos el fragmento
                fragments.Add(intCode.Skip(i).Take(4).ToArray()); ;
            }
            return fragments;
        }

        //Método que transforma un array de valores String a un array de valores enteros
        public static int[] StringToIntegerArray(string[] strings)
        {
            int[] integers = new int[strings.Length];

            for (int i = 0; i < strings.Length; i++)
            {
                integers[i] = int.Parse(strings[i]);
            }

            return integers;
        }

        //Método que transforma un array de valores enteros a un array de valores string
        public static string[] IntegerToStringArray(int[] intArray)
        {
            string[] stringArray = new string[intArray.Length];

            for (int i = 0; i < intArray.Length; i++)
            {
                stringArray[i] = intArray[i].ToString();
            }

            return stringArray;
        }

    }
}

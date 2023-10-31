using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Parte3;

namespace intCodeProgram
{
    public class ProcessIntCode
    {
        //Valores constantes para el operador
        private const int SUM_OPERAND = 1;
        private const int MULTIPLY_OPERAND = 2;
        private const int STOP_PROCESS = 99;

        //Valores constantes indicativos de la función de cada valor del fragmento
        private const int OPERATOR_PARAM = 0;
        private const int SUSTANTIVE_PARAM = 1;
        private const int VERB_PARAM = 2;
        private const int POSITION_TO_SAVE_PARAM = 3;

        //Método que procesa toda la cadena intCode y devuelve el intCode resultante
        public String[] run(String[] contentFile)
        {

            //Convierte el array de String de valores del fichero a un array de enteros
            int[] intCode = Utils.StringToIntegerArray(contentFile);

            //Solo procesamos si el sustantivo y el verbo se encuentran dentro de los limites de valores de intCode.
            //En caso contrario lanzamos un error.
            if (intCode[1] < intCode.Length && intCode[2] < intCode.Length)
            {
                //Fragmetamos el array de enteros en fragmentos de 4 valores
                List<int[]> fragments = Utils.getFragments(intCode);

                //Recorremos cada fragmento
                foreach (int[] fragment in fragments)
                {
                    // Si el primer número es 1, sumamos los dos siguientes números y lo guardamos en el cuarto
                    if (fragment[OPERATOR_PARAM] == SUM_OPERAND)
                    {
                        intCode[fragment[POSITION_TO_SAVE_PARAM]] = intCode[fragment[SUSTANTIVE_PARAM]] + intCode[fragment[VERB_PARAM]];
                    }

                    // Si el primer número es 2, multiplicamos los dos siguientes números y lo guardamos en el cuarto
                    else if (fragment[OPERATOR_PARAM] == MULTIPLY_OPERAND)
                    {
                        intCode[fragment[POSITION_TO_SAVE_PARAM]] = intCode[fragment[SUSTANTIVE_PARAM]] * intCode[fragment[VERB_PARAM]];
                    }

                    // Si el primer número es 99, terminamos el proceso
                    else if (fragment[OPERATOR_PARAM] == STOP_PROCESS)
                    {
                        break;
                    }
                }
            }
            else
            {
                throw new IntCodeException(IntCodeException.ERROR_OVERFLOW_SUSTANTIVE_VERB);
            }
            return Utils.IntegerToStringArray(intCode);
        }
    }
}


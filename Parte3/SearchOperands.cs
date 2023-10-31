using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Parte3;

namespace intCodeProgram
{
    public class SearchOperands
    {
        //Tope de sustantivo y verbo a buscar
        private const int MAX_SUSTANTIVE_VALUE = 999;
        private const int MAX_VERB_VALUE = 999;

        //Método que busca el sustantivo y el verbo para un resultado de forma sincrona
        public Dictionary<String, Object> search(Dictionary<String, Object> arguments)
        {
            Dictionary<String, Object> response = new Dictionary<String, Object>();

            ProcessIntCode processIntCode = new ProcessIntCode();
            String[] contentFile = (String[])arguments["contentFile"];
            
            //Probamos con todos los valores del sustantivo desde el 0 hasta maxSustantive
            int intCodeLength = contentFile.Length;
            int maxSustantive = getMaxSustantive(intCodeLength);
            for (int sustantive = 0; sustantive < maxSustantive; sustantive++)
            {

                Dictionary<String, Object> parameters = new Dictionary<String, Object>();
                parameters.Add("sustantive", sustantive);
                parameters.Add("contentFile", contentFile);
                parameters.Add("mainwindow", arguments["mainwindow"]);
                parameters.Add("clock", arguments["clock"]);
                parameters.Add("result", arguments["result"].ToString());
                parameters.Add("isasync", false);
                
                //Lanzamos el método que procesa los operandos
                String[] result = processSearchOperands(parameters);
            }
            //Si existe algun problema lanzamos un error
            throw new IntCodeException(IntCodeException.ERROR_OPERANDS_NOT_FOUND);
        }

        
        //Método que encuentra el sustantivo y verbo para un resultado de forma asincrona en paralelo
        public Dictionary<String, Object> searchAsync(Dictionary<String, Object> arguments)
        {
            Dictionary<String, Object> response = new Dictionary<String, Object>();
            List<Task<String[]>> tasks = new List<Task<String[]>>();

            ProcessIntCode processIntCode = new ProcessIntCode();
            String[] contentFile = (String[])arguments["contentFile"];
            
            //Busca todas los resultados del sustantivo desde 0 hasta maxSustantive
            int intCodeLength = contentFile.Length;
            int maxSustantive = getMaxSustantive(intCodeLength);

            for (int sustantive = 0; sustantive < maxSustantive; sustantive++)
            {

                Dictionary<String, Object> parameters = new Dictionary<String, Object>();
                parameters.Add("sustantive", sustantive);
                parameters.Add("mainwindow", arguments["mainwindow"]);
                parameters.Add("contentFile", contentFile);
                parameters.Add("clock", arguments["clock"]);
                parameters.Add("result", arguments["result"].ToString());
                parameters.Add("tasks", tasks);
                parameters.Add("isasync", true);
                
                //Lanzamos una tarea asincrona por cada sustantivo
                Task<String[]> task = taskOverload(parameters);
                task.Start();
                tasks.Add(task);
            }
            //Si existe algun problema lanzamos un error
            throw new IntCodeException(IntCodeException.ERROR_OPERANDS_NOT_FOUND);
        }

        //Método que crea una tarea asincrona en paralelo para un sustantivo determinado
        private Task<String[]> taskOverload(Dictionary<String, Object> arguments)
        {
            return new Task<String[]>(() =>
            {
                return processSearchOperands(arguments);
            });
        }

        //Método que se encarga de procesar una tarea intCode para el sustantivo y una secuencia
        //de verbos
        private String[] processSearchOperands(Dictionary<String, Object> arguments)
        {
            try
            {
                
                //Procesa cada intCode con el sustantivo y verbo indicado
                int intCodeLength = ((String[])arguments["contentFile"]).Length;
                int maxVerb = getMaxVerb(intCodeLength);
                
                for (int verb = 0; verb < maxVerb; verb++)
                {
                    ProcessIntCode processIntCode = new ProcessIntCode();
                    Dictionary<String, Object> parameters = new Dictionary<String, Object>();
                    Dictionary<String, Object> result;

                    //Añadimos el sustantivo y el verbo a la cadena contentFile (intCode) a procesar
                    String sustantive = arguments["sustantive"].ToString();
                    ((String[])arguments["contentFile"])[1] = sustantive;
                    ((String[])arguments["contentFile"])[2] = verb.ToString();

                    parameters.Add("contentFile", (String[])arguments["contentFile"]);
                    parameters.Add("clock", Stopwatch.StartNew());
                    
                    //Lanzamos el proceso
                    result = processIntCode.run(parameters);

                    //Si el resultado del proceso coincide con el resultado que buscamos devolvemos los datos
                    //mediante un delegate
                    if (((String[])result["result"])[0].Equals(arguments["result"].ToString()))
                    {
                        Dictionary<String, Object> mainWindowResult = new Dictionary<String, Object>();
                        
                        //Obtenemos la clase MainWindow para devolver los delegados.
                        MainWindow mainwindow = (MainWindow)arguments["mainwindow"];
                        mainWindowResult.Add("result", new String[] { sustantive, verb.ToString() });
                        
                        //Paramos el crono y metemos el valor medido en el diccionario
                        ((Stopwatch)arguments["clock"]).Stop();
                        TimeSpan timespan = ((Stopwatch)arguments["clock"]).Elapsed;
                        mainWindowResult.Add("time", timespan.TotalMilliseconds + "\n");
                        if ((bool)arguments["isasync"])
                        {
                            List<Task<String[]>> tasks = (List<Task<String[]>>)arguments["tasks"];
                            
                            //Eliminamos las posibles tareas que puedan estar activas, para evitar problemas con nuevos
                            //procesos
                            tasks.ForEach(task => { if (task.Status == TaskStatus.Running) { task = null; } });
                            
                            mainwindow.delegateCalculateOperandsAsync(mainWindowResult);
                        }
                        else
                        {
                            mainwindow.delegateCalculateOperandsNormal(mainWindowResult);
                        }
                    }
                }
            }catch(IntCodeException ice)
            {
                MessageBox.Show(ice.Message);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        //Comprueba que la longitud de la cadena de valores esta por debajo del valor maximo del sustantivo.
        //Si es asi devuelve la longitud de la cadena de valores, en caso contrario devuelve el valor de la constante
        private int getMaxSustantive(int intCodeLength)
        {
            if ((intCodeLength - 1)<MAX_SUSTANTIVE_VALUE) return intCodeLength - 1;
            return MAX_SUSTANTIVE_VALUE;
        }

        //Comprueba que la longitud de la cadena de valores esta por debajo del valor maximo del verbo.
        //Si es asi devuelve la longitud de la cadena de valores, en caso contrario devuelve el valor de la constante
        private int getMaxVerb(int intCodeLength)
        {
            if ((intCodeLength - 1)<MAX_VERB_VALUE) return intCodeLength - 1;
            return MAX_VERB_VALUE;
        }
    }
}

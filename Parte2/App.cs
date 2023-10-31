
using System.Reflection;
using System.Resources;
using intCodeProgram;
using Parte2.Properties;

namespace Parte2 {

    internal class App{
        public static void Main(string[] args)
        {
            SearchOperands searchOperands = new SearchOperands();
            string[] operands = searchOperands.search(Resources.Input.Split(','), "19690720");    
            System.Console.WriteLine(string.Join(", ", string.Join(",",operands)));
        }
    }
}
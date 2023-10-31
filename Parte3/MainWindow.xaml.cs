using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using intCodeProgram;
using Microsoft.Win32;
using Parte3.Properties;

namespace Parte3
{
    
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void GetCalculation(Dictionary<String, Object> response);

        private const int CALCULATE_NORMAL = 1;
        private const int CALCULATE_ASYNC = 2;
        private const int CALCULATE_MULTITHREAD = 3;

        public MainWindow()
        {
            InitializeComponent();


            // Declaro 3 variables de delegado.
            GetCalculation calculateResultDelegate;
            GetCalculation calculateOperandsNormalDelegate;
            GetCalculation calculateOperandsAsyncDelegate;


            // Asigno las variables de delegado a sus respectivos métodos.
            calculateResultDelegate = delegateCalculateResult;
            calculateOperandsNormalDelegate = delegateCalculateOperandsNormal;
            calculateOperandsAsyncDelegate = delegateCalculateOperandsAsync;
        }

        //Evento click para el cálculo no asincrono en paralelo
        private void Button_Click_Calculate(object sender, RoutedEventArgs e)
        {
            try
            {
                String[] contentFile = FileManager.ReadFile(txtProgram.Text);
                String result = txtResult.Text;
                cleanTmpTextBoxs();
                if (isSearchingResult())
                {
                    //Añadimos el sustantivo y el verbo a la cadena de valores enteros en la posición 1 y 2
                    contentFile[1] = txtSustantive.Text;
                    contentFile[2] = txtVerb.Text;
                    
                    //Lanzamos una tarea asincrona para no bloquear la ventana
                    Action actionResultNormal = () => calculateResultNormal(contentFile);
                    Task taskResultNormal = launchTask(actionResultNormal);
                    taskResultNormal.Start();
                }
                else if (isSearchingOperands())
                {
                    //Lanzamos una tarea asincrona para no bloquear la ventana
                    Action actionOperandsNormal = () => calculateOperandsNormal(contentFile, result, this);
                    Task taskOperandsNormal = launchTask(actionOperandsNormal);
                    taskOperandsNormal.Start();
                }
                else
                {
                    manageErrors();
                }
            }

            catch (IntCodeException ice)
            {
                MessageBox.Show(ice.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //Evento click para el cálculo asincrono en paralelo
        private void Button_Click_CalculateAsync(object sender, RoutedEventArgs e)
        {
            try
            {
                String[] contentFile = FileManager.ReadFile(txtProgram.Text);
                String result = txtResult.Text;
                cleanTmpTextBoxs();
                if (isSearchingResult())
                {
                    //Añadimos el sustantivo y el verbo a la cadena de valores enteros en la posición 1 y 2
                    contentFile[1] = txtSustantive.Text;
                    contentFile[2] = txtVerb.Text;
                    
                    //Lanzamos una tarea asincrona para no bloquear la ventana
                    Action actionResultNormal = () => calculateResultNormal(contentFile);
                    Task taskResultNormal = launchTask(actionResultNormal);
                    taskResultNormal.Start();
                }
                else if (isSearchingOperands())
                {
                    //Lanzamos una tarea asincrona para no bloquear la ventana.
                    Action actionOperandsAsync = () => calculateOperandsAsync(contentFile, result, this);
                    Task taskOperandsAsync = launchTask(actionOperandsAsync);
                    taskOperandsAsync.Start();
                }
                else
                {
                    manageErrors();
                }
            }
            catch (IntCodeException ice)
            {
                MessageBox.Show(ice.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //Evento click para abrir un cuadro de dialogo para seleccionar la ruta del fichero Input.txt
        private void Button_Click_Program(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Filter = "Archivos de texto (*.txt)|*.txt";
                if (openFileDialog.ShowDialog() == true)
                {
                    string filename = openFileDialog.FileName;
                    txtProgram.Text = filename;
                }
            }
            catch (IntCodeException ice)
            {
                MessageBox.Show(ice.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Método que limpia los cuadros de texto que muestran los tiempos de cálculo
        private void cleanTmpTextBoxs()
        {
            txtTmpNormal.Text = "";
            txtTmpAsync.Text = "";
        }

        //Métoddo que lanza una tarea para el método indicado (action)
        private Task launchTask(Action action)
        {
            Task task = new Task(() =>
            {
                action();
            });
            return task;
        }

        //Delegado que se ejecuta una vez se llama en otro lugar. En este caso para obtener los
        //datos del calculo de los operandos por medio NO asincrono en paralelo
        public void delegateCalculateOperandsNormal(Dictionary<String, Object> response) {
            Application.Current.Dispatcher.Invoke(() =>
            {
                txtTmpNormal.Text = (string)response["time"];
                txtSustantive.Text = ((string[])response["result"])[0];
                txtVerb.Text = ((string[])response["result"])[1];
            });
        }
        
        //Delegado que se ejecuta una vez se llama en otro lugar. En este caso para obtener los
        //datos del calculo de los operandos por medio asincrono en paralelo.
        public void delegateCalculateOperandsAsync(Dictionary<String, Object> response)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                txtTmpAsync.Text = (string)response["time"];
                txtSustantive.Text = ((string[])response["result"])[0];
                txtVerb.Text = ((string[])response["result"])[1];
            });
        }

        //Delegado que se ejecuta una vez se llama en otro lugar. En este caso para obtener el
        //dato del resultado a partir del sustativo y el verbo
        private void delegateCalculateResult(Dictionary<String, Object> response)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                txtResult.Text = ((String[])response["result"])[0];
                txtTmpNormal.Text = (string)response["time"];
            });
        }

        //Método que calcula los operandos de forma no asincrono en paralelo.
        private void calculateOperandsNormal(String[] contentFile, String value, MainWindow mainwindow)
        {
            SearchOperands searchoperands = new SearchOperands();
            Dictionary<String, Object> parameters = new Dictionary<string, object>();
            parameters.Add("contentFile", contentFile);
            parameters.Add("result", value);
            parameters.Add("mainwindow", mainwindow);
            parameters.Add("clock", System.Diagnostics.Stopwatch.StartNew());
            searchoperands.search(parameters);

        }

        //Método que calcula los operandos de forma asincrono en paralelo
        private void calculateOperandsAsync(String[] contentFile, string value, MainWindow mainWindow)
        {
            SearchOperands searchoperands = new SearchOperands();
            Dictionary<String, Object> parameters = new Dictionary<string, object>();
            parameters.Add("contentFile", contentFile);
            parameters.Add("result", value);
            parameters.Add("clock", System.Diagnostics.Stopwatch.StartNew());
            parameters.Add("mainwindow", mainWindow);
            searchoperands.searchAsync(parameters);
        }

        //Método que ´calcula el resultado a partir del sustantivo y el verbo de forma sincrona
        private void calculateResultNormal(String[] contentFile)
        {
            ProcessIntCode processIntCode = new ProcessIntCode();
            Dictionary<String, Object> parameters = new Dictionary<string, object>();
            parameters.Add("contentFile", contentFile);
            parameters.Add("clock", System.Diagnostics.Stopwatch.StartNew());
            Dictionary<String, Object> response =processIntCode.run(parameters);
            
            //Llamamos al método processIntCode para realizar el cálculo y cuando tenemos el resultado llamamos al 
            //delegate para mostrar los datos.
            delegateCalculateResult(processIntCode.run(parameters));
        }

        //Método que se ejecuta cada vez que se introduce un caracter en el cuadro de texto del sustantivo
        //Y qué solo permite números del 0 en adelante
        private void txtSustantive_numeric_only(object sender, TextCompositionEventArgs e)
        {
            //Solo deja escribir si es un número y este no tiene 0 a la izquierdsa.
            if (  
                Regex.IsMatch(e.Text, "\\d")
                && !(txtSustantive.Text.Equals("0") && txtSustantive.Text.Length == 1)
              )
            {
                e.Handled = false;
            }else {
                e.Handled = true;
            }
        }

        //Método que se ejecuta cada vez que se introduce un caracter en el cuadro de texto del verbo
        //Y qué solo permite números del 0 en adelante
        private void txtVerb_numeric_only(object sender, TextCompositionEventArgs e)
        {
            //Solo deja escribir si es un número y este no tiene 0 a la izquierdsa.
            if (
                Regex.IsMatch(e.Text, "\\d")
                && !(txtVerb.Text.Equals("0") && txtVerb.Text.Length == 1)
              )
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //Método que se ejecuta cada vez que se introduce un caracter en el cuadro de texto del resultado
        //Y qué solo permite números del 0 en adelante
        private void txtResult_numeric_only(object sender, TextCompositionEventArgs e)
        {
            //Solo deja escribir si es un número y este no tiene 0 a la izquierdsa.
            if (
                Regex.IsMatch(e.Text, "\\d")
                && !(txtResult.Text.Equals("0") && txtResult.Text.Length == 1)
              )
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        //Devuelve verdadero si nos encontramos en una situación de calculo de Resultado
        private bool isSearchingResult()
        {
            return !isEmpty(txtProgram.Text) &&
                   !isEmpty(txtSustantive.Text) &&
                   !isEmpty(txtVerb.Text) &&
                   isEmpty(txtResult.Text);
        }

        //Devuelve verdadero si nos encontramos en una situación de calculo del sustantivo y el verbo
        private bool isSearchingOperands()
        {
            return !isEmpty(txtProgram.Text) &&
                   isEmpty(txtSustantive.Text) &&
                   isEmpty(txtVerb.Text) &&
                   !isEmpty(txtResult.Text);
        }

        //Método que controla los posibles errores que se pueden cometer en la introducción de datos
        private void manageErrors()
        {
            if (isEmptyPath())
                throw new IntCodeException(IntCodeException.ERROR_PATH);
            if (isEmptyData())
                throw new IntCodeException(IntCodeException.ERROR_EMPTY_DATA);
            if (isWrongInputOne())
                throw new IntCodeException(IntCodeException.ERROR_WRONG_INSERT_DATA_ONE);
            if (isWrongInputTwo())
                throw new IntCodeException(IntCodeException.ERROR_WRONG_INSERT_DATA_TWO);
            if (isNotVerb())
                throw new IntCodeException(IntCodeException.ERROR_NOT_VERB);
            if (isNotSustantive())
                throw new IntCodeException(IntCodeException.ERROR_NOT_SUSTANTIVE);
            if (isAllData())
                throw new IntCodeException(IntCodeException.ERROR_ALL_DATA);
        }

        //Devuelve verdadero si el campo de texto de la ruta del fichero esta en blanco
        private bool isEmptyPath()
        {
            return isEmpty(txtProgram.Text);
        }

        //Devuelve verdadero si no existe ningún dato para calcular
        private bool isEmptyData()
        {
            return isEmpty(txtSustantive.Text) &&
                   isEmpty(txtVerb.Text) &&
                   isEmpty(txtResult.Text);
        }

        //Devuelve verdadero si hemos añadido sustantivo y resultado a la vez
        private bool isWrongInputOne()
        {
            return !isEmpty(txtSustantive.Text) &&
                   isEmpty(txtVerb.Text) &&
                   !isEmpty(txtResult.Text);
        }

        //Devuelve verdadero si hemos añadido verbo y resultado a la vez
        private bool isWrongInputTwo()
        {
            return isEmpty(txtSustantive.Text) &&
                   !isEmpty(txtVerb.Text) &&
                   !isEmpty(txtResult.Text);
        }

        //Devuelve verdadero si nos encontramos buscando el resultado, pero no hemos introducido el verbo
        private bool isNotVerb()
        {
            return !isEmpty(txtSustantive.Text) &&
                   isEmpty(txtVerb.Text) &&
                   isEmpty(txtResult.Text);
        }

        //Devuelve verdadero si nos enctramos buscando el resultado, pero no hemos introducido el sustantivo
        private bool isNotSustantive()
        {
            return isEmpty(txtSustantive.Text) &&
                   !isEmpty(txtVerb.Text) &&
                   isEmpty(txtResult.Text);
        }

        //Devuelve verdadero si todos los campos de datos estan llenos a la vez.
        private bool isAllData()
        {
            return !isEmpty(txtSustantive.Text) &&
                   !isEmpty(txtVerb.Text) &&
                   !isEmpty(txtResult.Text);
        }

        //Devuelve verdadero si el string proporcionado esta vacio.
        private bool isEmpty(string value)
        {
            return value.Equals(string.Empty);
        }

     
    }
}

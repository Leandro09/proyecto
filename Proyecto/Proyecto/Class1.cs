using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
namespace Proyecto
{
    class tarea
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        /// 

        static int cant_bytes_palabra = 4;
        static int cant_bytes_bloque = 16;
        static int pos_pc = 32;
        static int cant_campos = 37;
        static int cant_cache = 64;
        static int cant_encache = 4;
        static int cant_memComp = 128;
        static int cant_memNoComp = 256;
        static int limite = 128;
        static int reloj = 1;
        static int hilosCorriendo = 4;
        static int pos_tiempo_inicial = 34;
        static int pos_tiempo_final = 35;

        //Almacena la cantidad de hilillos que se correrán en sistema.
        static int hilillos = 0;
        //Almacena el quantum ingresado por el usuario.
        static int quantum = 0;

        static int contadorProcesador1 = 0;
        static int contadorProcesador2 = 0;
        static int contadorProcesador3 = 0;

        static int[] cache1 = new int[64];
        //Cual es el bloque que esta en la cache en esa posicion
        static int[] enCache1 = new int[4];
        static int[] cache2 = new int[64];
        //Cual es el bloque que esta en la cache en esa posicion
        static int[] enCache2 = new int[4];
        static int[] cache3 = new int[64];
        //Cual es el bloque que esta en la cache en esa posicion
        static int[] enCache3 = new int[4];
        static int[] memComp = new int[128];
        static int[] memNoComp1 = new int[256];
        static int[] memNoComp2 = new int[256];
        static int[] memNoComp3 = new int[256];

        /// estructuras para observar los recursos del procesador

        static int[] procesador1 = new int[38];        //En la posición 32 se encuentra el PC
        static int[] procesador2 = new int[38];
        static int[] procesador3 = new int[38];         //En la posicion 37 va el nombre del HILILLO al que pertenece este contexto



        /// colas para los contextos de los hilillos
        static Queue contextoProcesador1 = new Queue();
        static Queue contextoProcesador2 = new Queue();
        static Queue contextoProcesador3 = new Queue();


        //colas para resultados de los hilillos
        static Queue terminadosProcesador1 = new Queue();
        static Queue terminadosProcesador2 = new Queue();
        static Queue terminadosProcesador3 = new Queue();

        //Delegados de cada proceso 
        static ThreadStart delegado_proceso_1 = new ThreadStart(nombrarHilo1);
        static ThreadStart delegado_proceso_2 = new ThreadStart(nombrarHilo2);
        static ThreadStart delegado_proceso_3 = new ThreadStart(nombrarHilo3);
        //Los hilos que simularán los procesos en cuestión 
        Thread proceso_1 = new Thread(delegado_proceso_1);
        Thread proceso_2 = new Thread(delegado_proceso_2);
        Thread proceso_3 = new Thread(delegado_proceso_3);
        //Barrera utilizada para sincronizar los hilos (procesos).
        static Barrier miBarrerita = new Barrier(4);
        //Maneja las principales funciones del procesador y sus hilos.
        public void administradorDeEjecucion()
        {
            inicializarEstructuras();
            //Lee y acomoda en memoria las instrucciones de los hilillos.
            leeArchivos();
            Thread.CurrentThread.Name = "0";
            //se crean loc hilos que van a actuar como procesadores en esta simulacion
            proceso_1.Start();
            proceso_2.Start();
            proceso_3.Start();

            funcionPrincipal();

            //Agregar método para desplegar resultados.
        }

        public static void finalizarEjecucion(int id_hilo)
        {
            if (id_hilo == 1)
            {
                if (contextoProcesador1.Count == 0)
                {
                    //finalizar el hilo
                    //Thread.CurrentThread.Abort();
                    //si hay un fin, este sea o no el ultimo hilillo ejecutandose en este procesador
                    //debemos guardar sus resultados
                    Thread.CurrentThread.Abort();//Este es mientras descubrimos porque es que da error lo de finalize
                    //Es solo temporal porque lo que hace es lanzar una excepcion que acaba con el hilo
                    miBarrerita.RemoveParticipant();
                    procesador1[35] = reloj;
                    terminadosProcesador1.Enqueue(procesador1);
                    
                }
                else
                {

                    procesador1[35] = reloj;
                    terminadosProcesador1.Enqueue(procesador1);

                }
            }
            else if (id_hilo == 2)
            {
                if (contextoProcesador2.Count == 0)
                {
                    //finalizar el hilo
                    //Thread.CurrentThread.Abort();
                    Thread.CurrentThread.Abort();//Este es mientras descubrimos porque es que da error lo de finalize
                    //Es solo temporal porque lo que hace es lanzar una excepcion que acaba con el hilo
                    miBarrerita.RemoveParticipant();
                    procesador2[35] = reloj;
                    terminadosProcesador2.Enqueue(procesador1);
                }
                else
                {
                    procesador2[35] = reloj;
                    terminadosProcesador2.Enqueue(procesador1);
                }
            }
            else if (id_hilo == 3)
            {
                {
                    if (contextoProcesador3.Count == 0)
                    {
                        Thread.CurrentThread.Abort();//Este es mientras descubrimos porque es que da error lo de finalize
                        //Es solo temporal porque lo que hace es lanzar una excepcion que acaba con el hilo
                        miBarrerita.RemoveParticipant();
                        //finalizar el hilo
                        //Thread.CurrentThread.Abort();
                        procesador3[35] = reloj;
                        terminadosProcesador3.Enqueue(procesador1);
                    }
                    else
                    {
                        procesador3[35] = reloj;
                        terminadosProcesador3.Enqueue(procesador1);
                    }
                }
            }
        }
        

        //Métodos para cambiar los nombres de los hilos a 1, 2 y 3 respectivamente.
        public static void nombrarHilo1()
        {
            Thread.CurrentThread.Name = "1";
            miBarrerita.AddParticipant();
            funcionPrincipal();
        }

        public static void nombrarHilo2()
        {
            Thread.CurrentThread.Name = "2";
            miBarrerita.AddParticipant();
            funcionPrincipal();
        }

        public static void nombrarHilo3()
        {
            Thread.CurrentThread.Name = "3";
            miBarrerita.AddParticipant();
            funcionPrincipal();
        }

        //inicializar estructuras
        public void inicializarEstructuras()
        {
            for (int i = 0; i < cant_cache; ++i )
            {
                cache1[i] = 0;
                cache2[i] = 0;
                cache2[i] = 0;
            }

            for(int i = 0; i < cant_encache; ++i){
                enCache1[i] = -1;
                enCache2[i] = -1;
                enCache3[i] = -1;
            }

            for (int i = 0; i < cant_campos; ++i)
            {
                procesador1[i] = 0;
                procesador2[i] = 0;
                procesador3[i] = 0; 
            }
            
        }
        //int[] pertenece = new int[12];
        String[] path = new String[12];

        public int pedirQuantum()
        {
            return 100;
        }

        public void distribucionHilillos()
        {

            string line = "";
            System.IO.StreamReader file = new System.IO.StreamReader(@"c:\test.txt");
            while ((line = file.ReadLine()) != null)
            {
                for (int indice = 0; indice < line.Count(); ++indice)
                {
                    if (line.Substring(indice, ++indice).Equals(' ') == true)
                    {
                        //procedimiento de almacenar en memoria no compartida desde la posicion de dicha memoria
                    }
                }
            }

        }


        //lectura de instrucciones 
        // id_hilo es el numero de procesador

        public static bool leerInstruccion(int id_hilo)
        {
            int indicador = 0;
            //inicializarEstructuras();
            int [] instruccion = new int[cant_bytes_palabra];
           
            // switch para leer pc de procesador 

            int PC;
            
            switch(id_hilo){
                case 1:
                    PC  = procesador1[pos_pc];
                    procesador1[pos_pc] = PC + 4;
                    break;
                case 2:
                    PC = procesador2[pos_pc];
                    procesador2[pos_pc] = PC + 4;
                    break;
                default:
                    PC = procesador3[pos_pc];
                    procesador3[pos_pc] = PC + 4;
                    break;
                
            }



            int bloque = PC / cant_bytes_bloque;
            int indice = bloque % cant_bytes_palabra;
            int palabra = bloque / cant_bytes_palabra;
            PC = PC - limite;
           
            if(id_hilo==0){
                if (enCache1[indice] != -1 && bloque == enCache1[indice])
                {
                    indicador = palabra * cant_bytes_palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i )
                    {
                        instruccion[i] = cache1[indicador];
                        indicador = indicador + cant_bytes_palabra;
                    }
                }
                else
                {
                    falloCache(id_hilo, PC );
                    indicador = palabra * cant_bytes_palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = cache1[indicador];
                        indicador = indicador + cant_bytes_palabra;
                    }

                }
            }
            else if (id_hilo == 1)
            {
                if (enCache2[indice] != -1 && bloque == enCache2[indice])
                {
                    indicador = palabra * cant_bytes_palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = cache2[indicador];
                        indicador = indicador + cant_bytes_palabra;
                    }
                }
                else
                {
                    falloCache(id_hilo, PC);
                    indicador = palabra * cant_bytes_palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = cache2[indicador];
                        indicador = indicador + cant_bytes_palabra;
                    }
                }
            }
            else {

                if (enCache3[indice] != -1 && bloque == enCache3[indice])
                {
                    indicador = palabra * cant_bytes_palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = cache3[indicador];
                        indicador = indicador + cant_bytes_palabra;
                    }
                }
                else
                {
                    falloCache(id_hilo, PC);
                    indicador = palabra * cant_bytes_palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = cache3[indicador];
                        indicador = indicador + cant_bytes_palabra;
                    }
                }

            }

           return realizarOperacion(instruccion, id_hilo, PC);

            
        }



        // realizar operaciones
        public static bool realizarOperacion(int[] instruccion, int procesador, int PC)
        {
            //realizar operaciones
            miBarrerita.SignalAndWait();
            //carga los valores de la instruccion en variables para poder hacer uso y modificar estas con mas facilidad

            int codigo = instruccion[0];
            int primerRegistro = instruccion[1];
            int segundoRegistro = instruccion[2];
            int ultimaParte= instruccion[3];

            //Dice cual es el resultado de la operacion
            int resultado = 0;

            //Dice en que registro debemos guardar el resultado de la operacion
            int guardarEn=0;
            bool resultadoFinal = true;

            switch (procesador)
            {
                case 1:
                    primerRegistro=procesador1[primerRegistro];
                    procesador1[pos_pc] = PC;
                    if(codigo!=8){
                        segundoRegistro=procesador1[segundoRegistro];
                    }
                    break;
                case 2:
                    primerRegistro=procesador2[primerRegistro];
                    procesador2[pos_pc] = PC;
                    if (codigo != 8)
                    {
                        segundoRegistro = procesador2[segundoRegistro];
                    }
                    break;
                //
                case 3:
                    primerRegistro=procesador3[primerRegistro];
                    procesador3[pos_pc] = PC;
                    if (codigo != 8)
                    {
                        segundoRegistro = procesador3[segundoRegistro];
                    }
                    break;
            }

/*
    Operación   Operandos     Acción       1                2           3           4
                                        Cód.Operación     Rf1      Rf2 ó Rd   Rd ó inmediato
    DADDI    RX, RY, #n     Rx<--(Ry)+n        8           Y           X           n
    DADD     RX, RY, RZ     Rx<--(Ry)+(Rz)     32          Y           Z           X
    DSUB     RX, RY, RZ     Rx <-- (Ry) - (Rz) 34          Y           Z           X
    DMUL     RX, RY, RZ     Rx <-- (Ry) * (Rz) 12          Y           Z           X
    DDIV     RX, RY, RZ     Rx <-- (Ry) / (Rz) 14          Y           Z           X
    BEQZ     RX, ETIQ     Si Rx = 0 SALTA       4          X           0           n
    BNEZ     RX, ETIQ     Si Rx <> 0 SALTA      5          X           0           n
    JAL      n R31<--PC,    PC<-- PC+n          3          0           0           n
    JR       RX             PC <-- (Rx)         2          X           0           0
    FIN                 Detiene el programa     63         0           0           0
*/
            // aca se puede ver el hilo que entra
            switch (codigo)
            {

                case 8:
                    resultado = primerRegistro + ultimaParte;
                    guardarEn = segundoRegistro;
                    break;
                case 32:
                    resultado = primerRegistro + segundoRegistro;
                    guardarEn = ultimaParte;
                    break;
                case 34:
                    resultado = primerRegistro - segundoRegistro;
                    guardarEn = ultimaParte;
                    break;
                case 12:
                    resultado = primerRegistro * segundoRegistro;
                    guardarEn = ultimaParte;
                    break;
                case 14:
                    resultado = primerRegistro / segundoRegistro;
                    guardarEn = ultimaParte;
                    break;
                case 4:
                    if(primerRegistro == 0){
                        resultado = PC + ultimaParte * 4;
                        guardarEn = pos_pc;
                    }
                    break;
                case 5:
                    if (primerRegistro != 0)
                    {
                        resultado = PC + ultimaParte * 4;
                        guardarEn = pos_pc; 
                    }
                    break;
                case 3:
                    resultado = PC + ultimaParte;
                    guardarEn = pos_pc;
                    break;
                case 2:
                    resultado = primerRegistro;
                    guardarEn = pos_pc;
                    break;
                case 63:
                    resultadoFinal = false;
                    break;
                default:
                   // resultadoFinal = false;
                    break;

            }
            //para guardar los resultados en el registro que corresponde
            switch (procesador)
            {
                case 1:
                   
                    procesador1[guardarEn] = resultado;
                    break;
                case 2:
                    procesador2[guardarEn] = resultado;
                    break;
                case 3:
                    procesador3[guardarEn] = resultado;
                    break;
            }


            return resultadoFinal;
        }





        //Se encarga de poner el bloque al que pertence la instruccion solicitada a la cache
        public static bool falloCache(int procesador, int direccion)
        {
            int bloque = direccion / 16;//calcula el bloque
            int posicion = bloque % 4;//calcula la posicion en que se debe almacenar la instruccion en la cache
            int direccionMemNoComp = bloque * 16 - 128;//calcula la direccion en que se ubica dentro de la memoria no compartida
            //para sincronizar el ciclo de reloj

            for (int i = 0; i < 16; ++i)
            {
                miBarrerita.SignalAndWait();
            }
            switch (procesador)
            {
                case 1:
                    enCache1[posicion] = bloque;
                    posicion = posicion * 4;
                    for (int i = 0; i < 16; ++i)
                    {
                        cache1[posicion + i] = procesador1[direccionMemNoComp + i];
                    }
                    Console.WriteLine("Case 1");
                    return true;
                case 2:
                    enCache2[posicion] = bloque;
                    posicion = posicion * 4;
                    for (int i = 0; i < 16; ++i)
                    {
                        cache2[posicion + i] = procesador2[direccionMemNoComp + i];
                    }
                    Console.WriteLine("Case 2");
                    return true;
                case 3:
                    enCache3[posicion] = bloque;
                    posicion = posicion * 4;
                    for (int i = 0; i < 16; ++i)
                    {
                        cache3[posicion + i] = procesador3[direccionMemNoComp + i];
                    }
                    Console.WriteLine("Case 3");
                    return true;
            }
            return false;
        }


        //En este metodo se consulta por la instruccion que se encuentra en el procesador actual 
        //(lo recibe como parametro) en la direccion que entra como parametro
        public int[] consultarCache(int procesador, int PC)
        {
            int bloque = PC / 16;
            int posicion = bloque % 4;
            int[] retorna = new int[4];
            //Revisa si la instruccion consultada se encuentra en la cache y si no la pone en la cache y devuelve lo consultado
            switch (procesador)
            {
                case 1:
                    if (!(enCache1[posicion] == bloque))
                    {
                        falloCache(procesador, PC);
                    }
                    //enCache1[posicion] = bloque;
                    posicion = posicion * 4;
                    for (int i = 0; i < 4; ++i)
                    {
                        retorna[i] = cache1[posicion + i];
                    }
                    Console.WriteLine("Case 1");
                    break;
                //return true;
                case 2:
                    if (!(enCache2[posicion] == bloque))
                    {
                        falloCache(procesador, PC);
                    }
                    //enCache1[posicion] = bloque;
                    posicion = posicion * 4;
                    for (int i = 0; i < 4; ++i)
                    {
                        retorna[i] = cache2[posicion + i];
                    }
                    Console.WriteLine("Case 2");
                    break;
                case 3:
                    if (!(enCache3[posicion] == bloque))
                    {
                        falloCache(procesador, PC);
                    }
                    //enCache1[posicion] = bloque;
                    posicion = posicion * 4;
                    for (int i = 0; i < 4; ++i)
                    {
                        retorna[i] = cache3[posicion + i];
                    }
                    break;
            }
            
            return retorna;
        }
        /// <summary>
        /// Presenta la función principal del programa (lo que hace el procesador).
        /// </summary>
        public static void funcionPrincipal()
        {
            // leer archivos
            // iniciar hilos
            // barrera de sincronizacion
            // nhay que definir como determinar que hilo esta corriendo
            string managedThreadId = Thread.CurrentThread.Name;
            Console.WriteLine("ManagedThreadIdzz = " + managedThreadId);
            if (Thread.CurrentThread.IsAlive == true && Thread.CurrentThread.Name.Equals("1") == true)
            {
                if (contextoProcesador1.Count != 0)//Necesario porque si intenta desencolar algo y la cola esta vacia se cae
                {
                    Console.Write("aca");
                    //Funciones del procesador 1.
                    procesador1 = (int[])contextoProcesador1.Dequeue();
                    contadorProcesador1 = 0;
                    if (procesador1[pos_tiempo_inicial] == 0)
                    {
                        procesador1[pos_tiempo_inicial] = reloj;
                    }
                }
                {//Este solo se va a usar cuando se lee solo un hilillo
                    Thread.CurrentThread.Abort();
                    miBarrerita.RemoveParticipant();
                }


            }
            else if (Thread.CurrentThread.IsAlive == true && Thread.CurrentThread.Name.Equals("2") == true)
            {
                if (contextoProcesador2.Count != 0)
                {
                    //Funciones del procesador 2.
                    procesador2 = (int[])contextoProcesador2.Dequeue();
                    //Esto era lo que habiamos hablado de que cada vez que se saca un hilillo de la cola, lo ibamos a poner en 0 con el fin de que nos ayude con lo del quantum
                    //Si alguien cree que debe ir en otro lugar sientanse libres de cambiarlo
                    contadorProcesador2 = 0;
                    if (procesador2[pos_tiempo_inicial] == 0)
                    {
                        procesador2[pos_tiempo_inicial] = reloj;
                    }
                }
                else
                {//Este solo se va a usar cuando se lee solo un hilillo
                    Thread.CurrentThread.Abort();
                    miBarrerita.RemoveParticipant();
                }
            }
            else if (Thread.CurrentThread.IsAlive == true && Thread.CurrentThread.Name.Equals("3") == true)
            {
                //Funciones del procesador 3.
                if (contextoProcesador3.Count != 0)
                {
                    procesador3 = (int[])contextoProcesador3.Dequeue();
                    contadorProcesador3 = 0;
                    if (procesador3[pos_tiempo_inicial] == 0)
                    {
                        procesador3[pos_tiempo_inicial] = reloj;
                    }
                }
                else
                {//Este solo se va a usar cuando se lee solo uno o dos hilillo
                    Thread.CurrentThread.Abort();
                    miBarrerita.RemoveParticipant();
                }
            }
            else
            {
                hiloPrincipal();
                //Funciones del procesador principal.
            }

            
            //miBarrerita.SignalAndWait();
        }

        public static void hiloPrincipal()
        {
            while (hilosCorriendo>1)
            {
                //sincronizacion de los ciclos de reloj con barreras y sumandole 1 al reloj
                miBarrerita.SignalAndWait();
                reloj = reloj + 1;


              //Esto era lo que habiamos hablado de ir viendo y controlar mejor lo que le queda de quantum a 
              //cada procesador. Hay que recordar ponerlo en 0 cada vez que sacamos un hilillo de la cola
              //Si a alguien le parece que va en otro lado solo lo quita y lo pone donde crea que se debe poner.
                contadorProcesador1 = contadorProcesador1 + 1;
                contadorProcesador2 = contadorProcesador2 + 1;
                contadorProcesador3 = contadorProcesador3 + 1;
            }

            //Esto es lo que debemos imprimir al final en los resultados
            //Esto no estoy segura si va aqui asi que si creen que queda mejor en otro lado, cambienlo mejor
            //DataTable dt = resultadosHilillos();

        }

        public static DataTable resultadosHilillos()
        {
            DataTable dt = new DataTable();
            //DataTable req = new DataTable();
            //DataTable dt = new DataTable();

            dt.Columns.Add("Id");
            for (int i = 0; i < 32; ++i)
            {
                string s = "R";
                s = s + i.ToString();
                dt.Columns.Add(s);
            }
            dt.Columns.Add("Cant. de Ciclos");
            dt.Columns.Add("T inicial");
            dt.Columns.Add("T final");
            dt.Columns.Add("Procesador en que corrío");

            while (terminadosProcesador1.Count != 0)
            {
                int[] cont = (int[])terminadosProcesador1.Dequeue();
                Object[] datos = new Object[38];
                datos[0] = cont[37];
                for (int j = 0; j < 36; ++j)
                {
                    datos[1 + j] = cont[j];
                }
                datos[37] = 1;
                dt.Rows.Add(datos);
            }
            while (terminadosProcesador2.Count != 0)
            {
                int[] cont = (int[])terminadosProcesador2.Dequeue();
                Object[] datos = new Object[38];
                datos[0] = cont[37];
                for (int j = 0; j < 36; ++j)
                {
                    datos[1 + j] = cont[j];
                }
                datos[37] = 2;
                dt.Rows.Add(datos);
            }
            while (terminadosProcesador2.Count != 0)
            {
                int[] cont = (int[])terminadosProcesador1.Dequeue();
                Object[] datos = new Object[38];
                datos[0] = cont[37];
                for (int j = 0; j < 36; ++j)
                {
                    datos[1 + j] = cont[j];
                }
                datos[37] = 3;
                dt.Rows.Add(datos);
            }
            return dt;
        }

        public static void hiloPrincipal()
        {
            while (hilosCorriendo>1)
            {
                miBarrerita.SignalAndWait();
            }
            DataTable dt = new DataTable();
            DataTable req = new DataTable();
            //DataTable dt = new DataTable();

            dt.Columns.Add("Id");
            for(int i = 0; i < 32; ++i)
            {
                string s = "R";
                s = s + i.ToString();
                dt.Columns.Add(s);
            }
            dt.Columns.Add("Cant. de Ciclos");
            dt.Columns.Add("T inicial");
            dt.Columns.Add("T final");
            dt.Columns.Add("Procesador en que corrío");

            Object[] datos = new Object[38];
            /*
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    datos[0] = dr[0].ToString();
                    datos[1] = dr[1];
                    req.Rows.Add(datos);

                }
            }

            else
            {

                Object[] datos1 = new Object[2];
                datos1[0] = "-";
                datos1[1] = "-";
                req.Rows.Add(datos);
            }*/
        }

        /// <summary>
        /// Recibe un integer "a" y coloca el valor en la variable hilillos.
        /// </summary>
        public void setHilillos(int a)
        {
            hilillos = a;
        }
        /// <summary>
        /// Recibe un integer "a" y coloca el valor en la variable quantum.
        /// </summary>
        public void setQuantum(int q)
        {
            quantum = q;
        }

        /// <summary>
        /// Encargado de leer y almacenar las instrucciones a ejecutar en memoria.
        /// </summary>
        public void leeArchivos()
        {
            //Obtiene como dirección: ...\bin\Debug\
            string directorio_raiz = AppDomain.CurrentDomain.BaseDirectory;
            //Almacena la ruta en donde se encuentran los archivos a leer.
            string directorio_archivo;
            
            string line;
            //Índice para conocer en cuál memoria del procesador se introducirá el hilillo.
            int contador = 1;
            //Se utilizan para conocer la última posición de memoria en donde se agregaron datos.
            int index_memoria1 = 0;
            int index_memoria2 = 0;
            int index_memoria3 = 0;
            //Almacena temporalmente el número leído
            string temporal = "";
            //Recorre cada linea del archivo
            int a = 0;
            //Bandera utilizada para conocer si es el inicio de un código en memoria (PC).
            bool es_pc = true;
            //El índice i será utilizado para el nombre de los archivos.
            for (int i = 0; i< hilillos; ++i)
            {
                //Ruta del archivo que será leído.
                directorio_archivo = directorio_raiz + (i+1) + ".txt";


                //Procede a leer el archivo linea a linea
                System.IO.StreamReader file =
                    new System.IO.StreamReader(@directorio_archivo);
                while ((line = file.ReadLine()) != null)
                {
                    line = line + '\n';
                    if (contador == 1)
                    {
                        while (line[a] != '\n')
                        {
                            if(line[a] != ' ')
                            { 
                                //Guarda el número en cuestión en una string
                                while(line[a] != ' ')
                                {
                                    if (line[a] != '\n')
                                    {
                                        temporal = temporal + line[a];
                                        ++a;
                                    }
                                    else
                                        line = line.Substring(0, line.Length - 1) + " ";
                                }
                                //Agrega a memoria la primera instrucción
                                memNoComp1[index_memoria1] = Int32.Parse(temporal);
                                //Si es la primera instrucción del inicio del programa, entonces se agrega el pc al contexto y este se encola
                                if (es_pc)
                                {

                                    contextoProcesador1.Enqueue(procesador1);
                                    es_pc = false;
                                }

                                ++index_memoria1;
                                temporal = "";
                            }
                            if (a < line.Length-1)
                                ++a;
                            else
                                line = line.Substring(0, line.Length - 1) + '\n';
                        }
                        a = 0;
                        temporal = "";

                    }
                    else if (contador == 2)
                    {
                        while (line[a] != '\n')
                        {
                            if (line[a] != ' ')
                            {
                                //Guarda el número en cuestión en una string
                                while (line[a] != ' ')
                                {
                                    if (line[a] != '\n')
                                    {
                                        temporal = temporal + line[a];
                                        ++a;
                                    }
                                    else
                                        line = line.Substring(0, line.Length - 1) + " ";
                                }
                                //Agrega a memoria la primera instrucción
                                memNoComp2[index_memoria2] = Int32.Parse(temporal);
                                //Si es la primera instrucción del inicio del programa, entonces se agrega el pc al contexto y este se encola
                                if (es_pc)
                                {
                                    procesador2[pos_pc] = index_memoria2;
                                    contextoProcesador2.Enqueue(procesador2);
                                    es_pc = false;
                                }
                                ++index_memoria2;
                                temporal = "";
                            }
                            if (a < line.Length - 1)
                                ++a;
                            else
                                line = line.Substring(0, line.Length - 1) + '\n';
                        }
                        a = 0;
                        temporal = "";

                    }
                    else
                    {
                        while (line[a] != '\n')
                        {
                            if (line[a] != ' ')
                            {
                                //Guarda el número en cuestión en una string
                                while (line[a] != ' ')
                                {
                                    if (line[a] != '\n')
                                    {
                                        temporal = temporal + line[a];
                                        ++a;
                                    }
                                    else
                                        line = line.Substring(0, line.Length - 1) + " ";
                                }
                                //Agrega a memoria la primera instrucción
                                memNoComp3[index_memoria3] = Int32.Parse(temporal);
                                //Si es la primera instrucción del inicio del programa, entonces se agrega el pc al contexto y este se encola
                                if (es_pc)
                                {
                                    procesador3[pos_pc] = index_memoria3;
                                    contextoProcesador3.Enqueue(procesador3);
                                    es_pc = false;
                                }
                                ++index_memoria3;
                                temporal = "";
                            }
                            if (a < line.Length - 1)
                                ++a;
                            else
                                line = line.Substring(0, line.Length - 1) + '\n';
                        }
                        a = 0;
                        temporal = "";
                    }
                }

                file.Close();
                es_pc = true;
                //Cambia la memoria del procesador para el siguiente hilillo.
                if (contador == 3)
                {
                    contador = 1;
                }
                else
                {
                    ++contador;
                }
                // Suspend the screen.
                System.Console.ReadLine();
                directorio_archivo = "";
            }
        }

    }
}


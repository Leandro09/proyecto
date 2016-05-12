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
        static int reloj = 0;
        static int hilosCorriendo = 4;
        //Almacena la cantidad de hilillos que se correrán en sistema.
        static int hilillos = 0;
        //Almacena el quantum ingresado por el usuario.
        static int quantum = 0;

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
        static int[] procesador1 = new int[37];        //En la posición 32 se encuentra el PC
        static int[] procesador2 = new int[37];
        static int[] procesador3 = new int[37];


        /// colas para los contextos de los hilillos
        static Queue contextoProcesador1 = new Queue();
        static Queue contextoProcesador2 = new Queue();
        static Queue contextoProcesador3 = new Queue();

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
            proceso_1.Start();
            proceso_2.Start();
            proceso_3.Start();

            funcionPrincipal();

            //Agregar método para desplegar resultados.
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
        public static void leerInstruccion(int id_hilo)
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

            realizarOperacion(instruccion, id_hilo, PC);

            
        }



        // realizar operaciones
        public static void realizarOperacion(int[] instruccion, int procesador, int PC)
        {
            //realizar operaciones
            miBarrerita.SignalAndWait();
            int codigo = instruccion[0];
            int primerRegistro = instruccion[1];
            int segundoRegistro = instruccion[2];
            int ultimaParte= instruccion[3];
            int resultado = 0;
            int guardarEn=0;

            switch (procesador)
            {
                case 1:
                    primerRegistro=procesador1[primerRegistro];
                    procesador1[pos_pc] = PC;
                    if(codigo!=8){
                        segundoRegistro=procesador1[segundoRegistro];
                    }
                    break;
                //return true;
                case 2:
                    primerRegistro=procesador2[primerRegistro];
                    procesador2[pos_pc] = PC;
                    if (codigo != 8)
                    {
                        segundoRegistro = procesador2[segundoRegistro];
                    }
                    break;
                case 3:
                    primerRegistro=procesador3[primerRegistro];
                    procesador3[pos_pc] = PC;
                    if (codigo != 8)
                    {
                        segundoRegistro = procesador3[segundoRegistro];
                    }
                    break;
            }


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
                default:
                    break;

            }

            switch (procesador)
            {
                case 1:
                    procesador1[guardarEn] = resultado;
                    break;
                case 2:
                    procesador1[guardarEn] = resultado;
                    break;
                case 3:
                    procesador1[guardarEn] = resultado;
                    break;
            }

            

        }





        public static bool falloCache(int procesador, int direccion)
        {
            int bloque = direccion / 16;
            int posicion = bloque % 4;
            int direccionMemNoComp = bloque * 16 - 128;
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



        public int[] consultarCache(int procesador, int PC)
        {
            int bloque = PC / 16;
            int posicion = bloque % 4;
            int[] retorna = new int[4];

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

            if (Thread.CurrentThread.IsAlive == true && Thread.CurrentThread.Name.Equals("1")==true)
            {
                Console.Write("aca");
                //Funciones del procesador 1.
                int[] contexto = (int[])contextoProcesador1.Dequeue();
                for (int i = 0; i < cant_campos; ++i )
                {
                    procesador1[i] = contexto[i];
                }
                leerInstruccion(1);
                for (int i = 0; i < cant_campos; ++i )
                {
                    Console.Write(procesador1[i]);
                }
            }
            else if (Thread.CurrentThread.IsAlive == true && Thread.CurrentThread.Name.Equals("2") == true)
            {
                //Funciones del procesador 2.
            }
            else if (Thread.CurrentThread.IsAlive == true && Thread.CurrentThread.Name.Equals("3") == true)
            {
                //Funciones del procesador 3.
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
                                    procesador1[pos_pc] = index_memoria1;
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
                    contador = 1;
                else
                    ++contador;
                // Suspend the screen.
                System.Console.ReadLine();
                directorio_archivo = "";
            }
        }

    }
}


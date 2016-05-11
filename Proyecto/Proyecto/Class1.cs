using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Proyecto
{
    class tarea
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        /// 

        int cant_bytes_palabra = 4;
        int cant_bytes_bloque = 16;
        int pos_pc = 32;
        int cant_campos = 37;
        int cant_cache = 64;
        int cant_encache = 4;
        int cant_memComp = 128;
        int cant_memNoComp = 256;
        int limite = 128;
        //Almacena la cantidad de hilillos que se correrán en sistema.
        int hilillos = 0;
        //Almacena el quantum ingresado por el usuario.
        int quantum = 0;

        int[] cache1 = new int[64];
        //Cual es el bloque que esta en la cache en esa posicion
        int[] enCache1 = new int[4];
        int[] cache2 = new int[64];
        //Cual es el bloque que esta en la cache en esa posicion
        int[] enCache2 = new int[4];
        int[] cache3 = new int[64];
        //Cual es el bloque que esta en la cache en esa posicion
        int[] enCache3 = new int[4];
        int[] memComp = new int[128];
        int[] memNoComp1 = new int[256];
        int[] memNoComp2 = new int[256];
        int[] memNoComp3 = new int[256];

        /// estructuras para observar los recursos del procesador
        int[] procesador1 = new int[37];
        int[] procesador2 = new int[37];
        int[] procesador3 = new int[37];


        /// colas para los contextos de los hilillos
        Queue contextoProcesador1 = new Queue();
        Queue contextoProcesador2 = new Queue();
        Queue contextoProcesador3 = new Queue();

        Queue terminadosProcesador1 = new Queue();
        Queue terminadosProcesador2 = new Queue();
        Queue terminadosProcesador3 = new Queue();

        //Delegados de cada proceso 
        static ThreadStart delegado_proceso_1 = new ThreadStart(funcionPrincipal);
        static ThreadStart delegado_proceso_2 = new ThreadStart(funcionPrincipal);
        static ThreadStart delegado_proceso_3 = new ThreadStart(funcionPrincipal);
        //Los hilos que simularán los procesos en cuestión 
        Thread proceso_1 = new Thread(delegado_proceso_1);
        Thread proceso_2 = new Thread(delegado_proceso_2);
        Thread proceso_3 = new Thread(delegado_proceso_3);


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

            int[] prueba = new int[cant_campos];
            for (int i = 0; i < cant_campos; ++i)
            {
                prueba[i] = 0;
            }
            prueba[pos_pc] = 128;
            contextoProcesador1.Enqueue(prueba);
            proceso_1.Start();
            proceso_2.Start();
            proceso_3.Start();

            funcionPrincipal();
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
        public int[] leerInstruccion(int id_hilo)
        {
            int indicador = 0;
            inicializarEstructuras();
            int [] instruccion = new int[cant_bytes_palabra]; 
            int[] contexto = (int[])contextoProcesador1.Peek();


            // switch para leer pc de procesador 

            int PC = contexto[pos_pc];
            contexto[pos_pc] = PC + 4;
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

            return contexto;
        }



        // realizar operaciones
        public void realizarOperacion(int[] instruccion, int procesador, int PC)
        {
            //realizar operaciones

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

            

        

        public bool falloCache(int procesador, int direccion)
        {
            int bloque = direccion / 16;
            int posicion = bloque % 4;
            int direccionMemNoComp = bloque * 16 - 128;

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
        /// Presenta la función principal del programa.
        /// </summary>
        public static void funcionPrincipal()
        {
            // leer archivos
            // iniciar hilos
            // barrera de sincronizacion
            // nhay que definir como determinar que hilo esta corriendo
            int managedThreadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("ManagedThreadId = " + managedThreadId);



        }
        /// <summary>
        /// Recibe un integer "a" y coloca el valor en la variable hilillos.
        /// </summary>
        public void setHilillos(int a)
        {
            hilillos = a;
            
        }

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


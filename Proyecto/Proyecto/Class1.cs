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
        /// Punto de entrada principal para la aplicación.
        /// 
        //Enteros que son para recordar tanto posiciones, como tamaños, de modo que si se modifica alguno de estos valores
        //sera mas sencillo modificar el codigo
        static int cant_bytes_palabra = 4;
        static int cant_bytes_bloque = 16;
        static int pos_pc = 32;
        static int cant_campos = 39;
        static int cant_cache_inst = 64;
        static int cant_encache_inst = 4;
        static int cant_memComp = 128;
        static int cant_memNoComp = 256;
        static int limite = 128;
        static int hilosCorriendo = 4;
        static int pos_tiempo_inicial = 34;
        static int pos_tiempo_final = 35;       
        static int hilillos = 0;
        static int pos_nombre_hilillos = 36;
        static int pos_nombre_procesador = 37;
        static int pos_rl = 38;
        static int cant_cache_datos = 16;
        static int cant_campos_directorio = 8;
        static int cant_campos_ubicacion_directorio = 24;


        //Varibles indispensables para la implementacion de la simulacion debido a que son muy significativas
        //Varible que almacena el valor del reloj
        static int reloj = 1;
        
        //Almacena el quantum ingresado por el usuario.
        static int quantum = 0;

        //Para llevar un mejor control del quantum
        //Dice cuantos ciclos de reloj lleva corriendo el hilillo que esta en el procesador actualmente
        static int contadorProcesador1 = 0;
        static int contadorProcesador2 = 0;
        static int contadorProcesador3 = 0;

        //Banderas de LL activo de cada procesador
        //1 es el proc 1, 2 es el proc 2 y 0 es el proc 3
        static bool[] LLactivo1 = new bool[3];

        //Caché de instrucciones de los procesadores.
        static int[] cache_inst1 = new int[64];
        static int[] cache_inst2 = new int[64];
        static int[] cache_inst3 = new int[64];

        //Cual es el bloque que esta en la caché de instrucciones en esa posicion.
        static int[] encache_inst1 = new int[4];
        static int[] encache_inst2 = new int[4];
        static int[] encache_inst3 = new int[4];

        //Caché de instrucciones de los procesadores.
        static int[] cache_datos1 = new int[16];
        static int[] cache_datos2 = new int[16];
        static int[] cache_datos3 = new int[16];
        //Cual es el bloque que esta en la caché de instrucciones en esa posicion.
        static int[] encache_datos1 = new int[4];
        static int[] encache_datos2 = new int[4];
        static int[] encache_datos3 = new int[4];
        //Almacena los estados de los directorios
        static char[] estadoCache1 = new char[4];
        static char[] estadoCache2 = new char[4];
        static char[] estadoCache3 = new char[4]; 

        //Memoria compartida
        static int[] memComp1 = new int[128];
        static int[] memComp2 = new int[128];
        static int[] memComp3 = new int[128];

        //Memoria no compartida de cada procesador
        static int[] memNoComp1 = new int[256];
        static int[] memNoComp2 = new int[256];
        static int[] memNoComp3 = new int[256];

        /// estructuras para observar los recursos del procesador
        static int[] procesador1 = new int[39];        //En la posición 32 se encuentra el PC
        static int[] procesador2 = new int[39];
        static int[] procesador3 = new int[39];         //En la posicion 37 va el nombre del HILILLO al que pertenece este contexto



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

        //Almacena los estados de los directorios
        static char[] estadoDir1 = new char[8];
        static char[] estadoDir2 = new char[8];
        static char[] estadoDir3 = new char[8]; 
 
        //Contiene la ubicación de los bloques en las caché
        static bool[] ubicacionDir1 = new bool[24];
        static bool[] ubicacionDir2 = new bool[24];
        static bool[] ubicacionDir3 = new bool[24];



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

        //Se encarga de finalizar la ejecucion de un hilillo
        public static void finalizarEjecucion(int id_hilo)
        {
            if (id_hilo == 1)
            {
                if (contextoProcesador1.Count == 0)
                {
                    //finalizar el hilo 1
                    //si hay un fin, este sea o no el ultimo hilillo ejecutandose en este procesador
                    //debemos guardar sus resultados

                    miBarrerita.RemoveParticipant();//Un participante menos a esperar en la barrera
                    procesador1[35] = reloj;//tiempo en que finalizo el hilillo
                    terminadosProcesador1.Enqueue(procesador1);//Cola de terminados
                    procesador1[pos_pc] = 0;//Limpiamos el valor del pc en el procesador
                    --hilosCorriendo;//Hay un hilo menos que esta corriendo
                    Thread.CurrentThread.Abort();//Abortamos el hilo actual

                }
                else
                {

                    procesador1[35] = reloj;//tiempo en que finalizo el hilillo
                    int[] contenedor = new int[cant_campos];
                    //Esto puede parecer un poco ineficiente pero a veces tiraba error al no hacerlo
                    //Copia los valores del procesador en un nuevo array
                    for (int i = 0; i < cant_campos; ++i)
                    {
                        contenedor[i] = procesador1[i];
                        procesador1[i] = 0;
                    }

                    
                    terminadosProcesador1.Enqueue(contenedor);//Cola de terminados

                }
            }
            else if (id_hilo == 2)
            {
                if (contextoProcesador2.Count == 0)
                {
                    //finalizar el hilo 2
                    //si hay un fin, este sea o no el ultimo hilillo ejecutandose en este procesador
                    //debemos guardar sus resultados
                    
                    miBarrerita.RemoveParticipant();//Un participante menos a esperar en la barrera
                    procesador2[35] = reloj;//tiempo en que finalizo el hilillo
                    terminadosProcesador2.Enqueue(procesador2);//Cola de terminados

                    --hilosCorriendo;//Hay un hilo menos que esta corriendo
                    Thread.CurrentThread.Abort();//Abortamos el hilo actual
                }
                else
                {
                    procesador2[35] = reloj;//tiempo en que finalizo el hilillo
                    int[] contenedor = new int[cant_campos];
                    //Esto puede parecer un poco ineficiente pero a veces tiraba error al no hacerlo
                    //Copia los valores del procesador en un nuevo array
                    for (int i = 0; i < cant_campos; ++i)
                    {
                        contenedor[i] = procesador2[i];
                        procesador2[i] = 0;
                    }


                    terminadosProcesador2.Enqueue(contenedor);//Cola de terminados
                }
            }
            else if (id_hilo == 3)
            {
                {
                    if (contextoProcesador3.Count == 0)
                    {

                        miBarrerita.RemoveParticipant();//Un participante menos a esperar en la barrera
                        procesador3[35] = reloj;//tiempo en que finalizo el hilillo
                        terminadosProcesador3.Enqueue(procesador3);//Cola de terminados

                        --hilosCorriendo;//Hay un hilo menos que esta corriendo
                        Thread.CurrentThread.Abort();//Abortamos el hilo actual
                    }
                    else
                    {
                        procesador3[35] = reloj;//tiempo en que finalizo el hilillo
                        int[] contenedor = new int[cant_campos];
                        //Esto puede parecer un poco ineficiente pero a veces tiraba error al no hacerlo
                        //Copia los valores del procesador en un nuevo array
                        for (int i = 0; i < cant_campos; ++i)
                        {
                            contenedor[i] = procesador3[i];
                            procesador3[i] = 0;
                        }


                        terminadosProcesador3.Enqueue(contenedor);//Cola de terminados
                    }
                }
            }
        }

        //Verifica si el quantum finalizó. Recibe el id del procesador que lo llamó.
        public static void cambiar_hilillo_quantum(int num_procesador)
        {
            switch (num_procesador)
            {
                case 1:
                    if (contadorProcesador1 >= quantum)
                    {
                        int[] contenedor = new int[cant_campos];
                        for (int i = 0; i < cant_campos; ++i)
                        {
                            contenedor[i]=procesador1[i];
                            procesador1[i] = 0;
                        }

                        contextoProcesador1.Enqueue(contenedor);

                    }
                    break;
                case 2:
                    if (contadorProcesador2 >= quantum)
                    {
                        int[] contenedor = new int[cant_campos];
                        for (int i = 0; i < cant_campos; ++i)
                        {
                            contenedor[i] = procesador2[i];
                            procesador2[i] = 0;
                        }

                        contextoProcesador2.Enqueue(contenedor);
                    }
                    break;
                case 3:
                    if (contadorProcesador3 >= quantum)
                    {
                        int[] contenedor = new int[cant_campos];
                        for (int i = 0; i < cant_campos; ++i)
                        {
                            contenedor[i] = procesador3[i];
                            procesador3[i] = 0;
                        }

                        contextoProcesador3.Enqueue(contenedor);
                    }
                    break;
            }
        }

        //Métodos para cambiar los nombres de los hilos a 1, 2 y 3 respectivamente.
        public static void nombrarHilo1()
        {
            Thread.CurrentThread.Name = "1";
            funcionPrincipal();
        }

        public static void nombrarHilo2()
        {
            Thread.CurrentThread.Name = "2";
            funcionPrincipal();
        }

        public static void nombrarHilo3()
        {
            Thread.CurrentThread.Name = "3";
            funcionPrincipal();
        }

        //inicializar estructuras
        public void inicializarEstructuras()
        {
            for (int i = 0; i < cant_cache_inst; ++i)
            {
                cache_inst1[i] = 0;
                cache_inst2[i] = 0;
                cache_inst2[i] = 0;
            }

            for (int i = 0; i < cant_encache_inst; ++i)
            {
                encache_inst1[i] = -1;
                encache_inst2[i] = -1;
                encache_inst3[i] = -1;
            }

            for (int i = 0; i < cant_campos; ++i)
            {
                procesador1[i] = 0;
                procesador2[i] = 0;
                procesador3[i] = 0;
            }

            for (int i = 0; i < cant_cache_datos; ++i)
            {
                cache_datos1[i] = 0;
                cache_datos2[i] = 0;
                cache_datos2[i] = 0;
            }

            for (int i = 0; i < cant_encache_inst; ++i)
            {
                encache_datos1[i] = -1;
                encache_datos2[i] = -1;
                encache_datos3[i] = -1;
            }

            for (int i = 0; i < cant_memComp; ++i)
            {
                memComp1[i] = 1;
                memComp2[i] = 1;
                memComp3[i] = 1;
            }

            for (int i = 0; i < cant_campos_directorio; ++i)
            {
                   estadoDir1[i] = 'U';
                   estadoDir2[i] = 'U';
                   estadoDir3[i] = 'U';
            }

            for (int i = 0; i < cant_campos_directorio; ++i)
            {
                estadoCache1[i] = 'I';
                estadoCache2[i] = 'I';
                estadoCache3[i] = 'I';
            }

            for (int i = 0; i < cant_campos_ubicacion_directorio; ++i)
            {
                ubicacionDir1[i] = false;
                ubicacionDir2[i] = false;
                ubicacionDir3[i] = false;
            }


        }
        //int[] pertenece = new int[12];
        String[] path = new String[12];
        // metodo para realizar la lectura de las instrucciones
        // id_hilo es el numero de procesador
        public static bool leerInstruccion(int id_hilo)
        {
            int indicador = 0;
            // switch para leer pc de procesador 
            int[] instruccion = new int[cant_bytes_palabra];

            int PC = 0;

            switch (id_hilo)
            {
                case 1:
                    PC = procesador1[pos_pc];
                    procesador1[pos_pc] = PC + 4;
                    break;
                case 2:
                    PC = procesador2[pos_pc];
                    procesador2[pos_pc] = PC + 4;
                    break;
                case 3:
                    PC = procesador3[pos_pc];
                    procesador3[pos_pc] = PC + 4;
                    break;

            }

            //variables para almacenar datos importantes con respecto al PC
            int bloque = PC / cant_bytes_bloque;
            int indice = bloque % cant_bytes_palabra;
            int cantidad_bytes = PC % cant_bytes_bloque;
            int palabra = cantidad_bytes / cant_bytes_palabra;

            
            if (id_hilo == 1)
            {
                //verificar si la intruccion que señala el PC se encuentra en la cache_inst
                if (encache_inst1[indice] != -1 && bloque == encache_inst1[indice])
                {
                    indicador = indice * cant_bytes_bloque + cant_bytes_palabra * palabra;

                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = cache_inst1[indicador + i];
                    }
                }
                else // encaso de que no este, realiza fallo de cache_inst para traerla desde la memoria no compatida
                {
                    fallocache_inst(id_hilo, PC);
                    indicador = indice * cant_bytes_bloque + cant_bytes_palabra * palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = cache_inst1[indicador + i];
                    }
                }
            }
            else if (id_hilo == 2)
            {
                //verificar si la intruccion que señala el PC se encuentra en la cache_inst
                if (encache_inst2[indice] != -1 && bloque == encache_inst2[indice])
                {
                    indicador = indice * cant_bytes_bloque + cant_bytes_palabra * palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = cache_inst2[indicador + i];
                    }
                }
                else // encaso de que no este, realiza fallo de cache_inst para traerla desde la memoria no compatida
                {
                    fallocache_inst(id_hilo, PC);
                    indicador = indice * cant_bytes_bloque + cant_bytes_palabra * palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = cache_inst2[indicador + i];
                    }
                }
            }
            else
            {
                //verificar si la intruccion que señala el PC se encuentra en la cache_inst
                if (encache_inst3[indice] != -1 && bloque == encache_inst3[indice])
                {
                    indicador = indice * cant_bytes_bloque + cant_bytes_palabra * palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = cache_inst3[indicador + i];
                    }
                }
                else // encaso de que no este, realiza fallo de cache_inst para traerla desde la memoria no compatida
                {
                    fallocache_inst(id_hilo, PC);
                    indicador = indice * cant_bytes_bloque + cant_bytes_palabra * palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = cache_inst3[indicador + i];
                    }
                }

            }

            // una vez encontrada la instruccion, se proceda a realizarla
            return realizarOperacion(instruccion, id_hilo, PC + 4);
        }



        // realizar operaciones
        //Recibe un array con la instruccion, el id del procesador y el valor del PC
        public static bool realizarOperacion(int[] instruccion, int procesador, int PC)
        {
            //Para sincronizar un ciclo de reloj mas
            miBarrerita.SignalAndWait();

            //Pasamos los pedazos de la instruccion a variables diferentes para manejar con mayor facilidad
            //los registros, codigos e inmediatos
            int codigo = instruccion[0];
            int primerRegistro = instruccion[1];
            int segundoRegistro = instruccion[2];
            int ultimaParte = instruccion[3];
            int auxiliar = instruccion[1];

            //Para que el usuario pueda ver en consola cual es la instruccion que se esta ejecutando en que procesador
            Console.WriteLine("SOY INSTRUCCION " + codigo + " " + primerRegistro + " " + segundoRegistro + " " + ultimaParte + " Procesador " + procesador + " PC " + (PC - 4) + "  ");
            Console.WriteLine("");

            //Dice cual es el resultado de la operacion
            int resultado = 0;

            //Dice en que registro debemos guardar el resultado de la operacion
            int guardarEn = 0;
            bool resultadoFinal = true;

            //Para leer los valores de los registros
            switch (procesador)
            {
                case 1:
                    primerRegistro = procesador1[primerRegistro];
                    if (codigo != 8)//Si es el 8 no cambia nada porque el segundo registro del DADDI es en que registro se guardara el resultado
                    {
                        segundoRegistro = procesador1[segundoRegistro];
                    }
                    break;
                case 2:
                    primerRegistro = procesador2[primerRegistro];
                    //Si es el 8 no cambia nada porque el segundo registro del DADDI es en que registro se guardara el resultado
                    if (codigo != 8)
                    {
                        segundoRegistro = procesador2[segundoRegistro];
                    }
                    break;

                case 3:
                    primerRegistro = procesador3[primerRegistro];
                    //Si es el 8 no cambia nada porque el segundo registro del DADDI es en que registro se guardara el resultado
                    if (codigo != 8)
                    {
                        segundoRegistro = procesador3[segundoRegistro];
                    }
                    break;
            }

            //Ejecuta la instruccion de acuerdo al codigo de operacion correspondiente
            switch (codigo)
            {

                case 8: // DADDI
                    resultado = primerRegistro + ultimaParte;
                    guardarEn = segundoRegistro;
                    break;
                case 32: //DADD
                    resultado = primerRegistro + segundoRegistro;
                    guardarEn = ultimaParte;
                    break;
                case 34://DSUB
                    resultado = primerRegistro - segundoRegistro;
                    guardarEn = ultimaParte;
                    break;
                case 12: //DMUL
                    resultado = primerRegistro * segundoRegistro;
                    guardarEn = ultimaParte;
                    break;
                case 14: //DDIV
                    resultado = primerRegistro / segundoRegistro;
                    guardarEn = ultimaParte;
                    break;
                case 4: // BEQZ
                    if (primerRegistro == 0)
                    {
                        resultado = PC + ultimaParte * 4;
                        guardarEn = pos_pc;
                    }
                    break;
                case 5: //BNEZ
                    if (primerRegistro != 0)
                    {
                        resultado = PC + ultimaParte * 4;
                        guardarEn = pos_pc;
                    }
                    break;
                case 3: //JAL
                    resultado = PC + ultimaParte;
                    guardarEn = pos_pc;
                    if (procesador == 1)
                    {
                        procesador1[31] = PC;
                    }
                    else if (procesador == 2)
                    {
                        procesador2[31] = PC;
                    }
                    else
                    {
                        procesador3[31] = PC;
                    }
                    break;
                case 2: //JR
                    resultado = primerRegistro;
                    guardarEn = pos_pc;
                    break;
                case 35: //LW
                    cache_Load(0,0);
                    break;
                case 50: //LL

                    break;
                case 51: //SC

                    break;
                case 43: //SW

                    break;
                case 63: //FIN
                    resultadoFinal = false;
                    break;
                default:
                    break;

            }
            //para guardar los resultados en los registros del procesador que corresponde
            if (codigo != 63)
            {
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
            }
            return resultadoFinal;
        }

        //Se encarga de verificar si el bloque que se va a leer ya esta en la cache y si no esta lo sube.
        public static bool cache_Load(int procesador, int direccion)
        {
            int bloque = direccion / 16;
            int posicionCache = bloque % 4;
            int posicionDir = bloque % 8;
            bool r = false;
            while (!r)
            {
                r = true;
                while (true)//trylock de mi cache
                {
                    miBarrerita.SignalAndWait();
                }
                switch (procesador)
                {
                    case 1:
                        if ((encache_datos1[posicionCache] == bloque) && ((estadoCache1[posicionCache] == 'C') || (estadoCache1[posicionCache] == 'M')))
                        {
                            return true;
                        }
                        else
                        {
                            if (estadoDir1[posicionCache] == 'M')
                            {
                                r = escribirBloqueEnMem(bloque, 1, posicionCache, true, true);
                                if (r)
                                {
                                    if (true)//trylock directorio del bloque solicitado
                                    {
                                        if ((bloque / 8)== 0)
                                        {
                                            for (int i = 0; i < 2; ++i)
                                            {
                                                miBarrerita.SignalAndWait();
                                            }
                                        }
                                        else
                                        {
                                            for (int i = 0; i < 4; ++i)
                                            {
                                                miBarrerita.SignalAndWait();
                                            }
                                        }
                                        if ((estadoDir1[posicionDir]) == 'M') 
                                        {
                                            r = escribirBloqueEnMem(bloque, 1, posicionCache, true, false);
                                            if (r) 
                                            { 
                                                //Pone el bloque en el directorio
                                                //bloque=compartido
                                                //Estoy usando bloque en C
                                                //Subir Bloque a mi cache
                                                //Soltar dir
                                                //Soltar cache 
                                            } 
                                            else 
                                            { 
                                                //suelta directorio
                                                //suelta cache
                                            }
                                        }
                                        else 
                                        {
                                            //Pone el bloque en el directorio
                                            //bloque=compartido
                                            //Estoy usando bloque en C
                                            //Subir Bloque a mi cache
                                            //Soltar dir
                                            //Soltar cache 
                                            if ((bloque / 8) == 0)
                                            {
                                                for (int i = 0; i < 16; ++i)
                                                {
                                                    miBarrerita.SignalAndWait();
                                                }
                                            }
                                            else
                                            {
                                                for (int i = 0; i < 20; ++i)
                                                {
                                                    miBarrerita.SignalAndWait();
                                                }
                                            }
                                        }
                                    }
                                    else 
                                    {
                                        r = false;
                                        //suelta cache
                                    }
                                }
                            }
                        }
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                }
            }
            return false;
        }

        public static bool escribirBloqueEnMem(int bloque, int numcache,int posicion, bool esLoad, bool reemplazo)
        {
            return false;
        }

        public static bool reemplazarBloqueCompartido(int procesador, int direccion)
        {
            return false;
        }






        //Se encarga de poner el bloque al que pertence la instruccion solicitada a la cache_inst
        public static bool fallocache_inst(int procesador, int direccion)
        {
            //Una instruccion tiene 1 palabra de MIPS y 4 ints de como lo estamos trabajando
            string name = Thread.CurrentThread.Name;
            int bloque = direccion / 16;//calcula el bloque
            int posicion = bloque % 4;//calcula la posicion en que se debe almacenar la instruccion en la cache_inst
            int direccionMemNoComp = bloque * 16 - 128;//calcula la direccion en que se ubica dentro de la memoria no compartida
            
            //para sincronizar el ciclo de reloj
            for (int i = 0; i < 16; ++i)
            {
                miBarrerita.SignalAndWait();
            }

            switch (procesador)
            {
                case 1:
                    //pone el valor del bloque en el array del indice
                    encache_inst1[posicion] = bloque;
                    posicion = posicion * 16;
                    for (int i = 0; i < 16; ++i)
                    {//pasa las 4 palabras MIPS a la cache_inst 
                        cache_inst1[posicion + i] = memNoComp1[direccionMemNoComp + i];
                    }
                    return true;
                case 2:
                    //pone el valor del bloque en el array del indice
                    encache_inst2[posicion] = bloque;
                    posicion = posicion * 16;
                    for (int i = 0; i < 16; ++i)
                    {
                        //pasa las 4 palabras MIPS a la cache_inst
                        cache_inst2[posicion + i] = memNoComp2[direccionMemNoComp + i];
                    }
                    return true;
                case 3:
                    //pone el valor del bloque en el array del indice
                    encache_inst3[posicion] = bloque;
                    posicion = posicion * 16;
                    for (int i = 0; i < 16; ++i)
                    {
                        //pasa las 4 palabras MIPS a la cache_inst
                        cache_inst3[posicion + i] = memNoComp3[direccionMemNoComp + i];
                    }
                    return true;
            }
            return false;
        }


        //En este metodo se consulta por la instruccion que se encuentra en el procesador actual 
        //(lo recibe como parametro) en la direccion que entra como parametro
        public int[] consultarcache_inst(int procesador, int PC)
        {
            int bloque = PC / 16;
            int posicion = bloque % 4;
            int[] retorna = new int[4];
            //Revisa si la instruccion consultada se encuentra en la cache_inst y si no la pone en la cache_inst y devuelve lo consultado
            switch (procesador)
            {
                case 1:
                    if (!(encache_inst1[posicion] == bloque))
                    {
                        fallocache_inst(procesador, PC);
                    }
                    posicion = posicion * 4;
                    for (int i = 0; i < 4; ++i)
                    {
                        retorna[i] = cache_inst1[posicion + i];
                    }
                    break;
                case 2:
                    if (!(encache_inst2[posicion] == bloque))
                    {
                        fallocache_inst(procesador, PC);
                    }
                    posicion = posicion * 4;
                    for (int i = 0; i < 4; ++i)
                    {
                        retorna[i] = cache_inst2[posicion + i];
                    }
                    break;
                case 3:
                    if (!(encache_inst3[posicion] == bloque))
                    {
                        fallocache_inst(procesador, PC);
                    }
                    posicion = posicion * 4;
                    for (int i = 0; i < 4; ++i)
                    {
                        retorna[i] = cache_inst3[posicion + i];
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
            string managedThreadId = Thread.CurrentThread.Name;

            if (Thread.CurrentThread.IsAlive == true && Thread.CurrentThread.Name.Equals("1") == true)
            {

                while (contextoProcesador1.Count != 0)//Necesario porque si intenta desencolar algo y la cola esta vacia se cae
                {
                    //Funciones del procesador 1.
                    bool indicador = true;
                    procesador1 = (int[])contextoProcesador1.Dequeue();
                    procesador1[38] = -1;
                    int p1 = procesador1[pos_pc];
                    int id1 = procesador1[1];
                    contadorProcesador1 = 0;
                    if (procesador1[pos_tiempo_inicial] == 0)
                    {
                        procesador1[pos_tiempo_inicial] = reloj;
                    }
                    while (contadorProcesador1 < quantum && indicador == true)
                    {
                        indicador = leerInstruccion(1);
                    }
                    int p = procesador1[pos_pc];
                    int id = procesador1[1];

                    if (!indicador)
                    {
                        finalizarEjecucion(1);
                    }
                    else
                    {
                        cambiar_hilillo_quantum(1);
                    }
                }
               
                miBarrerita.RemoveParticipant();
                --hilosCorriendo;//Hay un hilo menos que esta corriendo
                Thread.CurrentThread.Abort();//Abortamos el hilo actual
            }
            else if (Thread.CurrentThread.IsAlive == true && Thread.CurrentThread.Name.Equals("2") == true)
            {

                while (contextoProcesador2.Count != 0)
                {
                    bool indicador = true;
                    procesador2 = (int[])contextoProcesador2.Dequeue();
                    procesador2[38] = -1;
                    int p1 = procesador2[pos_pc];
                    int id1 = procesador2[1];
                    contadorProcesador2 = 0;
                    if (procesador2[pos_tiempo_inicial] == 0)
                    {
                        procesador2[pos_tiempo_inicial] = reloj;
                    }
                    while (contadorProcesador2 < quantum && indicador == true)
                    {
                        indicador = leerInstruccion(2);
                    }
                    int p = procesador2[pos_pc];
                    int id = procesador2[1];
                    if (!indicador)
                    {
                        finalizarEjecucion(2);
                    }
                    else
                    {
                        cambiar_hilillo_quantum(2);
                    }
                }
                
                miBarrerita.RemoveParticipant();
                --hilosCorriendo;//Hay un hilo menos que esta corriendo
                Thread.CurrentThread.Abort();//Abortamos el hilo actual
            }
            else if (Thread.CurrentThread.IsAlive == true && Thread.CurrentThread.Name.Equals("3") == true)
            {

                //Funciones del procesador 3.
                while (contextoProcesador3.Count != 0)
                {
                    bool indicador = true;
                    procesador3 = (int[])contextoProcesador3.Dequeue();
                    procesador3[38] = -1;
                    int p1 = procesador3[pos_pc];
                    int id1 = procesador3[1];
                    contadorProcesador3 = 0;
                    if (procesador3[pos_tiempo_inicial] == 0)
                    {
                        procesador3[pos_tiempo_inicial] = reloj;
                    }
                    while (contadorProcesador3 < quantum && indicador == true)
                    {
                        indicador = leerInstruccion(3);
                    }
                    int p = procesador3[pos_pc];
                    int id = procesador3[1];
                    if (!indicador)
                    {
                        finalizarEjecucion(3);
                    }
                    else
                    {
                        cambiar_hilillo_quantum(3);
                    }
                }
                
                miBarrerita.RemoveParticipant();
                --hilosCorriendo;//Hay un hilo menos que esta corriendo
                Thread.CurrentThread.Abort();//Abortamos el hilo actual
                miBarrerita.RemoveParticipant();
            }
            else
            {
                hiloPrincipal();// barrera de sincronizacion
                //Funciones del procesador principal.
            }

        }

        //Este metodo se encarga de meter los resultados de los hilillos en un DataTable
        public DataTable resultadosHilillos()
        {
            DataTable dt = new DataTable();
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
                Object[] datos = new Object[37];
                datos[0] = cont[pos_nombre_hilillos];
                cont[33] = cont[35] - cont[34];
                for (int j = 0; j < 32; ++j)
                {
                    datos[1 + j] = cont[j];
                }
                for (int j = 33; j < 36; ++j)
                {
                    datos[j] = cont[j];
                }
                datos[36] = 1;
                dt.Rows.Add(datos);
            }
            while (terminadosProcesador2.Count != 0)
            {
                int[] cont = (int[])terminadosProcesador2.Dequeue();
                Object[] datos = new Object[37];
                datos[0] = cont[pos_nombre_hilillos];
                cont[33] = cont[35] - cont[34];
                for (int j = 0; j < 32; ++j)
                {
                    datos[1 + j] = cont[j];
                }
                for (int j = 33; j < 36; ++j)
                {
                    datos[ j] = cont[j];
                }
                datos[36] = 2;
                dt.Rows.Add(datos);
            }
            while (terminadosProcesador3.Count != 0)
            {
                int[] cont = (int[])terminadosProcesador3.Dequeue();
                Object[] datos = new Object[37];
                datos[0] = cont[pos_nombre_hilillos];
                cont[33] = cont[35] - cont[34];
                for (int j = 0; j < 32; ++j)
                {
                    datos[1 + j] = cont[j];
                }
                for (int j = 33; j < 36; ++j)
                {
                    datos[j] = cont[j];
                }
                datos[36] = 3;
                dt.Rows.Add(datos);
            }
            return dt;
        }
        //Se encarga de hacer la barrera de sincronizacion
        public static void hiloPrincipal()
        {
            while (hilosCorriendo > 1)
            {
                //sincronizacion de los ciclos de reloj con barreras y sumandole 1 al reloj
                miBarrerita.SignalAndWait();
                reloj = reloj + 1;


                //Para ver y controlar mejor lo que le queda de quantum a cada procesador. 
                contadorProcesador1 = contadorProcesador1 + 1;
                contadorProcesador2 = contadorProcesador2 + 1;
                contadorProcesador3 = contadorProcesador3 + 1;
            }
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
            //Variable que controla cuál hilillo está leyendo.
            int nombre_hilillo = 1;
            //El índice i será utilizado para el nombre de los archivos.
            for (int i = 0; i < hilillos; ++i)
            {
                //Ruta del archivo que será leído.
                directorio_archivo = directorio_raiz + (i + 1) + ".txt";


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
                                memNoComp1[index_memoria1] = Int32.Parse(temporal);
                                //Si es la primera instrucción del inicio del programa, entonces se agrega el pc al contexto y este se encola
                                if (es_pc)
                                {
                                    int[] contenedor = new int[cant_campos];
                                    for (int z = 0; z < cant_campos; ++z)
                                        contenedor[z] = 0;
                                    contenedor[pos_nombre_hilillos] = nombre_hilillo;
                                    ++nombre_hilillo;
                                    //Para conocer en cuál procesador está corriendo.
                                    contenedor[pos_nombre_procesador] = 1;
                                    contenedor[pos_pc] = (index_memoria1) + 128;
                                    contenedor[pos_tiempo_inicial] = 0;
                                    contextoProcesador1.Enqueue(contenedor);
                                    es_pc = false;
                                }

                                ++index_memoria1;
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
                                    int[] contenedor = new int[cant_campos];
                                    for (int z = 0; z < cant_campos; ++z)
                                        contenedor[z] = 0;
                                    contenedor[pos_nombre_hilillos] = nombre_hilillo;
                                    ++nombre_hilillo;
                                    //Para conocer en cuál procesador está corriendo.
                                    contenedor[pos_nombre_procesador] = 2;
                                    contenedor[pos_pc] = (index_memoria2) + 128;
                                    contenedor[pos_tiempo_inicial] = 0;
                                    contextoProcesador2.Enqueue(contenedor);
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
                                    int[] contenedor = new int[cant_campos];
                                    for (int z = 0; z < cant_campos; ++z)
                                        contenedor[z] = 0;
                                    contenedor[pos_nombre_hilillos] = nombre_hilillo;
                                    ++nombre_hilillo;
                                    //Para conocer en cuál procesador está corriendo.
                                    contenedor[pos_nombre_procesador] = 3;
                                    contenedor[pos_pc] = (index_memoria3) + 128;
                                    contenedor[pos_tiempo_inicial] = 0;
                                    contextoProcesador3.Enqueue(contenedor);
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
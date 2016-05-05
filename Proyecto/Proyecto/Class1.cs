using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        int id_hilo = 0;

        int[] cache1 = new int[64];
        int[] enCache1 = new int[4];//Cual es el bloque que esta en la cache en esa posicion
        int[] cache2 = new int[64];
        int[] enCache2 = new int[4];//Cual es el bloque que esta en la cache en esa posicion
        int[] cache3 = new int[64];
        int[] enCache3 = new int[4];//Cual es el bloque que esta en la cache en esa posicion
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

            int[] prueba = new int[cant_campos];
            for (int i = 0; i < cant_campos; ++i)
            {
                prueba[i] = 0;
            }
            prueba[pos_pc] = 128;
            contextoProcesador1.Enqueue(prueba);
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

        public int[] leerInstruccion()
        {
            int indicador = 0;
            inicializarEstructuras();
            int [] instruccion = new int[cant_bytes_palabra]; 
            int[] contexto = (int[])contextoProcesador1.Dequeue();
            int PC = contexto[pos_pc];
            contexto[pos_pc] = PC + 4;
            PC = PC - limite;
            int bloque = PC / cant_bytes_bloque;
            int indice = bloque % cant_bytes_palabra;
            int palabra = bloque / cant_bytes_palabra;
           
            if(id_hilo==0){
                if(enCache1[indice]!=-1){
                    indicador = palabra * cant_bytes_palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i )
                    {
                        instruccion[i] = indicador;
                        indicador = indicador + cant_bytes_palabra;
                    }
                }
                else
                {
                    //en caso de fallo de cache
                }
            }
            else if (id_hilo == 1)
            {
                if (enCache2[indice] != -1)
                {
                    indicador = palabra * cant_bytes_palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = indicador;
                        indicador = indicador + cant_bytes_palabra;
                    }
                }
                else
                {
                    //en caso de fallo de cache
                }
            }
            else {

                if (enCache3[indice] != -1)
                {
                    indicador = palabra * cant_bytes_palabra;
                    for (int i = 0; i < cant_bytes_palabra; ++i)
                    {
                        instruccion[i] = indicador;
                        indicador = indicador + cant_bytes_palabra;
                    }
                }
                else
                {
                    //en caso de fallo de cache
                }

            }


            return contexto;
        }

        public void realizarOperacion(int[] instruccion)
        {
            //realizar operaciones
        }


    }
}


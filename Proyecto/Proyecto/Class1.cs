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

        int[] cache = new int[64];
        int[] enCache = new int[4];//Cual es el bloque que esta en la cache en esa posicion
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



        public void Probando()
        {
            int[] prueba = new int[37];
            for (int i = 0; i < 37; ++i)
            {
                prueba[i] = 0;
            }
            prueba[32] = 128;
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
            Probando();
            int[] contexto = (int[])contextoProcesador1.Dequeue();
            int PC = contexto[32];
            Console.WriteLine("PC " + PC);
            return contexto;
        }
    }
}

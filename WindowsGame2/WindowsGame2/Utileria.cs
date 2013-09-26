using System;
using System.IO;
using System.Collections;

namespace WindowsGame2
{
    class Utileria
    {
        /// <summary>
        /// Este metodo sirve para leer el archivo donde se encuentran guardadas las rutas de un mundo en particular
        /// El archivo contiene entradas de la direccion del alien. Por cada linea debe de haber una direccion. 
        /// Ejemplo:
        ///             Direccion(IZQUIERDA(0), DERECHA(1), ARRIBA(2), ABAJO(3)) | PosicionX  | PosicionY
        ///             1|175|126
        ///             3|175|400
        ///             1|325|400
        ///             2|325|150
        ///             1|475|150
        ///             3|475|275
        ///             1|700|275
        ///             3|700|375
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Ruta[] getRuta(String file)
        {
            StreamReader objReader = new StreamReader("..\\..\\..\\..\\..\\" + file);            
            string sLine = "";
            ArrayList arregloText = new ArrayList();
            // Guardamos cada linea de texto del archivo en un elemento de una ArrayList
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null)
                    arregloText.Add(sLine);
            }
            // Cerramos el archivo
            objReader.Close();
            String [] tmp;
            // Creamos un arreglo de rutas con el tamaño de las lineas encontradas
            Ruta[] ruta = new Ruta[arregloText.Count];
            for (int n = 0; n < arregloText.Count; n++)
            {
                String cadena = (String)arregloText[n];
                tmp = cadena.Split('|');
                switch (Int32.Parse(tmp[0]))
                {
                    case 0: ruta[n].direccion = Direccion.IZQUIERDA; break;
                    case 1: ruta[n].direccion = Direccion.DERECHA; break;
                    case 2: ruta[n].direccion = Direccion.ARRIBA; break;
                    case 3: ruta[n].direccion = Direccion.ABAJO; break;
                }
                ruta[n].X = Int32.Parse(tmp[1]); // valor de X
                ruta[n].Y = Int32.Parse(tmp[2]); // valor de Y
            }
            return ruta;
        }
    }
}

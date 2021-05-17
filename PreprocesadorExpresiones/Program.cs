using System;

namespace PreprocesadorExpresiones
{
    class Program
    {
        static string msg()
        {
            return "Programa para convertir input.txt a output.asm"
                    + "\nInstrucciones soportadas:"
                    + "\n\tleer(<variable>)"
                    + "\n\timprimir(<variable>)"
                    + "\n\timprimir('<cadena>')"
                    + "\n\t<variable> = <expresión>"
                    + "\n\twhile <condición>"
                    + "\n\t   ..."
                    + "\n\t   <instruccion>"
                    + "\n\t   ..."
                    + "\n\tend"
                    + "\n\tif <condición>"
                    + "\n\t   ..."
                    + "\n\t   <instruccion>"
                    + "\n\t   ..."
                    + "\n\tend"
                    + "\n<var> es cualquier variable de la letra 'a' a la 'z' cuyos valores son 0"
                    + "\n<expresión> puede incluir cualquier operador aritmético, relacional o lógico"
                    + "\nEl programa es sensible a mayúsculas, minúsculas y espacios."
                    + "\n\nPresione cualquier tecla para cargar el archivo input.txt de la carpeta local.";
        }
        static void Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.Clear();
                    //System.Diagnostics.Process.Start("mensaje.txt");
                    //Console.WriteLine("Presione cualquier tecla para cargar el archivo input.txt de la carpeta local.");

                    Console.WriteLine(msg());
                    Console.ReadLine();
                    new Compilador();
                    Console.Clear();
                    Console.WriteLine("Programa creado con éxito y guardado en la carpeta local");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    
                }
                Console.ReadLine();
            }
        }
    }
}

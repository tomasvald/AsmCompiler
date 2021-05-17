using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;//aqui

namespace PreprocesadorExpresiones
{
    class Compilador
    {
        private StreamReader input;
        private StreamWriter output;
        private int contLinea;  // contador de líneas en el archivo de entrada input.txt
        private int cont_etiq;  // contador de etiquetas para saltos del programa ensamblado
        private int cont_cad;   // contador de nombres de cadenas del programa ensamblado
        private string cadmsgs; // string que guarda el contenido de las cadenas a imprimir
        private List<string> pilaCiclos;
        private List<string> pilaFinIf;

        public Compilador()
        {
            contLinea = 1;
            cont_etiq = 0;
            cont_cad = 0;
            cadmsgs = null;
            pilaCiclos = new List<string>();
            pilaFinIf = new List<string>();

            input = new StreamReader("input.txt");
            output = new StreamWriter("expresiones.asm");

            init();
            main();
            end();

            output.Close();
        }
        
        // cuerpo del programa

        private void main()
        {
            write();
            write("main:");
            
            while (!input.EndOfStream)
            {
                validar(input.ReadLine());
                contLinea++;
            }
            

        }
        private void init()
        {
            write("; --------------------------------------------");
            write("; Programa:\texpresiones.asm");
            write("; Funcion:\tEfectuar operaciones de lectura, escritura");
            write("; y operaciones aritméticas y lógicas");
            write("; --------------------------------------------");
            write();
            write("text\tsegment");
            write("\t\tassume cs:text, ds:text, sstext");
            write("\t\torg 100h");

        }
        private void end()
        {
            // fin del programa
            write();
            write("int 20h");

            // declaración de procedimientos para lectura y escritura
            write();
            impdec();
            lecdec();
            saltaren();

            write();
            write("msgLec\tdb 'Ingrese valor de '");
            write("var1\tdb 0");
            write("\tdb ': ','$'");

            write("msgImp\tdb 'El valor de '");
            write("var2 \tdb 0");
            write("\tdb ' es: $'");

            write(cadmsgs);

            write();
            write("buffer db 6,0,6 dup(?)");

            // declarar todas las letras del abecedario con variables
            write();
            char c = 'a';
            for (int i = 0; i < 26; i++)
                write(c++ + " dw 0");

            // fin del código
            write();
            write("text\tends");
            write("\t\tend main");
        }

        // funciones de validación de input.txt

        private void validar(string linea)
        {
            if      (esLineaVacia(linea)) ; //ignorar
            else if (esComentario(linea)) ; // ignorar
            else if (esLecVar(linea)) lecVar(getLecVar(linea));
            else if (esImpVar(linea)) impVar(getImpVar(linea));
            else if (esImpCad(linea)) impCad(getImpCad(linea));
            else if (esAsign(linea))  asign(linea);
            else if (esIf(linea))     instrIf(linea);
            else if (esElse(linea))   instrElse();
            else if (esWhile(linea))  instrWhile(linea);
            else if (esEnd(linea))    instrEnd();
            else                      throw new Exception("Instrucción en línea " + contLinea + " no válida.");
        }
        private bool esLineaVacia(string linea)
        {
            // <>
            return Regex.IsMatch(linea, @"^\s*$");
        }
        private bool esComentario(string linea)
        {
            // ; <cadena>
            return Regex.IsMatch(linea, @"^\s*\;[\s\S]*$");
        }
        private bool esLecVar(string linea)
        {
            // leer(a)
            return Regex.IsMatch(linea, @"^\s*leer\s*\(\s*[a-z]\s*\)$");
        }
        private bool esImpVar(string linea)
        {
            // imprimir(a)
            return Regex.IsMatch(linea, @"^\s*imprimir\s*\(\s*[a-z]\s*\)$");
        }
        private bool esImpCad(string linea)
        {
            // imprimir('<cadena>')
            return Regex.IsMatch(linea, @"^\s*imprimir\s*\(\s*\'[\s\S]{1,}\'\s*\)$");
        }
        private bool esAsign(string linea)
        {
            // <var> = <expr>
            if (!Regex.IsMatch(linea, @"^\s*[a-z]\s*\=\s*[\s\S]+\s*$"))
                return false;

            // .esValida(<expr>)
            var postexpr = new PreprocesadorExpresiones(getAsignExpr(linea));
            return PreprocesadorExpresiones.esValida(postexpr.imp_pilaexpr());


        }
        private bool esIf(string linea)
        {
            // if <condicion>
            if (!Regex.IsMatch(linea, @"^\s*if\s+[\s\S]+\s*$"))
                return false;

            // .esValida(<expr>)
            var postexpr = new PreprocesadorExpresiones(getIfCond(linea));
            return PreprocesadorExpresiones.esValida(postexpr.imp_pilaexpr());
        }
        private bool esElse(string linea)
        {
            // else <condicion>
            return Regex.IsMatch(linea, @"^\s*else\s*$") ;
        }
        private bool esWhile(string linea)
        {
            // while <condicion>
            if(!Regex.IsMatch(linea, @"^\s*while\s+[\s\S]+\s*$"))
                return false;

            // .esValida(<expr>)
            var postexpr = new PreprocesadorExpresiones(getWhileCond(linea));
            return PreprocesadorExpresiones.esValida(postexpr.imp_pilaexpr());

        }
        private bool esEnd(string linea)
        {
            // end
            return Regex.IsMatch(linea, @"^\s*end\s*$");
        }

        // funciones soportadas

        private void lecVar(char x)
        {
            // el valor leído es almacenado en AX y copiado a la variable x
            write("; leer(" + x + ")");

            //write_tab("mov var1,'" + x + "'");
            //write_tab("mov dx, offset msgLec");
            //write_tab("mov ah,9");
            //write_tab("int 21h");
            write_tab("call lecdec");
            write_tab("mov " + x + ",ax");
            write_tab("call saltaren");
        }
        private void impVar(char x)
        {
            // el valor de x es copiado al registro ax y luego éste es impreso
            write("; imprimir(" + x + ")");

            //write_tab("mov var2,'" + x + "'");
            //write_tab("mov dx, offset msgImp");
            //write_tab("mov ah,9");
            //write_tab("int 21h");
            write_tab("mov ax," + x);
            write_tab("call impdec");
            write_tab("call saltaren");

        }
        private void impCad(string s)
        {
            write("; impresion cadena");
            write_tab("mov dx,offset cad"+ (cont_cad).ToString("000"));
            write_tab("mov ah,9");
            write_tab("int 21h");
            write_tab("call saltaren");

            cadmsgs += "\ncad"+(cont_cad).ToString("000")+" db '" + s+"$'";
            cont_cad++;


        }
        private void asign(string linea)
        {
            // Explicación del valor del string posfija:

            // getExprAsign(linea)                                  Primero se obtiene la parte de la expresión de la asignación
            // new PreprocesadorExpresiones(<expr>)            Luego se convierte a post-expresión
            // <expr>.imp_pilaexpr()                           Y se procede a extraer la post-expresión
            // Posfija.preexpr_aPosfija(<pexpr>)               Como se verificó previamente que la expresión estuviera
            //                                                 bien escrita se procede a convertirla a posfija

            char var = getAsignVar(linea);
            var pexpr = new PreprocesadorExpresiones(getAsignExpr(linea));

            string posfija = Posfija.preexpr_aPosfija( pexpr.imp_pilaexpr() );
            int[] constantes = pexpr.get_const();

            write("; ASIGNACION");
            posfija_asm(posfija, constantes);

            // el resultado final en el ensamblado se guarda al final de la pila
            // el valor se recupera en AX
            // y se procede a copiarlo a <var>

            //write("; Resultado final");
            write_tab("pop ax");
            write_tab("mov "+var+",ax");
            //write("; FIN ASIGNACION");


        }
        private void instrIf(string linea)
        {
            string condicion = getIfCond(linea);
            //int etiquetaInicioIf;
            int etiquetaSalto;
            int etiquetaFinIf;

            write("; if " + condicion);
            //etiquetaInicioIf = cont_etiq;
            //cont_etiq++;
            //write("e" + (etiquetaInicioIf).ToString("000") + ": ");

            var postExpr = new PreprocesadorExpresiones(condicion);
            string posfija = Posfija.preexpr_aPosfija(postExpr.imp_pilaexpr());
            int[] constantes = postExpr.get_const();

            write("; condicion");
            posfija_asm(posfija, constantes);

            write_tab("pop ax");
            write_tab("cmp ax,0");
            etiquetaSalto = cont_etiq;
            cont_etiq++;
            write_tab("jz e" + (etiquetaSalto).ToString("000"));

            etiquetaFinIf = cont_etiq;
            pilaFinIf.Add("e" + (etiquetaFinIf).ToString("000") + ": ");
            cont_etiq++;

            // guardar la etiqueta de salto en la pila
            pilaCiclos.Add("e" + (etiquetaSalto).ToString("000") + ": ");
        }
        private void instrElse()
        {
            if (pilaCiclos.Count > 0)
            {
                int etiquetaSalto = cont_etiq;
                write_tab("jmp e" + (etiquetaSalto).ToString("000") + ": ");

                // se escribe el contenido del último elemento de la pila
                // como la instrucción ELSE va precedida de la instrucción IF
                // entonces lo que se escribe es la etiqueta de fin del IF
                write(pilaCiclos[pilaCiclos.Count - 1]);
                pilaCiclos.RemoveAt(pilaCiclos.Count - 1);

                // se agrega otra etiqueta de fin a la pila
                List<string> list = new List<string>();
                list.Add("e" + (etiquetaSalto).ToString("000") + ": ");
                pilaCiclos.Add("e" + (etiquetaSalto).ToString("000") + ": ");
                cont_etiq++;
            }
            else throw new Exception("No hay un if precedido a la instrucción else");

        }
        private void instrWhile(string linea)
        {
            // se extrae la condición de la primera linea
            string condicion = getWhileCond(linea);
            int etiquetaInicioWhile;
            int etiquetaSalto;

            write("; while "+condicion);
            etiquetaInicioWhile = cont_etiq;
            write("e"+(etiquetaInicioWhile).ToString("000")+": ");
            cont_etiq++;

            // la condición se converte a expresión, luego a postfija
            // y se almacena la postfija y la cadena de constantes
            // como ya se verificó que la condición está bien escrita
            // ya no se valida la expresión
            var postExpr = new PreprocesadorExpresiones(condicion);
            string posfija = Posfija.preexpr_aPosfija(postExpr.imp_pilaexpr());
            int[] constantes = postExpr.get_const();

            // la condición se convierte a ensamblado
            write("; condicion");
            posfija_asm(posfija, constantes);

            // verificación de condición para continuar el ciclo while
            write_tab("pop ax");
            write_tab("cmp ax,0");
            etiquetaSalto = cont_etiq;
            write_tab("jz e"+ (etiquetaSalto).ToString("000"));
            cont_etiq++;

            // salto hacia el inicio del while para validar la condicion
            pilaCiclos.Add(
                "  jmp e" + (etiquetaInicioWhile).ToString("000")+
                "\ne" + (etiquetaSalto).ToString("000") + ": ");


        }
        private void instrEnd()
        {
            if(pilaCiclos.Count > 0)
            {
                write(pilaCiclos[pilaCiclos.Count - 1]);
                pilaCiclos.RemoveAt(pilaCiclos.Count - 1);
            }
            else throw new Exception("Uso innecesario de instrucción end");
        }

        // convertir de expresión posfija a ensamblado

        private void posfija_asm(string posfija, int[] constantes)
        {
            //Console.WriteLine("Posfija: {0}",posfija);
            //Console.ReadLine();

            for (int i = 0; i < posfija.Length; i++)
            {
                if      (PreprocesadorExpresiones.esVar(posfija[i])) var(posfija[i]);
                else if (char.IsDigit(posfija[i])) constnt(posfija[i], constantes);
                else if (PreprocesadorExpresiones.esOperAritm(posfija[i])) operAritm(posfija[i]);
                else if (PreprocesadorExpresiones.esOperRelac(posfija[i])) operRelac(posfija[i]);
                else if (PreprocesadorExpresiones.esOperLog(posfija[i])) operLog(posfija[i]);
            }
        }
        private void var(char c)
        {
            //write("; variable");
            write_tab("push " + c);
        }
        private void constnt(char c, int[] constantes)
        {
            //write("; constante");
            write_tab("mov ax," + constantes[int.Parse(c.ToString())]);
            write_tab("push ax");
        }
        private void operAritm(char c)
        {
            // el primer valor se queda recupera en AX
            // el segundo en BX
            // el resultado de la operación se guarda en AX
            write("; operAritm "+c);
            write_tab("pop bx");
            write_tab("pop ax");

            switch (c)
            {
                case '+':
                    write_tab("add ax,bx");
                    break;
                case '-':
                    write_tab("sub ax,bx");
                    break;
                case '*':
                    write_tab("mul bx");
                    break;
                case '/':
                    write_tab("xor dx dx");
                    write_tab("div bx");
                    break;
                case '%':
                    write_tab("xor dx,dx");
                    write_tab("div bx");
                    write_tab("mov ax,dx");
                    break;
            }

            write_tab("push ax");
        }
        private void operRelac(char c)
        {
            write("; operRelac "+c);
            write_tab("pop bx");
            write_tab("pop ax");
            write_tab("cmp ax,bx");

            string jmp = null;
            switch (c)
            {
                case '>': jmp = "jg"; break;
                case ']': jmp = "jge"; break;
                case '<': jmp = "jb"; break;
                case '[': jmp = "jbe"; break;
                case ':': jmp = "je"; break;
                case '$': jmp = "jne"; break;
            }

            write_tab(jmp + " e" + (cont_etiq).ToString("000"));
            write_tab("mov cx,0");
            write_tab("jmp e" + (cont_etiq + 1).ToString("000"));

            write_tab("e" + (cont_etiq).ToString("000") + ": mov cx,1");
            write_tab("e" + (cont_etiq + 1).ToString("000") + ": push cx");

            cont_etiq = cont_etiq + 2;
        }
        private void operLog(char c)
        {
            write("; operLog");

            string jmp = null;
            switch (c)
            {
                case '|': jmp = "jge"; break;
                case '&': jmp = "je"; break;
            }

            write_tab("pop bx");
            write_tab("pop ax");
            write_tab("add ax,bx");
            write_tab("cmp ax,1");

            write_tab(jmp+" e" + (cont_etiq).ToString("000"));
            write_tab("mov cx,0");
            write_tab("jmp e" + (cont_etiq + 1).ToString("000"));

            write_tab("e" + (cont_etiq).ToString("000") + ": mov cx,1");
            write_tab("e" + (cont_etiq + 1).ToString("000") + ": push cx");

            cont_etiq = cont_etiq + 2;

        }
           
        // escribir en output.asm

        private void write(string s)
        {
            output.WriteLine(s);
        }
        private void write_tab(string s)
        {
            output.WriteLine("  "+s);
        }
        private void write()
        {
            output.WriteLine();
        }

        // funciones auxiliares para lectura y escritura de variables en memoria
        
        private void lecdec()
        {
            // var is stored on ax register
            write();

            write("lecdec\tproc near");
            write_tab("push bx");
            write_tab("push cx");
            write_tab("push dx");
            write_tab("push si");
            write_tab("mov dx, offset buffer");
            write_tab("mov ah,10");
            write_tab("int 21h");
            write_tab("xor ax,ax");
            write_tab("mov bx,10");
            write_tab("mov si, offset buffer");
            write_tab("inc si");
            write_tab("mov cl,[si]");
            write_tab("xor ch, ch");

            write("lecdec0: inc si");
            write_tab("mul bx");
            write_tab("mov dl,[si]");
            write_tab("sub dl,'0'");
            write_tab("xor dh, dh");
            write_tab("add ax, dx");
            write_tab("loop lecdec0");
            write_tab("pop si");
            write_tab("pop dx");
            write_tab("pop cx");
            write_tab("pop bx");
            write_tab("ret");

            write("lecdec\tendp");

        }
        private void impdec()
        {
            write();

            write("impdec\tproc near");
            write_tab("push ax");
            write_tab("push bx");
            write_tab("push cx");
            write_tab("push dx");
            write_tab("xor cx,cx");
            write_tab("mov bx,10");

            write("impdec0: xor dx, dx");
            write_tab("div bx");
            write_tab("add dl,'0'");
            write_tab("push dx");
            write_tab("inc cx");
            write_tab("cmp ax,0");
            write_tab("jne impdec0");

            write("impdec1: pop dx");
            write_tab("mov ah,2");
            write_tab("int 21h");
            write_tab("loop impdec1");
            write_tab("pop dx");
            write_tab("pop cx");
            write_tab("pop bx");
            write_tab("pop ax");
            write_tab("ret");

            write("impdec\tendp");
        }
        private void saltaren()
        {
            write();
            write("saltaren proc near");
            write_tab("push ax");
            write_tab("push dx");
            write_tab("mov dl,13");
            write_tab("mov ah,2");
            write_tab("int 21h");
            write_tab("mov dl,10");
            write_tab("mov ah,2");
            write_tab("int 21h");
            write_tab("pop dx");
            write_tab("pop ax");
            write_tab("ret");
            write("saltaren endp");

        }

        // funciones para obtener los datos de entrada de las instrucciones

        private char getLecVar(string linea)
        {
            int i =0;
            
            while (linea[i] == ' ') i++;
            i = i + 4; // ignorar la palabra "leer"
            while (linea[i] == ' ') i++;
            i = i + 1;
            while (linea[i] == ' ') i++;

            return linea[i];
        }
        private char getImpVar(string linea)
        {
            int i = 0;

            while (linea[i] == ' ') i++;
            i = i + 8; // ignorar la palabra "imprimir"
            while (linea[i] == ' ') i++;
            i = i + 1; // ignorar paréntesis (
            while (linea[i] == ' ') i++;

            return linea[i];

        }
        private string getImpCad(string linea)
        {
            string s = null;
            int i1 = 0, i2 = linea.Length - 1;

            while (linea[i1] == ' ') i1++;
            i1 = i1 + 8; // ignorar la palabra "imprimir"
            while (linea[i1] == ' ') i1++;
            i1 = i1 + 1; // ignorar paréntesis (
            while (linea[i1] == ' ') i1++;
            i1 = i1 + 1; // ignorar apóstrofe '

            while (linea[i2] == ' ') i2--;
            i2 = i2 - 1; // ignorar paréntesis )
            while (linea[i2] == ' ') i2--;
            i2 = i2 - 1; // ignorar apóstrofe '

            for (int i = i1; i <= i2; i++) s += linea[i];

            return s;
        }
        private char getAsignVar(string linea)
        {
            int i = 0;

            while (linea[i] == ' ') i++;

            return linea[i];
        }
        private string getAsignExpr(string linea)
        {
            string s = null;
            int i1 = 0, i2 = linea.Length -1;

            while (linea[i1] == ' ') i1++;
            i1 = i1 + 1; // ignorar variable de asignación
            while (linea[i1] == ' ') i1++;
            i1 = i1 + 1; // ignorar signo igual =
            while (linea[i1] == ' ') i1++;

            while (linea[i2] == ' ') i2--;

            for (int i = i1; i <= i2; i++) s += linea[i];

            return s;

        }
        private string getIfCond(string linea)
        {
            string s = null;
            int i1 = 0, i2 = linea.Length - 1;

            while (linea[i1] == ' ') i1++;
            i1 = i1 + 2; // ignorar palabra "if"
            while (linea[i1] == ' ') i1++;

            while (linea[i2] == ' ') i2--;

            for (int i = i1; i <= i2; i++) s += linea[i];

            return s;
        }
        private string getWhileCond(string linea)
        {
            string s = null;
            int i1 = 0, i2 = linea.Length - 1;

            while (linea[i1] == ' ') i1++;
            i1 = i1 + 5; // ignorar palabra "while"
            while (linea[i1] == ' ') i1++;

            while (linea[i2] == ' ') i2--;

            for (int i = i1; i <= i2; i++) s += linea[i];

            return s;
        }

    }
}

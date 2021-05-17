using System;

namespace PreprocesadorExpresiones
{
    class PreprocesadorExpresiones
    {
        string s;
        char[] pexpr;
        int[] constantes;
        int p = 0, c = 0;

        public PreprocesadorExpresiones(string s)
        {
            this.s = s;
            pexpr = new char[20];
            constantes = new int[10];

            a_pilaexpr();
        }

        private void a_pilaexpr()
        {
            // Converite una expresión a otra más sencilla
            // No convierte los operadores lógicos unarios & y |

            for (int i = 0; i < s.Length; i++)
            {
                // ignorar si es espacio en blanco
                if (s[i] == ' ')
                    ;
                // constante numérica
                else if (char.IsDigit(s[i]))
                {
                    string a = s[i].ToString();
                    while ( (i + 1 < s.Length) && char.IsDigit(s[i + 1]))
                    {
                        a += s[++i].ToString();
                    }
                    add_const(int.Parse(a));
                    add_pexpr(get_constant().ToString()[0]);
                    //No me sirvieron (char)i , Convert.ToChar(i)

                }
                // a-z
                else if (esVar(s[i]))
                    add_pexpr(s[i]);
                // operador
                else if (esOperAritm(s[i]))
                    add_pexpr(s[i]);
                // >    >=   ]
                else if (s[i] == '>')
                    if ((i + 1 < s.Length) && s[i + 1] == '=')
                    { add_pexpr(']'); ++i; }
                    else
                        add_pexpr('>');
                // <    <=   [
                else if (s[i] == '<')
                    if ((i + 1 < s.Length) && s[i + 1] == '=')
                    { add_pexpr('['); ++i; }
                    else
                        add_pexpr('<');
                // !=   $
                else if (s[i] == '!')
                    if ((i + 1 < s.Length) && s[i + 1] == '=')
                    { add_pexpr('$'); ++i; }
                    else ;
                // &&
                else if (s[i] == '&')
                    if ((i + 1 < s.Length) && s[i + 1] == '&')
                    { add_pexpr('#'); ++i; }
                    else;
                // ||
                else if (s[i] == '|')
                    if ((i + 1 < s.Length) && s[i + 1] == '|')
                    { add_pexpr('°'); ++i; }
                    else;
                // ==    :
                else if (s[i] == '=')
                    if ((i + 1 < s.Length) && s[i + 1] == '=')
                    { add_pexpr(':'); ++i; }
                    else;
                else
                    add_pexpr(s[i]);
            }

            return;
        }

        // insertar en pilas

        private void add_pexpr(char c)
        {
            if (p < pexpr.Length)
                pexpr[p++] = c;
            else
                throw new Exception("La pila de expresión se ha llenado");
        }
        private void add_const(int i)
        {
            if (c < constantes.Length)
                constantes[c++] = i;
            else
                throw new Exception("La pila de constantes se ha llenado");
        }
        private int get_constant()
        {
            return c - 1;
        }

        // impresión de pilas

        public string imp_pilaexpr()
        {
            string s = null;
            for (int i = 0; i < p; i++)
            {
                s += pexpr[i];
            }

            if (s == null) return "No hay expresión.";
            return s;
        }
        public string imp_pilaconst()
        {
            string s = null;
            for (int i = 0; i < c; i++)
            {
                s += "\ni:0"+i+"\t" + constantes[i];
            }

            if (s == null) return "No hay constantes.";
            return s;
        }
        public string imp_tabla()
        {
            return 
                 "\n>=   ]"
                +"\n<=   ["
                + "\n!=   $"
                + "\n==   :"
                + "\n&&   #"
                +"\n||   °";
        }
        public int[] get_const()
        {
            int[] _const = new int[c];
            for (int i = 0; i < c; i++)
            {
                _const[i] = constantes[i];
            }

            return _const;
        }

        // verificar sintaxis

        public static bool esValida(string s)
        {
            int e = 0, i = 0, cp = 0;
            string estadoOper = null;
            bool resultadoAritm = true;

            while (e != -1 && i < s.Length)
            {
                switch (e)
                {
                    case 0:
                        if (esVar(s[i]) || char.IsDigit(s[i])) e = 1;
                        else if (esOperAritm(s[i]) || esOperRelac(s[i]) || esOperLog(s[i])) e = -1;
                        else if (s[i] == '(') { e = 0; cp++; }
                        else if (s[i] == ')') e = -1;
                        break;
                    case 1:
                        if (esVar(s[i]) || char.IsDigit(s[i])) e = -1;
                        else if (esOperAritm(s[i]) || esOperRelac(s[i]) || esOperLog(s[i]))
                        {
                            e = 0;
                            if (esOperRelac(s[i]))
                            {
                                resultadoAritm = false;
                                estadoOper = "bool";
                            }
                            else if (esOperLog(s[i]))
                            {
                                if (estadoOper == "log") e = -1;
                                estadoOper = "log";
                            }
                        }
                        else if (s[i] == '(') e = 0;
                        else if (s[i] == ')') { e = 1; cp--; }
                        break;
                }
                ++i;
            }

            return e == 1 && cp == 0 && (estadoOper == "bool" || resultadoAritm);
        }
        public static bool esVar(char c)
        {
            return c >= 'a' && c <= 'z';
        }
        public static bool esOperAritm(char c)
        {
            return c == '+' || c== '-' || c == '*' || c == '/' || c == '%';
        }
        public static bool esOperRelac(char c)
        {
            return c == '>' || c == '<' || c == ']' || c == '[' || c == ':' || c == '$';
        }
        public static bool esOperLog(char c)
        {
            return c == '#' || c == '°';
        }


        
        
    }
}

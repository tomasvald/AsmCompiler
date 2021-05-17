//using System;

namespace PreprocesadorExpresiones
{
    static class Posfija
    {
        private static int pEnt(char c)
        {
            if (c == '(') return 6;
            if (c == '*' || c == '/' || c == '%') return 5;
            if (c == '+' || c == '-') return 4;
            if (c == '>' || c == '<' || c == ']' || c == '[' || c == '$' || c == ':') return 3;
            if (c == '#') return 2;
            if (c == '°') return 1;
            return 0;
        }
        private static int pSal(char c)
        {
            if (c == '(') return 0;
            if (c == '*' || c == '/' || c == '%') return 5;
            if (c == '+' || c == '-') return 4;
            if (c == '>' || c == '<' || c == ']' || c == '[' || c == '$' || c == ':') return 3;
            if (c == '#') return 2;
            if (c == '°') return 1;
            return 0;
        }
        public static string preexpr_aPosfija(string s)
        {
            string post = null;
            char[] pila = new char[16];
            int tope = 0, i = 0;
            char c;

            while (i < s.Length)
            {
                if ((s[i] >= 'a' && s[i] <= 'z') || char.IsDigit(s[i]))
                    post += s[i];
                else if (s[i] == ')')
                    while ((c = pila[--tope]) != '(') post += c;
                else
                {
                    while (tope != 0 && pSal(pila[tope - 1]) >= pEnt(s[i])) post += pila[--tope];
                    pila[tope++] = s[i];
                }
                ++i;
            }

            while (tope != 0) post += pila[--tope];
            return post;

        }

    }
}

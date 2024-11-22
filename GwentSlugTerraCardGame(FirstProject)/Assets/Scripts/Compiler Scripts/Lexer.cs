using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
class Lexer 
{

    public List<Token> tokens = new List<Token>();

    private string a_analizar;
    
        public Lexer(string entrada)
        {
            this.a_analizar = entrada;
        }

    public List<Token> Tokenizar()
    {
        // con esto estaremos eliminando los espacios en blanco antes y despues del primer y ultimo caracter respectivamene
        a_analizar = a_analizar.Trim();
        // con esto estaremos separando cada fragmento del codigo atendiendo al lenguaje para su posterior uso
        //definir expresiones regulares
        string PalabrasReservadas = @"if|else|while|for|in|effect|card";
        string Booleano = @"true|false";
        string Identificadores = @"^[a-zA-Z_][a-zA-Z0-9_]*";
        string Delimitadores = @"[\(\)\{\}\[\]]";
        string OperadoresAritmeticos = @"(?:[+\-*/%])";
        string OperadoresDeComparacion = @"(?:==|!=|>=|<=|>|<)";
        string OperadoresDeAsignacion =  @"(?:=|\+=|-=|\*=|/=|%=|:)";
        string OperadoresLogicos = @"(?:&&|\|\||!)";
        string OperadoresDeIncDec = @"(?:\+\+|--)";
        string OperadoresDeCadenas = @" @@ | @ ";
        string OperadorLanda = @"=>"; 
        string Identificadorestring = @"""(([^""\\]|\.)*?)""";
        string PatronDeNumero = @"\d+";
        string PuntoComa = @";";
        string Punto = @"\.";
        string Coma = @",";

    //crear un diccionario de expresiones regulares en virtud del uso del metodo Match de la clase Regex de expresiones regulares de C#
    var Diccionario = new Dictionary<string, string>
    {
        { PalabrasReservadas, "PalabrasReservadas" },
        { Booleano , "Booleano"},
        { Identificadores, "Identificadores" },
        { Delimitadores, "Delimitadores" },
        { OperadoresAritmeticos, "OperadoresAritmeticos" },
        { OperadoresDeComparacion, "OperadoresDeComparacion" },
        { OperadoresDeAsignacion, "OperadoresDeAsignacion" },
        { OperadoresLogicos, "OperadoresLogicos" },
        { OperadoresDeIncDec, "OperadoresDeIncDec" },
        { OperadoresDeCadenas, "OperadoresDeCadenas" },
        { Identificadorestring, "Identificadorestring" },
        { OperadorLanda , "OperadorLanda" },
        { PatronDeNumero, "PatronDeNumero" },
        { PuntoComa , "PuntoComa" },
        { Punto , "Punto" },
        { Coma , "Coma" }

    };

        //recorro todos los tipos de tokens definidos en Diccionario. Para cada iteracion se busca una coincidencia al principio de la entrada 
        //Si encuentra una coincidencia que es más larga que cualquier coincidencia anterior actualizo<<>>juju
        while(!string.IsNullOrEmpty(a_analizar))
        {
            string MejorToken = null!;
            string MejorTipoDeToken = null!;
            int IMejorCoincidencia = 0;
            //pasar cada expresion regular definida para encontrar el mejor patron
            foreach (var item in Diccionario)
            {
                var token = Regex.Match(a_analizar , item.Key);
                if (token.Success && token.Index == 0 && token.Length > IMejorCoincidencia)
                {
                    MejorToken = token.Value;
                    MejorTipoDeToken = item.Value;
                    IMejorCoincidencia = token.Length;
                }
            }
            //eliminar los espacios en blanco a partir del mejor token encontrado
            if(MejorToken == null)
            {
                a_analizar = a_analizar.Substring(1).Trim();
                continue;
            }

            tokens.Add(new Token(MejorTipoDeToken , MejorToken));
            
            a_analizar = a_analizar.Substring(IMejorCoincidencia).Trim();

            //verificar el caso de que sea un operador aritmetico concatenar los valores consiguientes 
            if (MejorTipoDeToken == "OperadorAritmetico" && !string.IsNullOrEmpty(a_analizar))
            {

                var SiguienteToken = Regex.Match(a_analizar, OperadoresAritmeticos);

                if (SiguienteToken.Success && SiguienteToken.Index == 0)
                {
                    tokens[tokens.Count - 1].TokenValue += SiguienteToken.Value;
                    a_analizar = a_analizar.Substring(SiguienteToken.Length).Trim();
                }

            } 
        }
        return tokens;
    }
}

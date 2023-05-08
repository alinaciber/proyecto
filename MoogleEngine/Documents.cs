namespace MoogleEngine;
using System.Collections.Generic;
public static class Document
{
    public static string ReadFile(string file)
    {
        string contenido;
        StreamReader sr = new StreamReader(file);
        contenido = sr.ReadToEnd();
        sr.Close();
        return contenido;
    }

    public static string EliminarAcentos(string contenido)
    {
        string contenido_minusculas;
        string contenido_sinacentos;
        contenido_minusculas = contenido.ToLower();
        contenido_sinacentos = contenido_minusculas.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ú', 'u');
        return contenido_sinacentos;
    }

    public static string[] Normalizar(string contenido_sinacentos)
    {
        string[] words;
        char[] separadores = new char[] { ' ', '\n', '\t', '\r', '.', ',', ';', ':', '"', '?', '!', '{', '}', '[', ']', '(', ')', '*', '#', '$', '@', '%', '^', '-', '_', '=', '+', '<', '>', '/', '|' };
        words = contenido_sinacentos.Split(separadores, StringSplitOptions.RemoveEmptyEntries);
        return words;
    }

    public static Dictionary<string, int> ContarPalabras(string word,Dictionary<string, int> conteo)
    {
        if (conteo.ContainsKey(word))
            conteo[word]++;
        else
            conteo.Add(word, 1);
        return conteo;
    }

}

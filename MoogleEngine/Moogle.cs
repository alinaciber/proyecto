namespace MoogleEngine;
using System.IO;
using System.Collections.Generic;
public static class Moogle
{
    public static SearchResult Query(string query)
    {
        var presearchitems = new List<SearchItem>();
        var currentDirectory = Directory.GetCurrentDirectory();
        string path = currentDirectory + @"\Content\";
        string[] files = Directory.GetFiles(path);
        string contenido;
        string contenido_sinacentos;
        string[] words;
        string file_name;
        List<string> List_fila = new List<string>();
        List<string> List_columna = new List<string>();
        List<string> list_document_texto = new List<string>();
        List<Dictionary<string, int>> list_dictionary = new List<Dictionary<string, int>>();//Aquí creo mi lista de los diccionarios por documento
        string query_sinacentos = Document.EliminarAcentos(query);
        string[] query_normalizada = Document.Normalizar(query_sinacentos);
        Dictionary<string, int> dict_query = new Dictionary<string, int>();
        double param1_query;
        double param2_query;
        double param3_query;
        int contar;
        double suma_pesos_query = 0;
        double peso_query;
        Dictionary<string, double> dic_palabras_pesos = new Dictionary<string, double>();
        double suma_pesos;
        double suma_peso_documento;
        double similitud_coseno;
        Dictionary<string, double> dic_doc_score = new Dictionary<string, double>();


        foreach (var file in files)
        {
            contenido = Document.ReadFile(file);
            contenido_sinacentos = Document.EliminarAcentos(contenido);
            words = Document.Normalizar(contenido_sinacentos);
            list_document_texto.Add(contenido_sinacentos);
            Dictionary<string, int> dic_wordCount = new Dictionary<string, int>();
            file_name = Path.GetFileNameWithoutExtension(file);
            List_fila.Add(file_name);

            foreach (var word in words)
            {
                dic_wordCount = Document.ContarPalabras(word, dic_wordCount);//Aqui estoy llenando el diccionario con las palabras de cada texto y la cantidad de veces que aparecen.

                if (!List_columna.Contains(word)) // Aqui lleno lista con todas las palbras sin repetir
                {
                    List_columna.Add(word);
                }
            }
            list_dictionary.Add(dic_wordCount);
        }


        int count_fila = List_fila.Count;
        int count_columna = List_columna.Count;
        double[,] matriz_TF_IDF = new double[count_fila, count_columna];
        matriz_TF_IDF = TFidfCalculator.MatrizTFIDF(count_fila, count_columna, list_dictionary, List_columna);


        //Aqui voy a llenar el dicionario de la query
        foreach (string palabra in query_normalizada)
        {
            dict_query = Document.ContarPalabras(palabra, dict_query);
        }

        param2_query = TFidfCalculator.Calcula_param2(dict_query);

        foreach (var item in dict_query)
        {
            if (List_columna.Contains(item.Key))
            {
                param1_query = item.Value;
                contar = 0;
                foreach (var elemento in list_dictionary)
                {
                    if (elemento.ContainsKey(item.Key))
                    {
                        contar++;
                    }
                }
                param3_query = contar;
                peso_query = TFidfCalculator.TF_IDF_Calc(param1_query, param2_query, Convert.ToDouble(count_fila), param3_query);
                //Calculando la suma de los pesos de la query 
                suma_pesos_query += Math.Pow(peso_query, 2);
                // Vamos ahora a llenar un diccionario con las palabras de la query y sus respectivos pesos 
                dic_palabras_pesos.Add(item.Key, peso_query);
            }
        }

        for (int i = 0; i < matriz_TF_IDF.GetLength(0); i++)
        {
            suma_pesos = 0;
            suma_peso_documento = 0;


            for (int j = 0; j < matriz_TF_IDF.GetLength(1); j++)
            {

                if (dic_palabras_pesos.ContainsKey(List_columna[j]))
                {
                    suma_pesos += matriz_TF_IDF[i, j] * dic_palabras_pesos[List_columna[j]];
                }
                suma_peso_documento += Math.Pow(matriz_TF_IDF[i, j], 2);
            }

            similitud_coseno = suma_pesos / (Math.Sqrt(suma_peso_documento * suma_pesos_query));
            if (similitud_coseno != 0)
            {
                dic_doc_score.Add(List_fila[i], similitud_coseno);
            }
        }

        int count = 1;
        string snippet;
        string previo_snippet;
        int pos_word_query;
        int pos_punto;
        int dif;

        foreach (var elemento in dic_doc_score.OrderByDescending(x => x.Value))
        {
            count++;
            snippet = " ";
            if (count <= 5) //Para tener en cuenta los 5 primeros ficheros de mas score
            {
                previo_snippet = list_document_texto[List_fila.IndexOf(elemento.Key)];
                foreach (var element in dic_palabras_pesos.OrderByDescending(x => x.Value))
                {
                    if (element.Value > 20)//palabras con peso superior a 20
                    {
                        pos_word_query = previo_snippet.IndexOf(element.Key);
                        if (pos_word_query > 0)
                        {
                            pos_punto = previo_snippet.IndexOf('.', pos_word_query);
                            if (pos_punto > 0 && pos_punto > pos_word_query)
                            {
                                dif = pos_punto - pos_word_query;
                                snippet = snippet + '<' + previo_snippet.Substring(pos_word_query, dif) + '>';
                            }
                            else
                                snippet = snippet + '<' + previo_snippet + '>';
                        }

                    }

                    else break;

                }

                if (snippet != " ") presearchitems.Add(new SearchItem(elemento.Key, snippet, elemento.Value));
                else break;
            }
            else break;
        }

        string sugerencia = "";
        string presugerencia = "";

        foreach (var item in query_normalizada)
        {
            if (!List_columna.Contains(item))
            {
                presugerencia = CrearSugerencia.FindMostSimilarString(item, List_columna);
                sugerencia = sugerencia + presugerencia + " ";
            }
            else
            {
                sugerencia = sugerencia + item + " ";
            }
        }
        return new SearchResult(presearchitems.ToArray(), sugerencia);
    }
}

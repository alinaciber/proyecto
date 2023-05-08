namespace MoogleEngine;
public static class TFidfCalculator
{
    public static double Calcula_param2(Dictionary<string, int> diccionario)
    {
        double param2 = 0;
        foreach (var element in diccionario)
        {
            param2 += element.Value;
        }
        return param2;
    }
    public static double Calcula_param3(string palabra, List<Dictionary<string, int>> list_dictionary, int count_fila)
    {
        double count_param3 = 0;
        int param3;

        for (int i = 0; i < count_fila; i++)
        {
            try
            {
                param3 = list_dictionary[i][palabra];
                count_param3++;
            }
            catch
            {

            }
        }
        //System.Console.WriteLine("param3"+ count_param3);
        return count_param3;
    }
    public static double TF_IDF_Calc(double ConteoPalabraDocActual, double TotalPalabrasDocActual, double TotalArchivos, double TotalDocsconPalabra)
    {
        double resultado = 0;

        resultado = (((ConteoPalabraDocActual / TotalPalabrasDocActual) * Math.Log(TotalArchivos / TotalDocsconPalabra)) * 100);
        return resultado;
    }

    public static double [,] MatrizTFIDF(int count_fila, int count_columna, List<Dictionary<string, int>> list_dictionary, List<string> List_columna)
    {
        double[,] matriz_TF_IDF = new double[count_fila, count_columna];
        double param1;
        double param2;
        double param3;

        for (int i = 0; i < matriz_TF_IDF.GetLength(0); i++)
        {
            param2 = TFidfCalculator.Calcula_param2(list_dictionary[i]);

            for (int j = 0; j < matriz_TF_IDF.GetLength(1); j++)
            {

                try
                {
                    param1 = list_dictionary[i][List_columna[j]];

                }
                catch
                {
                    param1 = 0;
                }

                param3 = TFidfCalculator.Calcula_param3(List_columna[j], list_dictionary, count_fila);

                matriz_TF_IDF[i, j] = TFidfCalculator.TF_IDF_Calc(param1, param2, Convert.ToDouble(count_fila), param3);

            }
        }
        return matriz_TF_IDF;
    }
}
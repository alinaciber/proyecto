namespace MoogleEngine;
public static class CrearSugerencia
{
    static int CalculateLevenshteinDistance(string s1, string s2)
        {
            int[,] d = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= s2.Length; j++)
            {
                d[0, j] = j;
            }

            for (int j = 1; j <= s2.Length; j++)
            {
                for (int i = 1; i <= s1.Length; i++)
                {
                    if (s1[i - 1] == s2[j - 1])
                    {
                        d[i, j] = d[i - 1, j - 1];
                    }
                    else
                    {
                        d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + 1);
                    }
                }
            }

            return d[s1.Length, s2.Length];
        }
    public static string FindMostSimilarString(string principalCadena, List<string> cadena)
        {
            string mostSimilarString = "";
            int minDistance = int.MaxValue;

            foreach (string str in cadena)
            {
                int distance = CalculateLevenshteinDistance(principalCadena, str);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    mostSimilarString = str;
                }
            }

            return mostSimilarString;
        }
}
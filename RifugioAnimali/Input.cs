public class Input
{
    public static int Int() //chiede un input all'utente e verifica che sia un intero, altrimenti riprova
    {
        while (true)
        {
            if (int.TryParse(Console.ReadLine(), out int output))
            {
                return output;
            }
            else
            {
                Console.WriteLine("Input non valido, riprovare");
            }
        }
    }

    public static string String() //chiede un input all'utente e verifica che non sia una stringa vuota, altrimenti riprova
    {
        while (true)
        {
            string? output = Console.ReadLine();
            if (!string.IsNullOrEmpty(output))
            {
                return output;
            }
            else
            {
                Console.WriteLine("Input non valido, riprovare");
            }
        }
    }

    public static DateTime Datetime() //chiede un input all'utente e verifica che sia un datetime, altrimenti riprova
    {
        while (true)
        {
            if (DateTime.TryParse(Console.ReadLine(), out DateTime output))
            {
                return output;
            }
            else
            {
                Console.WriteLine("Input non valido, riprovare");
            }
        }
    }

    public static bool Bool() //chiede un input all'utente e verifica che sia un bool, altrimenti riprova
    {
        while (true)
        {
            if (bool.TryParse(Console.ReadLine(), out bool output))
            {
                return output;
            }
            else
            {
                Console.WriteLine("Input non valido, riprovare");
            }
        }
    }

    public static float Float() //chiede un input all'utente e verifica che sia un float, altrimenti riprova
    {
        while (true)
        {
            if (float.TryParse(Console.ReadLine(), out float output))
            {
                return output;
            }
            else
            {
                Console.WriteLine("Input non valido, riprovare");
            }
        }
    }

    public static int SelectId(List<int> idList) //chiede un input all'utente e verifica che sia nella lista, altrimenti riprova
    {
        int id = -1;
        do
        {
            id = Input.Int();
            if (!idList.Contains(id))
            {
                Console.WriteLine("Input non valido, riprovare");
            }
        } while (!idList.Contains(id));
        return id;
    }
}
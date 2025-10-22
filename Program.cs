using System;
using System.Timers;

class Program
{
    //Локализация постоянного текста в программе реализована при помощи словарей
    static Dictionary<string, string> Lang(string choose)
    {
        //Словарь содержит используемые фразы на русском и английском языках
        //При выборе языка в начале программы подключается и используется только нужный словарь
        if (choose == "ru")
            return new Dictionary<string, string>
            {
                //Русский язык
                {"begin", "Игрок №1 вводит первое слово:"},
                {"select", "Выбран русский язык."},
                {"press", "Нажмите любую клавишу, чтобы продолжить."},
                {"errlen", "Задано слово неверной длины!"},
                {"errsym", "Слово содержит неверные символы!"},
                {"ctrue", "Слово подходит под условие!"},
                {"cfalse", "Но при этом слово повторяется!"},
                {"cwrong", "Задание не выполнено!"},
                {"tech", "Победил игрок №2, так как игрок №1 ввёл неверное значение!"},
                {"turn", "Ход №"},
                {"pl", ". Игрок №"},
                {"input", " вводит слово: "},
                {"timeout", "Время вышло!"},
                {"victory", "Победил игрок №"}
            };
        else
            return new Dictionary<string, string>
            {
                //Английский язык
                {"begin", "Player 1 enters the first word:"},
                {"select", "English language was chosen."},
                {"press", "Press any button to continue."},
                {"errlen", "Incorrect length!"},
                {"errsym", "Incorrect symbols!"},
                {"ctrue", "The word is correct!"},
                {"cfalse", "But the word has already been used!"},
                {"cwrong", "The word is incorrect!"},
                {"tech", "Player 2 won, because Player 1 used wrong inputs!"},
                {"turn", "Turn "},
                {"pl", ". Player "},
                {"input", " enters a word: "},
                {"timeout", "Time's up!"},
                {"victory", "Victory goes to Player "}
            };
    }

    //Таймер и функция, отвечающая за проверку времени
    static bool timesup = false;
    static System.Timers.Timer timer1;

    //Для работы с таймером используется данная функция
    static async Task<string?> WordInput(int mil)
    {
        timesup = false;

        //Настройка таймера и привязка события NoTimeLeft
        timer1 = new System.Timers.Timer(mil);
        timer1.Elapsed += NoTimeLeft;
        timer1.AutoReset = false;
        timer1.Start();

        //Task.Run используется, так как Console.Readline блокирует вызов
        Task<string?> input = Task.Run(() => Console.ReadLine());

        //mil - переменная, содержащая в себе время задержки в милисекундах
        //по умолчанию в программе mil = 15000 (15 секунд)
        Task ends = await Task.WhenAny(input, Task.Delay(mil));

        //Остановка таймера и его очистка
        timer1.Stop();
        timer1.Dispose();

        //Возвращает строку, если пользователь успевает ввести, иначе - ничего не возвращает
        if (ends == input && !timesup)
            return await input;
        else
            return null;
    }
    
    //Событие, отвечающее за проверку времени
    static void NoTimeLeft(object? sender, ElapsedEventArgs e)
    {
        timesup = true;
    }

    //Эта функция используется для проверки слова на соответствие условию задачи
    static bool Check(int[] arr, int dlina, string sword, char fletter, int mas)
    {
        //Создается массив целых чисел, который считает, сколько раз в слове используется каждая буква алфавита
        //mas - размер алфавита (в зависимости от версии игры)
        int[] arrcheck = new int[mas];

        //Цикл проходит по слову и считает количество использований
        foreach (char c in sword)
        {
            //fletter - первая буква алфавита (в зависимости от версии игры)
            int nomer = c - fletter;

            //Проверка на то, используются ли в слове только буквы нужного алфавита
            if (nomer >= 0 && nomer < mas)
                arrcheck[nomer]++;
            else
            {
                return false;
            }
        }

        //Для того, чтобы проверить условие игры, нужно сравнить массив с уже созданным заранее массивом для первого слова
        //rule отвечает за правдивость условия
        bool rule = true;

        //Количество использованных букв не должно превышать количество таких же букв в первом слове
        for (int i = 0; i < mas; i++)
        {
            if (arr[i] < arrcheck[i])
            {
                rule = false;
                break;
            }
        }

        //возвращается значение rule
        return rule;
    }

    //Используем async, так как используется таймер
    static async Task Main()
    {
        //Очищается консоль и объявляются нужные переменные и массивы
        Console.Clear();
        char fletter;
        int mas;
        string lang = "";

        //Программа начинается с выбора языка, пока не будет выбран верный вариант, игра не начнется
        while (true)
        {
            Console.WriteLine("Выберите язык/Choose your language");
            Console.WriteLine("Напишите 'ru' для поддержки русского языка");
            Console.WriteLine("Type 'en' for english language support");
            lang = Console.ReadLine();
            if ((lang == "ru") || (lang == "en"))
            {
                break;
            }
            else
            {
                //Повторяется запрос на ввод языка в случае неверного ввода
                Console.WriteLine("Неверный ввод/Wrong input");
                Console.WriteLine("Нажмите любую клавишу/Press any key");
                Console.ReadKey();
                Console.Clear();
                continue;
            }
        }

        //Подключается словарь
        var dict = Lang(lang);

        //Используются правильные значения в соответствии с локализацией
        if (lang == "ru")
        {
            fletter = 'а';  //Русская а
            mas = 33;
        }
        else
        {
            fletter = 'a';  //Английская a
            mas = 26;
        }

        //Очистка консоли
        Console.WriteLine(dict["select"]);
        Console.WriteLine(dict["press"]);
        Console.ReadKey();
        Console.Clear();

        //Считывается первое слово
        Console.WriteLine(dict["begin"]);
        string fword = Console.ReadLine();

        //Обработка первого слова, чтобы избежать ошибок
        fword = fword.Trim();
        fword = fword.ToLower();

        //Проверка ввода, слово должно быть определенной длины
        int dlina = fword.Length;
        if (dlina < 8 || dlina > 30)
        {
            Console.WriteLine(dict["errlen"]);
            Console.WriteLine(dict["tech"]);
            return;
        }

        //Создается массив целых чисел для подсчета количества использований букв первого слова
        int[] arr = new int[mas];

        //Подсчет букв
        foreach (char c in fword)
        {
            int nomer = c - fletter;

            //Проверка на использование правильных символов
            if (nomer >= 0 && nomer < mas)
                arr[nomer]++;
            else
            {
                Console.WriteLine(dict["errsym"]);
                Console.WriteLine(dict["tech"]);
                return;
            }
        }

        //Очистка консоли
        Console.WriteLine(dict["press"]);
        Console.ReadKey();
        Console.Clear();

        //Отсчет ходов начинается с 2
        int count = 2;

        //Создается словарь для того, чтобы можно было легко проверять, было ли слово использовано ранее
        List<string> words = new List<string> { fword };

        //Цикл для последующих слов
        while (true)
        {   
            //Подсчет номера игрока
            int player = (count % 2 == 0) ? 2 : 1;
            Console.WriteLine(dict["turn"] + count + dict["pl"] + player + dict["input"]);

            //Использование таймера на 15 секунд
            string? sword = await WordInput(15000);
            if (sword == null)
            {
                Console.WriteLine(dict["timeout"]);
                break;
            }

            //Обработка строки, чтобы избежать ошибок
            sword = sword.Trim();
            sword = sword.ToLower();

            count++;

            //Проверка на условие
            bool rule = Check(arr, dlina, sword, fletter, mas);

            //Проверка на уже использованное слово
            if (rule)
            {
                Console.WriteLine(dict["ctrue"]);
                if (words.Contains(sword))
                {
                    Console.WriteLine(dict["cfalse"]);
                    break;
                }

                //Добавляем слово в словарь
                words.Add(sword);

                //Очистка консоли
                Console.WriteLine(dict["press"]);
                Console.ReadKey();
                Console.Clear();
            }
            else
            {
                Console.WriteLine(dict["cwrong"]);
                break;
            }
        }
        ;

        //Подсчет номера игрока
        count = (count % 2 == 0) ? 2 : 1;

        //В случае поражения одного игрока выводится финальное сообщение, описывающее победителя
        Console.WriteLine(dict["victory"] + count + "!");
    }

}

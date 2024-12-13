using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Решение задачи 4.11 (замена оборудования)
        Solve4_11();

        // Решение задачи 4.12 (распределение капитала)
        Solve4_12();

        // Решение задачи 4.14 (план производства)
        Solve4_14();
    }

    /// <summary>
    /// Задача 4.11: Минимизация затрат на замену оборудования.
    /// Используем динамическое программирование.
    /// </summary>
    static void Solve4_11()
    {
        // Данные: Z(τ) – затраты при возрасте оборудования τ
        // S – стоимость нового оборудования
        // N – число лет планирования
        int N = 10;
        int S = 10; // тыс. руб.
        
        // Дано:
        // R(τ) не используется в данном решении явно, так как в условии ключевые затраты - Z(τ) и S
        int[] R = { 25, 24, 24, 23, 23, 23, 22, 22, 21, 20 };
        int[] Z = { 15, 15, 16, 16, 17, 17, 18, 18, 19, 20 };

        // DP: F(t) – минимальные затраты за t лет
        // F(0) = 0
        // F(t) = min_{1 <= τ <= t} [F(t-τ) + Z(τ) + S]
        // Восстановим также решение.

        int[] F = new int[N + 1];
        int[] choice = new int[N + 1]; // для восстановления решения

        F[0] = 0;
        for (int t = 1; t <= N; t++)
        {
            int best = int.MaxValue;
            int bestTau = -1;
            for (int tau = 1; tau <= t && tau <= Z.Length; tau++)
            {
                int cost = F[t - tau] + Z[tau - 1] + S;
                if (cost < best)
                {
                    best = cost;
                    bestTau = tau;
                }
            }
            F[t] = best;
            choice[t] = bestTau;
        }

        // Восстановление лет замены
        List<int> replacementYears = new List<int>();
        int year = N;
        while (year > 0)
        {
            int tau = choice[year];
            replacementYears.Add(year - tau + 1); 
            year -= tau;
        }

        replacementYears.Reverse();

        Console.WriteLine("Задача 4.11: Минимальные затраты: " + F[N]);
        Console.Write("Годы замены оборудования: ");
        foreach (var y in replacementYears)
        {
            Console.Write(y + " ");
        }
        Console.WriteLine("\n");
    }

    /// <summary>
    /// Задача 4.12: Распределение капитала для максимизации прироста продукции.
    /// DP по принципу похожему на задачу рюкзака.
    /// </summary>
    static void Solve4_12()
    {
        // Тестовые данные:
        // Предполагаем, что индекс в массиве соответствует вложению с шагом 20 тыс. руб.
        // Например, f[i][0] – прирост при 0 тыс. руб., f[i][1] – при 20 тыс. руб., ... f[i][5] – при 100 тыс. руб.
        int[][] f = new int[][]
        {
            new int[]{0, 20, 40, 60, 80, 100}, // предприятие 1
            new int[]{0, 12, 28, 38, 47, 56},  // предприятие 2
            new int[]{0, 14, 38, 48, 67, 79},  // предприятие 3
            new int[]{0, 13, 37, 47, 58, 66}   // предприятие 4
        };

        // Общий бюджет S = 100 тыс. руб. 
        // Будем считать, что шаг инвестирования: 0, 20, 40, 60, 80, 100 тыс. руб. => всего 6 шагов (0..5)
        // i - номер предприятия (1..4), n - количество шагов (0..5)
        int enterprises = f.Length;
        int steps = f[0].Length - 1; // 5 шагов по 20 тыс, индекс 5 соответствует 100 тыс.
        int maxSteps = 5; // соответствует 100 тыс. руб. при шаге 20 тыс.

        // DP:
        // F(i,n) – максимум прироста при распределении n шагов по i предприятиям.
        // F(i,n) = max_{x=0..n} [F(i-1,n-x) + f[i][x]]
        // Будем считать, что i и n идут с 0.
        // i=0 – предприятие 1, i=1 – предприятие 2, ...;
        // n=0..5

        int[,] F = new int[enterprises, maxSteps + 1];
        int[,,] choice = new int[enterprises, maxSteps + 1, 2]; 
        // choice хранит x для восстановления решения

        // Инициализация для первого предприятия
        for (int x = 0; x <= maxSteps; x++)
        {
            F[0, x] = f[0][x];
        }

        // Заполнение для остальных
        for (int i = 1; i < enterprises; i++)
        {
            for (int n = 0; n <= maxSteps; n++)
            {
                int best = int.MinValue;
                int bestX = -1;
                for (int x = 0; x <= n; x++)
                {
                    int val = F[i - 1, n - x] + f[i][x];
                    if (val > best)
                    {
                        best = val;
                        bestX = x;
                    }
                }
                F[i, n] = best;
                choice[i, n, 0] = bestX;   // сколько дать предприятию i
                choice[i, n, 1] = n - bestX; // остаток для предыдущих
            }
        }

        int maxValue = F[enterprises - 1, maxSteps];

        // Восстановление решения
        int budgetLeft = maxSteps;
        int[] allocation = new int[enterprises];
        for (int i = enterprises - 1; i >= 0; i--)
        {
            int x = choice[i, budgetLeft, 0];
            allocation[i] = x * 20; // переводим шаги в тыс. руб.
            budgetLeft = choice[i, budgetLeft, 1];
        }

        Console.WriteLine("Задача 4.12: Максимальный прирост: " + maxValue);
        Console.WriteLine("Распределение капитала (тыс. руб.) по предприятиям:");
        for (int i = 0; i < enterprises; i++)
        {
            Console.WriteLine("Предприятие " + (i + 1) + ": " + allocation[i]);
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Задача 4.14: Оптимальный план производства.
    /// Реализуем простой перебор с DP по месяцам и запасам.
    /// </summary>
    static void Solve4_14()
    {
        // Данные:
        // C_t – затраты на производство 1000 изделий в месяц t
        int[] C = {13, 15, 17, 19};
        // H – затраты хранения на 1000 изделий
        int H = 1;
        // Потребности V_t в тыс. изделий:
        int[] V = {2, 3, 3, 2}; // в тысячах
        // начальные запасы
        int S0 = 2; // 2000 изделий
        // ограничения:
        int maxProd = 4; // 4000 изделий в тысячах
        int maxStore = 4; // 4000 изделий в тысячах
        int months = 4;

        // DP(t, s) – минимальная стоимость от t-го месяца до конца, если на начало месяца t на складе s тыс. изделий.
        // DP(5, s) = 0 (нет месяцев дальше)
        // DP(t, s) = min_{p=0..maxProd} [ C_t*p + H*s_new + DP(t+1, s_new) ]
        // где s_new = s + p - V_t при условии 0 <= s_new <= maxStore.

        // Размеры: t=1..4, s=0..4
        // Для удобства 0-индекс: t=0..3
        // DP будет размером [months+1, maxStore+1]
        int[,] DP = new int[months+1, maxStore+1];
        for (int s = 0; s <= maxStore; s++)
            DP[months, s] = 0; // после последнего месяца затраты = 0

        // для восстановления решения
        int[,,] choice = new int[months+1, maxStore+1, 2];

        for (int t = months - 1; t >= 0; t--)
        {
            for (int s = 0; s <= maxStore; s++)
            {
                int best = int.MaxValue;
                int bestP = -1;
                // перебираем производство
                for (int p = 0; p <= maxProd; p++)
                {
                    int s_new = s + p - V[t];
                    if (s_new < 0 || s_new > maxStore) continue;
                    int cost = C[t]*p + H*s_new + DP[t+1, s_new];
                    if (cost < best)
                    {
                        best = cost;
                        bestP = p;
                    }
                }
                DP[t, s] = best;
                choice[t, s, 0] = bestP;
            }
        }

        int minCost = DP[0, S0];

        // Восстановим решение
        int[] productionPlan = new int[months];
        int curS = S0;
        for (int t = 0; t < months; t++)
        {
            int p = choice[t, curS, 0];
            productionPlan[t] = p;
            curS = curS + p - V[t];
        }

        Console.WriteLine("Задача 4.14: Минимальная стоимость: " + minCost);
        Console.WriteLine("Оптимальный план производства по месяцам (в тыс. изделий):");
        for (int t = 0; t < months; t++)
        {
            Console.WriteLine("Месяц " + (t+1) + ": произвести " + productionPlan[t]*1000 + " изделий");
        }
        Console.WriteLine();
    }
}

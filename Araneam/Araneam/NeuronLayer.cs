using System;
using VectorSpace;
using MyParallel;
using ArrayHelper;

namespace Araneam
{
    /*
     * Общие пояснения для тех, кто попытается разобраться:
     * 0) Этот класс - реализация одного слоя нейронов.
     * !1) Работа с ним должна производиться в многопоточном апартаменте, потому что жизнь не справедлива
     * !2) После завершения работы необходимо вызвать метод Dispose()
     * 3) Для создания вызовете любой конструктор с нужными параметрами
     *  3.1) Можно вызвать затем метод NormalInitialize() - для нормализации всех весов всех нейронов
     *  3.2) Для полносвязаной сети требуется затем вызвать метод CalcInvers(bool), в котором аргумент равен true если пред идущий слой имеет порог.
     *              Что в свою очеред можно проверить по свойству WithThreshold.
     * 4) Для вычисления выполните следующие команды
     *  4.1) Присвойте входной вектор свойству Input.
     *  4.2) Вызовете метод Calc() - он вернет результирующий вектор
     * 5) Для обучения требуется выполнить полностью пунты 4 а затем вызвать метод Сorrection(Vector).
    */

    /// <summary>
    /// Представление одного слоя нейронов
    /// !!!Работа со слоем должна производиться в многопоточном апартаменте!!!
    /// </summary>
    [Serializable]
    public class NeuronLayer: ICloneable, IDisposable
    {
        public Neuron[] neuros;

        /// <summary>
        /// Входной сигнал
        /// </summary>
        [NonSerialized]
        Vector input;

        /// <summary>
        /// Матрица связи синапсов каждого нейрона с входным сигналом
        /// </summary>
        int[][] inputIndex;
        public int[][] InputIndex
        {
            get { return inputIndex; }
        }

        /// <summary>
        /// Матрица связи каждого входного сигнала с синапсами нейрона
        /// </summary>
        TwoArray[] inversIndex;
        public TwoArray[] InversIndex
        { 
            get { return inversIndex; }
            private set { inversIndex = value; }
        }

        /// <summary>
        /// Выходной сигнал
        /// </summary>
        public Vector Output
        {
            get;
            private set;
        }

        /// <summary>
        /// Число потоков, равное числу ядер процессора
        /// </summary>
        [NonSerialized]
        int threadCount;

        int inputLength;

        bool withThreshold;
        public bool WithThreshold
        { get { return withThreshold; } }

        #region Информация необходимая для загрузки сети с диска
        readonly FuncFactory fi;
        #endregion

        #region Группа параллельных обработчиков
        [NonSerialized]
        SetInputWorker InputWorker;
        [NonSerialized]
        CorrectionWorker Correcter;
        [NonSerialized]
        NeurosCalcWorker Calcer;
        #endregion

        /// <summary>
        /// Внутренний конструктор для метода Copy
        /// </summary>
        NeuronLayer(int n, int[][] indexs, bool useThreshold, int count, FuncFactory f)
        {
            fi = f;
            withThreshold = useThreshold;

            threadCount = count;

            neuros = new Neuron[n];

            SetOutput();

            inputIndex = indexs;
        }

        /// <summary>
        /// Конструктор для не полносвязных сетей
        /// </summary>
        /// <param name="n">Число нейронов в слои</param>
        /// <param name="indexs">Матрица связей синапсов нейрона и входного сигнала</param>
        /// <param name="useThreshold">Добавить ли порог для выхода</param>
        /// <param name="name">Имя функции активации</param>
        /// <param name="p">Параметры функци активации</param>
        public NeuronLayer(int n, int[][] indexs, bool useThreshold, int count, string name, params Double[] p)
        {
            fi = new FuncFactory(name, p);
            withThreshold = useThreshold;

            neuros = new Neuron[n];

            SetOutput();

            inputIndex = indexs;
            for (int i = 0; i < n; i++)
            {
                neuros[i] = new Neuron(inputIndex[i].Length, fi.f);
            }

            SetWorker(count);
        }

        /// <summary>
        /// Конструктор для полносвязных сетей
        /// </summary>
        /// <param name="n">Число нейронов в слои</param>
        /// <param name="m">Размерность входа</param>
        /// <param name="useThreshold">Добавить ли порог для выхода</param>
        /// <param name="name">Имя функции активации</param>
        /// <param name="p">Параметры функци активации</param>
        public NeuronLayer(int n, int m, bool useThreshold, int count, string name, params Double[] p)
        {
            withThreshold = useThreshold;
            inputLength = m;
            fi = new FuncFactory(name, p);

            neuros = new Neuron[n];

            SetOutput();

            inputIndex = new int[n][];
            for (int i = 0; i < n; i++)
            {
                inputIndex[i] = new int[m];
                for (int j = 0; j < m; j++) inputIndex[i][j] = j;
            }

            for (int i = 0; i < n; i++)
            {
                neuros[i] = new Neuron(inputIndex[i].Length, fi.f);
            }
             
            SetWorker(count);
        }
        
        /// <summary>
        /// Коррекция весов w=w+rate*input
        /// </summary>
        public void Сorrection(Vector rate)
        {
                Correcter.Run(rate);
        }

        public Vector CalcDer()
        {
            if (fi.df == null) throw new ArgumentNullException("Функция активации не имеет производной.");
            return new Vector(neuros.Length).Set(i => fi.df(Output[i]));
        }

        public Vector CalcDer(Vector v)
        {
            if (fi.df == null) throw new ArgumentNullException("Функция активации не имеет производной.");
            if (v.Length != neuros.Length) throw new ArgumentNullException("Плохой вектор");
            return v.Set(i => fi.df(Output[i]));
        }

        /// <summary>
        /// Расчет матрицы связи каждого входного сигнала с синапсами нейрона
        /// </summary>
        /// <param name="missLast">Пропускать ли последний вход, если он константный</param>
        public void CalcInvers(bool missLast)
        {
            if (inputLength==0) throw new ArgumentOutOfRangeException("Вход не может быть нулевым, назначте вход или используйте другой конструктор");
            TwoList[] inv;
            if (missLast) inv = new TwoList[inputLength-1];
            else inv = new TwoList[inputLength];
            int i, j;

            for (i = 0; i < inv.Length; i++)
            {
                inv[i] = new TwoList();
            }
            int k;

            for (i = 0; i < neuros.Length; i++)
                for (j = 0; j < inputIndex[i].Length; j++)
                {
                    k = inputIndex[i][j];
                    if (k != inv.Length)
                    {
                        //номер нейрона
                        inv[k].index.Add(i);
                        //номер весов в нейроне
                        inv[k].subIndex.Add(j);
                    }
                }

            inversIndex = new TwoArray[inv.Length];
            for (i = 0; i < inv.Length; i++)
            {
                inversIndex[i] = new TwoArray(inv[i].index.ToArray(), inv[i].subIndex.ToArray());
            }
        }

        /// <summary>
        /// Расчет матрицы связи каждого входного сигнала с синапсами нейрона
        /// </summary>
        /// <param name="missLast">Пропускать ли несколько последних входов, если они не обучаемая часть</param>
        public void CalcInvers(int missLast)
        {
            if (inputLength == 0) throw new ArgumentOutOfRangeException("Вход не может быть нулевым, назначте вход или используйте другой конструктор");
            TwoList[] inv;
            inv = new TwoList[inputLength - missLast];
            int i, j;

            for (i = 0; i < inv.Length; i++)
            {
                inv[i] = new TwoList();
            }
            int k;

            for (i = 0; i < neuros.Length; i++)
                for (j = 0; j < inputIndex[i].Length; j++)
                {
                    k = inputIndex[i][j];
                    if (k < inv.Length)
                    {
                        //номер нейрона
                        //номер весов в нейроне
                        inv[k].index.Add(i);
                        inv[k].subIndex.Add(j);
                    }
                }

            inversIndex = new TwoArray[inv.Length];
            for (i = 0; i < inv.Length; i++)
            {
                inversIndex[i] = new TwoArray(inv[i].index.ToArray(), inv[i].subIndex.ToArray());
            }
        }

        /// <summary>
        /// Входной сигнал
        /// </summary>
        public Vector Input
        {
            get
            { return input; }

            set
            {
                input = value;
                inputLength = input.Length;
                InputWorker.Run(input, inputIndex);
            }
        }

        void SetOutput()
        {
            if (withThreshold)
            {
                Output = new Vector(neuros.Length + 1);
                Output.element[neuros.Length] = 1.0 / Output.Length;
            }
            else Output = new Vector(neuros.Length);
        }

        /// <summary>
        /// Восстанавление несериализуемых данных
        /// </summary>
        public void Refresh()
        {
            SetOutput();

            fi.Refresh();

            //Востановление функций активации и высвобождение памяти под синапсы
            for (int i = 0; i < neuros.Length; i++)
            {
                neuros[i].activationFunction = fi.f;
                neuros[i].synapse = new Vector(neuros[i].Length);
            }
            SetWorker(0);
        }

        /// <summary>
        /// Расчет выходного сигнала
        /// </summary>
        /// <returns>Выходной сигнал</returns>
        public Vector Calc()
        {
            Calcer.Run(Output);
            return Output;
        }

        /*
        /// <summary>
        /// Инициализация весов случайными числами от a до b
        /// </summary>
        public void Initialize(double a, double b)
        {
            for (int i = 0; i < neuros.Length; i++)
                neuros[i].weight.SetRandom(a, b);
        }

        /// <summary>
        /// Инициализация весов случайными числами от -1 до 1
        /// </summary>
        public void Initialize()
        {
            for (int i = 0; i < neuros.Length; i++)
                neuros[i].weight.SetRandom();
        }
        */
        /// <summary>
        /// Инициализация весов случайными числами по модулю не превосходящих обратного квадратного корня от числа связей
        /// </summary>
        public void NormalInitialize(Random random)
        {
            double x;
            for (int i = 0; i < neuros.Length; i++)
            {
                x = 1.0 / (Math.Sqrt(neuros[i].Length));
                neuros[i].weight.SetRandomAroundZero(x, x / 100, random);
            }
        }

        public void ReSetWorker(int n)
        {
            Dispose();
            SetWorker(n);
        }

        void SetWorker(int n)
        {
            if (n == 0)
            {
                threadCount = Environment.ProcessorCount;
                if (threadCount > neuros.Length) threadCount = neuros.Length;
            }
            else
            {
                if (n > neuros.Length) threadCount = neuros.Length;
                else threadCount = n;
            }
            InputWorker = new SetInputWorker(threadCount, neuros);
            Correcter = new CorrectionWorker(threadCount, neuros);
            Calcer = new NeurosCalcWorker(threadCount, neuros);
        }

        public object Clone()
        {
            int[][] indx = inputIndex.CloneOk();

            NeuronLayer nl = new NeuronLayer(neuros.Length, inputIndex, withThreshold, threadCount, fi);

            for (int i = 0; i < neuros.Length; i++)
            {
                nl.neuros[i] = neuros[i].CloneOk();
            }

            nl.inputLength = inputLength;
            if (inversIndex != null)
                //было inversIndex.Length!=inputLength
                nl.CalcInvers(inputLength - inversIndex.Length);

            nl.SetWorker(threadCount);

            return nl;
        }

        public bool haveNaN()
        {
            for (int i = 0; i < neuros.Length; i++)
                if (neuros[i].haveNaN()) return true;
            return false;
        }

        /// <summary>
        /// Метод высвобождает ресурсы под потоки, требуется вызвать перед завершением приложения
        /// </summary>
        public void Dispose()
        {
            InputWorker.Dispose();
            Correcter.Dispose();
            Calcer.Dispose();
        }
    }
}
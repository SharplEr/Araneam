using System;
using VectorSpace;
using IOData;

namespace Araneam
{
    /// <summary>
    /// Базовый класс для представления однослойного персептрона использующий алгоритм LMS для обучения с моделью отжига
    /// </summary>
    [Serializable]
    public class LMSNetwork: Network
    {
        double rateStart;
        double timeLearn;

        public LMSNetwork(double r, double t)
        {
            rateStart = r;
            timeLearn = t;
            fixedLayers = new NeuronLayer[1];
        }

        /// <summary>
        /// Конструктор создающий выходной слой и параметры обучения
        /// </summary>
        /// <param name="r">Начальное значение параметра обучения</param>
        /// <param name="t">Время обучения</param>
        /// <param name="n">Число нейронов</param>
        /// <param name="indexs">Матрица связей</param>
        /// <param name="name">Имя функции активации</param>
        /// <param name="p">Параметры функции активации</param>
        public LMSNetwork(double r, double t, int n, int[][] indexs, string name, params Double[] p)
        {
            rateStart = r;
            timeLearn = t;
            layers = new NeuronLayer[1];
            layers[0] = new NeuronLayer(n, indexs, false, 1, name, p);
            fixedLayers = new NeuronLayer[1];
        }

        /// <summary>
        /// Конструктор создающий выходной слой и параметры обучения для полносвязного положения
        /// </summary>
        /// <param name="r">Начальное значение параметра обучения</param>
        /// <param name="t">Время обучения</param>
        /// <param name="n">Число нейронов</param>
        /// <param name="m">Размерность входного сигнала</param>
        /// <param name="name">Имя функции активации</param>
        /// <param name="p">Параметры функции активации</param>
        public LMSNetwork(double r, double t, int n, int m, string name, params Double[] p)
        {
            rateStart = r;
            timeLearn = t;
            layers = new NeuronLayer[1];
            layers[0] = new NeuronLayer(n, m, false, 1, name, p);
            fixedLayers = new NeuronLayer[1];
        }

        public override double Learn(Vector x, Vector d)
        {
            Vector y = Calculation(x);
            Vector errorSignal = d - y;
            double ans = (double)errorSignal;

            layers[0].Сorrection(errorSignal.Multiplication(rateStart / (1.0 + (double)step / timeLearn)));
            
            step++;
            return ans;
        }

        public override double Learn(Vector x, Vector d, double r)
        {
            Vector y = Calculation(x);
            Vector errorSignal = d - y;
            double ans = (double)errorSignal;

            layers[0].Сorrection(errorSignal.Multiplication(r*rateStart / (1.0 + (double)step / timeLearn)));

            step++;
            return ans;
        }

        public double LearnNew(Vector x, Vector d)
        {
            Vector y = Calculation(x);
            Vector errorSignal = d - y;
            double ans = (double)errorSignal;

            layers[0].Сorrection(errorSignal.Multiplication(rateStart / (1.0 + (double)step / timeLearn)));

            return ans;
        }

        public void Learn(Vector[] inputDate, Vector[] resultDate, int max)
        {
            Random rnd = new Random();

            step = 0;

            int[] indexs;
            for (int epoch = 1; epoch <= max; epoch++)
            {
                indexs = Statist.getRandomIndex(inputDate.Length, rnd);
                for (int i = 0; i < inputDate.Length; i++)
                {
                    int k = indexs[i];
                    LearnNew(inputDate[k], resultDate[k]);
                }
                step += inputDate.Length;
            }
        }
    }
}
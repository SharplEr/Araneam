using System;
using IOData;

namespace Araneam
{
    //!Недописанный класс!
    /// <summary>
    /// Базовый класс для нейронных сетей решающих задачу классификации
    /// </summary>
    public abstract class ClassificationNetwork:Network
    {
        //В будущем подсчет рейтинга можно выделить
        public void LearnWithRatios(Simples[] simples, int maxEpoch)
        {
            int maxCount = 0;
            int n = 0;          //Суммарное число всех тестовых примеров
            for (int i = 0; i < simples.Length; i++)
            {
                if (simples[i].Length > maxCount) maxCount = simples[i].Length;
                n+=simples[i].Length;
            }

            //Такой коэффициент решает проблему неравного числа классов в выборки
            double[] ratios = new double[simples.Length];
            for (int i = 0; i < ratios.Length; i++)
                ratios[i] = (double)maxCount / simples[i].Length;

            double[] errors = new double[simples.Length];

            for (int i = 0; i < errors.Length; i++)
                errors[i] = 0.0;

            //Индексы по которым можно перебрать все примеры как линейный список
            Tuple<int,int>[] indexes = new Tuple<int,int>[n];
            int k = 0;
            for (int i = 0; i < simples.Length; i++)
                for (int j = 0; j < simples[i].Length; j++)
                {
                    indexes[k] = new Tuple<int, int>(i,j);
                    k++;
                }

            int epoch = 1;
            step = 0;

            do
            {
                //Понадобятся среднии ошибки на каждом классе
                int lastMaxIndex=0;
                int maxIndex=0;
                for (int i = 0; i < errors.Length; i++)
                    errors[i] = 0.0;

                Statist.shuffle2DIndex(indexes, random);
                //Собственно обучение. Примеры в случайном порядке, с рейтингами.
                for (int i = 0; i < indexes.Length; i++ )
                {
                    int tk = indexes[i].Item1;
                    int tm = indexes[i].Item2;

                    errors[tk] += LearnWithNewError(simples[tk][tm].input, simples[tk].result.Spectrum, ratios[tk]);
                }
                double maxError = 0;
                for (int i = 0; i < errors.Length; i++)
                {
                    errors[i] /= simples[i].Length;
                    if (errors[i] > maxError)
                    {
                        maxError = errors[i];
                        maxIndex = i;
                    }
                }

                //Что бы сеть не скатилась к распознованию только одного класса должна наращиваться значимость.
                if (maxIndex != lastMaxIndex)
                    for (int i = 0; i < ratios.Length; i++)
                        ratios[i] = maxCount / simples[i].Length * errors[i] / maxError;
                else
                    for (int i = 0; i < ratios.Length; i++)
                        ratios[i] *= errors[i] / maxError;

                //Ниже мы нормализуем рейтинги, что бы они не были больше 1.
                double maxRation = 0;
                for (int i = 0; i < ratios.Length; i++)
                    if (maxRation < ratios[i]) maxRation = ratios[i];
                for (int i = 0; i < ratios.Length; i++)
                    ratios[i] /= maxRation;

                epoch++;
            }
            while (epoch < maxEpoch);
        }
    }
}

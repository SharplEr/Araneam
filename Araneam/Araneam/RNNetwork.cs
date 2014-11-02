using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using MyParallel;
using IOData;
using VectorSpace;
using ArrayHelper;

namespace Araneam
{
    public class RNNetwork
    {
        /// <summary>
        /// Слои R-части
        /// </summary>
        protected BackPropagationNetworkWithPlugs<int[]> network;


        /// <summary>
        /// Слои N-части
        /// </summary>
        protected NLayer[] NLayers;

        protected double xcret = 4.05;

        public RNNetwork()
        {
        
        }

        /// <summary>
        /// Обработка входного сигнала
        /// </summary>
        /// <param name="input">Входной сигнал</param>
        /// <returns>Выходной сигнал</returns>
        public Vector Calculation(MixData input)
        {
            return network.Calculation(input.continuous, input.discrete);
        }

        /// <summary>
        /// Обработка входных сигналов
        /// </summary>
        /// <param name="input">входные значения</param>
        /// <returns>Результаты</returns>
        public Vector[] Calculation(MixData[] input)
        {
            Vector[] ans = new Vector[input.Length];

            for (int i = 0; i < ans.Length; i++)
                ans[i] = network.Calculation(input[i].continuous, input[i].discrete);
            
            return ans;
        }

        int[] restCounts;

        /// <summary>
        /// Обучение сети
        /// </summary>
        public void Learning(MixData[] input, Results results, int[] maxs, int s, double x, int one, int two, int epo, double xxx)
        {
            //Обучение N-части
            xcret = xxx;
            //Cоздание начального списка вхождений
            List<int>[][] rootList = new List<int>[maxs.Length][];
            for (int i = 0; i < maxs.Length; i++)
            {
                rootList[i] = new List<int>[maxs[i]];
                for (int j = 0; j < rootList[i].Length; j++)
                    rootList[i][j] = new List<int>();
            }

            //Заполнение списка из выборки
            for (int i = 0; i < input.Length; i++)
                for (int j = 0; j < input[i].discrete.Length; j++)
                    rootList[j][input[i].discrete[j]].Add(i);

            //Необъясненное число объектов по классам
            restCounts = results.Counts.CloneOk<int[]>();

            //Создание корневого слоя дерева N-обучения:
            RNLearningNode[][] rootLevel = new RNLearningNode[maxs.Length][];
            for (int i = 0; i < maxs.Length; i++)
                rootLevel[i] = new RNLearningNode[maxs[i]];

            //Начинаем прямой проход

            //Добавляются узлы для всех слоев
            List<List<RNLearningNode>> allNode = new List<List<RNLearningNode>>();

            //Задаем функцию эффективности
            Func<int[], Tuple<double, int>> f = (arr) => GetEffect(results, arr);

            //Первый слой
            allNode.Add(new List<RNLearningNode>());
            for (int i = 0; i < rootLevel.Length; i++)
                for (int j = 0; j < rootLevel[i].Length; j++)
                {
                    int[] links = rootList[i][j].ToArray();
                    rootLevel[i][j] = new RNLearningNode(new Tuple<int, int>[] { new Tuple<int, int>(i, j) }, links, f, null, null);
                    if (rootLevel[i][j].effect >= x)
                    {
                        rootLevel[i][j].skip = true;
                    }
                    if (rootLevel[i][j].links.Length>=s)
                        allNode[0].Add(rootLevel[i][j]);
                }

            //Отсечение объясненных примеров
            List<int> skipObject = new List<int>();
            //поиск объясненных примеров
            for (int i = 0; i < allNode[0].Count; i++)
            {
                if (allNode[0][i].skip)
                    for (int j = 0; j < allNode[0][i].links.Length; j++)
                    {
                        int indx = allNode[0][i].links[j];

                        if (results[indx].Number == allNode[0][i].cls)
                            if (!skipObject.Contains(indx)) skipObject.Add(indx);
                    }
            }
           
            int[] tempSkip = skipObject.ToArray();

            //Их удаление из ссылок
            for (int i = 0; i < allNode[0].Count; i++)
            {
                allNode[0][i].links = RNLearningNode.Xor(allNode[0][i].links, tempSkip);
            }

            //Уменьшение restCounts
            for(int i = 0; i < tempSkip.Length; i++)
                restCounts[results[tempSkip[i]].Number]--;

            int t = 0;

            while(allNode[t].Count>0)
            {
                allNode.Add(new List<RNLearningNode>());
                if (skipObject.Count > results.Length - s) break; //Пока необъясненная часть выборки не станет слишком маленькой что бы её объяснить

                int tempC = 0;
                for (int i = 0; i < restCounts.Length; i++)
                    if (restCounts[i] == 0) tempC++;

                if (restCounts.Length - tempC <= 1) break;   //Все примеры объяснены (осталось не более одного класса)

                for (int i = 0; i < allNode[t].Count; i++)
                {
                    if (!allNode[t][i].skip)
                        for (int j = i + 1; j < allNode[t].Count; j++)
                        {
                            if (!allNode[t][j].skip)
                            {
                                RNLearningNode node = allNode[t][i].Concatenation(allNode[t][j], s, f);
                                if (node != null)
                                {
                                    allNode[t + 1].Add(node);
                                    if (node.effect >= x)
                                    {
                                        node.skip = true;
                                    }
                                }
                            }
                        }
                }

                t++;

                for (int i = 0; i < allNode[t].Count; i++)
                {
                    if (allNode[t][i].skip)
                        for (int j = 0; j < allNode[t][i].links.Length; j++)
                        {
                            int indx = allNode[t][i].links[j];

                            if (results[indx].Number == allNode[t][i].cls)
                                if (!skipObject.Contains(indx)) skipObject.Add(indx);
                        }
                }

                tempSkip = skipObject.ToArray();

                //Их удаление из ссылок
                for (int i = 0; i < allNode[t].Count; i++)
                {
                    allNode[t][i].links = RNLearningNode.Xor(allNode[t][i].links, tempSkip);
                }

                restCounts = results.Counts.CloneOk<int[]>();

                for (int i = 0; i < tempSkip.Length; i++)
                    restCounts[results[tempSkip[i]].Number]--;
            }

            //Обратный проход объединяется с построением обученной N-части

            List<NLayer> allLayers = new List<NLayer>();

            int tBack = 1;
            tempSkip = skipObject.ToArray();

            for (int i = 0; i < tBack; i++)
            {
                List<Tuple<int, int>[]> tupls = new List<Tuple<int, int>[]>();
                List<double> ws = new List<double>();

                for (int j = 0; j < allNode[i].Count; j++)
                {
                        var temp = f(RNLearningNode.Xor(allNode[i][j].links, tempSkip));
                        if (temp.Item1 > 0.0)
                        {
                            tupls.Add(allNode[i][j].group);
                            ws.Add(temp.Item1);
                        }
                }
                if (ws.Count>0)
                allLayers.Add(new NLayer((k)=>tupls[k], ws.ToArray()));
            }

            List<Tuple<int, int>[]> finalTupls = new List<Tuple<int, int>[]>();
            List<double> finalws = new List<double>();

            for (int i = tBack; i < allNode.Count-1; i++)
            {
                for (int j = 0; j < allNode[i].Count; j++)
                {
                    if (allNode[i][j].skip)
                    {
                        finalTupls.Add(allNode[i][j].group);
                        finalws.Add(allNode[i][j].effect);
                    }
                }
            }

            if (finalws.Count > 0)
                allLayers.Add(new NLayer((k) => finalTupls[k], finalws.ToArray()));

            //Довай до tBack в отдельные слое все подряд, после tBack токо няшек в один слой.

            NLayers = allLayers.ToArray();

            //Конец обучения N-части

            List<Vector> pvso = new List<Vector>();
            List<Vector> nvso = new List<Vector>();
            List<Vector> pvsi = new List<Vector>();
            List<Vector> nvsi = new List<Vector>();

            List<int[]> pii = new List<int[]>();
            List<int[]> nii = new List<int[]>();

            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].Number == 0)
                {
                    pvso.Add(results[i].Spectrum);
                    pvsi.Add(input[i].continuous);
                    pii.Add(input[i].discrete);
                }
                else
                {
                    nvso.Add(results[i].Spectrum);
                    nvsi.Add(input[i].continuous);
                    nii.Add(input[i].discrete);
                }
            }
            int count = pvso.Count;

            pvso.AddRange(nvso);
            pvsi.AddRange(nvsi);
            pii.AddRange(nii);

            int[] counts = new int[] { count, results.Length - count };

            double a = 1.7159, b = 2.0 / 3.0;

            network = new BackPropagationNetworkWithPlugs<int[]>(NLayers, 0.01, 2800, new int[] {one, two, 2 }, input[0].continuous.Length, "tanh", a, b);

            network.AddTestDate(pvsi.ToArray(), pii.ToArray(), pvso.ToArray(), counts);
            network.NewLearn(false, epo);
        }

        Tuple<double, int> GetEffect(Results results, int[] indexs)
        {
            int[] counts = new int[results.MaxNumber];

            for (int i = 0; i < indexs.Length; i++)
                counts[results[indexs[i]].Number]++;

            if ((counts[0] == 0) && (counts[0] == 0))
            {
                return new Tuple<double,int>(0,0);
            }

            if ((counts[0] == 0) || (counts[0] == 0))
            {
                if (counts[0] == 0) return new Tuple<double, int>(1, 1);
                else return new Tuple<double, int>(1, 0);
            }

            //Важно что тут идет уже относительно не объясненной части
            double x = (double)counts[0] / restCounts[0];
            double y = (double)counts[1] / restCounts[1];

            int cls;
            if (x > y) cls = 0;
            else cls = 1;

            double t = Math.Abs((x * x - y * y) / (x * y));

            double ans = Math.Tanh(t/xcret);

            return new Tuple<double,int>(ans,cls);;
        }

        public bool haveNaN()
        {
            return network.haveNaN();
        }

        /// <summary>
        /// Высвобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            network.Dispose();
        }
    }
}
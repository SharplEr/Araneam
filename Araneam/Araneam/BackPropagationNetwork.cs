using System;
using System.Collections.Generic;
using System.Linq;
using VectorSpace;
using IOData;
using MyParallel;
using ArrayHelper;

namespace Araneam
{
    /// <summary>
    /// Представление многослойного персептрона с алгоритмом обратного распространения
    /// </summary>
    [Serializable]
    public class BackPropagationNetwork: Network
    {
        protected Vector[] LocalGrads = null;

        double rateStart;
        double timeLearn;

        protected Vector[] inputDate = null;
        protected Vector[] resultDate = null;
        double[] ratios;
        int[] classCount;

        public BackPropagationNetwork(double r, double t, int LayerCount)
        {
            rateStart = r;
            timeLearn = t;

            layers = new NeuronLayer[LayerCount];
            fixedLayers = new NeuronLayer[LayerCount];
            LocalGrads = new Vector[LayerCount];
        }

        public BackPropagationNetwork(double r, double t, int[] LayerCounts, int inputDem, string name, params double[] k)
        {
            rateStart = r;
            timeLearn = t;

            layers = new NeuronLayer[LayerCounts.Length];
            fixedLayers = new NeuronLayer[LayerCounts.Length];
            LocalGrads = new Vector[LayerCounts.Length];

            layers[0] = new NeuronLayer(LayerCounts[0], inputDem, true, 1, name, k);
            layers[0].NormalInitialize(random);

            for (int i = 1; i < LayerCounts.Length; i++)
            {
                layers[i] = new NeuronLayer(LayerCounts[i], LayerCounts[i - 1] + 1, i < LayerCounts.Length - 1, 1, name, k);
                layers[i].NormalInitialize(random);
            }

            for (int i = 1; i < LayerCounts.Length; i++)
                layers[i].CalcInvers(true);
        }

        public void AddTestDate(Vector[] tests, Vector[] results, int[] cc)
        {
            classCount = cc.CloneOk<int[]>();
            int max = 0;

            for(int i = 0; i<classCount.Length; i++)
                if (max<classCount[i]) max = classCount[i];
            
            ratios = new double[results.Length];
            
            int k = 0;

            for(int i = 0; i<classCount.Length; i++)
            {
                for (int j = 0; j < classCount[i]; j++)
                {
                    ratios[k] = (double)max / classCount[i];                    
                    k++;
                }
            }

            List<Vector> t = tests.ToList();
            
            if (inputDate != null) t.AddRange(inputDate);
            inputDate = t.ToArray();

            t = results.ToList();
            if (resultDate != null) t.AddRange(resultDate);
            resultDate = t.ToArray();
        }

        Vector[] tempforlg;

        protected void setLocalGrads(Vector e)
        {
            int l, i, j, k;
            Vector t;
            TwoArray inv;
            
            if (tempforlg==null)
            {
                tempforlg = new Vector[layers.Length-1];
                for (i = 0; i < tempforlg.Length; i++)
                    tempforlg[i] = new Vector(layers[i].neuros.Length);
            }

            if (LocalGrads[layers.Length - 1].IsNull)
                LocalGrads[layers.Length - 1] = e.Multiplication(layers[layers.Length - 1].CalcDer());
            else
                layers[layers.Length - 1].CalcDer(LocalGrads[layers.Length - 1]).Multiplication(e);

            for (l = layers.Length - 2; l >= 0; l--)
            {
                if (LocalGrads[l].IsNull)
                    LocalGrads[l] = layers[l].CalcDer();
                else
                    layers[l].CalcDer(LocalGrads[l]);

                t = tempforlg[l];

                for (i = 0; i < t.Length; i++)
                {
                    inv = layers[l + 1].InversIndex[i];
                    t[i] = 0.0;
                    for (j = 0; j < inv.index.Length; j++)
                    {
                        k = inv.index[j];
                        t[i] += layers[l + 1].neuros[k].weight[inv.subIndex[j]] * LocalGrads[l + 1][k];
                    }
                }
                LocalGrads[l].Multiplication(t);
            }
        }

        public virtual LearnLog EarlyStoppingLearn(bool flag)
        {
            Random rnd = new Random();
            int epoch = 0;
            const double r = 1.0 - 0.2;
            double[] rats = ratios.CloneOk<double[]>();

            step = 0;

            int finish = (int)Math.Round(inputDate.Length * r);

            double error = Double.PositiveInfinity;
            double errorMin = Double.PositiveInfinity;

            int[] indexs;
            int mmm=-1;
            int count = 0;
            //m=5//10
            int max = 20;
            indexs = Statist.getRandomIndex(inputDate.Length, rnd);
            for (int i = 0; i < inputDate.Length; i++)
            {
                int k = indexs[i];
                Learn(inputDate[k], resultDate[k], rats[k]);
            }

            Vector[] calcDate = Calculation(inputDate);
            double[] errors = new double[classCount.Length];
            double maxError = 0.0;
            int l = 0;
            for (int i = 0; i < errors.Length; i++)
            {
                errors[i] = 0.0;
                int k = classCount[i];
                for (int j = 0; j < k; j++)
                {
                    errors[i] += (double)(calcDate[j + l] - resultDate[j + l]);
                }
                errors[i] /= k;
                l += k;
                if (maxError < errors[i]) maxError = errors[i];
            }

            l = 0;
            for (int i = 0; i < classCount.Length; i++)
            {
                for (int j = 0; j < classCount[i]; j++)
                {
                    rats[l] = rats[l] / maxError * errors[i];
                    l++;
                }
            }

            do
            {
                indexs = Statist.getRandomIndex(inputDate.Length, rnd);
                int k;

                for (int i = 0; i < finish; i++)
                {
                    k = indexs[i];
                    Learn(inputDate[k], resultDate[k], rats[k]);
                }

                calcDate = Calculation(inputDate);
                maxError = -1;
                l = 0;
                for (int i = 0; i < errors.Length; i++)
                {
                    errors[i] = 0.0;
                    int m = classCount[i];
                    for (int j = 0; j < m; j++)
                    {
                        errors[i] += Math.Sqrt((double)(calcDate[j + l] - resultDate[j + l]));
                    }
                    errors[i] /= m;
                    l += m;
                    if (maxError < errors[i])
                    {
                        maxError = errors[i];
                        mmm = i;
                    }
                }

                l = 0;
                for (int i = 0; i < classCount.Length; i++)
                {
                    for (int j = 0; j < classCount[i]; j++)
                    {
                        if (((mmm==i)&&(rats[l]<1.0))||((mmm != i)&&(rats[l])==1.0))
                            rats[l] = ratios[l] / maxError * errors[i];
                        else rats[l] *= errors[i]/maxError;

                        //rats[l] = rats[l] / maxError * errors[i];
                        l++;
                    }
                }

                double maxrat = 0.0;

                for (int i = 0; i < rats.Length; i++)
                    if (maxrat < rats[i]) maxrat = rats[i];

                for (int i = 0; i < rats.Length; i++)
                    rats[i]/=maxrat;

                error = 0.0;
                double eee;
                for (int i = finish; i < inputDate.Length; i++)
                {
                    k = indexs[i];
                    eee = ratios[k]*(double)(resultDate[k] - Calculation(inputDate[k]));
                    eee = Math.Sqrt(eee);
                    error += eee;
                }

                error = error / inputDate.Length;

                if (error < errorMin)
                {
                    errorMin = error;
                    count = 0;
                    Fix();
                }
                else count++;

                epoch++;
                step += inputDate.Length;
            } while (count<max);
            //Нармировать r!!11
            ReFix();
            if (flag)
            {
                calcDate = Calculation(inputDate);
                double err=0.0;
                for (int i = 0; i < calcDate.Length; i++)
                    err += Math.Sqrt((double)(calcDate[i] - resultDate[i]));
                err /= calcDate.Length;
                return new LearnLog(step, epoch, err);
            }
            else return new LearnLog(step, epoch);
        }


        public virtual LearnLog NewLearn(bool flag, int max)
        {
            Random rnd = new Random();
            int epoch = 1;
            
            double[] rats = ratios.CloneOk<double[]>();

            step = 0;

            int[] indexs;
            int mmm = -1;
            indexs = Statist.getRandomIndex(inputDate.Length, rnd);
            for (int i = 0; i < inputDate.Length; i++)
            {
                int k = indexs[i];
                Learn(inputDate[k], resultDate[k], rats[k]);
            }

            step += inputDate.Length;

            Vector[] calcDate = Calculation(inputDate);
            double[] errors = new double[classCount.Length];
            double maxError = 0.0;
            int l = 0;
            for (int i = 0; i < errors.Length; i++)
            {
                errors[i] = 0.0;
                int k = classCount[i];
                for (int j = 0; j < k; j++)
                {
                    errors[i] += (double)(calcDate[j + l] - resultDate[j + l]);
                }
                errors[i] /= k;
                l += k;
                if (maxError < errors[i]) maxError = errors[i];
            }

            l = 0;
            for (int i = 0; i < classCount.Length; i++)
            {
                for (int j = 0; j < classCount[i]; j++)
                {
                    rats[l] = rats[l] / maxError * errors[i];
                    l++;
                }
            }

            do
            {
                indexs = Statist.getRandomIndex(inputDate.Length, rnd);
                int k;

                for (int i = 0; i < inputDate.Length; i++)
                {
                    k = indexs[i];
                    Learn(inputDate[k], resultDate[k], rats[k]);
                }

                calcDate = Calculation(inputDate);
                maxError = -1;
                l = 0;
                for (int i = 0; i < errors.Length; i++)
                {
                    errors[i] = 0.0;
                    int m = classCount[i];
                    for (int j = 0; j < m; j++)
                    {
                        errors[i] += (double)(calcDate[j + l] - resultDate[j + l]);
                    }
                    errors[i] /= m;
                    l += m;
                    if (maxError < errors[i])
                    {
                        maxError = errors[i];
                        mmm = i;
                    }
                }

                l = 0;
                for (int i = 0; i < classCount.Length; i++)
                {
                    for (int j = 0; j < classCount[i]; j++)
                    {
                        if (((mmm == i) && (rats[l] < 1.0)) || ((mmm != i) && (rats[l]) == 1.0))
                            rats[l] = ratios[l] / maxError * errors[i];
                        else rats[l] *= errors[i] / maxError;
                        l++;
                    }
                }

                double maxrat = 0.0;

                for (int i = 0; i < rats.Length; i++)
                    if (maxrat < rats[i]) maxrat = rats[i];

                for (int i = 0; i < rats.Length; i++)
                    rats[i] /= maxrat;

                epoch++;
                step += inputDate.Length;
            } while (epoch < max);
            
            if (flag)
            {
                calcDate = Calculation(inputDate);
                double err = 0.0;
                for (int i = 0; i < calcDate.Length; i++)
                    err += Math.Sqrt((double)(calcDate[i] - resultDate[i]));
                err /= calcDate.Length;
                return new LearnLog(step, epoch, err);
            }
            else return new LearnLog(step, epoch);
        }

        public virtual LearnLog NewLearn2(bool flag, int max)
        {
            Random rnd = new Random();
            int epoch = 1;

            step = 0;

            int[] indexs;
            for (epoch = 1; epoch <= max; epoch++)
            {
                indexs = Statist.getRandomIndex(inputDate.Length, rnd);
                for (int i = 0; i < inputDate.Length; i++)
                {
                    int k = indexs[i];
                    Learn(inputDate[k], resultDate[k]);
                }
                step += inputDate.Length;
            }

            if (flag)
            {
                Vector[] calcDate = Calculation(inputDate);
                double err = 0.0;
                for (int i = 0; i < calcDate.Length; i++)
                    err += Math.Sqrt((double)(calcDate[i] - resultDate[i]));
                err /= calcDate.Length;
                return new LearnLog(step, epoch, err);
            }
            else return new LearnLog(step, epoch);
        }

        public virtual LearnLog NewLearn(bool flag, int max, double[] rating)
        {
            Random rnd = new Random();
            int epoch = 1;

            double[] rats = ratios.CloneOk<double[]>();

            step = 0;

            int[] indexs;
            int mmm = -1;
            indexs = Statist.getRandomIndex(inputDate.Length, rnd);
            for (int i = 0; i < inputDate.Length; i++)
            {
                int k = indexs[i];
                Learn(inputDate[k], resultDate[k], rats[k]*rating[k]);
            }

            Vector[] calcDate = Calculation(inputDate);
            double[] errors = new double[classCount.Length];
            double maxError = 0.0;
            int l = 0;
            for (int i = 0; i < errors.Length; i++)
            {
                errors[i] = 0.0;
                int k = classCount[i];
                for (int j = 0; j < k; j++)
                {
                    errors[i] += (double)(calcDate[j + l] - resultDate[j + l]);
                }
                errors[i] /= k;
                l += k;
                if (maxError < errors[i]) maxError = errors[i];
            }

            l = 0;
            for (int i = 0; i < classCount.Length; i++)
            {
                for (int j = 0; j < classCount[i]; j++)
                {
                    rats[l] = rats[l] / maxError * errors[i];
                    l++;
                }
            }

            do
            {
                indexs = Statist.getRandomIndex(inputDate.Length, rnd);
                int k;

                for (int i = 0; i < inputDate.Length; i++)
                {
                    k = indexs[i];
                    Learn(inputDate[k], resultDate[k], rats[k]*rating[k]);
                }

                calcDate = Calculation(inputDate);
                maxError = -1;
                l = 0;
                for (int i = 0; i < errors.Length; i++)
                {
                    errors[i] = 0.0;
                    int m = classCount[i];
                    for (int j = 0; j < m; j++)
                    {
                        errors[i] += Math.Sqrt((double)(calcDate[j + l] - resultDate[j + l]));
                    }
                    errors[i] /= m;
                    l += m;
                    if (maxError < errors[i])
                    {
                        maxError = errors[i];
                        mmm = i;
                    }
                }

                l = 0;
                for (int i = 0; i < classCount.Length; i++)
                {
                    for (int j = 0; j < classCount[i]; j++)
                    {
                        if (((mmm == i) && (rats[l] < 1.0)) || ((mmm != i) && (rats[l]) == 1.0))
                            rats[l] = ratios[l] / maxError * errors[i];
                        else rats[l] *= errors[i] / maxError;
                        l++;
                    }
                }

                double maxrat = 0.0;

                for (int i = 0; i < rats.Length; i++)
                    if (maxrat < rats[i]) maxrat = rats[i];

                for (int i = 0; i < rats.Length; i++)
                    rats[i] /= maxrat;

                epoch++;
            } while (epoch < max);

            if (flag)
            {
                calcDate = Calculation(inputDate);
                double err = 0.0;
                for (int i = 0; i < calcDate.Length; i++)
                    err += Math.Sqrt((double)(calcDate[i] - resultDate[i]));
                err /= calcDate.Length;
                return new LearnLog(step, epoch, err);
            }
            else return new LearnLog(step, epoch);
        }


        public virtual LearnLog FullLearn()
        {
            double minError = (double)layers[layers.Length - 1].Output.Length / (inputDate.Length + 1);
            return FullLearn(minError);
        }

        /// <summary>
        /// Полное обучение
        /// </summary>
        public virtual LearnLog FullLearn(double minError)
        {
            Random rnd = new Random();
            int epoch = 0;
            step = 0;

            double error;
            int[] indexs;

            do
            {
                error = 0.0;
                indexs = Statist.getRandomIndex(inputDate.Length, rnd);
                for (int i = 0; i < inputDate.Length; i++)
                {
                    error += Learn(inputDate[indexs[i]], resultDate[indexs[i]]);
                }
                error = Math.Sqrt(error) / inputDate.Length;
                epoch++;
            } while (error > minError);

            return new LearnLog(step, epoch, error);
        }

        public override double Learn(Vector x, Vector d, double r)
        {
            if (layers == null) throw new ArgumentNullException();

            double ans = 0.0;

            Vector y = Calculation(x);
            Vector errorSignal = d - y;
            ans = (double)errorSignal;

            setLocalGrads(errorSignal);

            double h = r * rateStart / (1.0 + (double)(step) / timeLearn);
            //h = r * rateStart; //!!! Тестовая строчка, пробуем без модели аля отжиг

            int max = Int32.MinValue;
            int min = Int32.MaxValue;
            int now;
            for (int i = 0; i < layers.Length; i++)
            {
                now = layers[i].Input.Length;
                if (now > max) max = now;
                if (now < min) min = now;
            }

            double t = Math.Sqrt(min);
            double m = 1.0 / (Math.Sqrt(max) - t);
            double b = 1.0 - t * m;

            double p;

            //Можно расспаралелить, так как корректировка не зависит от последовательности

            for (int i = 0; i < layers.Length; i++)
            {
                p = (Math.Sqrt(layers[i].Input.Length) * m + b);
                layers[i].Сorrection(LocalGrads[i].Multiplication(h / p));
            }

            //step++;
            return ans;
        }

        public override double Learn(Vector x, Vector d)
        {
            if (layers == null) throw new ArgumentNullException("Learn: нет слоев");

            double ans = 0.0;

            Vector y = Calculation(x);
            Vector errorSignal = d - y;

            ans = (double)errorSignal;

            setLocalGrads(errorSignal);

            double h = rateStart / (1.0 + (double)(step) / timeLearn);

            int max = Int32.MinValue;
            int min = Int32.MaxValue;
            int now;
            for (int i = 0; i < layers.Length; i++)
            {
                now = layers[i].Input.Length;
                if (now > max) max = now;
                if (now < min) min = now;
            }

            double t = Math.Sqrt(min);
            double m;
            if (min != max)
                m = 1.0 / (Math.Sqrt(max) - t);
            else m = 0;
            double b = 1.0 - t * m;

            double p;

            //Можно расспаралелить, так как корректировка не зависит от последовательности
            for (int i = 0; i < layers.Length; i++)
            {   
                if (min != max)
                {
                    p = (Math.Sqrt(layers[i].Input.Length) * m + b);
                    layers[i].Сorrection(LocalGrads[i].Multiplication(h / p));
                }
                else
                {
                    layers[i].Сorrection(LocalGrads[i].Multiplication(h));
                }
            }

            //step++;
            return ans;
        }

        public double Learn(Vector x, Vector d, bool flag)
        {
            if (layers == null) throw new ArgumentNullException();

            double ans = 0.0;

            Vector y = Calculation(x);
            Vector errorSignal = d - y;

            ans = (double)errorSignal;

            setLocalGrads(errorSignal);

            double h = rateStart / (1.0 + (double)(step) / timeLearn);

            int max = Int32.MinValue;
            int min = Int32.MaxValue;
            int now;
            for (int i = 0; i < layers.Length; i++)
            {
                now = layers[i].Input.Length;
                if (now > max) max = now;
                if (now < min) min = now;
            }

            double t = Math.Sqrt(min);
            double m;
            if (min != max)
                m = 1.0 / (Math.Sqrt(max) - t);
            else m = 0;
            double b = 1.0 - t * m;

            double p;

            //Можно расспаралелить, так как корректировка не зависит от последовательности
            for (int i = 0; i < layers.Length; i++)
            {
                if (min != max)
                {
                    p = (Math.Sqrt(layers[i].Input.Length) * m + b);
                    layers[i].Сorrection(LocalGrads[i].Multiplication(h / p));
                }
                else
                {
                    layers[i].Сorrection(LocalGrads[i].Multiplication(h));
                }
            }

            if (flag) step++;
            return ans;
        }
    }
}
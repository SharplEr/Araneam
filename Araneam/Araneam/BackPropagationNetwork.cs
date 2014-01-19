using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorSpace;
using IODate;

namespace Araneam
{
    /// <summary>
    /// Представление многослойного персептрона с алгоритмом обратного распространения
    /// </summary>
    public class BackPropagationNetwork: Network
    {
        protected Vector[] LocalGrads = null;

        double rateStart;
        double timeLearn;

        protected Vector[] testDate = null;
        protected Vector[] resultDate = null;
        protected double[] testCount = null;
        double[] ratios;
        int[] classCount;

        public BackPropagationNetwork(double r, double t, int LayerCount)
        {
            rateStart = r;
            timeLearn = t;

            hidden = new NeuronLayer[LayerCount];
            fixedLayers = new NeuronLayer[LayerCount];
            LocalGrads = new Vector[LayerCount];
        }

        public void AddTestDate(Vector[] tests, Vector[] results, int[] cc)
        {
            classCount = cc;
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
            
            if (testDate != null) t.AddRange(testDate);
            testDate = t.ToArray();

            t = results.ToList();
            if (resultDate != null) t.AddRange(resultDate);
            resultDate = t.ToArray();
            testCount = new double[testDate.Length];
        }

        protected void setLocalGrads(Vector e)
        {
            int l, i, j;
            Vector t;
            TwoArray inv;
            
            LocalGrads[hidden.Length - 1] = e.Multiplication(hidden[hidden.Length - 1].CalcDer());

            for (l = hidden.Length - 2; l >= 0; l--)
            {
                LocalGrads[l] = hidden[l].CalcDer();
                t = new Vector(LocalGrads[l].Length);

                for (i = 0; i < t.Length; i++)
                {
                    inv = hidden[l + 1].InversIndex[i];
                    t[i] = 0.0;
                    for (j = 0; j < inv.index.Length; j++)
                    {
                        t[i] += hidden[l + 1].neuros[inv.index[j]].weight[inv.subIndex[j]] * LocalGrads[l + 1][inv.index[j]];
                    }
                }
                LocalGrads[l].Multiplication(t);
            }
        }

        public virtual LearnLog EarlyStoppingLearn(bool flag)
        {
            int epoch = 0;
            const double r = 1.0 - 0.2;
            double[] rats = (double[]) ratios.Clone();

            n = 0;

            int finish = (int)Math.Round(testDate.Length * r);

            double error = Double.PositiveInfinity;
            double errorMin = Double.PositiveInfinity;

            int[] indexs;
            int mmm=-1;
            int count = 0;
            //m=10
            int max = 5;

            //double maxTestCount;
            //double minTestCount;
            //double N;

            for (int i = 0; i < testDate.Length; i++)
            {
                indexs = Statist.getRandomIndex(testDate.Length);
                int k = indexs[i];
                Learn(testDate[k], resultDate[k], rats[k]);
            }

            Vector[] calcDate = Calculation(testDate);
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
                indexs = Statist.getRandomIndex(testDate.Length);
                int k;
                /*
                maxTestCount = 1;
                minTestCount = 1;
                for (int i = 0; i < finish; i++)
                {
                    k = indexs[i];
                    if (maxTestCount < testCount[k]) maxTestCount = testCount[k];
                    if (minTestCount > testCount[k]) minTestCount = testCount[k];
                }
                */
                //N = maxTestCount / minTestCount;
                //N = (int)Math.Sqrt(N / Math.E);
                for (int i = 0; i < finish; i++)
                {
                    k = indexs[i];
                    Learn(testDate[k], resultDate[k], rats[k]);

                    //double c = testCount[k];
                    //testCount[k] += Math.Sqrt(Learn(testDate[k], resultDate[k], ratios[k]));
                    //int m = (int)((N - 1.0) * minTestCount * ((double)maxTestCount / c - 1.0) / (maxTestCount - minTestCount) + 1.0);
                    /*
                    for (int j = 0; j < m; j++)
                    {
                        Learn(testDate[k], resultDate[k]);
                    }
                    */
                }

                calcDate = Calculation(testDate);
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
                for (int i = finish; i < testDate.Length; i++)
                {
                    k = indexs[i];
                    eee = ratios[k]*(double)(resultDate[k] - Calculation(testDate[k]));
                    eee = Math.Sqrt(eee);
                    testCount[k] += (int) eee;
                    error += eee;
                }

                error = error / testDate.Length;

                if (error < errorMin)
                {
                    errorMin = error;
                    count = 0;
                    Fix();
                }
                else count++;

                epoch++;
            } while (count<max);
            //Нармировать r!!11
            ReFix();
            if (flag)
            {
                calcDate = Calculation(testDate);
                double err=0.0;
                for (int i = 0; i < calcDate.Length; i++)
                    err += Math.Sqrt((double)(calcDate[i] - resultDate[i]));
                err /= calcDate.Length;
                return new LearnLog(n, epoch, err);
            }
            else return new LearnLog(n, epoch);
        }


        public virtual void NewLearn()
        {
            int epoch = 0;
           
            double[] rats = (double[])ratios.Clone();

            n = 0;

            int[] indexs;
            int mmm = -1;

            for (int i = 0; i < testDate.Length; i++)
            {
                indexs = Statist.getRandomIndex(testDate.Length);
                int k = indexs[i];
                Learn(testDate[k], resultDate[k], rats[k]);
            }

            Vector[] calcDate = Calculation(testDate);
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
                indexs = Statist.getRandomIndex(testDate.Length);
                int k;

                for (int i = 0; i < testDate.Length; i++)
                {
                    k = indexs[i];
                    Learn(testDate[k], resultDate[k], rats[k]);
                }

                calcDate = Calculation(testDate);
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
            } while (n<100000);
        }

        public virtual LearnLog FullLearn()
        {
            double minError = (double)hidden[hidden.Length - 1].Output.Length / (testDate.Length + 1);
            return FullLearn(minError);
        }

        /// <summary>
        /// Полное обучение
        /// </summary>
        public virtual LearnLog FullLearn(double minError)
        {
            int epoch = 0;
            n = 0;

            double error;
            int[] indexs;

            do
            {
                error = 0.0;
                indexs = Statist.getRandomIndex(testDate.Length);
                for (int i = 0; i < testDate.Length; i++)
                {
                    error += Learn(testDate[indexs[i]], resultDate[indexs[i]]);
                }
                error = Math.Sqrt(error) / testDate.Length;
                epoch++;
            } while (error > minError);

            return new LearnLog(n, epoch, error);
        }

        public double Learn(Vector x, Vector d, double r)
        {
            if (hidden == null) throw new ArgumentNullException();

            double ans = 0.0;

            Vector y = Calculation(x);
            Vector errorSignal = d - y;
            ans = (double)errorSignal;

            setLocalGrads(errorSignal);

            double h = r * rateStart / (1.0 + (double)(n) / timeLearn);

            int max = Int32.MinValue;
            int min = Int32.MaxValue;
            int now;
            for (int i = 0; i < hidden.Length; i++)
            {
                now = hidden[i].Input.Length;
                if (now > max) max = now;
                if (now < min) min = now;
            }

            double t = Math.Sqrt(min);
            double m = 1.0 / (Math.Sqrt(max) - t);
            double b = 1.0 - t * m;

            double p;

            //Можно расспаралелить, так как корректировка не зависит от последовательности

            for (int i = 0; i < hidden.Length; i++)
            {
                p = (Math.Sqrt(hidden[i].Input.Length) * m + b);
                hidden[i].Сorrection(LocalGrads[i].Multiplication(h / p));
            }

            n++;
            return ans;
        }

        public override double Learn(Vector x, Vector d)
        {
            if (hidden == null) throw new ArgumentNullException();

            double ans = 0.0;

            Vector y = Calculation(x);
            Vector errorSignal = d - y;
            ans = (double)errorSignal;

            setLocalGrads(errorSignal);

            double h = rateStart / (1.0 + (double)(n) / timeLearn);

            int max = Int32.MinValue;
            int min = Int32.MaxValue;
            int now;
            for (int i = 0; i < hidden.Length; i++)
            {
                now = hidden[i].Input.Length;
                if (now > max) max = now;
                if (now < min) min = now;
            }

            double t = Math.Sqrt(min);
            double m = 1.0 / (Math.Sqrt(max) - t);
            double b = 1.0 - t * m;

            double p;

            //Можно расспаралелить, так как корректировка не зависит от последовательности
            
            for (int i = 0; i < hidden.Length; i++)
            {
                p = (Math.Sqrt(hidden[i].Input.Length) * m + b);
                hidden[i].Сorrection(LocalGrads[i].Multiplication(h / p));
            }

            n++;
            return ans;
        }
    }
}
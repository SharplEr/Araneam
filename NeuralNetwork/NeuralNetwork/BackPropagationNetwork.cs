using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorSpace;

namespace Araneam
{
    public class BackPropagationNetwork: Network
    {
        protected Vector[] LocalGrads = null;

        double rateStart;
        double timeLearn;

        protected Vector[] testDate = null;
        protected Vector[] resultDate = null;
        protected int[] testCount = null;

        public BackPropagationNetwork(double r, double t, int LayerCount)
        {
            rateStart = r;
            timeLearn = t;

            hidden = new NeuronLayer[LayerCount];
            fixedLayers = new NeuronLayer[LayerCount];
            LocalGrads = new Vector[LayerCount];
        }

        public void AddTestDate(Vector[] tests, Vector[] results)
        {
            List<Vector> t = tests.ToList();

            if (testDate != null) t.AddRange(testDate);
            testDate = t.ToArray();

            t = results.ToList();
            if (resultDate != null) t.AddRange(resultDate);
            resultDate = t.ToArray();
            testCount = new int[testDate.Length];
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
                        t[i] += hidden[l + 1].neuros[inv.index[j]].Weight[inv.subIndex[j]] * LocalGrads[l + 1][inv.index[j]];
                    }
                }
                LocalGrads[l].Multiplication(t);
            }
        }

        public virtual void EarlyStoppingLearn()
        {
            const double r = 1.0 - 0.2;

            n = 0;
            for (int i = 0; i < testCount.Length; i++)
                testCount[i] = 1;

            int finish = (int)Math.Round(testDate.Length * r);

            double error = Double.PositiveInfinity;
            double errorMin = Double.PositiveInfinity;

            int[] indexs;

            int count = 0;
            int max = 30;

            int maxTestCount = 0;
            int minTestCount = Int32.MaxValue;

            int N;

            do
            {
                indexs = Statist.getRandomIndex(testDate.Length);
                int k;
                maxTestCount = 0;
                minTestCount = Int32.MaxValue;
                for (int i = 0; i < testCount.Length; i++)
                {
                    if (maxTestCount < testCount[i]) maxTestCount = testCount[i];
                    if (minTestCount > testCount[i]) minTestCount = testCount[i];
                }
                N = maxTestCount / minTestCount;
                N = (int)Math.Sqrt(N);
                for (int i = 0; i < finish; i++)
                {
                    k = indexs[i];

                    int c = testCount[k];
                    testCount[k] += (int)Learn(testDate[k], resultDate[k]);
                    int m = (int)((N - 1.0) * minTestCount * ((double)maxTestCount / c - 1.0) / (maxTestCount - minTestCount) + 1.0);
                    for (int j = 0; j < m; j++)
                    {
                        Learn(testDate[k], resultDate[k]);
                    }
                }

                error = 0.0;
                double eee;
                for (int i = finish; i < testDate.Length; i++)
                {
                    k = indexs[i];
                    eee = (double)(resultDate[k] - Calculation(testDate[k]));
                    testCount[k] += (int) eee;
                    error += eee;
                }

                indexs = Statist.getRandomIndex(testDate.Length);
                error = Math.Sqrt(error) / testDate.Length;

                if (error < errorMin)
                {
                    errorMin = error;
                    count = 0;
                    Fix();
                }
                else count++;
                
            } while (count<max);

            ReFix();
        }

        public virtual double FullLearn()
        {
            double minError = (double)hidden[hidden.Length - 1].Output.Length / (testDate.Length + 1);
            return FullLearn(minError);
        }

        /// <summary>
        /// Полное обучение
        /// </summary>
        public virtual double FullLearn(double minError)
        {
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

            } while (error > minError);

            return error;
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
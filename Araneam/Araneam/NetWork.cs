﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using ArrayHelper;
using VectorSpace;

namespace Araneam
{
    /*
     * Общие пояснения для тех, кто попытается разобраться:
     * 0) Базовый класс для нейронных сетей
     * 1) Для реализации достаточно реализовать метод Learn()
     *      Смотри пример в LMSNetwork - это однослойный персептрон.
     * 2!) Работа должна производиться в многопоточном апартаменте, потому что см. класс NeuronLayer
     * 3!) После завершения работы необходимо вызвать метод Dispose() - это вызовет Dispose() во всех слоях
     * 4) Для любой реализации этого класс достаточно вызвать метод Calculation() - для обработки входных данных.
    */
    /// <summary>
    /// Представление нейронной сети
    /// </summary>
    [Serializable]
    public abstract class Network:IDisposable
    {
        /// <summary>
        /// Скрытые слои
        /// </summary>
        protected NeuronLayer[] layers;

        /// <summary>
        /// Де/сериализатор
        /// </summary>
        [NonSerialized] readonly BinaryFormatter deser = new BinaryFormatter();

        /// <summary>
        /// Номер шага обучения
        /// </summary>
        protected int step = 0;

        [NonSerialized]
        protected NeuronLayer[] fixedLayers;

        protected int maxThread = 0;

        [NonSerialized]
        protected Random random = new Random();

        /// <summary>
        /// Свойство, задающее максимальное число потоков на слое. Может быть динамически изменено во время работы сети.
        /// Значение 0 соответствет автоматическому определению всех доступных ядер.
        /// </summary>
        public int MaxThread
        {
            get
            {
                return maxThread;
            }

            set
            {
                maxThread = (value < 0) ? 0 : value;
                for (int i = 0; i < layers.Length; i++)
                    layers[i].ReSetWorker(maxThread);
            }
        }

        /// <summary>
        /// Обработка входного сигнала
        /// </summary>
        /// <param name="input">Входной сигнал</param>
        /// <returns>Выходной сигнал</returns>
        public Vector Calculation(Vector input)
        {
            input = input.CloneOk();
            if (layers == null) throw new ArgumentNullException();
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].Input = input;
                input = layers[i].Calc();
            }

            return input.CloneOk();
        }

        public Vector[] Calculation(Vector[] input)
        {
            Vector[] ans = new Vector[input.Length];

            for (int i = 0; i < ans.Length; i++)
                ans[i] = Calculation(input[i]);

            return ans;
        }

        /// <summary>
        /// Сериализация сети в поток
        /// </summary>
        /// <param name="s">Поток</param>
        /// <returns>true в случае удачи, false в случае неудачи</returns>
        public bool Save(Stream s)
        {
            try
            {
                deser.Serialize(s, this);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Десериализация сети из потока
        /// </summary>
        /// <param name="s">Поток</param>
        /// <returns>true в случае удачи, false в случае неудачи</returns>
        public bool Load(Stream s)
        {
            try
            {
                Network nw = (Network)deser.Deserialize(s);
                //Высвобождение ресурсов занятыми потоками
                if (layers != null)
                {
                    for (int i = 0; i < layers.Length; i++)
                        this.layers[i].Dispose();
                }
                
                this.layers = nw.layers;
                this.step = nw.step;
                
                if (layers != null)
                {
                    for (int i = 0; i < layers.Length; i++)
                        layers[i].Refresh();
                }

                //Можно и не делать так, но пусть с новой загрузкой -- начнется новая жизнь)
                random = new Random();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Обучение сети
        /// </summary>
        /// <param name="x">Входной сигнал (не должен быть изменен)</param>
        /// <param name="d">Желаемый отклик</param>
        /// <returns>Квадрат сигнала ошибки до обучения</returns>
        public abstract double Learn(Vector x, Vector d);

        
        public abstract double Learn(Vector x, Vector d, double r);

        /// <summary>
        /// Обучение сети
        /// </summary>
        /// <param name="x">Входной сигнал (не должен быть изменен)</param>
        /// <param name="d">Желаемый отклик</param>
        /// <returns>Квадрат сигнала ошибки после обучения</returns>
        public double LearnWithNewError(Vector x, Vector d)
        {
            Learn(x, d);
            return (double)(d - Calculation(x));
        }

        public double LearnWithNewError(Vector x, Vector d, double r)
        {
            Learn(x, d, r);
            return (double)(d - Calculation(x));
        }

        /// <summary>
        /// Сохранение в памяти текущего состояния сети
        /// </summary>
        public void Fix()
        {
            if (layers == null) return;

            for (int i = 0; i < layers.Length; i++)
            {
                if (fixedLayers[i] != null) fixedLayers[i].Dispose();
                NeuronLayer nl = layers[i].CloneOk();
                fixedLayers[i] = nl;
            }
        }

        /// <summary>
        /// Загрузка ранее сохраненного состояния сети из памяти
        /// </summary>
        public void ReFix()
        {
            if ((fixedLayers == null) || (layers == null)) return;

            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].Dispose();
                layers[i] = fixedLayers[i];
            }
        }

        public bool haveNaN()
        {
            if (layers == null) return true;

            for (int i = 0; i < layers.Length; i++)
                if (layers[i].haveNaN()) return true;
            return false;
        }

        public int[] UnusedInput( double min)
        {
            NeuronLayer l = layers[0];
            double[] weights = new double[l.Input.Length];
            List<int> ans = new List<int>();

            for (int i = 0; i < l.InputIndex.Length; i++)
                for (int j = 0; j < l.InputIndex[i].Length; j++)
                    weights[l.InputIndex[i][j]] += Math.Abs(l.neuros[i].weight[j]);

            for (int i = 0; i < weights.Length; i++)
                if (weights[i] < min) ans.Add(i);

            return ans.ToArray();
        }

        /// <summary>
        /// Высвобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            layers.Let((o) => o.Dispose());
            fixedLayers.Let((o) => o.Dispose()); 
        }
    }
}
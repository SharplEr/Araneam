using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VectorSpace;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

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
    public abstract class Network
    {
        /// <summary>
        /// Скрытые слои
        /// </summary>
        protected NeuronLayer[] hidden;

        /// <summary>
        /// Де/сериализатор
        /// </summary>
        [NonSerialized]
        BinaryFormatter deser = new BinaryFormatter();

        /// <summary>
        /// Номер шага обучения
        /// </summary>
        protected  int n = 0;

        [NonSerialized]
        protected NeuronLayer[] fixedLayers;        

        /// <summary>
        /// Обработка входного сигнала
        /// </summary>
        /// <param name="input">Входной сигнал</param>
        /// <returns>Выходной сигнал</returns>
        public Vector Calculation(Vector input)
        {
            input = (Vector)input.Clone();
            if (hidden == null) throw new ArgumentNullException();
            for (int i = 0; i < hidden.Length; i++)
            {
                hidden[i].Input = input;
                input = hidden[i].Calc();
            }

            return (Vector)input.Clone();
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
                if (hidden != null)
                {
                    for (int i = 0; i < hidden.Length; i++)
                        this.hidden[i].Dispose();
                }
                
                this.hidden = nw.hidden;
                this.n = nw.n;
                
                if (hidden != null)
                {
                    for (int i = 0; i < hidden.Length; i++)
                        hidden[i].Refresh();
                }
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
        /// <param name="x">Входной сигнал</param>
        /// <param name="d">Желаемый отклик</param>
        /// <returns>Квадрат сигнала ошибки</returns>
        public abstract double Learn(Vector x, Vector d);

        /// <summary>
        /// Сохранение в памяти текущего состояния сети
        /// </summary>
        public void Fix()
        {
            if (hidden == null) return;

            for (int i = 0; i < hidden.Length; i++)
            {
                if (fixedLayers[i] != null) fixedLayers[i].Dispose();
                NeuronLayer nl = (NeuronLayer)hidden[i].Clone();
                fixedLayers[i] = nl;
            }
        }

        /// <summary>
        /// Загрузка ранее сохраненного состояния сети из памяти
        /// </summary>
        public void ReFix()
        {
            if ((fixedLayers == null) || (hidden == null)) return;

            for (int i = 0; i < hidden.Length; i++)
            {
                hidden[i].Dispose();
                hidden[i] = fixedLayers[i];
            }
        }

        /// <summary>
        /// Высвобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            if (hidden != null)
                for (int i = 0; i < hidden.Length; i++)
                {
                    if (hidden[i] != null)
                        hidden[i].Dispose();
                }

            if (fixedLayers != null)
                if (fixedLayers != hidden)
                {
                    bool flag = true;
                    if (hidden != null)
                    {
                        for (int i = 0; i < fixedLayers.Length; i++)
                            flag = flag && (fixedLayers[i] != hidden[i]);
                    }
                    if (flag)
                        for (int i = 0; i < fixedLayers.Length; i++)
                        {
                            if (fixedLayers[i] != null)
                                fixedLayers[i].Dispose();
                        }
                }
        }
    }
}
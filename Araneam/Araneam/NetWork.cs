using System;
using VectorSpace;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MyParallel;
using ArrayHelper;

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
        protected NeuronLayer[] layers;

        /// <summary>
        /// Де/сериализатор
        /// </summary>
        [NonSerialized]
        BinaryFormatter deser = new BinaryFormatter();

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
        /// <returns>Квадрат сигнала ошибки</returns>
        public abstract double Learn(Vector x, Vector d);

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

        /// <summary>
        /// Высвобождение ресурсов
        /// </summary>
        public void Dispose()
        {
            layers.let((o) => o.Dispose());
            fixedLayers.let((o) => o.Dispose()); 
        }
    }
}
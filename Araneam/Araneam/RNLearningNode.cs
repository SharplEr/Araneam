using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Araneam
{
    public class RNLearningNode
    {
        public Tuple<int, int>[] group;     //Группа
        public bool skip = false;           //Является ли данная группа занятой
        public int[] links;                 //Номера в выборки, где эта группа есть
        public double effect;               //величина дляпроверки x-критерия
        public RNLearningNode LeftNode;     //Ссылка на левого предка
        public RNLearningNode RightNode;    //Ссылка на правого предка
        //Лево и право не вполне определены

        public int cls; //Класс на который больше указывает данная группа, когда эффективность 0 -- не имеет смысла

        //По умолчанию true
        //public bool fullness = false;        //Является ли данная группа полностью выраженной верхнем слоем. Если skip = true этот флаг не имеет смысла.
        
        //Реализовать требуется операцию объединения двух нодов,
        //Если операция не может быть осуществлена,
        //То вернется null
        //Что хреново, ну да ладно.
        //Во-первых надо что бы group были совместимы
        //А это значит они должны быть все одинаковы
        //Кроме одного
        //Не забыть про случай когда всего 1
        //К тому же этот один разный
        //Не должен быть из одной координаты 
        //Затем требуется смешать links
        //В сумме пересечение должно давать минимум s элементов

        public RNLearningNode(Tuple<int, int>[] group, int[] links, Func<int[], Tuple<double, int>> getEffect, RNLearningNode LN, RNLearningNode RN)
        {
            this.group = group;
            this.links = links;
            var temp = getEffect(links);
            effect = temp.Item1;
            cls = temp.Item2;
            LeftNode = LN;
            RightNode = RN;
        }

        public RNLearningNode Concatenation(RNLearningNode node, int s, Func<int[], Tuple<double, int>> getEffect)
        {
            Tuple<int, int>[] newgroup = Union(this.group, node.group);
            if (newgroup == null) return null;

            int[] newlinks = Intersection(this.links, node.links);
            if (newlinks.Length < s)
            {
                return null;
            }

            return new RNLearningNode(newgroup, newlinks, getEffect, this, node);
        }

        public static int[] Intersection(int[] listOne, int[] listTwo)
        {
            List<int> ans = new List<int>();

            int i = 0, j = 0;

            while ((i < listOne.Length) && (j < listTwo.Length))
            {
                if (listOne[i]<listTwo[j]) 
                    i++;
                else if (listTwo[j] < listOne[i])
                    j++;
                else
                {
                    ans.Add(listOne[i]);
                    i++;
                    j++;
                }
            }

            return ans.ToArray();
        }

        public static List<int> Union(int[] listOne, int[] listTwo)
        {
            List<int> ans = new List<int>(listOne);

            for (int i = 0; i < listTwo.Length; i++)
                if (!ans.Contains(listTwo[i])) ans.Add(listTwo[i]);

            return ans;
        }

        public static List<int> Union(List<int>listOne, int[] listTwo)
        {
            List<int> ans = new List<int>(listOne);

            for (int i = 0; i < listTwo.Length; i++)
                if (!ans.Contains(listTwo[i])) ans.Add(listTwo[i]);

            return ans;
        }

        public static int[] Xor(int[] listOne, int[] listTwo)
        {
            List<int> ans = new List<int>(listOne);

            for (int i = 0; i < listTwo.Length; i++)
                if (ans.Contains(listTwo[i])) ans.Remove(listTwo[i]);

            return ans.ToArray();
        }

        /*
         * Важно что бы list'-ы были упорядоченны внутри, по первому элементу Tuple
         * И listTwo шел после listOne
        */ 
        public static Tuple<int, int>[] Union(Tuple<int, int>[] listOne, Tuple<int, int>[] listTwo)
        {
            int n = listOne.Length;
            if (listTwo.Length != n) return null;

            for (int i = 1; i < n; i++)
                if ((listOne[i].Item1 != listTwo[i - 1].Item1) || (listOne[i].Item2 != listTwo[i - 1].Item2))
                    return null;
            if (listOne[0].Item1 == listTwo[n - 1].Item1)
                return null;

            Tuple<int, int>[] ans = new Tuple<int, int>[n + 1];
            ans[0] = listOne[0];
            ans[n] = listTwo[n - 1];

            for (int i = 1; i < n; i++)
                ans[i] = listOne[i];

            return ans;
        }
    }
}
Araneam
=============

This is a library for building and training of some neural networks.
Araneam - "spider" in Latin.

Now have the means to read learning-data from csv file. 
You can also build a multi-layered perceptrons.
In some places use parallel processing.

All comments and instruction below, in Russian - I'm sorry.
=============

Вся библиотека состоит из следующих библиотек:
1. VectorSpace
2. MyParallel
3. IODate
4. Araneam

1. VectorSpace - библиотека для некоторых операций из линейной алгербы.
Важные классы:
	1) Vector: представляет n-мерные вещественный вектор и операции для работы с ним. Он используется везде.
2. MyParallel - библиотека с некоторыми важными методами расширения и классами для паралельной обработки массивов.

Важные классы:
	1) ParallelWorker - базовый класс для многопоточной обработки данных, которые можно представить в виде одномерного массива. Он используется для ресурсоемких вычислений. Поскольку жизнь не справедлива его можно использовать только в многопоточном апартаменте.
	2) static ParallelArray. Содержит методы расширения, которые в целом не очень важны для понимания остального кода, но могут пригодится лично вам - зайдите почитайте.
	3) static ParallelLambda. Содержит методы расширения, которые в целом не очень важны для понимания остального кода, но могут пригодится лично вам - зайдите почитайте. Самый удобный очень маленький метод расширения для потоков, которые запускают его в многопоточном апартаменте - InMTA().
3. IODate - библиотека для считывания данных обучения из csv файлов. А так же для некоторой статистической обработки.

Важные классы:
	1) static Statist. Содержит некоторые статистические методы. Которые используются в разны местах. Посмотрите, возможно вам что-то понравится.
	2) DateAnalysis. Главный класс в этой библиотеке - он считывает данные из csv файлов (нескольких) пронумеровывает дискретные значения, которые изначально не представляются как числа, и возвращает два массива векторов (Vector[]) - соответственно как входные и выходные данные, а так же нормирует их.
	3) Классы DateReader, DateInfo и CSVReader - служат в помощь классу DateAnalysis и напрямую мало используются.
4. Araneam - главная библиотке и с тем же названием как и у всей библиотеке для построения нейронных сетей.
Главные классы:

1) Neuron. Представление одного нейрона.

2) NeuronLayer. Представление одного слоя нейронов.

3) Network. Базовый класс, для нейронных сетей.

4) LMSNetwork. Представление однослойного персептрона использующий алгоритм LMS для обучения с моделью отжига.

5) BackPropagationNetwork. Представление многослойного персептрона с алгоритмом обратного распространения.

6) FuncInfo. Инкапсулирует информацию о функции активации. Данный класс существует по той причине, что код не может быть сериализован, и его восстановление после сохранения на диск осуществляется таким вот образом.
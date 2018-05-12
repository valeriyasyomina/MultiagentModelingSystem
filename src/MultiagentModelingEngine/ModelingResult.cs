using Common;
using MultiagentModelingEngine.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiagentModelingEngine
{
    public class ModelingResult
    {
        public const int SECONDS_IN_MINUTE = 60;
        public ModelingResult()
        {
            PeopleInCovers = new Dictionary<int, int>();
            PersonalityPsychotypeAmount = new Dictionary<PersonalityTemperament, int>();
            PersonSexAmount = new Dictionary<Sex, int>();
            PersonAgeAmount = new Dictionary<DecimalRange, int>();
        }
        public int AccidentTime { get; set; }
        // распределение людей п опротиводымным укрытиям
        public Dictionary<int, int> PeopleInCovers { get; set; }
        // кол-во спесшихся людей (кто попал в урытие)
        public int SavedInSmokeCoverPeopleAmount { get; set; }
        // кол-во спасшихся на автомобилях людей (те, кто успел уехать)
        public int SavedInCarsPeopleAmount { get; set; }
        // кол-вл умерших людей
        public int DiedPeople { get; set; }
        // кол-во раненых людей
        public int DamagedPeople { get; set; }
        // кол-во всех людей
        public int AllPeopleAmount { get; set; }
        // распределение людей по психъотипу личности
        public Dictionary<PersonalityTemperament, int> PersonalityPsychotypeAmount { get; set; }
        // распределение по половому признаку
        public Dictionary<Sex, int> PersonSexAmount { get; set; }
        // распределение людей по возрастам (по десяткам лет)
        public Dictionary<DecimalRange, int> PersonAgeAmount { get; set; }
        // общее время эвакуации
        public int EvacuationTime { get; set; }
        // общее число автомобилей
        public int AllVehiclesAmount { get; set; }
        // число автомобилей, покинувших тоннель
        public int LeftTunnelVehiclesAmount { get; set; }
        // число сгоревших автомобилей
        public int BurnedVehiclesAmount { get; set; }
        // среднее время эвакуации
        public double AverageEvacuationTime => SavedInSmokeCoverPeopleAmount != 0 ? (double) SummaryEvacuationTimes / (double) SavedInSmokeCoverPeopleAmount : 0;

        public void AddEvacuationTime(int time)
        {
            SummaryEvacuationTimes += time;
        }

        public void AddPersonBySmokeCover(int smokeCoverIndex)
        {
            if (!PeopleInCovers.ContainsKey(smokeCoverIndex))
                PeopleInCovers.Add(smokeCoverIndex, 1);
            else
                PeopleInCovers[smokeCoverIndex]++;
        }

        public void AddPersonAgeAmount(int age)
        {
            var down = (int)(age / 10) * 10;
            var range = new DecimalRange()
            {
                Down = down,
                Up = down + 10,
            };
            if (!PersonAgeAmount.ContainsKey(range))
                PersonAgeAmount.Add(range, 1);
            else
                PersonAgeAmount[range]++;
        }

        public void AddPersonByPersonalityPsychotype(PersonalityTemperament psychotype)
        {
            if (!PersonalityPsychotypeAmount.ContainsKey(psychotype))
                PersonalityPsychotypeAmount.Add(psychotype, 1);
            else
                PersonalityPsychotypeAmount[psychotype]++;
        }

        public void AddPersonBySex(Sex sex)
        {
            if (!PersonSexAmount.ContainsKey(sex))
                PersonSexAmount.Add(sex, 1);
            else
                PersonSexAmount[sex]++;
        }

        public void WriteFile(string fileName, ModelingEngineConfiguration config)
        {
            StreamWriter outputFile = new StreamWriter(fileName);
            if (config.ConsiderTemperament)
                outputFile.WriteLine("Моделирование с учетом темперамента лисчности");
            else
                outputFile.WriteLine("Моделирование без учета темперамента личности");
            if (config.MaxPeopleAmount != 0)
            {
                outputFile.WriteLine($"Время начала моделирования: {config.StartModelingTime}");
                outputFile.WriteLine($"Время окончания моделирования: {config.EndModelingTime}");
            }
            outputFile.WriteLine($"Время возникновения ЧС: {AccidentTime}");
            outputFile.WriteLine("-----------------------------------------");
            outputFile.WriteLine($"Общее количество человек: {AllPeopleAmount}");
            outputFile.WriteLine($"Количество раненых: {DamagedPeople}");
            outputFile.WriteLine($"Количество жертв: {DiedPeople}");
            outputFile.WriteLine($"Количество человек, спасшихся в противодымных укрытиях: {SavedInSmokeCoverPeopleAmount}");
            outputFile.WriteLine($"Количество человек, покинувших тоннель на автомобиле: {SavedInCarsPeopleAmount}");
            outputFile.WriteLine("-----------------------------------------");
            WritePeopleInCoversDistribution(outputFile);
            outputFile.WriteLine("-----------------------------------------");
            WritePersonSexAmountDistribution(outputFile);
            outputFile.WriteLine("-----------------------------------------");
            WritePeopleAgeAmountDistribution(outputFile);
            if (config.ConsiderTemperament)
            {
                outputFile.WriteLine("-----------------------------------------");
                WritePersonalityPsychotypeAmountDistribution(outputFile);
            }
            outputFile.WriteLine("-----------------------------------------");
            outputFile.WriteLine($"Общее количество автомобилей: {AllVehiclesAmount}");
            outputFile.WriteLine($"Количество сгоревших автомобилей: {BurnedVehiclesAmount}");
            outputFile.WriteLine("-----------------------------------------");
            outputFile.WriteLine($"Общее время эвакуации: {EvacuationTime } сек");
            outputFile.WriteLine($"Общее время эвакуации: ~{EvacuationTime / SECONDS_IN_MINUTE} минут");
          //  outputFile.WriteLine($"Среднее время эвакуации: {AverageEvacuationTime}");

            outputFile.Close();
        }

        void WritePeopleAgeAmountDistribution(StreamWriter outputFile)
        {
            outputFile.WriteLine("Распределение людей по возрасту");
            foreach (var pair in PersonAgeAmount)
                outputFile.WriteLine($"     от {pair.Key.Down} до {pair.Key.Up} лет: {pair.Value} человек");
        }

        void WritePeopleInCoversDistribution(StreamWriter outputFile)
        {
            outputFile.WriteLine($"Количество противодымных укрытий: {PeopleInCovers.Count}");
            outputFile.WriteLine("Расределение людей по противодымным укрытиям");
            foreach (var pair in PeopleInCovers)            
                outputFile.WriteLine($"     Противодымное укрытие № {pair.Key + 1}: {pair.Value} человек");            
        }

        void WritePersonSexAmountDistribution(StreamWriter outputFile)
        {
            outputFile.WriteLine("Распределение людей по половому признаку");
            if (PersonSexAmount.ContainsKey(Sex.MALE))            
                outputFile.WriteLine($"     Мужчины: {PersonSexAmount[Sex.MALE]} человек");
            if (PersonSexAmount.ContainsKey(Sex.FEMALE))
                outputFile.WriteLine($"     Женщины: {PersonSexAmount[Sex.FEMALE]} человек");            
        }

        void WritePersonalityPsychotypeAmountDistribution(StreamWriter outputFile)
        {
            outputFile.WriteLine("Распределение людей по виду темперамента");
            foreach (var pair in PersonalityPsychotypeAmount)
                outputFile.WriteLine($"     Темперамент {pair.Key.ToString()}: {pair.Value} человек");
        }

        // сумма времен эвакуации
        private int SummaryEvacuationTimes { get; set; }
    }
}
